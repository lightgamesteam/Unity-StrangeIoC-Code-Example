using Conditions;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Services.Localization;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class UISearchPanelView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [SerializeField] private UITopPanelScreenView uiTopPanelScreenView;
        [SerializeField] private TMP_InputField serch;
        [SerializeField] private Button buttonSerch;
        [SerializeField] private Transform arrowLanguage;
        [SerializeField] private Transform arrowStage;
        [SerializeField] private Transform arrowInterest;
        [SerializeField] private GameObject langugePanel;
        [SerializeField] private GameObject stagePanel;
        [SerializeField] private GameObject interestPanel;
        [SerializeField] private Transform langugeContent;
        [SerializeField] private Transform stageContent;
        [SerializeField] private Transform interestContent;
        [SerializeField] private GameObject langugeItem;
        [SerializeField] private GameObject stageItem;
        [SerializeField] private GameObject interestItem;
        [SerializeField] private TMP_Text langugeLabel;
        [SerializeField] private TMP_Text stageLabel;
        [SerializeField] private TMP_Text interestLabel;
        [SerializeField] private SearchDropdown langugeDropdown;
        [SerializeField] private SearchDropdown stageDropdown;
        [SerializeField] private SearchDropdown interestDropdown;

        private List<LanguageItemView> languageItems = new List<LanguageItemView>();
        private List<StageItemView> stageItems = new List<StageItemView>();
        private List<InterestItemView> interestItems = new List<InterestItemView>();

        private bool isSearchResultViewOpened = false;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            buttonSerch.onClick.AddListener(() =>
            {
                ProcessSearch();
                CLoaseAllDropdowns();
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            });

            serch.onSubmit.AddListener((string value) => ProcessSearch());

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            Dispatcher.AddListener(EventGlobal.E_ProcessBooksSearch, ProcessSearch);

            SetLocalizedTexts();
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            Dispatcher.RemoveListener(EventGlobal.E_ProcessBooksSearch, ProcessSearch);
        }

        public void InitializePanel()
        {
            InitializeLanguages();
            InitializeStages();
            InitializeInterests();
        }

        private void SetLocalizedTexts()
        {
            langugeLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.LanguageTitleKey);
            stageLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey);
            interestLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ChooseCategoryLabelKey);
        }

        public void OpenPanel()
        {
            this.gameObject.SetActive(true);
        }

        public void ClosePanel()
        {
            if (isSearchResultViewOpened)
            {
                //Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
                //Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
                Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
                //Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIMainMenu, showSwitchAnim = true });
                isSearchResultViewOpened = false;
            }

            this.gameObject.SetActive(false);
            ClearFilterItems();
            CLoaseAllDropdowns();
            serch.text = "";
        }

        private void InitializeLanguages()
        {
            List<Languages> availibleLanguages = new List<Languages>();
            //availibleLanguages.Add(LanguagesModel.DefaultLanguage);
            //availibleLanguages.AddRange(LanguagesModel.AdditionalLanguage);
            availibleLanguages = LanguagesModel.allowedTranslation;
            RectTransform a = langugeContent.GetComponent<RectTransform>();
            RectTransform b = langugeDropdown.GetPanel();



            switch (availibleLanguages.Count)
            {
                case 1:
                    langugeDropdown.gameObject.SetActive(false);
                    break;
                case 2:
                    b.sizeDelta = new Vector2(b.sizeDelta.x, langugeItem.gameObject.GetComponent<RectTransform>().rect.height * availibleLanguages.Count + a.GetComponent<VerticalLayoutGroup>().spacing * availibleLanguages.Count);
                    langugeDropdown.SetPanelSize(b.sizeDelta);
                    a.sizeDelta = new Vector2(a.sizeDelta.x, langugeItem.gameObject.GetComponent<RectTransform>().rect.height * availibleLanguages.Count);
                    break;
                default:
                    b.sizeDelta = new Vector2(b.sizeDelta.x, langugeItem.gameObject.GetComponent<RectTransform>().rect.height * 3 + a.GetComponent<VerticalLayoutGroup>().spacing * 2);
                    langugeDropdown.SetPanelSize(b.sizeDelta);
                    a.position = new Vector3(a.position.x, -(langugeItem.gameObject.GetComponent<RectTransform>().rect.height * (availibleLanguages.Count - 3)) - (a.GetComponent<VerticalLayoutGroup>().spacing * (availibleLanguages.Count - 4)), a.position.y);
                    a.sizeDelta = new Vector2(a.sizeDelta.x, langugeItem.gameObject.GetComponent<RectTransform>().rect.height * availibleLanguages.Count);
                    break;
            }
            for (int i = 0; i < availibleLanguages.Count; i++)
            {
                var item = Instantiate(langugeItem, langugeContent);
                LanguageItemView view = item.GetComponent<LanguageItemView>();
                view.InitItem(availibleLanguages[i]);
                languageItems.Add(view);



            }
        }

        private void InitializeStages()
        {
            //be attantion if someone will add new stage - the UI also will be changed
            int countOfStages = Enum.GetNames(typeof(SimplifiedLevels)).Length - 1;

            for (int i = 0; i < countOfStages; i++)
            {
                var item = Instantiate(stageItem, stageContent);
                StageItemView view = item.GetComponent<StageItemView>();
                view.InitItem((SimplifiedLevels)i);

                stageItems.Add(view);
            }
        }

        private void InitializeInterests()
        {
            //be attantion if someone will add new Category - the UI also will be changed
            int countOfInterest = child.CategoriesForStrategy.Length;

            for (int i = 0; i < countOfInterest; i++)
            {
                var item = Instantiate(interestItem, interestContent);
                InterestItemView view = item.GetComponent<InterestItemView>();
                view.InitItem(child.CategoriesForStrategy[i]);

                interestItems.Add(view);
            }
        }

        private void ClearFilterItems()
        {
            foreach (var stageItem in stageItems)
            {
                stageItem.ResetItem();
            }

            foreach (var languageItem in languageItems)
            {
                languageItem.ResetItem();
            }

            foreach (var interestItem in interestItems)
            {
                interestItem.ResetItem();
            }
        }

        private void CLoaseAllDropdowns()
        {
            langugeDropdown.CloseDropdownFast();
            stageDropdown.CloseDropdownFast();
            interestDropdown.CloseDropdownFast();
        }

        private void ProcessSearch()
        {

            List<SimplifiedLevels> stages = new List<SimplifiedLevels>();

            foreach (var stageItem in stageItems)
            {
                if (stageItem.IsSelected)
                {
                    stages.Add(stageItem.Stage);
                }
            }

            List<Languages> languages = new List<Languages>();

            foreach (var languageItem in languageItems)
            {
                if (languageItem.IsSelected)
                {
                    languages.Add(languageItem.Language);
                }
            }

            List<BooksCategory> interests = new List<BooksCategory>();

            foreach (var interestItem in interestItems)
            {
                if (interestItem.IsSelected)
                {
                    interests.Add(interestItem.Interest);
                }
            }

            BooksLibraryFilter filter = new BooksLibraryFilter(stages, languages, interests, serch.text);

            if (!filter.IsEmpty())
            {
                if (!isSearchResultViewOpened)
                {
                    isSearchResultViewOpened = true;
                    //GetDownloadedBookIdsCommand.NeedBackToUIDownloadedBooks = false;

                    Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
                    Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
                    Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBooksSearchResult, data = filter });
                }
                else
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
                    Dispatcher.Dispatch(EventGlobal.E_UpdateBooksSearchResults, filter);
                }
            }
        }
    }
}