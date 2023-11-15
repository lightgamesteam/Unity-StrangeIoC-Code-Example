using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using DG.Tweening;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views.Library;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UINewHomeworkItemView : BaseView
    {
        [Inject] public PoolModel Pool { get; set; }

        [Header("UI")]
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI bookName;
        [SerializeField] private TextMeshProUGUI modeText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image soundIcon;
        [SerializeField] private Image silentIcon;

        [Header("Components")]
        [SerializeField] private UIBookView bookLibraryView;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float itemAnimDuration;

        private Homework homework;

        private IEnumerator timer;

        [Inject] public ChildModel ChildModel { get; private set; }

        public void LoadView()
        {
            SetItemAnimation();

            if (homework == null)
            {
                Debug.LogError("UIHomeworkItemView => LoadView => homework - null");
                return;
            }

            InitHomework();
            SetLocalization();

            startButton.onClick.AddListener(OpenHomework);

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            if (timer != null)
            {
                StopCoroutine(timer);
            }

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void SetHomework(Homework work, Material material)
        {
            if (work == null)
            {
                Debug.LogError("UIHomeworkItemView => SetHomework => homework - null");
                return;
            }

            homework = work;
            bgImage.material = material;
        }

        private void InitHomework()
        {
            InitBookView();
            StartTimer();

            bookName.text = homework.homeworkBook.GetTranslation().BookName;

            silentIcon.gameObject.SetActive(homework.readOnly);
            soundIcon.gameObject.SetActive(!homework.readOnly);
        }

        private void InitBookView()
        {
            Conditions.Languages translation;
            homework.homeworkBook.CurrentTranslation = Enum.TryParse(homework.language, true, out translation) ? translation : homework.homeworkBook.CurrentTranslation;
            //homework.homeworkBook.CurrentTranslation = homework.language;
            bookLibraryView.SetBook(homework.homeworkBook, UIBookView.BookState.DisableButton | UIBookView.BookState.LateLoadImage | UIBookView.BookState.IgnoreGlobalLanguage);
        }

        private void StartTimer()
        {
            if (timer != null)
            {
                StopCoroutine(timer);
            }
            timer = UpdateTimer();
            StartCoroutine(timer);
        }

        private void OpenHomework()
        {
            Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIHomeworks);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIMainMenu);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIDetailedHomeworkScreen, data = homework });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private IEnumerator UpdateTimer()
        {
            while (true)
            {
                SetTimerString();
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void SetTimerString()
        {
            if (homework.endHomeworkDateTime.CompareTo(DateTime.UtcNow) > 0)
            {
                TimeSpan remainingTime = homework.endHomeworkDateTime - DateTime.UtcNow;

                if (remainingTime.ToString(@"dd") == "00")
                {
                    timerText.text = remainingTime.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    string cutDate = (remainingTime.ToString(@"dd").Substring(0, 1) == "0") ? remainingTime.ToString(@"dd").Substring(1, 1) : remainingTime.ToString(@"dd");
                    timerText.text = cutDate + "d " + remainingTime.ToString(@"hh\:mm\:ss");
                }

            }
            else
            {
                if (timer != null)
                {
                    StopCoroutine(timer);
                }

                Dispatcher.Dispatch(EventGlobal.E_UpdateHomeworks);
            }
        }

        private void SetLocalization()
        {
            string modeTextKey = homework.withQuiz ? LocalizationKeys.TestingModeKey : LocalizationKeys.ReadOnlyModeKey;

            modeText.text = string.Format("{0}: {1}", LocalizationManager.GetLocalizationText(LocalizationKeys.ModeTitleKey), LocalizationManager.GetLocalizationText(modeTextKey));
        }

        #region Animation
        private void SetItemAnimation()
        {
            transform.localScale = new Vector3(0.01f, 0.01f, 1f);
            transform.DOScale(Vector3.one, itemAnimDuration);
        }
        #endregion
    }
}