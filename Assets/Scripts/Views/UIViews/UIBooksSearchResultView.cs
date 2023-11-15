using Conditions;
using UnityEngine;
using TMPro;
using strange.extensions.dispatcher.eventdispatcher.api;

using static PFS.Assets.Scripts.Views.BooksGrid.UIBooksGridItemView;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.BooksGrid;

namespace PFS.Assets.Scripts.Views.BooksSearch
{
    public class UIBooksSearchResultView : UIBooksGridLoadingView
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI noSearchResultText;

        private int loadBooksPerPage = BooksInGridItem * 2;
        private BooksLibraryFilter filter;

        public override void LoadView()
        {
            SetScreenColliderSize();
            SetLocalization();

            if (InitOtherData())
            {
                base.InitBooks(BooksLibrary.searchResultBooks, clear: true);
            }

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.AddListener(EventGlobal.E_UpdateBooksSearchResults, UpdateBooksSearchResults);

            base.LoadView();
        }

        public override void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.RemoveListener(EventGlobal.E_UpdateBooksSearchResults, UpdateBooksSearchResults);

            base.RemoveView();
        }

        private bool InitOtherData()
        {
            if (otherData != null)
            {
                filter = otherData as BooksLibraryFilter;
                return filter != null;
            }
            else
            {
                Debug.LogError("BooksSearchResult => filter = null");
                return false;
            }
        }

        public override void GetBooksFromServer()
        {
            SetNoResult(false);

            GetBooksRequestModel request = new GetBooksRequestModel(BooksLibrary.searchResultBooks.currentPage,
                                                                    loadBooksPerPage,
                                                                    filter.interests,
                                                                    filter.stages,
                                                                    filter.languages,
                                                                    filter.searchTitle,
                                                                    BooksRequestType.SearchResult,
                                                                    () =>
                                                                    {
                                                                        Debug.Log("GetNextBooks - Done");
                                                                        BooksLibrary.searchResultBooks.books.ForEach(x => x.IsBookFromSearch = true);
                                                                        base.AfterLoadingLogic();

                                                                    },
                                                                    () => 
                                                                    { 
                                                                        Debug.Log("UIBooksSearchResult => GetNextBooks - Fail");
                                                                        SetNoResult(true);
                                                                        Dispatcher.Dispatch(EventGlobal.E_BooksGridRemoveLoader);
                                                                    });

            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, request);
        }

        private void UpdateBooksSearchResults(IEvent eventData)
        {
            if (eventData != null && eventData.data != null)
            {
                filter = eventData.data as BooksLibraryFilter;

                SetNoResult(false);

                base.InitBooks(BooksLibrary.searchResultBooks, clear: true);
            }
        }

        private void SetLocalization()
        {
            if (noSearchResultText != null)
            {
                noSearchResultText.text = LocalizationManager.GetLocalizationText(NoSearchResultKey);
            }
        }

        private void SetNoResultEvent()
        {
            SetNoResult(true);
        }

        private void SetNoResult(bool visible)
        {
            if (noSearchResultText != null && noSearchResultText.gameObject != null)
            {
                noSearchResultText.gameObject.SetActive(visible);
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
    }
}