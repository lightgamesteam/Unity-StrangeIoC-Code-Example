using Conditions;
using UnityEngine;
using System.Collections.Generic;
using System;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands;
using PFS.Assets.Scripts.Models.Requests;
using TMPro;
using PFS.Assets.Scripts.Views.BooksGrid;
using PFS.Assets.Scripts.Views.Library;
using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridItemView;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.MainMenu
{
    public class UIMainMenuView : UIBooksGridLoadingView
    {
        [Header("Titles")]
        [SerializeField] private TextMeshProUGUI booksGridTitle;

        [Header("FeaturedBooks")]
        [SerializeField] private UIFeaturedBooksItemView featuredBooksItemView;

        private int loadBooksPerPage = BooksInGridItem * 2;

        public override void LoadView()
        {
            SetLocalization();
            SetScreenColliderSize();

            //Load Furtured block
            Action initFeaturedBlock = () =>
            {
                if (featuredBooksItemView != null && featuredBooksItemView.gameObject != null)
                {
                    featuredBooksItemView.Init();
                }
            };

            Dispatcher.Dispatch(EventGlobal.E_GetFeaturedBooksCommand, new GetFeaturedBooksModel(new BooksCategory(), initFeaturedBlock, null));

            base.InitBooks(BooksLibrary.searchResultBooks, clear: true);
            base.LoadView();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public override void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);

            base.RemoveView();
        }

        public override void GetBooksFromServer()
        {
            GetBooksRequestModel request = new GetBooksRequestModel(BooksLibrary.searchResultBooks.currentPage,
                                                                    loadBooksPerPage,
                                                                    new List<BooksCategory>(),
                                                                    new List<SimplifiedLevels>(),
                                                                    new List<Languages>(),
                                                                    "",
                                                                    BooksRequestType.SearchResult,
                                                                    () =>
                                                                    {
                                                                        Debug.Log("GetNextBooks - Done");
                                                                        if (this != null)
                                                                        {
                                                                            base.AfterLoadingLogic();
                                                                        }
                                                                    },
                                                                    () => Debug.Log("UIBooksSearchResult => GetNextBooks - Fail"));

            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, request);
        }

        private void SetScreenColliderSize()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider)
            {
                collider.size = GetComponent<RectTransform>()?.sizeDelta ?? Vector2.zero;
            }
        }

        private void SetLocalization()
        {
            booksGridTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.RecommendedBooksKey);
        }
    }
}