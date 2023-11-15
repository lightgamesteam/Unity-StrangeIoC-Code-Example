using UnityEngine;
using TMPro;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using Assets.Scripts.Services.Analytics;
using DG.Tweening;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models;

namespace PFS.Assets.Scripts.Views.BookDetails
{
    public class UIBookDetailsInfoView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        [Header("UI texts")]
        [SerializeField] private TextMeshProUGUI bookNameText;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI categoriesText;
        [SerializeField] private TextMeshProUGUI writerText;
        [SerializeField] private TextMeshProUGUI narratorText;
        [SerializeField] private TextMeshProUGUI illustratorText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("More info panel")]
        [SerializeField] private CanvasGroup moreInfoPanel;
        [SerializeField] private UIMoreInfoButtonView moreInfoButton;

        [Header("UI localizations texts")]
        [SerializeField] private TextMeshProUGUI stageTextTitle;
        [SerializeField] private TextMeshProUGUI categoriesTextTitle;
        [SerializeField] private TextMeshProUGUI writerTextTitle;
        [SerializeField] private TextMeshProUGUI narratorTextTitle;
        [SerializeField] private TextMeshProUGUI illustratorTextTitle;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float fadeAnimDuration;

        public BookModel Book { get; private set; }

        public void LoadView()
        {
            SetLocalization();

            moreInfoButton.AddListener(() => MoreInfoSwitcherAnimation(moreInfoButton.Click));

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void SetLocalization()
        {
            stageTextTitle.text = LocalizationManager.GetLocalizationText(LevelTitleKey);
            categoriesTextTitle.text = LocalizationManager.GetLocalizationText(InterestsTitleKey);
            writerTextTitle.text = LocalizationManager.GetLocalizationText(WriterKey);
            narratorTextTitle.text = LocalizationManager.GetLocalizationText(NarratorKey);
            illustratorTextTitle.text = LocalizationManager.GetLocalizationText(IllustratorKey);

            categoriesText.text = Book?.GetInterests();
        }

        public void SetBookInfo(BookModel book)
        {
            if (book == null)
            {
                return;
            }

            Analytics.LogEvent(EventName.NavigationBookDetails,
                    new System.Collections.Generic.Dictionary<Property, object>()
                    {
                    { Property.ISBN, book.GetTranslation().Isbn},
                    { Property.Category,  book?.GetInterests()},
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.QuizId, book.QuizId},
                    { Property.BookId,  book.Id},
                    { Property.Translation, book.CurrentTranslation.ToDescription()}
                    });

            Book = book;

            var translation = book.GetTranslation();

            bookNameText.text = translation.BookName;
            stageText.text = ((int)book.SimplifiedLevelEnum + 1).ToString();
            categoriesText.text = Book?.GetInterests();
            writerText.text = translation.Writer;
            narratorText.text = translation.Narrator;
            illustratorText.text = translation.Illustrator;
            descriptionText.text = translation.DescriptionShort;

            DisableMoreInfo();
        }

        public void DisableMoreInfo()
        {
            moreInfoButton.DisableMoreInfo();
        }

        #region Animations
        private void MoreInfoSwitcherAnimation(bool moreInfo)
        {
            if (moreInfo)
            {
                moreInfoPanel.alpha = 0f;
                moreInfoPanel.gameObject.SetActive(true);
                descriptionText.gameObject.SetActive(false);

                moreInfoPanel.DOFade(1f, fadeAnimDuration);

            }
            else
            {
                descriptionText.alpha = 0f;
                moreInfoPanel.gameObject.SetActive(false);
                descriptionText.gameObject.SetActive(true);

                descriptionText.DOFade(1f, fadeAnimDuration);
            }
        }
        #endregion
    }
}