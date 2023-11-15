using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Views.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridView;

namespace PFS.Assets.Scripts.Views.BooksGrid
{
    public class UIBooksGridItemView : BaseView
    {
        public const byte BooksInGridItem = 10;

        [Header("Books")]
        [SerializeField] private GridBookItem[] booksViews;

        [Header("Aspect ratio")]
        [SerializeField] private AspectRatioFitter aspectRatioFitter;

        private BookModel[] books;
        private Orientation currentOrientation = Orientation.Vertical;

        public void LoadView()
        {
            if (booksViews == null || booksViews.Length != BooksInGridItem)
            {
                Debug.LogError("UIBooksGridItemView => Wrong booksViews count");
                return;
            }

            BuildBooks(booksViews, books);

            StartCoroutine(WaitToDestroyAspectRatioFitter());
        }

        public void RemoveView()
        {

        }

        public void SetBooks(BookModel[] books, Orientation orientation)
        {
            SetBooks(books);
            currentOrientation = orientation;
        }

        private void SetBooks(BookModel[] books)
        {
            if (this.books != null && this.books.Length >= 0)
            {
                return;
            }
            this.books = books;
        }

        private void BuildBooks(GridBookItem[] views, BookModel[] booksItems)
        {
            if (booksItems == null || booksItems.Length == 0 || booksItems.Length > BooksInGridItem)
            {
                Debug.LogError("UIBooksGridItemView => BuildBooks => wrong books");
            }
            else if (booksItems.Length == BooksInGridItem)
            {
                BuildBooksNormally(views, booksItems);
            }
            else
            {
                BuildBooksCut(views, booksItems);
            }
        }

        private GridBookItem[] BuildBooksNormally(GridBookItem[] views, BookModel[] booksItems)
        {
            List<GridBookItem> newBooksViews = views.OrderByDescending(view => view.priorityByContent).ToList<GridBookItem>();

            for (int i = 0; i < newBooksViews.Count; i++)
            {
                if (i < booksItems.Length)
                {
                    newBooksViews[i].bookView.SetBook(booksItems[i], UIBookView.BookState.LateLoadImage);
                }
                else
                {
                    break;
                }
            }

            return newBooksViews.ToArray();
        }

        private void BuildBooksCut(GridBookItem[] views, BookModel[] booksItems)
        {
            List<GridBookItem> newBooksViews = new List<GridBookItem>();

            if (currentOrientation == Orientation.Horizontal)
            {
                newBooksViews = views.OrderByDescending(view => view.priorityByHorizontalGrid).ToList<GridBookItem>();
            }
            else if (currentOrientation == Orientation.Vertical)
            {
                newBooksViews = views.OrderByDescending(view => view.priorityByVerticalGrid).ToList<GridBookItem>();
            }

            for (int i = 0; i < newBooksViews.Count; i++)
            {
                newBooksViews[i].bookView.gameObject.SetActive(i < booksItems.Length);
            }

            if (newBooksViews.Count > booksItems.Length)
            {
                newBooksViews.RemoveRange(booksItems.Length, newBooksViews.Count - booksItems.Length);
            }

            BuildBooksNormally(newBooksViews.ToArray(), booksItems);
        }

        private IEnumerator WaitToDestroyAspectRatioFitter()
        {
            yield return new WaitForEndOfFrame();

            if (aspectRatioFitter)
            {
                Destroy(aspectRatioFitter);
            }
        }
    }

    [Serializable]
    public class GridBookItem
    {
        [Range(1, UIBooksGridItemView.BooksInGridItem)]
        public byte priorityByContent;

        [Range(1, UIBooksGridItemView.BooksInGridItem)]
        public byte priorityByVerticalGrid;

        [Range(1, UIBooksGridItemView.BooksInGridItem)]
        public byte priorityByHorizontalGrid;

        public UIBookView bookView;
    }
}