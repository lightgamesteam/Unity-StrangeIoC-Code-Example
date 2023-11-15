using System.Collections;
using UnityEngine;
using Conditions;
using UnityEngine.UI;
using PFS.Assets.Scripts.Models.SoundManagerModels;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizSplashScreenView : BaseView
    {
        [Header("Buttons")]
        [SerializeField] private Button exitButton;

        [Header("Animations")]
        [SerializeField] private Animation startAnimation;

        [Header("AudioClip")]
        [SerializeField] private AudioClip audioClip;

        [Header("Options")]
        [SerializeField] private float startAnimDelay = 0.0f;
        [SerializeField] private float endAnimDelay = 1.0f;
        [SerializeField, Range(0, 5)] private float waitBeforeShowClozeButton;

        [Header("Localization Objects")]
        [SerializeField] private GameObject englishTitleObject;
        [SerializeField] private GameObject norwegianTitleObject;
        [SerializeField] private GameObject nynorskTitleObject;
        [SerializeField] private GameObject chiniseTitleObject;

        public static bool ShowingSplashAnimation { get; private set; }

        public void LoadView()
        {
            InitExitButton();

            ShowingSplashAnimation = true;

            SetQuizTitleLanguage();

            StartCoroutine(StartAnimation());
        }

        private void InitExitButton()
        {
            exitButton.onClick.AddListener(() =>
            {
            //Dispatcher.Dispatch(EventGlobal.E_QuizQuittedCommand, new QuizQuittedRequestModel(quiz.quizId));

            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIQuizSplashScreen);
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            });
        }

        public void RemoveView()
        {
        }

        // event for animation
        public void StartSoundEvent()
        {
            if (audioClip)
            {
                Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(audioClip, Conditions.SoundType.Sound, 1, false, 1));
            }
        }

        private IEnumerator StartAnimation()
        {
            if (!startAnimation)
                yield break;

            exitButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(startAnimDelay);

            startAnimation.Play("SplashScreen");

            yield return new WaitUntil(() => !startAnimation.isPlaying);

            yield return new WaitForSeconds(endAnimDelay);

            ShowingSplashAnimation = false;
            startAnimation.Play("SplashScreenWait");

            yield return new WaitForSeconds(waitBeforeShowClozeButton);
            exitButton.gameObject.SetActive(true);
        }

        private void SetQuizTitleLanguage()
        {
            Languages language = Languages.English;
            if (otherData is Languages)
            {
                language = (Languages)otherData;
            }

            switch (language)
            {
                case Languages.Norwegian:
                    HideAllTitles();
                    norwegianTitleObject.SetActive(true);
                    break;
                case Languages.NyNorsk:
                    HideAllTitles();
                    nynorskTitleObject.SetActive(true);
                    break;
                case Languages.Danish: //TODO add danish title object
                    HideAllTitles();
                    norwegianTitleObject.SetActive(true);
                    break;
                case Languages.Chinese:
                    HideAllTitles();
                    chiniseTitleObject.SetActive(true);
                    break;
                default:
                    HideAllTitles();
                    englishTitleObject.SetActive(true);
                    break;
            }
        }

        private void HideAllTitles()
        {
            englishTitleObject.SetActive(false);
            norwegianTitleObject.SetActive(false);
            nynorskTitleObject.SetActive(false);
            chiniseTitleObject.SetActive(false);
        }
    }
}