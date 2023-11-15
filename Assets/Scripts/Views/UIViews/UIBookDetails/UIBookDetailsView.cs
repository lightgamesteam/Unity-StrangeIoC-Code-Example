using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views.Buttons;
using PFS.Assets.Scripts.Views.Library;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.BookDetails
{
    public class UIBookDetailsView : BaseView
    {
        [Inject] public PoolModel Pool { get; set; }
        [Inject] public Analytics Analytics { get; private set; }

        [Header("Books panel")]
        [SerializeField] private UIBookDetailsBooksPanelView booksPanel;

        [Header("Info")]
        [SerializeField] private UIBookDetailsInfoView bookInfo;

        [Header("UI buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private UILikeButtonView likeButton;
        [SerializeField] private StartBookButtonView openBookButton;
        [SerializeField] private UIDeleteButtonView deleteBookButton;
        [SerializeField] private UIChangeLanguageButtonView changeLanguage;

        [Header("UI")]
        [SerializeField] private Image bookBackgroundImage;

        private BookModel book;

        public void LoadView()
        {
            closeButton.onClick.AddListener(CloseScreen);

            if (!GetOtherData())
            {
                return;
            }

            booksPanel.BuildBooks(book);
            likeButton.AddListener(LikeBookProcess);

            Dispatcher.AddListener(EventGlobal.E_SelectBookDetails, InitBookInfoEvent);
            Dispatcher.AddListener(EventGlobal.E_ChangeBookDetailsLanguage, ReinitBookInfoEvent);
            Dispatcher.AddListener(EventGlobal.E_BookDownloadStart, StartDownloadBookEvent);
            Dispatcher.AddListener(EventGlobal.E_BookDownloadEnd, FinishDownloadedBookEvent);
            Dispatcher.AddListener(EventGlobal.E_BookLoadProcessEnd, FinishLoadBookProcessEvent);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);


            //DisableOtherScreens(true);
            SetVisualLike(book.IsLiked);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_SelectBookDetails, InitBookInfoEvent);
            Dispatcher.RemoveListener(EventGlobal.E_ChangeBookDetailsLanguage, ReinitBookInfoEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookDownloadStart, StartDownloadBookEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookDownloadEnd, FinishDownloadedBookEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookLoadProcessEnd, FinishLoadBookProcessEvent);
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);

            //DisableOtherScreens(false);
        }


        private bool GetOtherData()
        {
            book = otherData as BookModel;

            if (book == null)
            {
                Debug.LogError("UIBookDetailsView => wrong otherData");
                return false;
            }

            return true;
        }

        private void InitBookInfoEvent(IEvent e)
        {
            SetDefaultBookParams(this.book);

            BookModel book = e.data as BookModel;

            if (book == null)
            {
                return;
            }
            this.book = book;
            InitBookInfo(this.book);
        }

        private void InitBookInfo(BookModel book)
        {
            bookInfo.SetBookInfo(book);
            SetBookBackgroundImage(book);
            changeLanguage.InitButton(book);
            deleteBookButton.InitButton(book);
            SetVisualLike(book.IsLiked);

            openBookButton.InitializeStartBookButton(book);

            Dispatcher.Dispatch(EventGlobal.E_ChangeBookDetailsCover);
        }

        private void ReinitBookInfoEvent(IEvent e)
        {
            BookModel changeLanguageBook = (BookModel)e.data;

            if (this.book == changeLanguageBook)
            {
                InitBookInfo(this.book);
            }
        }

        private void SetDefaultBookParams(BookModel book)
        {
            if (book != null)
            {
                book.InitializeCurrentTranslationLanguage();
            }
        }

        private void SetBookBackgroundImage(BookModel book)
        {
            bookBackgroundImage.color = Color.black;

            Action<Sprite, string> setImageAction = (sprite, id) =>
            {
                if (bookBackgroundImage != null && id == this.book.Id)
                {
                    bookBackgroundImage.sprite = sprite;
                    bookBackgroundImage.color = Color.white;
                }
            };

            Action setImageActionFail = () =>
            {
                if (bookBackgroundImage != null && book.Id == this.book.Id)
                {
                    bookBackgroundImage.sprite = Pool.BookBackgroundDefault;
                    bookBackgroundImage.color = Color.white;
                }
            };

            book.LoadBackgroundImage(setImageAction, setImageActionFail);
        }

        private void CloseScreen()
        {

            Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UIBookDetails, showSwitchAnim = true });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);

            Analytics.LogEvent(EventName.NavigationBookCoverExit,
                  new Dictionary<Property, object>()
                  {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.ISBN, this.book.GetTranslation().Isbn},
                    { Property.Category, this.book.GetInterests()}
                  });
        }

        private void StartDownloadBookEvent()
        {
            changeLanguage.NoInteractableButton();
            deleteBookButton.NoInteractableButton();
        }

        private void FinishDownloadedBookEvent()
        {
            changeLanguage.SetDownloadedIcon(this.book.IsDownloaded);
            deleteBookButton.SetInteracButton();
        }

        private void FinishLoadBookProcessEvent()
        {
            changeLanguage.SetInteracButton();
        }

        #region Book like
        private void LikeBookProcess()
        {
            likeButton.SetInteractable(false);

            Dispatcher.Dispatch(EventGlobal.E_SetLikeForBookToServer, new SetLikeForBookRequestModel(book.Id,
                (like) =>
                {
                    Debug.Log("Set like from server response done");
                    likeButton.SetInteractable(true);
                //set like for book
                book.IsLiked = like;
                    SetVisualLike(book.IsLiked);
                    LoadBooksHelper.UpdateBookLikeStatus(book);
                    Dispatcher.Dispatch(EventGlobal.E_UpdateBooksLikeStatus);
                },
                () =>
                {
                    Debug.LogError("Set like from server response error");
                    likeButton.SetInteractable(true);
                }));
        }

        private void SetVisualLike(bool liked)
        {
            likeButton.SetLike(liked);
        }
        #endregion

        private void ChangeGlobalLanguageEvent()
        {
            book?.InitializeCurrentTranslationLanguage();
            InitBookInfo(book);
        }


        /* 
        private readonly List<Canvas> unuseCanvases = new List<Canvas>();

        private void DisableOtherScreens(bool disable)
        {
            if (disable)
            {
                unuseCanvases.Clear();

                unuseCanvases.Add(GameObject.Find(UIScreens.UIBooksLibrary)?.GetComponent<Canvas>());
                unuseCanvases.Add(GameObject.Find(UIScreens.UIBooksSearchResult)?.GetComponent<Canvas>());
                unuseCanvases.Add(GameObject.Find(UIScreens.UIMainMenu)?.GetComponent<Canvas>());
                unuseCanvases.Add(GameObject.Find(UIScreens.UIMyProfile)?.GetComponent<Canvas>());
                unuseCanvases.Add(GameObject.Find(UIScreens.UIDownloadedBooks)?.GetComponent<Canvas>());

                foreach (Canvas go in unuseCanvases)
                {
                    if (go)
                    {
                        go.enabled = false;
                    }
                }
            }
            else
            {
                foreach (Canvas go in unuseCanvases)
                {
                    if (go)
                    {
                        go.enabled = true;
                    }
                }
            }
        }
        */
    }
}