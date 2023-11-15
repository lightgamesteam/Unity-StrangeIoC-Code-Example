using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Views.Library;
using PFS.Assets.Scripts.Views.Buttons;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIDetailedHomeworkView : BaseView
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }
        [Inject] public PoolModel Pool { get; set; }

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI stageTitle;
        [SerializeField] private TextMeshProUGUI categoriesTitle;
        [SerializeField] private TextMeshProUGUI writerTitle;
        [SerializeField] private TextMeshProUGUI narratorTitle;
        [SerializeField] private TextMeshProUGUI illustratorTitle;
        [SerializeField] private TextMeshProUGUI creationDateTitle;
        [SerializeField] private TextMeshProUGUI testingModeTitle;
        [SerializeField] private TextMeshProUGUI deadlineTitle;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI categoriesText;
        [SerializeField] private TextMeshProUGUI writerText;
        [SerializeField] private TextMeshProUGUI narratorText;
        [SerializeField] private TextMeshProUGUI illustratorText;
        [SerializeField] private TextMeshProUGUI creationDateText;
        [SerializeField] private TextMeshProUGUI testingModeText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Images")]
        [SerializeField] private Image languageIcon;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image silentIcon;
        [SerializeField] private Image soundIcon;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        [Header("Other")]
        [SerializeField] private Slider deadlineProrgress;
        [SerializeField] private UIBookView bookView;
        [SerializeField] private Transform particlesPosition;
        [SerializeField] private Transform particlesTransform;

        [Header("Components")]
        [SerializeField] private StartBookButtonView startBookButtonView;
        [SerializeField] private UIDeleteButtonView deleteBookButton;

        private Homework homework;

        private TimeSpan homeworkAllTimeSpan;

        private IEnumerator timer;

        public void LoadView()
        {
            if (otherData != null)
            {
                homework = otherData as Homework;
            }
            else
            {
                Debug.LogError("HomeworkDetailed => Homework - null");
            }

            closeButton.onClick.AddListener(() =>
            {
                CloseScreen();
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            });

            SetLocalization();

            LoadHomeworkBook();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            StopCoroutine(UpdateTimer());
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void Init()
        {
            SetBookBackgroundImage();
            UpdateBookDescriptionPanel();
            SetCreationDate();
            SetLanguageImage();
            StartTimer();

            startBookButtonView.InitializeStartBookButton(homework.homeworkBook);
            deleteBookButton.InitButton(homework.homeworkBook);
        }

        private void LoadHomeworkBook()
        {
            Conditions.Languages translation;
            homework.homeworkBook.CurrentTranslation = Enum.TryParse(homework.language, true, out translation) ? translation : homework.homeworkBook.CurrentTranslation;
            //homework.homeworkBook.CurrentTranslationString = homework.language;
            bookView.SetBook(homework.homeworkBook, UIBookView.BookState.IgnoreGlobalLanguage);
            Init();
        }

        private void SetLocalization()
        {
            stageTitle.text = LocalizationManager.GetLocalizationText(LevelTitleKey);
            categoriesTitle.text = LocalizationManager.GetLocalizationText(InterestsTitleKey);
            writerTitle.text = LocalizationManager.GetLocalizationText(WriterKey);
            narratorTitle.text = LocalizationManager.GetLocalizationText(NarratorKey);
            illustratorTitle.text = LocalizationManager.GetLocalizationText(IllustratorKey);
            creationDateTitle.text = LocalizationManager.GetLocalizationText(DateOfCreationTitleKey);
            testingModeTitle.text = LocalizationManager.GetLocalizationText(ModeTitleKey);
            deadlineTitle.text = LocalizationManager.GetLocalizationText(DeadlineTitleKey);
            testingModeText.text = LocalizationManager.GetLocalizationText(homework.withQuiz ? TestingModeKey : ReadOnlyModeKey);

            categoriesText.text = homework.homeworkBook?.GetInterests();
        }

        private void UpdateBookDescriptionPanel()
        {
            titleText.text = homework.homeworkBook.GetTranslation().BookName;
            descriptionText.text = homework.homeworkBook.GetTranslation().DescriptionShort;

            stageText.text = ((int)homework.homeworkBook.SimplifiedLevelEnum + 1).ToString();
            writerText.text = homework.homeworkBook.GetTranslation().Writer;
            narratorText.text = homework.homeworkBook.GetTranslation().Narrator;
            illustratorText.text = homework.homeworkBook.GetTranslation().Illustrator;

            categoriesText.text = homework.homeworkBook?.GetInterests();

            silentIcon.gameObject.SetActive(homework.readOnly);
            soundIcon.gameObject.SetActive(!homework.readOnly);
        }

        private void SetBookBackgroundImage()
        {
            backgroundImage.color = Color.black;

            Action<Sprite, string> setImageAction = (sprite, id) =>
            {
                if (backgroundImage != null && id == this.homework.homeworkBook.Id)
                {
                    backgroundImage.sprite = sprite;
                    backgroundImage.color = Color.white;
                }
            };

            Action setImageActionFail = () =>
            {
                if (backgroundImage != null)
                {
                    backgroundImage.sprite = Pool.BookBackgroundDefault;
                    backgroundImage.color = Color.white;
                }
            };

            homework.homeworkBook?.LoadBackgroundImage(setImageAction, setImageActionFail);
        }

        private void SetCreationDate()
        {
            creationDateText.text = homework.beginHomeworkDateTime.ToString("d");
        }

        private void SetLanguageImage()
        {
            languageIcon.sprite = Pool.GetLanguageSprite(homework.languageEnum);
        }

        private void StartTimer()
        {
            homeworkAllTimeSpan = homework.endHomeworkDateTime - homework.beginHomeworkDateTime;

            if (timer != null)
            {
                StopCoroutine(timer);
            }
            timer = UpdateTimer();
            StartCoroutine(timer);
        }

        private IEnumerator UpdateTimer()
        {
            while (true)
            {
                UpdateTimerPanel();
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void UpdateTimerPanel()
        {
            //if (homeworkAllTimeSpan == null)
            //{
            //    Debug.LogError("HomeworkDetailed => UpdateTimerPanel => endDateTime || homeworkAllTimeSpan - null");
            //    return;
            //}

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
                deadlineProrgress.value = (float)(remainingTime.TotalHours / homeworkAllTimeSpan.TotalHours);

                particlesTransform.position = particlesPosition.position;
            }
            else
            {
                CloseScreen();
            }
        }

        private void CloseScreen()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
            Dispatcher.Dispatch(EventGlobal.E_ScreenManagerBack);
        }
    }
}