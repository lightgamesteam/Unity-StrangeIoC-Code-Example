using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;

namespace PFS.Assets.Scripts.Views.BookLoading
{
    public class UIBookLoadingView : BaseView
    {
        static bool mInit = false;
        public Image loadingIndicator;
        public GameObject sorryPanel;
        public Button closeButton;
        public Animator loaderAnimator;

        private BookLoadModel data;

        [Header("Animation params")]
        [SerializeField] private RectTransform[] backgroundStars;
        [SerializeField, Range(0.0f, 2.0f)] private float starScaleChangingValue;
        [SerializeField, Range(0.0f, 3.0f)] private float starAnimDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float starAnimStartingDelay;

        public void LoadView()
        {
            Dispatcher.Dispatch(EventGlobal.E_UnblockStartButtons);
            GetOtherData();
            StartStarsAnimation();

#if PRODUCTION
        XLog.CloseLog(XLog.LogType.All);
#endif
            // 屏幕横竖屏的问题
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            XResourceLoader.Instance.Reset();
            XBookModel.Instance.Reset();
            XConfigModel.Instance.Reset();
            XUIModel.Instance.Reset();

            closeButton.onClick.AddListener(() =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_DeleteBookForDescription);
                    Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookLoading);
                });

            closeButton.gameObject.SetActive(true);

            LoadByMode("Page");

            if (mInit)
            {
                XPagePlatform.ClosePage();
            }
            else
            {

            }
            mInit = true;
        }

        public void RemoveView()
        {

        }

        private void GetOtherData()
        {
            if (otherData != null)
            {
                data = otherData as BookLoadModel;
            }
            else
            {
                Debug.LogError("Other data = null");
            }
        }

        void LoadByMode(string mode)
        {
            switch (mode)
            {
                case "Page":
                    StartCoroutine(LoadPage());
                    break;
                case "Game":
                    //SceneManager.LoadScene(mode);
                    break;
                default:
                    break;
            }
        }

        IEnumerator LoadPage()
        {
            yield return StartCoroutine(PlayLoaderAnimation("LoaderShowingAnimation"));

            string path = data.path;

            XConfigModel config = XConfigModel.Instance;
            //config.Init(path);
            yield return StartCoroutine(XResourceLoader.Instance.LoadAssetBundleAsync(path, loadingIndicator, sorryPanel));

            yield return StartCoroutine(PlayLoaderAnimation("LoaderHidingAnimation"));

            if (sorryPanel.activeSelf)
            {
                yield break;
            }

            Debug.Log("config.BookName = " + config.BookName);

            if (data.book.TypeEnum == Conditions.BookContentType.Song)
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBookSong, data = data.book, isAddToScreensList = false, showSwitchAnim = false });
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBookAnimated, data = data.book, isAddToScreensList = false, showSwitchAnim = false });
            }

            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookLoading);
        }

        private IEnumerator PlayLoaderAnimation(string animationName)
        {
            loaderAnimator.Play(animationName);

            yield return new WaitForSeconds(loaderAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        private void StartStarsAnimation()
        {
            foreach (RectTransform star in backgroundStars)
            {
                star.DOScale(starScaleChangingValue, starAnimDuration).SetDelay(Random.Range(0.0f, starAnimStartingDelay)).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
}