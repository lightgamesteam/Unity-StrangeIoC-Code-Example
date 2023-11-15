using Assets.Scripts.Services.Analytics;
using BooksPlayer;
using Conditions;
using DG.Tweening;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views.DebugScreen;
using strange.extensions.dispatcher.eventdispatcher.api;
using strangeBooksPlayer.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Buttons
{
    public class StartBookButtonView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        [Header("UI")]
        [SerializeField] private Button loadButton;
        [SerializeField] private Button readMyselfButton;
        [Space(5)]
        [SerializeField] private GameObject downloadInProcess;
        [SerializeField] private TextMeshProUGUI downloadProgressText;
        [Space(5)]
        [SerializeField] private Image downloadProgress;
        [SerializeField] private Image animFillImage;
        [SerializeField] private Image playButtonImage;
        [SerializeField] private Image voiceSprite;
        [SerializeField] private RectTransform downloadProgressStartPointFill;
        [SerializeField] private RectTransform downloadProgressStartPointArea;
        [SerializeField] private RectTransform downloadProgressEndPointArea;

        [Header("Animations params")]
        [SerializeField, Range(0f, 10f)] private float preloadAnimSpeed;
        [SerializeField, Range(0.0f, 1.0f)] private float fillImageScaleDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float fillImageFadeDuration;
        [Space(10)]
        [SerializeField, Range(0.0f, 1.0f)] private float playButtonFadeDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float playButtonPunchDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float playButtonAnimPunch;
        [SerializeField, Range(0, 10)] private int playButtonPunchVibrato;
        [SerializeField, Range(0.0f, 1.0f)] private float playButtonPunchElastic;

        [Header("ReadmySelf Button parameters")]
        [SerializeField] private Color readmyselfButtonColor;
        [SerializeField] private Sprite readmyselfButtonImage;

        [Header("Autoplay Button parameters")]
        [SerializeField] private Color autoplayButtonColor;
        [SerializeField] private Sprite autoplayButtonImage;

        private BookModel book;
        private BookReadingType readingType;
        [Inject] public ChildModel ChildModel { get; private set; }
        public StartBookButtonView[] allStartBookButtons;

        private IEnumerator loadBookFromStorageIEnum;

        public void LoadView()
        {
            DisableAllOtherStartBookButtons();
            Dispatcher.AddListener(EventGlobal.E_DownloadUnityBookProgress, ProcessUnityBookProgressUpdate);
            Dispatcher.AddListener(EventGlobal.E_FinishedDownloadUnityBook, UnityBookDownloadFinished);
            Dispatcher.AddListener(EventGlobal.E_CancelDownloadNativeBook, CancelDownloadNativeBook);
            Dispatcher.AddListener(EventGlobal.E_UnblockStartButtons, UnblockButtons);
        }

        public void RemoveView()
        {
            EnableAllOtherStartBookButtons();
            Dispatcher.RemoveListener(EventGlobal.E_DownloadUnityBookProgress, ProcessUnityBookProgressUpdate);
            Dispatcher.RemoveListener(EventGlobal.E_FinishedDownloadUnityBook, UnityBookDownloadFinished);
            Dispatcher.RemoveListener(EventGlobal.E_CancelDownloadNativeBook, CancelDownloadNativeBook);
            Dispatcher.RemoveListener(EventGlobal.E_UnblockStartButtons, UnblockButtons);
            CancelDownload();
        }

        /// <summary>
        /// Assigns the book to the button to run
        /// </summary>
        /// <param name="bookModel"></param>
        public void InitializeStartBookButton(BookModel bookModel)
        {
            if (bookModel == null)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(InitializeStartBookButton)} => bookModel - null");
                return;
            }
            book = bookModel;
            InitializeFunctionality(book);
            ResetButton();
        }

        private void InitializeFunctionality(BookModel bookModel)
        {
            loadButton.onClick.RemoveAllListeners();
            readMyselfButton.onClick.RemoveAllListeners();

            //if book has readOnly mode (readmyself), then we should make autoplay the single button
            //and make it as readmyself button
            loadButton.GetComponent<Image>().color = bookModel.ReadOnly ? readmyselfButtonColor : autoplayButtonColor;
            playButtonImage.sprite = bookModel.ReadOnly ? readmyselfButtonImage : autoplayButtonImage;
            readMyselfButton.gameObject.SetActive(bookModel.ReadOnly || bookModel.TypeEnum == BookContentType.Native);
            loadButton.onClick.AddListener(() =>
            {
                StartBook(bookModel.ReadOnly ? BookReadingType.ReadMyself : BookReadingType.AutoRead);
            });

            if (bookModel.ReadOnly == true)
            {
                return;
            }

            readMyselfButton.onClick.AddListener(() =>
            {
                StartBook(BookReadingType.ReadMyself);
            });
        }

        private void ResetButton()
        {
            loadButton.interactable = true;
            playButtonImage.color = new Color(playButtonImage.color.r, playButtonImage.color.g, playButtonImage.color.b, 1.0f);
            HideVoiceIcon();
            SetDownloadAreaActive(false);
        }

        private void StartBook(BookReadingType readingType)
        {
            Debug.Log("Book Reference(Id) = " + book.Reference);

            Analytics.LogEvent(EventName.ActionBookOpen,
                new Dictionary<Property, object>()
                {
                   { Property.BookId, book.Id },
                   { Property.Uuid, PlayerPrefsModel.CurrentChildId },
                   { Property.ISBN, book.GetTranslation().Isbn },
                   { Property.Category, book.GetInterests() },
                   { Property.ReadingMode, readingType.ToDescription() },
                   { Property.Translation, book.CurrentTranslation.ToDescription() }
                });

            this.readingType = readingType;

            if (CanChildPlayTheBook() == false)
            {
                return;
            }

            if (book == null)
            {
                Debug.LogError("StartBookButtonView => StartBook => book - null");
                return;
            }

            if (book.TypeEnum == BookContentType.Native || book.TypeEnum == BookContentType.Newspaper)
            {
                InitBooksPlayer(book, this.readingType);
            }
            else if (book.TypeEnum == BookContentType.Animated || book.TypeEnum == BookContentType.Song)
            {
                StartUnityBook();
            }
        }

        private bool CanChildPlayTheBook()
        {
            ChildModel child = ChildModel?.GetChild(PlayerPrefsModel.CurrentChildId);

            if (SwitchModeModel.Mode == GameModes.SchoolModeForChildFeide && !child.IsInClass)
            {
                Dispatcher.Dispatch(EventGlobal.E_CheckSchoolAccess);
                return false;
            }

            if (SwitchModeModel.Mode == GameModes.SchoolModeForChildLogin && !child.IsInClass && child.IsSubscriptionExpired)
            {
                Action startBookAction = () => Invoke(nameof(StartBook), 1); //Automatically start to download book after all success works
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.SubscriptionScreen, data = startBookAction, isAddToScreensList = false });
                return false;
            }

            return true;
        }

        private void DisableAllOtherStartBookButtons()
        {
            allStartBookButtons = FindObjectsOfType<StartBookButtonView>();

            foreach (StartBookButtonView item in allStartBookButtons)
            {
                item.gameObject.SetActive(item == this);
            }

            if (allStartBookButtons.Length > 2)
            {
                Debug.LogError("StartBookButtonView => DisabledAllOtherStartBookButtons => potencial problem");
            }
        }

        private void EnableAllOtherStartBookButtons()
        {
            foreach (StartBookButtonView item in allStartBookButtons)
            {
                if (item != null && item.gameObject != null && item != this)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }

        private void CancelDownload()
        {
            if (book == null)
            {
                return;
            }

            if (book.IsUnityBook)
            {
                CancelDownloadUnityBook();
            }
            else
            {
                CancelDownloadNativeBook();
            }

        }

        private void CancelDownloadUnityBook()
        {
            Dispatcher.Dispatch(EventGlobal.E_CancelDownloadUnityBook, book);
        }

        private void CancelDownloadNativeBook()
        {
            BooksPlayerMainContextView.DispatchStrangeEvent(ExternalEvents.CloseBooksPlayer);
        }

        private void SetDownloadAreaActive(bool visible)
        {
            downloadInProcess.SetActive(visible);
            downloadProgressText.gameObject.SetActive(visible);
            playButtonImage.gameObject.SetActive(!visible);
        }    

        private IEnumerator StartDownloadProcessAnimation(UnityAction action)
        {
            SetVisibleDownloadFill(false);
            StartFillButtonAnimation();
            StartPlayButtonAnimation();
            yield return new WaitForSeconds(playButtonPunchDuration);
            SetVisibleDownloadFill(true);
            action?.Invoke();
        }

        private void SetVisibleDownloadFill(bool visible)
        {
            if (downloadProgress != null && downloadProgress.gameObject != null)
            {
                downloadProgress.gameObject.SetActive(visible);
            }
            if (downloadProgressEndPointArea != null && downloadProgressEndPointArea.gameObject != null)
            {
                downloadProgressEndPointArea.gameObject.SetActive(visible);
            }
            if (downloadProgressStartPointArea != null && downloadProgressStartPointArea.gameObject != null)
            {
                downloadProgressStartPointArea.gameObject.SetActive(visible);
            }
            if (downloadProgressText != null)
            {
                downloadProgressText.text = visible ? downloadProgressText.text : string.Empty;
            }
        }

        private void SetButtonLoadingState(bool downloaded)
        {
            SetDownloadAreaActive(!downloaded);
            SetButtonsInteractableState(downloaded);
        }

        private void SetButtonsInteractableState(bool isInteractable)
        {
            loadButton.interactable = isInteractable;
            readMyselfButton.interactable = isInteractable;
            Debug.Log("----------------SetButtonsInteractableState:  isInteractable = " + isInteractable);
        }

        private void UnblockButtons()
        {
            SetButtonsInteractableState(true);
        }

        private void ProgressUpdated(float progress)
        {
            ProgressLogic(progress, downloadProgress, downloadProgressStartPointFill, downloadProgressStartPointArea);
            if (downloadProgressText != null)
            {
                downloadProgressText.text = (int)(progress * 100) + "%";
            }
        }

        private void ProgressLogic(float progress, Image fillProgress, RectTransform startFill, RectTransform startArea)
        {
            if (progress < 0 || progress > 1)
            {
                Debug.LogError("StartBookButtonView => ProgressLogic => progress error - " + progress);
                return;
            }

            fillProgress.fillAmount = progress;

            float angle = progress * 360f;
            startFill.localEulerAngles = new Vector3(0, 0, angle);
            startArea.localEulerAngles = new Vector3(0, 0, -angle);
        }       

        #region Animation
        private void StartFillButtonAnimation()
        {
            if (animFillImage == null)
            {
                return;
            }
            animFillImage.color = new Color(animFillImage.color.r, animFillImage.color.g, animFillImage.color.b, 0.34f);
            animFillImage.transform.localScale = Vector3.zero;

            animFillImage.transform.DOScale(1.0f, fillImageScaleDuration);
            animFillImage.DOFade(0.0f, fillImageFadeDuration).SetDelay(fillImageScaleDuration);
        }

        private void StartPlayButtonAnimation()
        {
            if (playButtonImage == null)
            {
                return;
            }
            playButtonImage.color = new Color(playButtonImage.color.r, playButtonImage.color.g, playButtonImage.color.b, 1f);
            playButtonImage.transform.localScale = Vector3.one;

            playButtonImage.transform.DOPunchScale(new Vector3(playButtonAnimPunch, playButtonAnimPunch, 1.0f), playButtonPunchDuration, playButtonPunchVibrato, playButtonPunchElastic);
            playButtonImage.DOFade(0.0f, playButtonFadeDuration).SetDelay(playButtonPunchDuration);
        }
        #endregion

        #region UnityBook

        private void StartUnityBook()
        {
            if (book.IsDownloaded)
            {
                OpenUnityBook();
                return;
            }
            StartUnityBookDownloadProcess();
        }

        private void OpenUnityBook()
        {
            StartCoroutine(OpenUnityBookCoroutine());
        }

        private IEnumerator OpenUnityBookCoroutine()
        {
            SetButtonsInteractableState(false);
            yield return new WaitForSecondsRealtime(0.1f);
            Dispatcher.Dispatch(EventGlobal.E_SoundPauseMusic);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel
            {
                screenName = UIScreens.UIBookLoading,
                data = new BookLoadModel { book = book, path = LoadBooksHelper.GetFullPath(book.Id, book.GetTranslation().CountryCode) },
                isAddToScreensList = false,
                showSwitchAnim = false
            });
        }

        private void StartUnityBookDownloadProcess()
        {
            Dispatcher.Dispatch(EventGlobal.E_BookDownloadStart);

            ProgressUpdated(0);
            SetButtonLoadingState(false);

            UnityAction action = () =>
            {
                Dispatcher.Dispatch(EventGlobal.E_DownloadUnityBook, book);
            };

            StartCoroutine(StartDownloadProcessAnimation(action));
        }

        private void ProcessUnityBookProgressUpdate(IEvent e)
        {
            if (book == null)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(ProcessUnityBookProgressUpdate)} => book = null");
                return;
            }

            DownloadUnityBookProcess progress = e.data as DownloadUnityBookProcess;

            if (progress == null)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(ProcessUnityBookProgressUpdate)} => progress = null");
                return;
            }

            if (book.Id != progress.bookId)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(ProcessUnityBookProgressUpdate)} => wrong book id");
                return;
            }

            ProgressUpdated(progress.downloadProcess);
        }

        private void UnityBookDownloadFinished(IEvent e)
        {
            if (book == null)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(UnityBookDownloadFinished)} => book = null");
                return;
            }

            if(book.IsUnityBook == false)
            {
                return;
            }

            string bookId = e.data.ToString();

            if (book.Id != bookId)
            {
                Debug.LogError($"{nameof(StartBookButtonView)} => {nameof(ProcessUnityBookProgressUpdate)} => wrong book id");
                return;
            }

            StartCoroutine(ProcessUnityBookDownloadFinished());
        }

        private IEnumerator ProcessUnityBookDownloadFinished()
        {
            playButtonImage.DOFade(1.0f, playButtonFadeDuration);

            yield return new WaitForSeconds(playButtonFadeDuration);

            Dispatcher.Dispatch(EventGlobal.E_BookDownloadEnd);
            Dispatcher.Dispatch(EventGlobal.E_BookLoadProcessEnd);

            SetButtonLoadingState(true);

            Dispatcher.Dispatch(EventGlobal.E_StatsDownloadBookCommand, new DownloadedOpenedBookRequestModel(bookId: book.Id,
                                                                                                       homeworkId: book.HomeworkId,
                                                                                                       languageName: book.CurrentTranslation.ToDescription(),
                                                                                                       requestTrueAction: () => Debug.Log("Book download - done"),
                                                                                                       requstFalseAction: () => Debug.LogError("Book download - fail"),
                                                                                                       waitResponse: true));

            OpenUnityBook();
        }

        #endregion

        #region UBP

        public void InitBooksPlayer(BookModel bookModel, BookReadingType bookReadingType)
        {
            Debug.Log("Book Id = " + bookModel.Id + "");


            GameObject booksPlayerPrefab = Resources.Load<GameObject>("BooksPlayer/BookRoot/BooksPlayer");

            if (booksPlayerPrefab == null)
            {
                return;
            }

            GameObject booksPlayerInstance = Instantiate(booksPlayerPrefab);
            booksPlayerInstance.name = booksPlayerPrefab.name;

            BookModel.Translation bookTranslation = bookModel.GetTranslation();

            InitializeBookModel book = new InitializeBookModel(id: bookModel.Id,
                                                               bookName: bookTranslation.BookName,
                                                               bookLanguage: PFSforUBP.GetBookLanguage(bookModel.CurrentTranslation),
                                                               bookDataUrl: bookTranslation.UrlLanguageFile,
                                                               bookImagesUrl: bookTranslation.UrlCommunFile,
                                                               playRegime: PFSforUBP.GetReadType(bookReadingType, bookModel.ReadOnly),
                                                               bookObject: bookModel,
                                                               delayForFinalPopup: !string.IsNullOrEmpty(bookModel.QuizId));
            booksPlayerInstance.GetComponent<BooksPlayerMain>().BookInfo = book;

            StartCoroutine(WaitDispatcher());
        }

        private IEnumerator WaitDispatcher()
        {
            yield return new WaitUntil(() => BooksPlayerMainContextView.StrangeDispatcher != null);

            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.CloseBooksPlayer, CloseBooksPlayer);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.BackToBooks, BackToBooksEvent);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.CloseButtonClick, CloseButtonClickEvent);

            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.StartLoadBook, NativeBookLoadStarted);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.LoadBookProgressFromServer, LoadNativeBookProgressFromServer);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.LoadBookProgressFromStorage, LoadNativeBookProgressFromStorage);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.FinishLoadBook, FinishLoadBookEvent);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.BookStartedPlay, BookStartedPlayEvent);
            BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookFinished, BookFinishedTrackingEvent);

            Dispatcher.Dispatch(EventGlobal.E_StartNativeBookTracking);

            DownloadBookDataCommand.LoadListenersReady = true;
        }

        private void CloseBooksPlayer()
        {
            RemoveBookPlayerListeners();

            // reset all buttons if book crashed
            Dispatcher.Dispatch(EventGlobal.E_BookLoadProcessEnd);
            StopLoadNativeBookFromStorageAnimation();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ProcessLoadFinished());
            }
        }

        private void RemoveBookPlayerListeners()
        {
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.CloseBooksPlayer, CloseBooksPlayer);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.BackToBooks, BackToBooksEvent);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.CloseButtonClick, CloseButtonClickEvent);

            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.StartLoadBook, NativeBookLoadStarted);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.LoadBookProgressFromServer, LoadNativeBookProgressFromServer);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.LoadBookProgressFromStorage, LoadNativeBookProgressFromStorage);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.FinishLoadBook, FinishLoadBookEvent);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalEvents.BookStartedPlay, BookStartedPlayEvent);
            BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookFinished, BookFinishedTrackingEvent);
        }

        private void NativeBookLoadStarted()
        {
            Debug.Log("Books player -> StartLoadBook event");

            Dispatcher.Dispatch(EventGlobal.E_BookDownloadStart);
            SetButtonLoadingState(false);
            StartCoroutine(StartDownloadProcessAnimation(null));
        }

        private void LoadNativeBookProgressFromServer(IEventBooksPlayer e)
        {
            float.TryParse(e.data.ToString(), out float progress);
            ProgressUpdated(progress / 100f * 0.8f);
        }

        private void LoadNativeBookProgressFromStorage()
        {
            if (book.IsDownloaded == false)
            {
                LoadBooksHelper.SaveBook(book);
            }

            StopLoadNativeBookFromStorageAnimation();
            loadBookFromStorageIEnum = LoadNativeBookFromStorageAnimation();
            StartCoroutine(loadBookFromStorageIEnum);
        }

        private void StopLoadNativeBookFromStorageAnimation()
        {
            if (loadBookFromStorageIEnum == null)
            {
                return;
            }
            StopCoroutine(loadBookFromStorageIEnum);
        }

        private IEnumerator LoadNativeBookFromStorageAnimation()
        {
            float progress = 0.8f;
            while (progress <= 1)
            {
                ProgressUpdated(progress);
                progress += 0.8f / (playButtonPunchDuration * 200);
                yield return new WaitForSeconds(playButtonPunchDuration / 200);
            }
            downloadProgressText.text = "100%";

            ShowVoiceIcon();
        }

        private void FinishLoadBookEvent()
        {
            Debug.Log("Books palyer -> Finish download event");

            Dispatcher.Dispatch(EventGlobal.E_BookDownloadEnd);
        }

        private void BookStartedPlayEvent()
        {
            Debug.Log("Books palyer -> Book started event");

            Dispatcher.Dispatch(EventGlobal.E_BookLoadProcessEnd);
            StopLoadNativeBookFromStorageAnimation();
            StartCoroutine(ProcessLoadFinished());
        }

        private IEnumerator ProcessLoadFinished()
        {
            playButtonImage.DOFade(1.0f, playButtonFadeDuration);
            HideVoiceIcon();

            yield return new WaitForSeconds(playButtonFadeDuration);

            SetButtonLoadingState(true);
        }

        private void ShowVoiceIcon()
        {
            SetDownloadAreaActive(false);
            voiceSprite.gameObject.SetActive(true);
            voiceSprite.rectTransform.DOLocalRotate(new Vector3(0, 0, -360 * 10), 100);
        }
        private void HideVoiceIcon()
        {
            voiceSprite.color = Color.white;
            voiceSprite.gameObject.SetActive(false);
            DOTween.Kill(voiceSprite.rectTransform);
            voiceSprite.rectTransform.localRotation = Quaternion.identity;
        }

        private void BackToBooksEvent()
        {
            BooksPlayerMainContextView.DispatchStrangeEvent(ExternalEvents.CloseBooksPlayer);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UIBookDetails, showSwitchAnim = true });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void CloseButtonClickEvent()
        {
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            Analytics.LogEvent(EventName.ActionBookClose,
                 new Dictionary<Property, object>()
                 {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.ISBN, book.GetTranslation().Isbn},
                        { Property.BookId, book.Id },
                        { Property.Category, book.GetInterests()}
                 });
        }

        #endregion

        private IEnumerator WaitToClose()
        {
            yield return new WaitForEndOfFrame();
            BooksPlayerMainContextView.DispatchStrangeEvent(ExternalEvents.CloseBooksPlayer);
        }

        private void BookFinishedTrackingEvent(IEventBooksPlayer e)
        {
            if (e.data == null)
            {
                Debug.LogError("UBP Tracking => BookFinishedTrackingEvent - data null");
                return;
            }

            BookFinishedTrackingModel model = e.data as BookFinishedTrackingModel;

            Debug.Log($"UBP Tracking => BookFinishedTrackingEvent => book id - {model.BookId} | book regime - {model.BookRegime} | book time - {model.BookTime}");

            BookModel bookModel = model.BookModel as BookModel;
            if (bookModel == null)
            {
                bookModel = new BookModel();
            }

            Dispatcher.Dispatch(EventGlobal.E_StatsFinishedBookCommand, new FinishedBookRequestModel(model.BookId,
                                                                                                bookModel.HomeworkId,
                                                                                                bookModel.CurrentTranslation.ToDescription(),
                                                                                                () =>
                                                                                                {
                                                                                                    Debug.Log("Book finished - done");

                                                                                                    if (!string.IsNullOrEmpty(bookModel.QuizId))
                                                                                                    {
                                                                                                        Dispatcher.Dispatch(EventGlobal.E_ShowQuizSplashScreen, bookModel.CurrentTranslation);
                                                                                                        StartCoroutine(WaitToClose());
                                                                                                        //BooksPlayerMainContextView.DispatchStrangeEvent(ExternalEvents.CloseBooksPlayer);

                                                                                                        Dispatcher.Dispatch(EventGlobal.E_GetQuizFromServer, new QuizRequestModel(model.BookId,
                                                                                                                                                                                      DebugView.QuizQuestionsType,
                                                                                                                                                                                      bookModel.HomeworkId,
                                                                                                                                                                                      bookModel.CurrentTranslation.ToDescription(),
                                                                                                                                                                                      () =>
                                                                                                                                                                                      {
                                                                                                                                                                                          Debug.Log("Get quiz done");
                                                                                                                                                                                          Analytics.LogEvent(EventName.NavigationQuizOpen,
                                                                                                                                                                                                new System.Collections.Generic.Dictionary<Property, object>()
                                                                                                                                                                                                {
                                                                                                                                                                                                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                                                                                                                                                                                                    { Property.QuizId, bookModel.QuizId},
                                                                                                                                                                                                    { Property.ISBN, bookModel.GetTranslation().Isbn},
                                                                                                                                                                                                    { Property.Category, bookModel.GetInterests()}
                                                                                                                                                                                                });
                                                                                                                                                                                      },
                                                                                                                                                                                      () => Debug.LogError("Get quiz fail")));
                                                                                                    }
                                                                                                    else if (!string.IsNullOrEmpty(bookModel.HomeworkId) && bookModel.HideQuiz)
                                                                                                    {
                                                                                                        Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
                                                                                                        Dispatcher.Dispatch(EventGlobal.E_HideScreen, "UILoadingScreen");
                                                                                                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIHomeworks });
                                                                                                    }
                                                                                                },
                                                                                                () => Debug.LogError("Book finished - fail")));
        }

    }
}