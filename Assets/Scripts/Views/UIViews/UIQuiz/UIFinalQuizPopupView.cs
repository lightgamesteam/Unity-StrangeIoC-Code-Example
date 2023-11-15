using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIFinalQuizPopupView : BaseView
    {
        [Inject]
        public ChildModel ChildModel { get; set; }


        public GameObject birdObject;
        public TextMeshProUGUI smallResultText;
        public TextMeshProUGUI largeResultText;
        public TextMeshProUGUI buttonDoneText;
        public TextMeshProUGUI textCoins;
        public TextMeshProUGUI haveEarnedText;

        public Button buttonDone;
        [Space(5)]
        public Image[] imageEmptyStars;
        public Image[] imageFilledStars;
        [Space(5)]
        public Sprite[] starsSprite;


        [Header("Animation params")]
        [SerializeField] private Transform mainBackground;
        [SerializeField, Range(0.0f, 1.0f)] private float mainBackAnimDuration;
        [Space(10)]
        [SerializeField] private Transform resultsBackground;
        [SerializeField, Range(0.0f, 1.0f)] private float resultsBackAnimDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float resultsBackAnimDelay;
        [Space(10)]
        [SerializeField] private Transform birdImage;
        [SerializeField, Range(0.0f, 1.0f)] private float birdImageAnimDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float birdImageAnimScaleDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float birdImageAnimPunchDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float birdImageAnimPunch;
        [SerializeField, Range(0, 10)] private int birdImageAnimPunchVibrato;
        [SerializeField, Range(0.0f, 1.0f)] private float birdImageAnimPunchElastic;
        [Space(10)]
        [SerializeField] private Transform[] stars;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimStartDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimBetweenDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimScaleDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimPunchDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimPunch;
        [SerializeField, Range(0, 10)] private int starsAnimPunchVibrato;
        [SerializeField, Range(0.0f, 1.0f)] private float starsAnimPunchElastic;
        [Space(10)]
        [SerializeField] private Transform awardArea;
        [SerializeField, Range(0.0f, 1.0f)] private float awardAreaAnimDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float awardAreaAnimDuration;
        [Space(10)]
        [SerializeField] private Transform awardInfo;
        [SerializeField, Range(0.0f, 1.0f)] private float awardInfoAnimDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float awardInfoAnimDuration;
        [Space(10)]
        [SerializeField] private Transform doneButton;
        [SerializeField, Range(0.0f, 1.0f)] private float doneButtonAnimDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float doneButtonAnimDuration;
        [Space(10)]
        [SerializeField] private Transform[] resultTexts;
        [SerializeField, Range(0.0f, 1.0f)] private float resultTextAnimDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float resultTextAnimDuration;
        [Space(10)]
        [SerializeField, Range(0.0f, 1.0f)] private float receivedStarAnimStartDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float receivedStarAnimBtwDelay;
        [SerializeField, Range(0.0f, 1.0f)] private float receivedStarAnimDuration;
        [SerializeField, Range(0.0f, 3.0f)] private float receivedStarFromScale;
        [SerializeField] private GameObject receivedStarEffect;
        [Space(10)]
        [SerializeField] private GameObject winEffect;
        [SerializeField] private RectTransform winEffectArea;
        [SerializeField] private float winEffectDelay;

        private QuizResultFromServer quizResultFromServer;
        private const int StarsToShowBird = 3;

        public void LoadView()
        {
            buttonDone.onClick.AddListener(DoneProcess);

            if (otherData == null)
            {
                Debug.LogError("UIFinalQuizPopupView - No other data");
                return;
            }

            quizResultFromServer = otherData as QuizResultFromServer;

            if (quizResultFromServer == null)
            {
                Debug.LogError("UIFinalQuizPopupView => quizResultFromServer - null");
                return;
            }

            ShowPoints();
            SetResultText();
            SetLocalization();

            StartShowingAnimation();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void DoneProcess()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIFinalQuizPopup);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIQuiz);
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);

            if (quizResultFromServer.isHomework)
            {
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, UIScreens.UIHomeworks);
            }
        }

        private void ShowPoints()
        {
            textCoins.text = quizResultFromServer.coins.ToString();

            SaveChange();
        }

        private void SetResultText()
        {
            bool isShowBirdAndSmallText = quizResultFromServer.stars >= StarsToShowBird;

            birdObject.SetActive(isShowBirdAndSmallText);
            smallResultText.gameObject.SetActive(isShowBirdAndSmallText);
            largeResultText.gameObject.SetActive(!isShowBirdAndSmallText);
        }

        private void SetLocalization()
        {
            buttonDoneText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DoneKey, quizResultFromServer.QuizLanguage.ToString());
            haveEarnedText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.YouHaveEarnedKey, quizResultFromServer.QuizLanguage.ToString());

            TextMeshProUGUI enabledText = quizResultFromServer.stars >= StarsToShowBird ? smallResultText : largeResultText;

            if (quizResultFromServer.stars > StarsToShowBird)
            {
                enabledText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.GreatJobKey, quizResultFromServer.QuizLanguage.ToString());
            }
            else if (quizResultFromServer.stars == StarsToShowBird)
            {
                enabledText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.WellDoneKey, quizResultFromServer.QuizLanguage.ToString());
            }
            else
            {
                enabledText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.KeepTryingKey, quizResultFromServer.QuizLanguage.ToString());
            }
        }

        private void SaveChange()
        {
            ChildEditRequestModel childRequest = new ChildEditRequestModel();
            childRequest.InitData(ChildModel.GetChild(PlayerPrefsModel.CurrentChildId));



            childRequest.requestTrueAction = () =>
            {
                Debug.Log("Child edit succes");
            };
            childRequest.requestFalseAction = () =>
            {
                Debug.Log("Child edit fail");
            };
            Dispatcher.Dispatch(EventGlobal.E_EditChild, childRequest);
        }

        #region Animation

        private void StartShowingAnimation()
        {
            SetAnimScale(mainBackground, 0.0f, mainBackAnimDuration);
            SetAnimScale(resultsBackground, resultsBackAnimDelay, resultsBackAnimDuration);

            SetAnimScale(birdImage, birdImageAnimDelay, birdImageAnimScaleDuration);
            birdImage.DOPunchScale(new Vector3(birdImageAnimPunch, birdImageAnimPunch, 1.0f), birdImageAnimPunchDuration, birdImageAnimPunchVibrato, birdImageAnimPunchElastic).SetDelay(birdImageAnimScaleDuration + birdImageAnimDelay);

            StartCoroutine(StartStarsAnimation());

            SetAnimScale(awardArea, awardAreaAnimDelay, awardAreaAnimDuration);
            SetAnimScale(awardInfo, awardInfoAnimDelay, awardInfoAnimDuration);
            SetAnimScale(doneButton, doneButtonAnimDelay, doneButtonAnimDuration);

            StartCoroutine(StartReceivedStarsAnimation());
            StartCoroutine(InitWinEffect());

            foreach (var filledStar in imageFilledStars)
            {
                filledStar.transform.localScale = new Vector3(receivedStarFromScale, receivedStarFromScale);
                filledStar.color = new Color(filledStar.color.r, filledStar.color.g, filledStar.color.b, 0.0f);
            }

            foreach (var resultText in resultTexts)
            {
                SetAnimScale(resultText, resultTextAnimDelay, resultTextAnimDuration);
            }

            void SetAnimScale(Transform tr, float animDelay, float animDuration)
            {
                tr.localScale = Vector3.zero;
                tr.DOScale(1.0f, animDuration).SetDelay(animDelay);
            }
        }

        private IEnumerator StartStarsAnimation()
        {
            foreach (var star in stars)
            {
                star.localScale = Vector3.zero;
            }

            yield return new WaitForSeconds(starsAnimStartDelay);

            foreach (var star in stars)
            {
                star.DOScale(1.0f, starsAnimScaleDuration);
                star.DOPunchScale(new Vector3(starsAnimPunch, starsAnimPunch, 1.0f), starsAnimPunchDuration, starsAnimPunchVibrato, starsAnimPunchElastic).SetDelay(starsAnimScaleDuration);

                yield return new WaitForSeconds(starsAnimBetweenDelay);
            }
        }

        private IEnumerator InitWinEffect()
        {
            yield return new WaitForSeconds(winEffectDelay);

            if (winEffect && winEffectArea)
            {
                Instantiate(winEffect, winEffectArea);
            }
        }

        private IEnumerator StartReceivedStarsAnimation()
        {
            yield return new WaitForSeconds(receivedStarAnimStartDelay);

            for (int i = 0; i < imageFilledStars.Length; i++)
            {
                if (i < quizResultFromServer.stars)
                {
                    Image filledStar = imageFilledStars[i];

                    filledStar.transform.DOScale(1.0f, receivedStarAnimDuration);
                    filledStar.DOFade(1.0f, receivedStarAnimDuration).OnComplete(() => Instantiate(receivedStarEffect, filledStar.transform));

                    yield return new WaitForSeconds(receivedStarAnimBtwDelay);
                }
            }
        }
        #endregion
    }
}