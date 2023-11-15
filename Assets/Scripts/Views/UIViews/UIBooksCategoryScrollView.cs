using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Conditions;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models;
using TMPro;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.Library;

namespace PFS.Assets.Scripts.Views.MainMenu
{
    public class UIBooksCategoryScrollView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [Inject]
        public BooksLibrary BooksLibrary { get; set; }

        [Header("Options")]
        [SerializeField] private int booksPerPage = 20;
        [SerializeField, Range(0f, 5000f)] private float scrollBuffer;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI categoryTitle;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Other")]
        [SerializeField] private GameObject loader;
        private GameObject loaderGM;
        [SerializeField] private UIBookView bookItemPrefab;

        public BooksCategory ScrollCategory { get; set; }

        private bool isLoadHaveBooks = false;
        private bool isLoad;

        private Books categoryBooks;
        private GetBooksRequestModel request;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);

            InitCategoryBooks();
            SetBooksContent();
            SetScrollTitle();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetScrollTitle);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetScrollTitle);
        }

        private void Update()
        {
            CheckScrollPosition();
        }

        private void SetScrollTitle()
        {
            categoryTitle.text = child.GetBookCategory(ScrollCategory.Position).GetCategoryName();
        }

        private void InitCategoryBooks()
        {
            categoryBooks = BooksLibrary.categoriesBooks[ScrollCategory.Position];
        }

        private void CheckScrollPosition()
        {
            float contentSizeX = scrollRect.content.sizeDelta.x == 0 ? 1f : scrollRect.content.sizeDelta.x;
            float buffer = scrollBuffer / contentSizeX;


            if (scrollRect.gameObject.activeInHierarchy
            && (1f - buffer <= scrollRect.horizontalNormalizedPosition || scrollRect.content.childCount == 0)
            && isLoadHaveBooks == true
            && isLoad == false)
            {
                CheckNextBooks();
            }
        }

        private void CheckNextBooks()
        {
            int totalBooks = categoryBooks.totalBooks;

            if (totalBooks > categoryBooks.books.Count || totalBooks == -1)
            {
                isLoad = true;
                InitBookLoader();

                InitQueryToServer();
            }
        }

        private void InitQueryToServer()
        {
            if(categoryBooks == null)
            {
                Debug.LogError($"{nameof(UIBooksCategoryScrollView)} => {nameof(InitQueryToServer)} => categoryBooks is NULL");
                return;
            }

            request = new GetBooksRequestModel(categoryBooks.currentPage,
                                                booksPerPage,
                                                new List<BooksCategory>() { ScrollCategory },
                                                new List<SimplifiedLevels>(),
                                                new List<Languages>(),
                                                string.Empty,
                                                BooksRequestType.BooksByCategory,
                                                OnGetNextBooksDone,
                                                () => Debug.Log("GetNextBooks - Fail"));

            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, request);
        }

        private void OnGetNextBooksDone()
        {
            if(gameObject == null)
            {
                return;
            }

            Debug.Log("GetNextBooks - Done");
            DestroyLoader();
            AddBooksToContent();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(WaitInstanceBooks());
            }
        }


        private IEnumerator WaitInstanceBooks()
        {
            yield return new WaitForEndOfFrame();

            isLoad = false;
        }

        private void AddBooksToContent()
        {
            if (bookItemPrefab)
            {
                if (scrollRect == null)
                    return;
                Debug.Log("Add Books to Content");

                int lastBooksCount = scrollRect.content.childCount;

                if (categoryBooks.totalBooks == 0)
                {
                    gameObject.SetActive(false);
                    StopAllCoroutines();
                }
                else
                {
                    for (int i = lastBooksCount > 0 ? lastBooksCount : 0; i < categoryBooks.books.Count; i++)
                    {
                        UIBookView book = Instantiate(bookItemPrefab, scrollRect.content);
                        book.SetBook(categoryBooks.books[i], UIBookView.BookState.LateLoadImage);
                    }
                }
            }
            else
            {
                Debug.LogError("Resources.Load  BookLibraryItem == NULL");
            }
        }

        private void SetBooksContent()
        {
            if (bookItemPrefab)
            {
                if (categoryBooks.totalBooks > 0)
                {
                    DestroyLoader();

                    for (int i = 0; i < categoryBooks.books.Count; i++)
                    {
                        UIBookView book = Instantiate(bookItemPrefab, scrollRect.content);
                        book.SetBook(categoryBooks.books[i], UIBookView.BookState.LateLoadImage);
                    }
                }
                else if (categoryBooks.totalBooks == 0)
                {
                    //gameObject.SetActive(false);
                    //StopAllCoroutines();
                }
            }
            else
            {
                Debug.LogError("Resources.Load  BookLibraryItem == NULL");
            }

            isLoadHaveBooks = true;
        }

        #region Loader
        private void InitBookLoader()
        {
            if (loader)
            {
                loaderGM = Instantiate<GameObject>(loader);
                loaderGM.transform.SetParent(scrollRect.content, false);
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