using Assets.Scripts.Services.Analytics;
using DG.Tweening;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views.Avatar;
using PFS.Assets.Scripts.Views.SplashScreen;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class UIMenuPanelView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }
        [Inject] public AppVersionsModel AppVersionsModel { get; private set; }

        [SerializeField] private TMP_Text childName;
        [SerializeField] private TMP_Text countOfStars;
        [SerializeField] private TMP_Text titleMyProfile;
        [SerializeField] private TMP_Text titleDownloaded;
        [SerializeField] private TMP_Text titleFavorite;
        [SerializeField] private TMP_Text titleLogOut;
        [SerializeField] private TMP_Text titleFeedback;
        [SerializeField] private TMP_Text titleExit;
        [SerializeField] private TMP_Text currentAppVersion;
        [SerializeField] private TMP_Text latestAppVersion;
        [SerializeField] private Button myProfile;
        [SerializeField] private Button exit;
        [SerializeField] private Button downloadedBooks;
        [SerializeField] private Button favoriteBooks;
        [SerializeField] private Button logOut;
        [SerializeField] private Button blocker;
        [SerializeField] private Button feedbackButton;
        [SerializeField] private RectTransform avatarContainer;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform languageButtonsPanel;
        [SerializeField] private GameObject languageButtonPrefab;
        [SerializeField] private float closeInactiveTime;
        [Header("Group canvases")]
        [SerializeField] private CanvasGroup[] canvasGroups;
        [Header("Animations params")]
        [SerializeField, Range(0f, 5f)] private float languagesButtonsAnimDuration;
        [SerializeField] private Vector2 languagesButtonsSelectedScale;
        [SerializeField] private Vector2 languagesButtonsUnselectedScale;
        [Space(10)]
        [SerializeField, Range(0f, 5f)] private float panelAlphaAnimDuration;
        [SerializeField, Range(0f, 3f)] private float panelAlphaDelay;
        [SerializeField] private Vector2 panelSize;
        [SerializeField, Range(0f, 5f)] private float panelSizeAnimDuration;
        private ChildModel currentChild;
        private IEnumerator panelAnimIEnum, panelSizeAnimIEnum;
        private IEnumerator closeInactiveEnum;
        private readonly List<MenuLanguageButtonView> languageButtonViews = new List<MenuLanguageButtonView>();

        [Inject] public ChildModel ChildModel { get; set; }
        [Inject] public BooksLibrary BooksLibrary { get; set; }
        [Inject] public PoolModel Pool { get; set; }

        private void OnValidate()
        {
            panelRect = GetComponent<RectTransform>();
            panelSize = panelRect.sizeDelta;
        }


        public void LoadView()
        {
            currentAppVersion.text = "current version: " + AppVersionsModel.currentVersion.version;
            latestAppVersion.text = "latest version: " + AppVersionsModel.latestVersion.version;

#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
        exit.gameObject.SetActive(true);

        exit.onClick.AddListener(() =>
        {
            Dispatcher.Dispatch(EventGlobal.E_AppBackButton);
        });
#endif
            InitializeLanguageButtons();
            closeInactiveEnum = CloseAfterInactiveTime();
            downloadedBooks.onClick.AddListener(() =>
            {
                OpenScreen(UIScreens.UIDownloadedBooks);
                Analytics.LogEvent(EventName.NavigationLibrary,
                      new System.Collections.Generic.Dictionary<Property, object>()
                      {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.Translation,  BooksLibrary.downloadedBooks.books?.FirstOrDefault()?.CurrentTranslation.ToDescription()}
                      });
            });
            favoriteBooks.onClick.AddListener(() =>
            {
                OpenScreen(UIScreens.UIFavoriteBooks);
                Analytics.LogEvent(EventName.NavigationFavourite,
          new System.Collections.Generic.Dictionary<Property, object>()
          {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.Translation,  BooksLibrary.favoritesBooks.books?.FirstOrDefault()?.CurrentTranslation.ToDescription()}
          });
            });
            myProfile.onClick.AddListener(() => TryToOpenMyProfile());
            logOut.onClick.AddListener(GoLogOut);
            feedbackButton.onClick.AddListener(OpenFeedbackWindow);
            blocker.onClick.AddListener(() => Dispatcher.Dispatch(EventGlobal.E_HideMenuPanelInTopPanel));
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            Dispatcher.AddListener(EventGlobal.E_UserAvatarUpdated, InitAvatarItem);
            Dispatcher.AddListener(EventGlobal.E_LocalizationSelected, OnLangugeButtonClick);
            SetLocalizedTexts();
            InitAvatarItem();
        }

        public void RemoveView()
        {
            myProfile.onClick.RemoveAllListeners();
            downloadedBooks.onClick.RemoveAllListeners();
            favoriteBooks.onClick.RemoveAllListeners();
            logOut.onClick.RemoveAllListeners();
            blocker.onClick.RemoveAllListeners();
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            Dispatcher.RemoveListener(EventGlobal.E_UserAvatarUpdated, InitAvatarItem);
            Dispatcher.RemoveListener(EventGlobal.E_LocalizationSelected, OnLangugeButtonClick);

            StopAllCoroutines();
        }

        public void InitializeLanguageButtons()
        {
            //if we alredy have the languages in panel dont need to set them one more time
            if (languageButtonViews.Count > 0)
            {
                return;
            }

            List<Conditions.Languages> allAvailibleLanguages = new List<Conditions.Languages>
        {
            LanguagesModel.DefaultLanguage
        };
            allAvailibleLanguages.AddRange(LanguagesModel.AdditionalLanguage);

            foreach (var availibleLanguage in allAvailibleLanguages)
            {
                GameObject languageButtonInstance = Instantiate(languageButtonPrefab, languageButtonsPanel);
                MenuLanguageButtonView menuLanguageButtonView = languageButtonInstance.GetComponent<MenuLanguageButtonView>();
                menuLanguageButtonView.InitItem(availibleLanguage);
                languageButtonViews.Add(menuLanguageButtonView);
            }

            if (languageButtonViews.FirstOrDefault(x => x.IsSelected) == null)
            {
                var actualLanguageFlag = languageButtonViews.FirstOrDefault(x => x.Language == LanguagesModel.DefaultLanguage);
                if (actualLanguageFlag != null)
                {
                    actualLanguageFlag.SelectItem();
                }
            }
        }

        public void InitializePanel()
        {
            ShowAvatar();
        }

        private void SetLocalizedTexts()
        {
            titleMyProfile.text = LocalizationManager.GetLocalizationText(LocalizationKeys.MyProfileTitleKey);
            titleDownloaded.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DownloadedTitleKey);
            titleFavorite.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FavoriteTitleKey);
            titleLogOut.text = LocalizationManager.GetLocalizationText(LocalizationKeys.LogOutTitleKey);
            titleFeedback.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeedbackKey);
            titleExit.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ExitKey);

        }

        private void OpenFeedbackWindow()
        {
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIFeedbackPopup, data = null, isAddToScreensList = false, showSwitchAnim = false });
            Analytics.LogEvent(EventName.NavigationFeedback,
              new System.Collections.Generic.Dictionary<Property, object>()
              {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId}
              });
        }

        public void OpenPanel(bool withAnimation = true)
        {
            UpdateMenu();
            blocker.interactable = true;
            if (withAnimation)
            {
                UnityAction beforeLogic = () => this.gameObject.SetActive(true);
                UnityAction afterLogic = () => ResetCloseInactiveTimer();

                PanelAnim(openingAnimation: true, beforeLogic, afterLogic);
            }
            else
            {
                ResetCloseInactiveTimer();
                this.gameObject.SetActive(true);
            }
        }

        public void ClosePanel(bool withAnimation = true)
        {
            blocker.interactable = false;
            StopCoroutine(closeInactiveEnum);

            if (withAnimation)
            {
                UnityAction afterLogic = () => this.gameObject.SetActive(false);

                PanelAnim(openingAnimation: false, null, afterLogic);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private void OnLangugeButtonClick(IEvent eventData)
        {
            MenuLanguageButtonView menuLanguageButtonView = eventData.data as MenuLanguageButtonView;
            foreach (var languageButtonView in languageButtonViews)
            {
                if (languageButtonView != menuLanguageButtonView)
                {
                    languageButtonView.ResetItem();
                }
            }
            ResetCloseInactiveTimer();
            Dispatcher.Dispatch(EventGlobal.E_Reinitlocalization);
        }

        private void TryToOpenMyProfile()
        {
            ChildModel child = ChildModel?.GetChild(PlayerPrefsModel.CurrentChildId);

            if (SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForChildLogin && child.IsSubscriptionExpired && !child.IsInClass)
            {
                System.Action myProfileAction = () =>
                {
                    OpenScreen(UIScreens.UIMyProfile);
                };
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen,
                    new ShowScreenModel()
                    {
                        screenName = UIScreens.SubscriptionScreen,
                        data = myProfileAction,
                        isAddToScreensList = false
                    });
                return;
            }
            else
            {
                OpenScreen(UIScreens.UIMyProfile);
            }
        }

        private void OpenScreen(string openScreenName)
        {
            Debug.Log("HERE! " + openScreenName + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = openScreenName, showSwitchAnim = true });
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            ResetCloseInactiveTimer();
        }


        private void GoLogOut()
        {
            PopupModel popupModel = new PopupModel(
                     title: LocalizationKeys.NoticeKey,
                     description: LocalizationKeys.LogoutKey,
                     buttonText: LocalizationKeys.LogOutTitleKey,
                     isActiveCloseButton: true,
                     callback: LogoutCalback);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false, showSwitchAnim = false });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);

            Analytics.LogEvent(EventName.NavigationLogOut,
                  new System.Collections.Generic.Dictionary<Property, object>()
                  {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                  });

            Analytics.LogEvent(EventName.ActionOnSignOut,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.SignOutTime_s, DateTime.Now.ToString()}
            });
            ResetCloseInactiveTimer();
        }

        private void LogoutCalback()
        {
            //clear player prefs
            PlayerPrefsModel.ClearLogoutPlayerPrefs();

            //clear books
            BooksLibrary.ClearBooks();

            //clear game mode
            SwitchModeModel.Mode = Conditions.GameModes.None;


            Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UITopPanelScreen);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, "UIParentScreen");
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, "UILogOutPopup");
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIMainMenu);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);

            UnityAction<object> action = (object data) =>
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UILoginScreen, showSwitchAnim = false, data = data });
            };
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UISplashScreen, data = new UISplashScreenView.SplashParams { isLogIn = true, action = action }, isAddToScreensList = false, showSwitchAnim = false });
        }

        private void ShowAvatar()
        {
            currentChild = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            childName.text = currentChild.Nickname;
            countOfStars.text = currentChild.Stars + "";

            InitAvatarItem();
        }

        private void InitAvatarItem()
        {
            string savedAvatarId = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId).AvatarId;
            UIAvatarItemView currentAvatarItem = avatarContainer.GetComponentInChildren<UIAvatarItemView>();

            if (currentAvatarItem == null || (currentAvatarItem.Id != savedAvatarId && !string.IsNullOrEmpty(savedAvatarId)))
            {
                foreach (Transform child in avatarContainer)
                {
                    Destroy(child.gameObject);
                }

                UIAvatarItemView avatarPrefab = null;

                foreach (var avatar in Pool.Avatars)
                {
                    if (avatar.Id == savedAvatarId)
                    {
                        avatarPrefab = avatar;
                    }
                }

                if (avatarPrefab == null)
                {
                    avatarPrefab = Pool.Avatars[0];
                }

                UIAvatarItemView avatarInstance = Instantiate(avatarPrefab, avatarContainer);
                avatarInstance.Init(UIAvatarItemView.AvatarItemState.Profile);
            }
        }

        private void UpdateMenu()
        {
            childName.text = ChildModel?.GetChild(PlayerPrefsModel.CurrentChildId).Nickname;
        }

        #region Autoclosing Logic
        private IEnumerator CloseAfterInactiveTime()
        {
            yield return new WaitForSeconds(closeInactiveTime);

            Dispatcher.Dispatch(EventGlobal.E_HideMenuPanelInTopPanel);
        }

        private void ResetCloseInactiveTimer()
        {
            if (gameObject.activeSelf)
            {
                if (closeInactiveEnum != null)
                {
                    StopCoroutine(closeInactiveEnum);
                }

                closeInactiveEnum = CloseAfterInactiveTime();
                StartCoroutine(closeInactiveEnum);
            }
        }
        #endregion

        #region Panel animation
        private void PanelAnim(bool openingAnimation, UnityAction logicBeforeAnim, UnityAction logicAfterAnim)
        {
            logicBeforeAnim?.Invoke();

            if (panelAnimIEnum != null)
            {
                StopCoroutine(panelAnimIEnum);
            }

            if (panelSizeAnimIEnum != null)
            {
                StopCoroutine(panelSizeAnimIEnum);
            }

            panelAnimIEnum = openingAnimation ? OpenPanelAnimCoroutine(logicAfterAnim) : ClosePanelAnimCoroutine(logicAfterAnim);
            StartCoroutine(panelAnimIEnum);
        }

        private IEnumerator OpenPanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            foreach (var item in canvasGroups)
            {
                item.alpha = 0;
            }

            panelSizeAnimIEnum = SizePanelAnim(toSmall: false);
            yield return StartCoroutine(panelSizeAnimIEnum);

            for (int i = 0; i < canvasGroups.Length; i++)
            {
                canvasGroups[i].DOFade(1, panelAlphaAnimDuration);

                yield return new WaitForSeconds(panelAlphaDelay);
            }

            logicAfterAnim?.Invoke();
        }

        private IEnumerator ClosePanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            foreach (var item in canvasGroups)
            {
                item.alpha = 1;
            }

            for (int i = canvasGroups.Length - 1; i >= 0; i--)
            {
                canvasGroups[i].DOFade(0, panelAlphaAnimDuration);

                yield return new WaitForSeconds(panelAlphaDelay);
            }

            if (panelAlphaDelay < panelAlphaAnimDuration)
            {
                yield return new WaitForSeconds(panelAlphaAnimDuration - panelAlphaDelay);
            }

            panelSizeAnimIEnum = SizePanelAnim(toSmall: true);
            yield return StartCoroutine(panelSizeAnimIEnum);

            logicAfterAnim?.Invoke();
        }

        private IEnumerator SizePanelAnim(bool toSmall)
        {
            panelRect.sizeDelta = toSmall ? panelSize : Vector2.zero;
            panelRect.DOSizeDelta(toSmall ? Vector2.zero : panelSize, panelSizeAnimDuration);

            yield return new WaitForSeconds(panelSizeAnimDuration);
        }
        #endregion
    }
}