using Conditions;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.BooksGrid;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridItemView;

namespace PFS.Assets.Scripts.Views.DownloadedBooks
{
    public class UIFavoriteBooksView : UIBooksGridLoadingView
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI noSearchResultText;

        private int loadBooksPerPage = BooksInGridItem * 2;

        public override void LoadView()
        {
            SetScreenColliderSize();
            SetLocalization();

            InitFavoriteBooks();

            Dispatcher.AddListener(EventGlobal.E_UpdateBooksLikeStatus, InitFavoriteBooks);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.AddListener(EventGlobal.E_BooksGridHideNoMoreBooks, HideNoResultEvent);

            base.LoadView();
        }

        public override void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_UpdateBooksLikeStatus, InitFavoriteBooks);
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridHideNoMoreBooks, HideNoResultEvent);

            base.RemoveView();
        }

        public override void GetBooksFromServer()
        {
            //GetFavoritesBooksRequestModel request = new GetFavoritesBooksRequestModel(BooksLibrary.favoritesBooks.currentPage, loadBooksPerPage,
            //                                                        () =>
            //                                                        {
            //                                                            Debug.Log("GetNextFavoriteBooks - Done");
            //                                                            base.AfterLoadingLogic();
            //                                                        },
            //                                                        () => Debug.Log("UIFavoriteBooksView => GetNextBooks - Fail"));


            //Dispatcher.Dispatch(EventGlobal.E_GetFavoritesBooksCommand, request);

            GetBooksRequestModel request = new GetBooksRequestModel(BooksLibrary.favoritesBooks.currentPage,
                                                                  loadBooksPerPage,
                                                                  new List<BooksCategory>(),
                                                                  new List<SimplifiedLevels>(),
                                                                  new List<Languages>(),
                                                                  "",
                                                                  BooksRequestType.FavoritesBooks,
                                                                  () =>
                                                                  {
                                                                      base.AfterLoadingLogic();
                                                                      if (BooksLibrary.favoritesBooks.books.Count == 0)
                                                                      {
                                                                          Dispatcher.Dispatch(EventGlobal.E_BooksGridNoMoreBooks);
                                                                      }
                                                                  },
                                                                  () =>
                                                                  {
                                                                      Dispatcher.Dispatch(EventGlobal.E_BooksGridRemoveLoader);
                                                                      SetNoResultEvent();
                                                                      Debug.Log("UIBooksSearchResult => GetNextBooks - Fail");
                                                                  });

            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, request);
        }

        private void InitFavoriteBooks()
        {
            base.InitBooks(BooksLibrary.favoritesBooks, clear: true);
        }

        private void SetLocalization()
        {
            noSearchResultText.text = LocalizationManager.GetLocalizationText(NoFavouritesKey);
        }

        private void SetNoResultEvent()
        {
            SetNoResult(true);
        }

        private void HideNoResultEvent()
        {
            SetNoResult(false);
        }

        private void SetNoResult(bool visible)
        {
            noSearchResultText.gameObject.SetActive(visible);
        }

        private void SetScreenColliderSize()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider)
            {
                collider.size = GetComponent<RectTransform>()?.sizeDelta ?? Vector2.zero;
            }
        }
    }
}