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
    public class UIDownloadedBooksView : UIBooksGridLoadingView
    {

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI noSearchResultText;

        private readonly int loadBooksPerPage = BooksInGridItem * 2;

        public override void LoadView()
        {
            SetScreenColliderSize();
            SetLocalization();

            // if (InitOtherData())
            {
                base.InitBooks(BooksLibrary.downloadedBooks, clear: true);
            }


            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.AddListener(EventGlobal.E_UpdateDownloadedBooks, ProcessDownloadedBooksUpdate);

            base.LoadView();
        }

        public override void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_BooksGridNoMoreBooks, SetNoResultEvent);
            Dispatcher.RemoveListener(EventGlobal.E_UpdateDownloadedBooks, ProcessDownloadedBooksUpdate);

            base.RemoveView();
        }

        //private bool InitOtherData()
        //{
        //    if (otherData != null)
        //    {
        //        filter = otherData as BooksLibraryFilter;
        //        return filter != null;
        //    }
        //    else
        //    {
        //        Debug.LogError("BooksSearchResult => filter = null");
        //        return false;
        //    }
        //}

        public override void GetBooksFromServer()
        {
            GetBooksRequestModel request = new GetBooksRequestModel(BooksLibrary.downloadedBooks.currentPage,
                                                                   loadBooksPerPage,
                                                                   new List<BooksCategory>(),
                                                                   new List<SimplifiedLevels>(),
                                                                   new List<Languages>(),
                                                                   "",
                                                                   BooksRequestType.DownloadedBooks,
                                                                   () =>
                                                                   {
                                                                       Debug.Log("GetNextBooks - Done");
                                                                       base.AfterLoadingLogic();
                                                                       if (BooksLibrary.downloadedBooks.books.Count == 0)
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


        private void ProcessDownloadedBooksUpdate()
        {
            base.InitBooks(BooksLibrary.downloadedBooks, clear: true);
        }

        private void SetLocalization()
        {
            noSearchResultText.text = LocalizationManager.GetLocalizationText(NoDownloadsKey);
        }

        private void SetNoResultEvent()
        {
            SetNoResult(true);
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