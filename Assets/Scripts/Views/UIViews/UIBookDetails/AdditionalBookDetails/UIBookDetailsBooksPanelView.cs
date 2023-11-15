using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Views.BookDetails;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UIBookDetailsBooksPanelView : BaseView
    {
        [Header("UI")]
        [SerializeField] private ScrollRect scroll;

        [Header("Book item for details screen")]
        [SerializeField] private GameObject bookGM;

        public void LoadView()
        {

        }

        public void RemoveView()
        {

        }

        public void BuildBooks(BookModel book)
        {
            if (book == null)
            {
                Debug.LogError("UIBookDetailsBooksPanelView => BuildBooks - book error");
            }

            GameObject bookInst;
            for (int i = 0; i < book.BooksCollection.books.Count; i++)
            {
                bookInst = Instantiate(bookGM, scroll.content);
                var bookView = bookInst.GetComponent<UIBookDetailsItemView>();
                bookView?.SetBook(book.BooksCollection.books[i]);

                if (book.Id == book.BooksCollection.books[i].Id)
                {
                    bookView?.InvokeButton();
                    StartCoroutine(WaitScrollContent(i));
                }
            }
        }

        private IEnumerator WaitScrollContent(int pos)
        {
            yield return new WaitForEndOfFrame();
            SetButtonsScrollPosition(pos);
        }

        private void SetButtonsScrollPosition(int position)
        {
            int childCount = scroll.content.childCount - 2;
            float oneChildScrollSize = 1f / childCount;

            scroll.horizontalNormalizedPosition = position * oneChildScrollSize;
        }
    }
}