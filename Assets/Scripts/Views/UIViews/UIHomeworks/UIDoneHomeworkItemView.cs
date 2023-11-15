using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;
using DG.Tweening;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Views.Library;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIDoneHomeworkItemView : BaseView
    {
        [Inject] public PoolModel Pool { get; set; }

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI bookName;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private TextMeshProUGUI endDateText;
        [SerializeField] private Image languageIcon;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image levelColorImage;
        [SerializeField] private TextMeshProUGUI levelLongText;
        [SerializeField] private TextMeshProUGUI levelShortText;

        [Header("Other")]
        [SerializeField] private Transform starsPanel;
        [SerializeField] private Image[] starsImages;
        [SerializeField] private UIBookView bookView;

        [Header("Star colors")]
        [SerializeField] private Color enableStarColor;
        [SerializeField] private Color disableStarColor;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float itemAnimDuration;

        private Homework homework;
        private int spentMinutes;
        private string minutesShort;

        private void OnValidate()
        {
            starsImages = new Image[starsPanel.childCount];
            for (int i = 0; i < starsPanel.childCount; i++)
            {
                starsImages[i] = starsPanel.GetChild(i).GetChild(0)?.GetComponent<Image>();
            }
        }

        public void LoadView()
        {
            SetItemAnimation();

            if (homework == null)
            {
                Debug.LogError("UIDoneHomeworkItemView => LoadView => homework - null");
                return;
            }

            InitHomework();
            SetLocalization();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void SetHomework(Homework work, Material material)
        {
            if (work == null)
            {
                Debug.LogError("UIDoneHomeworkItemView => SetHomework => homework - null");
                return;
            }

            if (work.homeworkBook.HideQuiz)
            {
                starsPanel.gameObject.SetActive(false);
            }

            homework = work;
            bookView.SetBook(work.homeworkBook, UIBookView.BookState.DisableButton | UIBookView.BookState.LateLoadImage);

            bgImage.material = material;
        }

        private void InitHomework()
        {
            spentMinutes = Math.Max(1, homework.spentTime / 60);

            int pos = (int)homework.homeworkBook.SimplifiedLevelEnum;
            if (pos >= 0 && pos < Pool.SimplifiedColors.Length)
            {
                levelColorImage.color = Pool.SimplifiedColors[pos];
            }

            SetLanguage();
            SetEndDateText();
            SetMark();
        }

        private void SetLanguage()
        {
            languageIcon.sprite = Pool.GetLanguageSprite(homework.languageEnum);
            languageText.text = homework.language;
        }

        private void SetEndDateText()
        {
            endDateText.text = homework.endHomeworkDateTime.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        private void SetMark()
        {
            for (int i = 0; i < starsImages.Length; i++)
            {
                starsImages[i].color = i < homework.markForWork ? enableStarColor : disableStarColor;
            }
        }

        private void SetLocalization()
        {
            minutesShort = LocalizationManager.GetLocalizationText(LocalizationKeys.MinutesShortTitleKey);

            bookName.text = homework.homeworkBook.GetTranslation().BookName;
            timeText.text = string.Format("{0} {1}", spentMinutes, minutesShort);
            languageText.text = LocalizationManager.GetLocalizationText("ui." + homework.language);

            SetLevelTexts();
        }

        private void SetLevelTexts()
        {
            string firstLocalizedLetter = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey).Substring(0, 1);
            string stageNumber = homework.homeworkBook.SimplifiedLevelEnum.ToString().Substring(homework.homeworkBook.SimplifiedLevelEnum.ToString().Length - 1, 1);
            string localizedStageLabel = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey);
            levelShortText.text = firstLocalizedLetter + stageNumber;
            levelLongText.text = localizedStageLabel + " " + stageNumber;
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