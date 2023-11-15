using PFS.Assets.Scripts.Models.BooksLibraryModels;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.BooksLoading
{
    public static class LoadBooksHelper
    {
        /// <summary>
        /// Local json file name (contains downloaded books)
        /// </summary>
        public const string BooksIdPath = "/booksId.txt";

        #region Path
        /// <summary>
        /// Get or create full path for book
        /// </summary>
        /// <param name="bookReference"></param>
        /// <returns></returns>
        public static string GetFullPath(string bookReference, string language)
        {
            string fullPath;

            if (!Directory.Exists(GetDirectoryPath(bookReference, language)))
            {
                Directory.CreateDirectory(GetDirectoryPath(bookReference, language));
            }
            fullPath = GetDirectoryPath(bookReference, language) + GetBundleName();
            return fullPath;
        }

        /// <summary>
        /// Get directory path for book by reference
        /// </summary>
        /// <param name="bookReference"></param>
        /// <returns></returns>
        public static string GetDirectoryPath(string bookReference, string language)
        {
            return Application.persistentDataPath + string.Format("/Books/book/book_generated/{0}_{1}/{0}_{2}_{1}", bookReference, "1.0", language);
        }

        /// <summary>
        /// Get book bundle name by platform
        /// </summary>
        /// <returns></returns>
        public static string GetBundleName()
        {
            string fileName;

#if UNITY_EDITOR
            fileName = "/pages.editor";
#elif UNITY_IOS
        fileName = "/pages.ios";
#elif UNITY_ANDROID
        fileName = "/pages.android";
#elif UNITY_STANDALONE_WIN || UNITY_WSA
        fileName = "/pages.windows";
#elif UNITY_STANDALONE_OSX
        fileName = "/pages.macos";
#endif
            return fileName;
        }
        #endregion

        #region Save Delete books
        /// <summary>
        /// Init base params after book was downloaded (by book object)
        /// </summary>
        /// <param name="booksLibrary"></param>
        /// <param name="book"></param>
        public static void SaveBook(BookModel book)
        {
            SaveBookProcess(book);
        }

        private static void SaveBookProcess(BookModel book)
        {
            book.IsDownloaded = true;
            UpdateBookDownloadStatus(book);
            MainContextView.DispatchStrangeEvent(EventGlobal.E_FinishedDownloadUnityBook, book.Id);
        }

        /// <summary>
        /// Init base params after book was deleted
        /// </summary>
        /// <param name="BooksLibrary"></param>
        /// <param name="book"></param>
        public static void DeleteBook(BookModel book)
        {

            book.IsDownloaded = false;
            if (book.BooksCollection != null)
            {
                if (book.BooksCollection == BooksLibrary.Instance.downloadedBooks)
                {
                    book.BooksCollection.books.Remove(book);
                }
            }
            else
            {
                Debug.LogError($"Book \"{ book.Name}\" not in libraries");
            }
            UpdateBookDownloadStatus(book);
            MainContextView.DispatchStrangeEvent(EventGlobal.E_DeleteBookForDescription, book.Id);
        }

        public static void DeleteBook(string bookId)
        {
            BookModel book = BooksLibrary.Instance.GetBook(bookId);
            if (book != null)
            {
                DeleteBook(book);
            }
        }
        #endregion


        public static List<Books> GetBooksFromAllCollections()
        {
            BooksLibrary booksLibrary = BooksLibrary.Instance;
            List<Books> collections = new List<Books>();
            collections.AddRange(booksLibrary.categoriesBooks);
            collections.AddRange(new Books[6] { booksLibrary.searchResultBooks,
                                           booksLibrary.downloadedBooks,
                                           booksLibrary.favoritesBooks,
                                           booksLibrary.homeworkBooks,
                                           booksLibrary.statsMostReadBooks,
                                           booksLibrary.featuredBooks });

            return collections;
        }

        /// <summary>
        /// Update isDownload status for same book in all collections. 
        /// Same book (with the same id) can be in different collections. So we need to set isDownloaded for this book in all collections.
        /// </summary>
        /// <param name="book"></param>
        public static void UpdateBookDownloadStatus(BookModel book)
        {
            List<Books> collections = GetBooksFromAllCollections();
            foreach (Books collection in collections)
            {
                BookModel b = BooksLibrary.Instance.GetBook(book.Id, collection);
                if (b != null)
                {
                    b.IsDownloaded = book.IsDownloaded;
                }
            }
        }

        /// <summary>
        /// Update isLiked state for same book in all collections. 
        /// Same book (with the same id) can be in different collections. So we need to set isLiked for this book in all collections.
        /// </summary>
        /// <param name="book"></param>
        public static void UpdateBookLikeStatus(BookModel book)
        {
            BooksLibrary booksLibrary = BooksLibrary.Instance;
            List<Books> collections = GetBooksFromAllCollections();
            foreach (Books collection in collections)
            {
                BookModel b = booksLibrary.GetBook(book.Id, collection);
                if (b != null)
                {
                    b.IsLiked = book.IsLiked;
                }
            }
        }
    }

    public class DownloadUnityBookProcess
    {
        public string bookId;
        public float downloadProcess;
    }

    public class BooksId
    {
        public List<string> downloadBooksId = new List<string>();
    }
}