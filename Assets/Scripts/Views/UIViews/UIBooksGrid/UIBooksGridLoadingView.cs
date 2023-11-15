using PFS.Assets.Scripts.Models.BooksLibraryModels;
using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.BooksGrid
{
    public class UIBooksGridLoadingView : BaseView
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        /// <summary>
        /// Wait loading existing book from library model
        /// </summary>
        private bool isLoadedExistingBooks = false;

        /// <summary>
        /// Wait loading new books from server
        /// </summary>
        private bool isLoadingFromServer = false;

        private bool noMoreBooksEventDone = false;

        private Books books;

        public virtual void LoadView()
        {
            Dispatcher.AddListener(EventGlobal.E_BooksGridScrollEnd, CheckContentLoad);
        }

        public virtual void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridScrollEnd, CheckContentLoad);
        }

        public void InitBooks(Books books, bool clear = false)
        {
            this.books = books;

            if (clear)
            {
                books?.Clear();
                BooksLibrary.nonAllowedLanguageBooks?.books?.Clear();
            }

            //set loads false
            isLoadedExistingBooks = false;
            isLoadingFromServer = false;

            //set books have
            SetBooksContent(books);
        }

        private void SetBooksContent(Books books)
        {
            Dispatcher.Dispatch(EventGlobal.E_BooksGridClearContent);

            StartCoroutine(WaitClearingContent(books));
        }

        private IEnumerator WaitClearingContent(Books books)
        {
            yield return new WaitForEndOfFrame();

            Dispatcher.Dispatch(EventGlobal.E_BooksGridBuildContent, books?.books);

            StartCoroutine(WaitInstantiateBooks());
        }

        private IEnumerator WaitInstantiateBooks()
        {
            yield return new WaitForEndOfFrame();

            //load books done
            isLoadedExistingBooks = true;
        }

        private void CheckContentLoad()
        {
            if (isLoadedExistingBooks == true && isLoadingFromServer == false)
            {
                CheckNextBooks();
            }
        }

        private void CheckNextBooks()
        {
            if (books == null)
            {
                Debug.LogError("UIBooksGridLoadingView => CheckNextBooks => books - null");
                return;
            }

            //Counting summ of all books and stop download Coroutine
            int allovedAndNonAllowedBooks = BooksLibrary.nonAllowedLanguageBooks.books.Count + books.books.Count;
            int totalBooks = books.totalBooks;
            if (totalBooks > allovedAndNonAllowedBooks || totalBooks == -1)
            {
                isLoadingFromServer = true;
                noMoreBooksEventDone = false;
                Dispatcher.Dispatch(EventGlobal.E_BooksGridHideNoMoreBooks);
                Dispatcher.Dispatch(EventGlobal.E_BooksGridSetLoader);

                GetBooksFromServer();
            }
            else
            {
                if (!noMoreBooksEventDone && totalBooks == 0)
                {
                    noMoreBooksEventDone = true;
                    Dispatcher.Dispatch(EventGlobal.E_BooksGridNoMoreBooks);
                }
            }
        }

        public virtual void GetBooksFromServer()
        {

        }

        public void AfterLoadingLogic()
        {
            if (this != null)
            {
                Dispatcher.Dispatch(EventGlobal.E_BooksGridRemoveLoader);
                Dispatcher.Dispatch(EventGlobal.E_BooksGridBuildContent, books?.books);

                StartCoroutine(WaitInstanceBooks());
            }
        }

        private IEnumerator WaitInstanceBooks()
        {
            yield return new WaitForEndOfFrame();

            //load process done
            isLoadingFromServer = false;
        }
    }
}