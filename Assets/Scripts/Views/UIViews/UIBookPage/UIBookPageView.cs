using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Services.Localization;
using System.Collections;
using TMPro;
using UnityEngine;
using XFramework;

namespace PFS.Assets.Scripts.Views.BookPage
{
    public class UIBookPageView : BaseView
    {
        public static BookModel GetBook { get { return book; } }

        private static BookModel book = new BookModel();
        private static GameObject pageMain;

        [SerializeField] private TextMeshProUGUI finalPopupAnimatedTitle;
        [SerializeField] private TextMeshProUGUI finalPopupBackToBooks;
        [SerializeField] private TextMeshProUGUI finalPopupWatchAgain;
        [SerializeField] private TextMeshProUGUI finalPopupSongTitle;
        [SerializeField] private TextMeshProUGUI finalPopupListenAgain;
        [SerializeField] private XUIPage pageToOpen;


        private bool isPlayBook = false;
        private int timer = 0;
        private IEnumerator timerIEnum;

        public void LoadView()
        {
            GetOtherData();

            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;

            var go = Resources.Load<GameObject>("Prefabs/Page");
            pageMain = GameObject.Instantiate(go);
            pageMain.name = "Page-3D";

            XLog.Debug("path : {0}", Application.persistentDataPath);

            if (book.TypeEnum == Conditions.BookContentType.Song)
            {
                InitSongBookConfig();
            }

            Dispatcher.AddListener(EventGlobal.E_StatsStartTimerAnimatedBookCommand, StartTimer);
            Dispatcher.AddListener(EventGlobal.E_StatsStopTimerAnimatedBookCommand, StopTimer);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UIAnimatedBooksBlackScreen, isAddToScreensList = false, showSwitchAnim = false });
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            SetLocalization();


            StartCoroutine(CrontabPlay(0));

        }

        public void RemoveView()
        {
            Destroy(pageMain);

            Dispatcher.RemoveListener(EventGlobal.E_StatsStartTimerAnimatedBookCommand, StartTimer);
            Dispatcher.RemoveListener(EventGlobal.E_StatsStopTimerAnimatedBookCommand, StopTimer);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIAnimatedBooksBlackScreen);
        }

        IEnumerator CrontabPlay(float waitSeconds)
        {

            yield return new WaitForSeconds(waitSeconds);
            //yield return new WaitForSeconds(0.1f);
            pageToOpen.Open();
        }

        private void GetOtherData()
        {
            if (otherData != null)
            {
                book = otherData as BookModel;
            }
            else
            {
                Debug.LogError("Other data = null");
            }
        }

        private void StartTimer()
        {
            isPlayBook = true;
            if (timerIEnum != null)
            {
                StopCoroutine(timerIEnum);
            }
            timer = 0;
            timerIEnum = StartTimerIEnum();
            StartCoroutine(timerIEnum);
        }

        private IEnumerator StartTimerIEnum()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                timer++;
            }
        }

        private void StopTimer()
        {
            if (timerIEnum != null)
            {
                StopCoroutine(timerIEnum);
            }

            isPlayBook = false;
        }

        private void OnApplicationPause(bool pause)
        {
            if (isPlayBook)
            {
                if (pause)
                {
                    StopTimer();
                    isPlayBook = true;
                }
                else
                {
                    StartTimer();
                }
            }
        }

        private void InitSongBookConfig()
        {
            Transform xUiPageTransform = transform.Find("Page");

            if (xUiPageTransform)
            {
                XUIPage page = xUiPageTransform.GetComponent<XUIPage>();

                if (page)
                {
                    page.InitConfig();
                }
            }
        }

        private void SetLocalization()
        {
            if (finalPopupAnimatedTitle)
            {
                finalPopupAnimatedTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.AwesomAnimationKey);
            }
            if (finalPopupBackToBooks)
            {
                finalPopupBackToBooks.text = LocalizationManager.GetLocalizationText(LocalizationKeys.BackToBooksKey);
            }
            if (finalPopupWatchAgain)
            {
                finalPopupWatchAgain.text = LocalizationManager.GetLocalizationText(LocalizationKeys.WatchAgainKey);
            }
            if (finalPopupSongTitle)
            {
                finalPopupSongTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.GreatSongKey);
            }
            if (finalPopupListenAgain)
            {
                finalPopupListenAgain.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ListenAgainKey);
            }
        }
    }
}