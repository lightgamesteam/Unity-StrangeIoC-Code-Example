using PFS.Assets.Scripts.Models.BooksLibraryModels;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridItemView;

namespace PFS.Assets.Scripts.Views.BooksGrid
{
    public class UIBooksGridView : BaseView
    {
        public enum Orientation { Vertical = 0, Horizontal }

        [Header("UI")]
        [SerializeField] private ScrollRect scroll;

        [Header("Grid blocks GM")]
        [SerializeField] private GameObject gridType1;
        [SerializeField] private GameObject gridType2;

        [Header("Loader")]
        [SerializeField] private GameObject loader;
        private GameObject loaderGM;

        [Header("Params")]
        [SerializeField] private Orientation orientation = Orientation.Vertical;
        [SerializeField, Tooltip("false - gridType1 & gridType2 ||| true - gridType1")] private bool onlyGridType1 = false;
        [SerializeField, Range(0f, 5000f)] private float scrollBuffer;

        [Header("Auto init params")]
        [SerializeField] private bool onValidate = false;

        private void OnValidate()
        {
            if (onValidate)
            {
                scroll = gameObject.GetComponent<ScrollRect>() ?? gameObject.AddComponent<ScrollRect>();

                orientation = scroll.vertical ? Orientation.Vertical : Orientation.Horizontal;

                onlyGridType1 = orientation == Orientation.Horizontal;

                LoadGridsGameObjects();
                LoadLoaderGameObjects();
            }
        }

        private void LoadGridsGameObjects()
        {
            gridType1 = Resources.Load<GameObject>("Prefabs/UI/Items/BooksGrid/BooksMultiBlock1");
            gridType2 = Resources.Load<GameObject>("Prefabs/UI/Items/BooksGrid/BooksMultiBlock2");
        }

        private void LoadLoaderGameObjects()
        {
            loader = Resources.Load<GameObject>("Prefabs/UI/Items/BooksGrid/LoaderForBooksGridContent");
        }

        public void LoadView()
        {
            Dispatcher.AddListener(EventGlobal.E_BooksGridSetLoader, InitBookLoader);
            Dispatcher.AddListener(EventGlobal.E_BooksGridRemoveLoader, DestroyLoader);
            Dispatcher.AddListener(EventGlobal.E_BooksGridClearContent, ClearContent);
            Dispatcher.AddListener(EventGlobal.E_BooksGridBuildContent, SetBooksContentProcess);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridSetLoader, InitBookLoader);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridRemoveLoader, DestroyLoader);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridClearContent, ClearContent);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridBuildContent, SetBooksContentProcess);
        }

        private void Update()
        {
            CheckScrollPosition();
        }

        private void CheckScrollPosition()
        {

            if (orientation == Orientation.Vertical)
            {
                float contentSizeY = scroll.content.sizeDelta.y == 0 ? 1f : scroll.content.sizeDelta.y;
                float buffer = scrollBuffer / contentSizeY;

                if (scroll.verticalNormalizedPosition <= 0f + buffer || scroll.content.childCount == 0)
                {
                    Dispatcher.Dispatch(EventGlobal.E_BooksGridScrollEnd);
                }
            }
            else
            {
                float contentSizeX = scroll.content.sizeDelta.x == 0 ? 1f : scroll.content.sizeDelta.x;
                float buffer = scrollBuffer / contentSizeX;

                if (scroll.horizontalNormalizedPosition >= 1f - buffer || scroll.content.childCount == 0)
                {
                    Dispatcher.Dispatch(EventGlobal.E_BooksGridScrollEnd);
                }
            }
        }

        private void ClearContent()
        {
            foreach (Transform tr in scroll.content)
            {
                Destroy(tr.gameObject);
            }
        }

        private void SetBooksContentProcess(IEvent e)
        {
            List<BookModel> books = e.data as List<BookModel>;
            if (books == null)
            {
                Debug.LogError("UIBooksGridView => SetBooksContentProcess => books - null");
                return;
            }

            if (books.Count == 0)
            {
                Debug.LogWarning("No books with allowed language for show");
                return;
            }

            GameObject gridGM;
            Transform contentArea = scroll.content;

            int gridsCountLoaded = contentArea.childCount;
            int totalBooksLoaded = gridsCountLoaded * BooksInGridItem;
            int needLoadBooks = books.Count - totalBooksLoaded;
            int needLoadGrids = needLoadBooks > 0 ? (int)System.Math.Ceiling(needLoadBooks / (decimal)BooksInGridItem) : 0;
            int allGridsCountExist = gridsCountLoaded;

            bool gridType = true;

            for (int i = 0; i < needLoadGrids; i++)
            {
                gridGM = Instantiate<GameObject>(gridType ? gridType1 : gridType2, contentArea);
                var gridView = gridGM.GetComponent<UIBooksGridItemView>();
                gridView?.SetBooks(GetBooksForGrid(books, allGridsCountExist), orientation);

                allGridsCountExist++;

                if (!onlyGridType1)
                {
                    gridType = !gridType;
                }
            }
        }

        private BookModel[] GetBooksForGrid(List<BookModel> books, int allGridsCountExist)
        {
            int index = allGridsCountExist * BooksInGridItem;
            int booksCount = books.Count - allGridsCountExist * BooksInGridItem;
            int count = booksCount >= BooksInGridItem ? BooksInGridItem : booksCount;

            return books.GetRange(index, count).ToArray();
        }

        #region Loader
        private void InitBookLoader()
        {
            if (loader)
            {
                loaderGM = Instantiate<GameObject>(loader);
                loaderGM.transform.SetParent(scroll.content, false);
            }
            else
            {
                Debug.LogError("UIBooksGridView => loader - NULL");
            }
        }

        private void DestroyLoader()
        {
            if (loaderGM)
            {
                DestroyImmediate(loaderGM);
            }
        }
        #endregion
    }
}