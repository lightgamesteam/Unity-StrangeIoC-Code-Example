using Assets.Scripts.Services.Analytics;
using BooksPlayer;
using Conditions;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.DownloadedBooks;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Buttons
{
    public class UIDeleteButtonView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }
        [Header("UI")]
        [SerializeField] private Button button;
        [Space(5)]
        [SerializeField] private Image outline;
        [SerializeField] private Image icon;
        [SerializeField] private Image fillArea;

        [Header("Params")]
        [SerializeField] private Color likedColor;
        private BookModel book;

        public void LoadView()
        {
            button.onClick.AddListener(StartBookDeleting);
            Dispatcher.AddListener(EventGlobal.E_DeleteBookForDescription, DeleteBookUIEvent);
            Dispatcher.AddListener(EventGlobal.E_BookDownloadEnd, ShowButton);
        }

        public void RemoveView()
        {
            button.onClick.RemoveAllListeners();
        }

        protected override void OnDestroy()
        {
            Dispatcher.RemoveListener(EventGlobal.E_DeleteBookForDescription, DeleteBookUIEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookDownloadEnd, ShowButton);
        }

        public void InitButton(BookModel bookModel)
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            this.book = bookModel;
            if (book.IsDownloaded)
            {
                ShowButton();
            }
            else
            {
                //----------hotfix
                if (GameObject.FindObjectOfType<UIDownloadedBooksView>() != null && book.BooksCollection.ContainsBook(book))
                {
                    ShowButton();
                    return;
                }
                //-------------

                HideButton();
            }
        }

        public void NoInteractableButton()
        {
            button.interactable = false;
        }

        public void SetInteracButton()
        {
            button.interactable = true;
        }

        private void ShowButton()
        {
            gameObject.SetActive(true);
        }

        private void HideButton()
        {
            gameObject.SetActive(false);
        }

        private void StartBookDeleting()
        {
            CoroutineExecutor.instance.Execute(DeleteBook());
        }

        private IEnumerator DeleteBook()
        {
            List<Languages> languages = new List<Languages>();
            if (FindObjectOfType<UIDownloadedBooksView>())
            {
                languages.AddRange(Enum.GetValues(typeof(Languages)).Cast<Languages>());
            }
            else
            {
                languages.Add(book.CurrentTranslation);
            }


            foreach (Languages language in languages)
            {
                DeleteTranslation(language);
                yield return null;
            }
            Dispatcher.Dispatch(EventGlobal.E_UpdateDownloadedBooks);
        }

        private void DeleteTranslation(Languages language)
        {
            book.CurrentTranslation = language;
            BookModel.Translation translation = book.GetTranslation();

            Analytics.LogEvent(EventName.ActionDeleteBooksFromLibrary,
            new Dictionary<Property, object>()
            {
                        { Property.ISBN, translation.Isbn},
                        { Property.Category, book.GetInterests()},
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.BookId, book.Id}
            });

            LoadBooksHelper.DeleteBook(book);

            RemoveBookFromDownloadedRequestModel request = new RemoveBookFromDownloadedRequestModel(
                book.Id,
                language.ToDescription(),
                () =>
                {
                    if (book.TypeEnum == Conditions.BookContentType.Animated || book.TypeEnum == Conditions.BookContentType.Song)
                    {
                        Debug.Log("GetDownloadedBookIds - Done");
                        if (this != null)
                        {
                            DirectoryInfo di = new DirectoryInfo(LoadBooksHelper.GetDirectoryPath(book.Id, translation.CountryCode));
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                            Directory.Delete(LoadBooksHelper.GetDirectoryPath(book.Id, translation.CountryCode));
                        }

                    }
                    else
                    {
                        InitializeBookModel initializeBookModel = new InitializeBookModel(book.Id, translation.BookName, PFSforUBP.GetBookLanguage(language));
                        BooksPlayerExternal.DeleteBook(initializeBookModel);
                    }

                },
                () =>
                {
                    Debug.Log("UIDeleteButtonView => DeleteBookFromServer - Fail");
                });

            Dispatcher.Dispatch(EventGlobal.E_RemoveBookFromDownloadedCommand, request);
        }

        private void DeleteBookUIEvent(IEvent e)
        {
            string bookId = e.data.ToString();
            if (book.Id == bookId)
            {
                //invoke this event here only to update book language icon 
                Dispatcher.Dispatch(EventGlobal.E_BookDownloadEnd);
                HideButton();
            }
        }
    }
}