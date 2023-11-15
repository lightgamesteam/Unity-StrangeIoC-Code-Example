using PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.BooksGrid;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using UnityEngine;
using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridItemView;
using static PFS.Assets.Scripts.Views.Library.UILibraryButtonView;
using static PFS.Assets.Scripts.Views.Library.UIMovePanelView;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UILibraryView : UIBooksGridLoadingView
    {
        [Header("Categories panel")]
        [SerializeField] private UINavigationLibraryView categoriesPanel;

        [Header("Books panel")]
        [SerializeField] private UIMovePanelView booksPanel;

        [Header("Featured books")]
        [SerializeField] private UIFeaturedBooksItemView featuredBooksItemView;

        private BooksLibraryByCategory booksLibraryByCategory = new BooksLibraryByCategory();

        private int loadBooksPerPage = BooksInGridItem * 2;

        public override void LoadView()
        {
            SetScreenColliderSize();

            if (InitOtherData())
            {
                SetBooksList();
                InitLibrary();

                categoriesPanel.InitStartCategory(booksLibraryByCategory.BookCategory);
                LoadFeaturedBooks();
            }

            Dispatcher.AddListener(EventGlobal.E_MoveLibraryPanel, SetEventToTopPanel);
            Dispatcher.AddListener(EventGlobal.E_LibraryCategoriesButtonClick, ChangeCategoryEvent);

            base.LoadView();
        }

        public override void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_MoveLibraryPanel, SetEventToTopPanel);
            Dispatcher.RemoveListener(EventGlobal.E_LibraryCategoriesButtonClick, ChangeCategoryEvent);

            base.RemoveView();
        }

        private void InitLibrary()
        {
            base.InitBooks(booksLibraryByCategory.BooksByCategory);
        }

        private bool InitOtherData()
        {
            if (otherData != null)
            {
                booksLibraryByCategory = otherData as BooksLibraryByCategory;
                return booksLibraryByCategory != null;
            }
            else
            {
                Debug.LogError("UILibraryView => no other data");
                return false;
            }
        }

        private void SetBooksList()
        {
            booksLibraryByCategory.SetBooks(BooksLibrary.categoriesBooks[booksLibraryByCategory.BookCategory.Position]);
        }

        public override void GetBooksFromServer()
        {
            BooksCategory last = booksLibraryByCategory.BookCategory;
            GetBooksRequestModel request = new GetBooksRequestModel(booksLibraryByCategory.BooksByCategory.currentPage,
                                                                    loadBooksPerPage,
                                                                    booksLibraryByCategory.BookCategory,
                                                                    () =>
                                                                    {
                                                                        if(this == null)
                                                                        {
                                                                            return;
                                                                        }
                                                                        Debug.Log("GetNextBooks - Done");
                                                                        if (last == booksLibraryByCategory.BookCategory)
                                                                        {
                                                                            base.AfterLoadingLogic();
                                                                        }
                                                                        else
                                                                        {
                                                                            Debug.LogFormat("UILibraryView => <color=red>Ignore load {0} category books</color>", last);
                                                                        }
                                                                    },
                                                                    () => Debug.Log("UILibraryView => GetNextBooks - Fail"));

            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, request);
        }

        private void SetEventToTopPanel(IEvent e)
        {
            ScreenState res = (ScreenState)e.data;

            if (res == ScreenState.Up)
            {
                Dispatcher.Dispatch(EventGlobal.E_LibraryCategoryToTopPanel, GlobalLibraryCategory);
            }
            else if (res == ScreenState.Down)
            {
                Dispatcher.Dispatch(EventGlobal.E_HideLibraryPanelToTopPanel);
            }
        }

        private void ChangeCategoryEvent()
        {
            booksLibraryByCategory.SetCategory(GlobalLibraryCategory);

            SetBooksList();
            InitLibrary();
            LoadFeaturedBooks();

            if (booksPanel.LastState == ScreenState.Up)
            {
                Dispatcher.Dispatch(EventGlobal.E_LibraryCategoryToTopPanel, GlobalLibraryCategory);
            }
        }

        private void SetScreenColliderSize()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider)
            {
                collider.size = GetComponent<RectTransform>()?.sizeDelta ?? Vector2.zero;
            }
        }

        private void LoadFeaturedBooks()
        {
            Action initFeaturedBlock = () =>
            {
                if (featuredBooksItemView != null && featuredBooksItemView.gameObject != null)
                {
                    featuredBooksItemView.Init();
                }
                else
                {
                    Debug.LogWarning("UILibraryView => LoadFeaturedBooks: featuredBooksItemView == null in Action");
                }
            };
            Dispatcher.Dispatch(EventGlobal.E_GetFeaturedBooksCommand, new GetFeaturedBooksModel(GlobalLibraryCategory, initFeaturedBlock, null));
        }
    }
}