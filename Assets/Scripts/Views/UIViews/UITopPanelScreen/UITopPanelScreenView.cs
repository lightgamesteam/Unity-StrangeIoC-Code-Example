using Assets.Scripts.Services.Analytics;
using Conditions;
using DG.Tweening;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class UITopPanelScreenView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }
        [Inject] public Analytics Analytics { get; set; }

        [Header("SubPanels")]
        [SerializeField] private RectTransform panelDefault;
        [SerializeField] private RectTransform panelSearch;
        [SerializeField] private UISearchPanelView panelSearchView;
        [SerializeField] private UIMenuPanelView panelMenu;
        [SerializeField] private GameObject subPanelMiddleButtons;
        [SerializeField] private UICategoryPanelView subPanelCategory;

        [Header("Default Panel")]
        [SerializeField] private Button home;
        [SerializeField] private Button homework;
        [SerializeField] private Button library;
        [SerializeField] private Button search;
        [SerializeField] private Button menu;
        [SerializeField] private Button closeSearch;
        [SerializeField] private Image iconHomework;
        [SerializeField] private Image iconLibrary;

        [Header("Animations")]
        [SerializeField] private Animation menuButtonAnimation;

        [Header("Canvas groups")]
        [SerializeField] private CanvasGroup panelDefaultCanvasGroup;
        [SerializeField] private CanvasGroup panelSearchCanvasGroup;

        [Header("Colors")]
        [SerializeField] private Color32 unpressedButtonColor;
        [SerializeField] private Color32 pressedButtonColor;

        [Header("Animation params")]
        [SerializeField, Range(0f, 3f)] private float defaultPanelAnimationDuration;
        [SerializeField, Range(0f, 3f)] private float panelSearchAnimationDuration;
        [Space(5)]
        [SerializeField, Range(0f, 3f)] private float panelSearchAnimationPunchDuration;
        [SerializeField, Range(0f, 1f)] private float panelSearchAnimationPunchScale;
        [SerializeField, Range(0, 10)] private int panelSearchAnimationPunchVibration;
        [SerializeField, Range(0f, 1f)] private float panelSearchAnimationPunchElasticity;

        private bool isMenuOpened = false;
        private bool isSearchOpened = false;

        private IEnumerator panelAnimIEnum;

        private ChildModel child;

        private void OnValidate()
        {
            panelSearchView = panelSearch.GetComponent<UISearchPanelView>();
            menuButtonAnimation = menu.GetComponent<Animation>();
            panelDefaultCanvasGroup = panelDefault.GetComponent<CanvasGroup>();
            panelSearchCanvasGroup = panelSearch.GetComponent<CanvasGroup>();
        }

        public void LoadView()
        {
            child = ChildModel?.GetChild(PlayerPrefsModel.CurrentChildId);

            home.onClick.AddListener(GoHome);
            homework.onClick.AddListener(GoHomework);
            library.onClick.AddListener(GoLibrary);
            search.onClick.AddListener(OpenSearchPanel);
            closeSearch.onClick.AddListener(CloseSearchPanel);
            menu.onClick.AddListener(GoMenu);


            Dispatcher.AddListener(EventGlobal.E_LibraryCategoryToTopPanel, OpenCategoryPanel);
            Dispatcher.AddListener(EventGlobal.E_HideLibraryPanelToTopPanel, CloseCategoryPanel);
            Dispatcher.AddListener(EventGlobal.E_HideMenuPanelInTopPanel, CloseMenuPanelWithAnim);
            Dispatcher.AddListener(EventGlobal.E_ResetTopPanel, ResetTopPanel);

            panelSearchView.InitializePanel();
            panelMenu.InitializePanel();
            subPanelCategory.InitializePanel();

            if (PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherFeide || PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherLogin)
            {
                homework.gameObject.SetActive(false);
                return;
            }
        }

        public void RemoveView()
        {
            home.onClick.RemoveAllListeners();
            homework.onClick.RemoveAllListeners();
            library.onClick.RemoveAllListeners();
            search.onClick.RemoveAllListeners();
            closeSearch.onClick.RemoveAllListeners();
            menu.onClick.RemoveAllListeners();

            Dispatcher.RemoveListener(EventGlobal.E_LibraryCategoryToTopPanel, OpenCategoryPanel);
            Dispatcher.RemoveListener(EventGlobal.E_HideLibraryPanelToTopPanel, CloseCategoryPanel);
            Dispatcher.RemoveListener(EventGlobal.E_HideMenuPanelInTopPanel, CloseMenuPanelWithAnim);
            Dispatcher.RemoveListener(EventGlobal.E_ResetTopPanel, ResetTopPanel);
        }

        private void GoHome()
        {
            Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIMainMenu, showSwitchAnim = true });
            Analytics.LogEvent(EventName.NavigationHomeProfile,
                  new System.Collections.Generic.Dictionary<Property, object>()
                  {
                      { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                  });
        }

        private void GoHomework()
        {
            child = ChildModel?.GetChild(PlayerPrefsModel.CurrentChildId);

            Analytics.LogEvent(EventName.NavigationSchoolProfile,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.ClassId, child.GroupIds}
                });

            //Feide autorization
            if (SwitchModeModel.Mode == GameModes.SchoolModeForChildFeide && !child.IsInClass)
            {
                Dispatcher.Dispatch(EventGlobal.E_CheckSchoolAccess);
                return;
            }
            //Simple authorization
            else if (SwitchModeModel.Mode == GameModes.SchoolModeForChildLogin && child.IsSubscriptionExpired && !child.IsInClass)
            {
                Action openHomeworkScreenAction = () => Invoke("GoHomework", 1); //Automatically open Homework screen 
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.SubscriptionScreen, data = openHomeworkScreenAction, isAddToScreensList = false });
                return;
            }
            else
            {
                Debug.LogFormat("Mode: {0};  Subscribtion expired: {1};  Child is in class: {2}", SwitchModeModel.Mode, child.IsSubscriptionExpired, child.IsInClass);
            }

            Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            iconHomework.color = pressedButtonColor;
            Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIHomeworks, showSwitchAnim = false });
        }

        private void GoLibrary()
        {
            Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            iconLibrary.color = pressedButtonColor;
            Dispatcher.Dispatch(EventGlobal.E_HideAllScreens);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIDetailedHomeworkScreen);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookDetails);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBooksLibrary, data = new BooksLibraryByCategory(child.GetDefaultCategory()) });
            Analytics.LogEvent(EventName.NavigationReadProfile,
                  new System.Collections.Generic.Dictionary<Property, object>()
                  {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                  });
        }

        private void GoMenu()
        {
            if (!isMenuOpened)
            {
                Analytics.LogEvent(EventName.NavigationSettings,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                });

                OpenMenuPanel(withAnimation: true);
            }
            else
            {
                CloseMenuPanel(withAnimation: true);
            }
        }

        //-----SubPanels--------------------------------
        private void OpenSearchPanel()
        {
            Analytics.LogEvent(EventName.ActionSearch,
                new Dictionary<Property, object>(){
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                });

            UnityAction logicBeforeAnim = () =>
            {
                search.interactable = false;
                closeSearch.interactable = true;
                panelSearchView.OpenPanel();
            };

            UnityAction logicAfterAnim = () =>
            {
                panelDefault.gameObject.SetActive(false);
            };

            PanelAnim(openAnim: true, logicBeforeAnim, logicAfterAnim);
            isSearchOpened = true;
        }

        private void CloseSearchPanel()
        {
            if (!isSearchOpened)
            {
                return;
            }

            UnityAction logicBeforeAnim = () =>
            {
                panelDefault.gameObject.SetActive(true);
                search.interactable = true;
            };

            UnityAction logicAfterAnim = () =>
            {
                closeSearch.interactable = false;
                panelSearchView.ClosePanel();
            };

            PanelAnim(openAnim: false, logicBeforeAnim, logicAfterAnim);

            isSearchOpened = false;
        }
        //----------------------------------------------
        private void OpenMenuPanel(bool withAnimation)
        {
            panelMenu.OpenPanel();
            isMenuOpened = true;
            if (withAnimation)
            {
                PlayButtonAnimation(isMenuOpened, menuButtonAnimation, "TopPanelBurger");
            }
        }

        private void CloseMenuPanelWithAnim()
        {
            CloseMenuPanel(withAnimation: true);
        }

        private void CloseMenuPanel(bool withAnimation)
        {
            if (!isMenuOpened)
            {
                return;
            }

            panelMenu.ClosePanel(withAnimation);
            isMenuOpened = false;
            if (withAnimation)
            {
                PlayButtonAnimation(isMenuOpened, menuButtonAnimation, "TopPanelBurger");
            }
            else
            {
                ResetButtonAnimation(menuButtonAnimation, "TopPanelBurger");
            }
        }
        //----------------------------------------------
        private void OpenCategoryPanel(IEvent eventData)
        {
            BooksCategory category = new BooksCategory();
            if (eventData.data != null)
            {
                category = eventData.data as BooksCategory;
            }

            subPanelMiddleButtons.SetActive(false);
            subPanelCategory.OpenPanel(category);
        }

        private void CloseCategoryPanel()
        {
            subPanelMiddleButtons.SetActive(true);
            subPanelCategory.ClosePanel();
        }
        //----------------------------------------------

        private void ResetButtonColors()
        {
            iconHomework.color = unpressedButtonColor;
            iconLibrary.color = unpressedButtonColor;
        }

        private void ResetTopPanel()
        {
            CloseMenuPanel(withAnimation: false);
            CloseSearchPanel();
            CloseCategoryPanel();
            ResetButtonColors();
        }

        #region Panel animation
        private void PanelAnim(bool openAnim, UnityAction logicBeforeAnim, UnityAction logicAfterAnim)
        {
            logicBeforeAnim?.Invoke();

            if (panelAnimIEnum != null)
            {
                StopCoroutine(panelAnimIEnum);
            }

            panelAnimIEnum = openAnim ? OpenPanelAnimCoroutine(logicAfterAnim) : ClosePanelAnimCoroutine(logicAfterAnim);
            StartCoroutine(panelAnimIEnum);
        }

        private IEnumerator OpenPanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            panelDefaultCanvasGroup.alpha = 1f;
            panelDefault.anchorMax = Vector2.one;
            panelSearchCanvasGroup.alpha = 0f;
            panelSearch.localScale = new Vector3(0f, 1f, 1f);

            panelDefaultCanvasGroup.DOFade(0, defaultPanelAnimationDuration);
            panelDefault.DOAnchorMax(Vector2.up, defaultPanelAnimationDuration);

            yield return new WaitForSeconds(defaultPanelAnimationDuration);

            panelSearchCanvasGroup.DOFade(1, panelSearchAnimationDuration);
            panelSearch.DOScaleX(1f, panelSearchAnimationDuration);

            yield return new WaitForSeconds(panelSearchAnimationDuration);

            panelSearch.DOPunchScale(new Vector3(panelSearchAnimationPunchScale, 0f, 0f), panelSearchAnimationPunchDuration, vibrato: panelSearchAnimationPunchVibration, panelSearchAnimationPunchElasticity);

            yield return new WaitForSeconds(panelSearchAnimationPunchDuration);

            logicAfterAnim?.Invoke();
        }

        private IEnumerator ClosePanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            panelDefaultCanvasGroup.alpha = 0f;
            panelDefault.anchorMax = Vector2.up;
            panelSearchCanvasGroup.alpha = 1f;
            panelSearch.localScale = Vector3.one;

            panelSearchCanvasGroup.DOFade(0, panelSearchAnimationDuration);
            panelSearch.DOScaleX(0f, panelSearchAnimationDuration);

            yield return new WaitForSeconds(panelSearchAnimationDuration);

            panelDefaultCanvasGroup.DOFade(1f, defaultPanelAnimationDuration);
            panelDefault.DOAnchorMax(Vector2.one, defaultPanelAnimationDuration);

            yield return new WaitForSeconds(defaultPanelAnimationDuration);

            logicAfterAnim?.Invoke();
        }

        //----------Menu Button Animation
        private void PlayButtonAnimation(bool selected, Animation animationButton, string animName)
        {
            animationButton[animName].speed = selected ? 1 : -1;
            if (animationButton[animName].time == 0 || animationButton[animName].time == animationButton[animName].length)
            {
                animationButton[animName].time = selected ? 0 : animationButton[animName].length;
            }
            animationButton.Play();
        }

        private void ResetButtonAnimation(Animation animationButton, string animName)
        {
            animationButton[animName].speed = -1;
            animationButton[animName].time = 0.0f;
            animationButton.Play();
        }
        #endregion
    }
}