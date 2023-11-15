using Conditions;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    public class GetBooksCommand : BaseNetworkCommand
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        private string apiPath;
        private GetBooksRequestModel request;
        private List<BookModel> response;
        private int booksCountOnCategory = -1;

        public override void Execute()
        {
            Retain();
            if (EventData.data == null)
            {
                Debug.LogError("GetBooksCommand => data --- NULL");
                Fail();
                return;
            }

            request = EventData.data as GetBooksRequestModel;
            if (request == null)
            {
                Debug.LogError("GetBooksCommand => request --- NULL");
                Fail();
                return;
            }

            PrepareObject(request);

            if (request.booksRequestType == BooksRequestType.DownloadedBooks)
            {
                apiPath = APIPaths.GET_DOWNLOADED_BOOKS.ToDescription();

            }
            else if (request.booksRequestType == BooksRequestType.FavoritesBooks)
            {
                apiPath = APIPaths.GET_FAVORITES_BOOKS.ToDescription();
            }
            else
            {
                apiPath = APIPaths.GETBOOKS.ToDescription();
            }

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult));
        }

        private void CheckResult(ResultNetworkModel result)
        {
            void DisplayError(string error)
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel
                {
                    screenName = UIScreens.UIErrorPopup,
                    data = $"Technical info: {apiPath}, Error: {error}",
                    isAddToScreensList = false
                });
            }

            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                DisplayError("no answer");
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
                Fail();
                return;
            }

            Parse(result.jsonObject);

            Debug.Log("Count of book = " + response.Count);
            if (response.Count == 0)
            {
                request.requestFalseAction?.Invoke();
                Release();
                return;
            }

            for (int i = 0; i < response.Count; i++)
            {
                response[i].InitializeBook(GetDefaultLanguage(request.categories, request.languages));
                if (response[i].IsAllowedLanguageBook())
                {
                    AddBookToCategory(response[i]);
                }
                else
                {
                    AddBookToNonAllowedLanguageBookList(response[i]);
                }
            }

            SetTotalBooksToCategory();
            SetNextLoadPage();

            request.requestTrueAction?.Invoke();
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                //change response format on the server and unify it for all types of books. After this delete this if
                if (request.booksRequestType == BooksRequestType.FavoritesBooks)
                {
                    response = jsonObject["books"].ToObject<List<BookModel>>();
                    booksCountOnCategory = jsonObject["count"].ToObject<int>();
                }
                else
                {
                    response = jsonObject["list"].ToObject<List<BookModel>>();
                    booksCountOnCategory = jsonObject["total"].ToObject<int>();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetBooksCommand parse error. Error text: {0}", e);
                response = new List<BookModel>();
            }
        }

        private void AddBookToCategory(BookModel book)
        {
            switch (request.booksRequestType)
            {
                case BooksRequestType.BooksByCategory:
                    BooksLibrary.categoriesBooks[request.categories[0].Position].books.Add(book);
                    book.SetLibraryBooksLink(BooksLibrary.categoriesBooks[request.categories[0].Position]);
                    break;
                case BooksRequestType.SearchResult:
                    BooksLibrary.searchResultBooks.books.Add(book);
                    book.SetLibraryBooksLink(BooksLibrary.searchResultBooks);
                    break;
                case BooksRequestType.FurturedBooks:
                    BooksLibrary.featuredBooks.books.Add(book);
                    book.SetLibraryBooksLink(BooksLibrary.featuredBooks);
                    break;
                case BooksRequestType.DownloadedBooks:
                    BooksLibrary.downloadedBooks.books.Add(book);
                    book.SetLibraryBooksLink(BooksLibrary.downloadedBooks);
                    break;
                case BooksRequestType.FavoritesBooks:
                    BooksLibrary.favoritesBooks.books.Add(book);
                    book.SetLibraryBooksLink(BooksLibrary.favoritesBooks);
                    break;
                default:
                    Debug.Log("GetBooksCommand => AddBookToCategory => Conditions.BooksRequestType is Empty. Book name = " + book.Name + "Book ID = " + book.Id);
                    break;
            }
        }

        private void AddBookToNonAllowedLanguageBookList(BookModel book)
        {
            BooksLibrary.nonAllowedLanguageBooks.books.Add(book);
        }

        private void SetTotalBooksToCategory()
        {
            switch (request.booksRequestType)
            {
                case BooksRequestType.BooksByCategory:
                    BooksLibrary.categoriesBooks[request.categories[0].Position].totalBooks = booksCountOnCategory;
                    break;
                case BooksRequestType.SearchResult:
                    BooksLibrary.searchResultBooks.totalBooks = booksCountOnCategory;
                    break;
                case BooksRequestType.FurturedBooks:
                    BooksLibrary.featuredBooks.totalBooks = booksCountOnCategory;
                    break;
                case BooksRequestType.DownloadedBooks:
                    BooksLibrary.downloadedBooks.totalBooks = booksCountOnCategory;
                    break;
                case BooksRequestType.FavoritesBooks:
                    BooksLibrary.favoritesBooks.totalBooks = booksCountOnCategory;
                    break;
                default:
                    Debug.Log("GetBooksCommand => SetTotalBooksToCategory => Conditions.BooksRequestType is Empty");
                    break;
            }
        }

        private void SetNextLoadPage()
        {
            Books books = new Books();

            switch (request.booksRequestType)
            {
                case BooksRequestType.BooksByCategory:
                    books = BooksLibrary.categoriesBooks[request.categories[0].Position];
                    break;
                case BooksRequestType.SearchResult:
                    books = BooksLibrary.searchResultBooks;
                    break;
                case BooksRequestType.FurturedBooks:
                    books = BooksLibrary.featuredBooks;
                    break;
                case BooksRequestType.DownloadedBooks:
                    books = BooksLibrary.downloadedBooks;
                    break;
                case BooksRequestType.FavoritesBooks:
                    books = BooksLibrary.favoritesBooks;
                    break;
                default:
                    Debug.Log("GetBooksCommand => SetNextLoadPage => Conditions.BooksRequestType is Empty");
                    break;
            }

            if (books.totalBooks > books.books.Count)
            {
                books.currentPage++;
            }
        }

        private Languages GetDefaultLanguage(List<BooksCategory> categories, string[] languages)
        {
            Languages res = LanguagesModel.SelectedLanguage;
            if (categories != null && categories.Count == 1)
            {
                if (categories[0].TechnicalName == "Norwegian")
                {
                    res = Languages.Norwegian;
                }
                else if (categories[0].TechnicalName == "EnglishFactBooks")
                {
                    res = Languages.English;
                }
                else if (categories[0].TechnicalName == "StorybooksInEnglish")
                {
                    res = Languages.English;
                }
            }

            //if only one languge selected in search filter the book should be showen with this language transaltion
            if (languages.Length == 1)
            {
                Enum.TryParse(languages[0], true, out res);
            }

            return res;
        }
    }
}