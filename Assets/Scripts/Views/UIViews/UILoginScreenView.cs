using Assets.Scripts.Services.Analytics;
using DG.Tweening;
using PFS.Assets.Scripts.Commands.Authorization;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Authorization;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using TMPro;
using TMPTools;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

namespace PFS.Assets.Scripts.Views.Login
{
    public class UILoginScreenView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        public enum LoginButtonState
        {
            Default = 0,
            LogIn,
            CreateAnAccount
        }
        [Header("Login panels")]
        public GameObject loginPanel;
        public GameObject fadeLoginPanel;

        [Header("Login/Password Options")]
        public TMP_InputField loginInput;
        public TMP_InputField passwordInput;
        public Image loginErrorOutline;
        public Image passwordErrorOutline;
        public Button helpPasswordButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button regularLoginButton;
        [SerializeField] private Button visiblePasswordButton;
        [SerializeField] private Button backToFadeLoginButton;
        [SerializeField] private Button studentButton;
        [SerializeField] private Button teacherButton;
        [SerializeField] private Button exitButton;

        [Header("Feide Options")]
        [SerializeField] private Button feideLoginButton;
        [SerializeField] private UniWebView uniWebViewPrefab;
        [SerializeField] private BaseCanvasWebViewPrefab webView3DPrefab;

        [Header("Other Objects")]
        [SerializeField] private Image fakeBG;
        [SerializeField] private GameObject additionalLoginButtonsPanel;
        [Space(10)]
        [SerializeField] private CanvasGroup bgCharacterUpLeft;
        [SerializeField] private CanvasGroup bgCharacterUpRight;
        [SerializeField] private Button teacherSwitch;
        [SerializeField] private RectTransform teacherSwitchKnob;
        [SerializeField] private Image teacherSwitchKnobColor;
        [SerializeField] private Image backGlow;

        [SerializeField] private Image studentBorder;
        [SerializeField] private Image studentHad;
        [SerializeField] private Image teacherBorder;
        [SerializeField] private Image teacherHad;

        [Header("Labels")]
        public TextMeshProUGUI wrongPasswordLabel;
        [SerializeField] private TextMeshProUGUI loginButtonLabel;
        [SerializeField] private TextMeshProUGUI titleLoginLabel;
        [SerializeField] private TextMeshProUGUI titleDescriptionLabel;
        [SerializeField] private TextMeshProUGUI accountExistLabel;
        [SerializeField] private TextMeshProUGUI accountNotExistLabel;
        [SerializeField] private TextMeshProUGUI resetPasswordLabel;
        [SerializeField] private TextMeshProUGUI loginPlaceholderLabel;
        [SerializeField] private TextMeshProUGUI passwordPlaceholderLabel;

        [SerializeField] private TextMeshProUGUI iAmStudentLabel;
        [SerializeField] private TextMeshProUGUI iAmTeacherLabel;
        [SerializeField] private TextMeshProUGUI backLabel;
        [SerializeField] private TextMeshProUGUI regularLoginLabel;
        [SerializeField] private TextMeshProUGUI feideTitleLabel;
        [SerializeField] private TextMeshProUGUI feideTitleDescriptionLabel;
        [SerializeField] private TextMeshProUGUI exitLabel;


        [Header("Animations params")]
        [SerializeField, Range(0f, 5f)] private float loginButtonWaitTime;
        [SerializeField, Range(0f, 5f)] private float loginButtonAnimDuration;
        [SerializeField, Range(0f, 5f)] private float loginButtonTextAnimDuration;
        [Space(10)]
        [SerializeField] private RectTransform orLoginTextAnimPosition;
        [SerializeField, Range(0f, 5f)] private float logoTextAnimDuration;
        [Space(10)]
        [SerializeField] private RectTransform[] backgroundStars;
        [SerializeField, Range(0.0f, 2.0f)] private float starScaleChangingValue;
        [SerializeField, Range(0.0f, 3.0f)] private float starAnimDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float starAnimStartingDelay;
        [Space(20)]
        [SerializeField, Range(0f, 5f)] private float fakeBGAnimDuration;
        [Space(10)]
        [SerializeField, Range(0f, 5f)] private float waitCharacterAnimDelay;
        [SerializeField] private RectTransform bgCharacterUpLeftPoint;
        [SerializeField] private RectTransform bgCharacterUpRightPoint;
        [Space(10)]
        [SerializeField, Range(0f, 5f)] private float bgCharacterUpLeftAnimDuration;
        [SerializeField, Range(0f, 5f)] private float bgCharacterUpRightAnimDuration;
        [Space(10)]
        [SerializeField] private float teacherSwitchKnobOffset;
        [SerializeField] private float teacherSwitchKnobTime;
        [Space(10)]
        [SerializeField] private Color knobBlue;
        [SerializeField] private Color knobOrange;
        [SerializeField] private Sprite switchBlue;
        [SerializeField] private Sprite switchOrange;
        [Space(10)]
        [SerializeField] private Transform backgroundExpandable;
        [SerializeField] private CanvasGroup backgroundFade;
        [SerializeField] private float backExpansionOffset;
        [Space(10)]
        [SerializeField] private Transform feideCenterPosition;
        [SerializeField] private Transform feideRightPosition;
        [SerializeField] private float feideLabelOffset;
        [Space(10)]
        [SerializeField] private CanvasGroup ALBCanvasGroup;
        [SerializeField] private RectTransform ALBAdditionalAnchor;

        private bool isLoginAccauntExist = false;
        private string childTitle, childDescriptionTitle, childAccountNotExist, childLoginPlaceholder, childJoinOrLogin, childCreateAnAccount;
        private string teacherTitle, teacherDescriptionTitle, teacherAccountNotExist, teacherLoginPlaceholder;
        private string signIn, passwordPlaceholder;

        public bool IsTeacherLogin { get; private set; } = false;
        private bool isSymbolError = false;

        public void LoadView()
        {
            InitLocalization();
            InitScreenAnimations();
            //teacherSwitch.onClick.AddListener(OnTeacherSwitchClicked);
            teacherSwitch.gameObject.SetActive(false);
            /*#if !UNITY_ANDROID && !UNITY_IOS
                    feideLoginButton.GetComponent<Transform>().position = new Vector3(feideCenterPosition.position.x,
                        feideLoginButton.GetComponent<Transform>().position.y, feideLoginButton.GetComponent<Transform>().position.z);
            #endif*/

            InitAutorizationByLogin();
            InitAutorizationByFeide();

            if (PlayerPrefsModel.FirstRun)
            {
                Analytics.LogEvent(EventName.NavigationWelcome,
                    new System.Collections.Generic.Dictionary<Property, object>()
                    {
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                    });
            }

            if (PlayerPrefsModel.CountryCode != Conditions.CountryCodes.Norway.ToDescription())
            {
                loginPanel.gameObject.SetActive(true);
                fadeLoginPanel.gameObject.SetActive(false);
                backToFadeLoginButton.gameObject.SetActive(false);
            }
            else
            {
                loginPanel.gameObject.SetActive(false);
                fadeLoginPanel.gameObject.SetActive(true);
                backToFadeLoginButton.gameObject.SetActive(true);
            }

            Analytics.LogEvent(EventName.NavigationSignIn,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                      { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                });

#if DEVELOP
        Dispatcher.AddListener(EventGlobal.E_ShowHideLoginButtons, ProcessLoginButtonsDebug);
#endif

            if (DeepLinkCommandByFeide.isFeidDeepLink)
            {
                SetLoginButtonsActive(true);
                Dispatcher.Dispatch(EventGlobal.E_ShowBlocker);
            }

        }

        public void RemoveView()
        {
#if DEVELOP
        Dispatcher.RemoveListener(EventGlobal.E_ShowHideLoginButtons, ProcessLoginButtonsDebug);
#endif

        }

        public bool IsPasswordVisible() => passwordInput.contentType == TMP_InputField.ContentType.Standard;

        #region Login Autorization
        private void InitAutorizationByLogin()
        {
            teacherButton.onClick.AddListener(() =>
            {
                if (IsTeacherLogin)
                {
                    return;
                }
                OnTeacherSwitchClicked();
            });

            studentButton.onClick.AddListener(() =>
            {
                if (!IsTeacherLogin)
                {
                    return;
                }
                OnTeacherSwitchClicked();
            });

            backToFadeLoginButton.onClick.AddListener(() =>
            {
                loginPanel.gameObject.SetActive(false);
                fadeLoginPanel.gameObject.SetActive(true);
                if (!IsTeacherLogin)
                {
                    return;
                }
                OnTeacherSwitchClicked();
            });

#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WSA_10_0)
        exitButton.gameObject.SetActive(true);

        exitButton.onClick.AddListener(() =>
        {
            Dispatcher.Dispatch(EventGlobal.E_AppBackButton);
        });
#endif



            regularLoginButton.onClick.AddListener(() =>
            {
                loginPanel.gameObject.SetActive(true);
                fadeLoginPanel.gameObject.SetActive(false);
            });

            loginButton.onClick.AddListener(() =>
            {
                isSymbolError = false;
                if (isLoginAccauntExist)
                {
                    Dispatcher.Dispatch(EventGlobal.E_StartAutorizationByLogin, this);
                }
                else
                {
                    wrongPasswordLabel.gameObject.SetActive(false);
                    if (passwordInput.text.Length >= 6)
                    {
                        Dispatcher.Dispatch(EventGlobal.E_StartAutorizationByLogin, this);
                    }
                    else
                    {
                        isSymbolError = true;
                    //ShowPasswordHelpPopup();
                    wrongPasswordLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.PasswordSymbolError);
                        wrongPasswordLabel.gameObject.SetActive(true);
                    }
                }
            });
            helpPasswordButton.onClick.AddListener(ShowPasswordHelpPopup);
            loginInput.onSelect.AddListener((string value) => ResetInputFields());
            passwordInput.onSelect.AddListener((string value) => ResetInputFields());
            visiblePasswordButton.onClick.AddListener(InversePasswordVisibility);
            helpPasswordButton.onClick.AddListener(ShowPasswordHelpPopup);
            loginInput.onEndEdit.AddListener(StartCheckUser);
            ResetInputFields();
            SetLoginButtonState(LoginButtonState.Default);
        }

        private void StartCheckUser(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ResetInputFields();
                ResetUserExistlements();
                SetLoginButtonState(LoginButtonState.Default);
                return;
            }
            Dispatcher.Dispatch(EventGlobal.E_CheckUsernameCommand, new CheckUsernameRequestModel(value, IsTeacherLogin,
            ProcessUsernameInput,
            () =>
            {
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
            }));
        }

        private void ProcessUsernameInput(bool isExist)
        {
            isLoginAccauntExist = isExist;
            ResetWarninglements();
            accountExistLabel.gameObject.SetActive(isExist);
            accountNotExistLabel.gameObject.SetActive(!isExist);
            SetLoginButtonState(isExist ? LoginButtonState.LogIn : LoginButtonState.CreateAnAccount);
        }

        private void ShowPasswordHelpPopup()
        {
            string description = !IsTeacherLogin ? LocalizationKeys.PasswordErrorHelpKey : LocalizationKeys.HereKey;
            if (isSymbolError)
            {
                description = LocalizationKeys.PasswordSymbolError;
            }
            PopupModel popupModel = new PopupModel(
                       title: LocalizationKeys.ErrorKey,
                       description: description,
                       buttonText: LocalizationKeys.OkKey,
                       isActiveCloseButton: false,
                       callback: null);

            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false, showSwitchAnim = false });
            Analytics.LogEvent(EventName.NavigationForgetPassword,
                  new System.Collections.Generic.Dictionary<Property, object>()
                  {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                  });
        }

        private void ResetInputFields()
        {
            ResetWarninglements();

            loginPlaceholderLabel.color = Color.gray;
            loginPlaceholderLabel.text = IsTeacherLogin ? teacherLoginPlaceholder : childLoginPlaceholder;

            passwordPlaceholderLabel.color = Color.gray;
            passwordPlaceholderLabel.text = passwordPlaceholder;
        }

        private void ResetWarninglements()
        {
            wrongPasswordLabel.gameObject.SetActive(false);
            wrongPasswordLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorWrongPassword);
            loginErrorOutline.gameObject.SetActive(false);
            helpPasswordButton.gameObject.SetActive(false);
            passwordErrorOutline.gameObject.SetActive(false);
        }

        private void ResetUserExistlements()
        {
            accountExistLabel.gameObject.SetActive(false);
            accountNotExistLabel.gameObject.SetActive(false);
        }

        private void SetLoginButtonState(LoginButtonState state)
        {
            switch (state)
            {
                case LoginButtonState.Default:
                    loginButtonLabel.text = IsTeacherLogin ? signIn : childJoinOrLogin;
                    loginButton.interactable = false;
                    break;
                case LoginButtonState.LogIn:
                    loginButtonLabel.text = signIn;
                    loginButton.interactable = true;
                    break;
                case LoginButtonState.CreateAnAccount:
                    loginButtonLabel.text = IsTeacherLogin ? signIn : childCreateAnAccount;
                    loginButton.interactable = !IsTeacherLogin;
                    break;
                default:
                    break;
            }
        }

        public void ProcessLoginAuthorizationFinish()
        {
            Analytics.LogEvent(EventName.ActionSignIn,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.LoginType, "username and password"},
                    { Property.Region, PlayerPrefsModel.CountryCode}
            });

            Analytics.LogEvent(EventName.ActionOnSignIn,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                    { Property.SignInTime_s, DateTime.Now.ToString()}
            });

            if (!IsTeacherLogin && !isLoginAccauntExist)
            {
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UILoginScreen);
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UIJoinClassScreen, isAddToScreensList = true, showSwitchAnim = true });
            }
            else
            {
                OpenMenuScreen();
            }
        }

#if DEVELOP
    private void ProcessLoginButtonsDebug(IEvent e)
    {
        SetLoginButtonsActive((bool)e.data);
    }
#endif

        private void SetLoginButtonsActive(bool isActive)
        {
            // orLoginWithLabel.gameObject.SetActive(isActive);
            loginPanel.gameObject.SetActive(!isActive);
            fadeLoginPanel.gameObject.SetActive(isActive);

            additionalLoginButtonsPanel.SetActive(isActive);
        }

        private void InversePasswordVisibility()
        {
            passwordInput.contentType = IsPasswordVisible() ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
            visiblePasswordButton.transform.GetChild(0).gameObject.SetActive(!IsPasswordVisible());
            visiblePasswordButton.GetComponent<Image>().color = IsPasswordVisible() ? Color.black : new Color(0, 0, 0, 0);
        }
        #endregion

        #region Feide Autorization
        private void InitAutorizationByFeide()
        {
            feideLoginButton.onClick.AddListener(() => Dispatcher.Dispatch(EventGlobal.E_StartAutorizationByFeide, new StartAutorizationByFeideModel(this, uniWebViewPrefab, webView3DPrefab)));
        }
        #endregion

        private void OpenMenuScreen()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UILoginScreen);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UITopPanelScreen, isAddToScreensList = false });
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, UIScreens.UIMainMenu);
        }
        private void InitLocalization()
        {
            //global
            iAmStudentLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.IamStudentKey);
            iAmTeacherLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.IamTeachertKey);
            backLabel.text = "< " + LocalizationManager.GetLocalizationText(LocalizationKeys.BackKey);
            regularLoginLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.RegularLoginKey);
            feideTitleLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeideTitleTextKey);
            feideTitleDescriptionLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeideTitleDescriptionKey);
            exitLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ExitKey);

            accountExistLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.AccountExistKey);
            wrongPasswordLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.WrongKey);
            resetPasswordLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ResetPasswordKey);
            titleLoginLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinOrLoginKey);
            titleDescriptionLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterDataKey);

            signIn = LocalizationManager.GetLocalizationText(LocalizationKeys.SignInKey);
            passwordPlaceholder = LocalizationManager.GetLocalizationText(LocalizationKeys.PasswordPlaceholderKey);

            //child
            childTitle = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinOrLoginKey);
            childDescriptionTitle = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterDataKey);
            childAccountNotExist = LocalizationManager.GetLocalizationText(LocalizationKeys.AccountNotExistKey);
            childLoginPlaceholder = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginPlaceholderKey);
            childJoinOrLogin = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinOrLoginKey);
            childCreateAnAccount = LocalizationManager.GetLocalizationText(LocalizationKeys.CreateAccauntKey);

            //teacher
            teacherTitle = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginAsTeacherTitleKey);
            teacherDescriptionTitle = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterEmailTitleDescriptionKey);
            teacherAccountNotExist = LocalizationManager.GetLocalizationText(LocalizationKeys.AccountNotExistTeacherKey);
            teacherLoginPlaceholder = LocalizationManager.GetLocalizationText(LocalizationKeys.EmailPlaceholderKey);
        }



        #region Animations
        private void InitScreenAnimations()
        {
            StartCoroutine(LogoAreaAnimation());
            StartCoroutine(BackgroudAnimation());
            StarsAnimation();
        }

        private void OnTeacherSwitchClicked()
        {
            IsTeacherLogin = !IsTeacherLogin;
            string profile = string.Empty;
            if (IsTeacherLogin)
            {
                teacherButton.image.SetAlpha(1f);
                studentButton.image.SetAlpha(0.01f);
                profile = "teacher";
            }
            else
            {
                teacherButton.image.SetAlpha(0.01f);
                studentButton.image.SetAlpha(1f);
                profile = "student";
            }


            Analytics.LogEvent(EventName.ActionSelectProfile,
             new System.Collections.Generic.Dictionary<Property, object>()
             {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.RegionCountry, PlayerPrefsModel.CountryCode},
                    { Property.Profile, profile}
             });
            teacherSwitch.interactable = false;

            backGlow.DOColor(IsTeacherLogin ? new Color32(0, 86, 166, 163) : new Color32(0, 132, 255, 163), teacherSwitchKnobTime);
            //feideLoginButton.transform.DOLocalMove(IsTeacherLogin ? feideCenterPosition.localPosition : feideRightPosition.localPosition, teacherSwitchKnobTime);
            accountNotExistLabel.text = IsTeacherLogin ? teacherAccountNotExist : childAccountNotExist;
            titleLoginLabel.DOCrossfadeTMP(IsTeacherLogin ? teacherTitle : childTitle, teacherSwitchKnobTime);
            titleDescriptionLabel.DOCrossfadeTMP(IsTeacherLogin ? teacherDescriptionTitle : childDescriptionTitle, teacherSwitchKnobTime);
            loginPlaceholderLabel.DOCrossfadeTMP(IsTeacherLogin ? teacherLoginPlaceholder : childLoginPlaceholder, teacherSwitchKnobTime);
            loginButtonLabel.DOCrossfadeTMP(IsTeacherLogin ? signIn : childJoinOrLogin, teacherSwitchKnobTime);
            teacherSwitchKnobColor.DOBlendableColor(IsTeacherLogin ? knobBlue : knobOrange, teacherSwitchKnobTime);
            iAmStudentLabel.DOBlendableColor(IsTeacherLogin ? Color.gray : Color.white, teacherSwitchKnobTime);
            iAmTeacherLabel.DOBlendableColor(IsTeacherLogin ? Color.white : Color.gray, teacherSwitchKnobTime);


            studentBorder.DOBlendableColor(IsTeacherLogin ? Color.gray : Color.white, teacherSwitchKnobTime);
            studentHad.DOBlendableColor(IsTeacherLogin ? Color.gray : Color.white, teacherSwitchKnobTime);
            teacherBorder.DOBlendableColor(IsTeacherLogin ? Color.white : Color.gray, teacherSwitchKnobTime);
            teacherHad.DOBlendableColor(IsTeacherLogin ? Color.white : Color.gray, teacherSwitchKnobTime);

            teacherSwitch.image.DOCrossfadeImage(IsTeacherLogin ? switchOrange : switchBlue, teacherSwitchKnobTime);
            backgroundFade.DOFade(IsTeacherLogin ? 0 : 1, teacherSwitchKnobTime);
            backgroundExpandable.DOScale(IsTeacherLogin ? new Vector3(backExpansionOffset, backExpansionOffset, 0) : Vector3.one, teacherSwitchKnobTime);
            teacherSwitchKnob.DOAnchorPos(IsTeacherLogin ? new Vector2(teacherSwitchKnobOffset, teacherSwitchKnob.anchoredPosition.y) : new Vector2(-teacherSwitchKnobOffset, teacherSwitchKnob.anchoredPosition.y), teacherSwitchKnobTime)
                .OnComplete(() =>
                {
                    loginInput.text = string.Empty;
                    passwordInput.text = string.Empty;
                    teacherSwitch.interactable = true;
                });

            ResetUserExistlements();
            SetLoginButtonState(LoginButtonState.Default);
            ResetInputFields();
        }


        private IEnumerator LogoAreaAnimation()
        {
            loginButton.transform.localScale = Vector3.zero;
            feideLoginButton.transform.localScale = Vector3.one;

            // orLoginWithLabel.color = new Color(orLoginWithLabel.color.r, orLoginWithLabel.color.g, orLoginWithLabel.color.b, 0f);
            titleDescriptionLabel.color = new Color(titleDescriptionLabel.color.r, titleDescriptionLabel.color.g, titleDescriptionLabel.color.b, 0f);
            titleLoginLabel.color = new Color(titleLoginLabel.color.r, titleLoginLabel.color.g, titleLoginLabel.color.b, 0f);

            passwordInput.transform.localScale = Vector3.zero;
            loginInput.transform.localScale = Vector3.zero;

            //Vector3 orLoginTextPosition = orLoginWithLabel.transform.localPosition;
            // orLoginWithLabel.transform.localPosition = orLoginTextAnimPosition.localPosition;

            yield return new WaitForSeconds(loginButtonWaitTime);

            passwordInput.transform.DOScale(Vector3.one, loginButtonAnimDuration);
            loginInput.transform.DOScale(Vector3.one, loginButtonAnimDuration);

            loginButton.transform.DOScale(Vector3.one, loginButtonAnimDuration);
            feideLoginButton.transform.DOScale(Vector3.one, loginButtonAnimDuration);

            yield return new WaitForSeconds(loginButtonAnimDuration);

            titleDescriptionLabel.DOFade(1f, loginButtonTextAnimDuration);
            titleLoginLabel.DOFade(1f, loginButtonTextAnimDuration);
            ALBCanvasGroup.DOFade(1f, loginButtonTextAnimDuration);
            ALBCanvasGroup.GetComponent<RectTransform>().DOAnchorPos(ALBAdditionalAnchor.anchoredPosition, loginButtonTextAnimDuration);
            //orLoginWithLabel.DOFade(1f, logoTextAnimDuration);
            //orLoginWithLabel.transform.DOLocalMove(orLoginTextPosition, logoTextAnimDuration);
        }

        private void StarsAnimation()
        {
            foreach (RectTransform star in backgroundStars)
            {
                star.DOScale(starScaleChangingValue, starAnimDuration).SetDelay(UnityEngine.Random.Range(0.0f, starAnimStartingDelay)).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private IEnumerator BackgroudAnimation()
        {
            fakeBG.color = Vector4.one;
            fakeBG.DOFade(0f, fakeBGAnimDuration);

            bgCharacterUpLeft.alpha = 0f;
            bgCharacterUpRight.alpha = 0f;

            yield return new WaitForSeconds(waitCharacterAnimDelay);

            void SetAnimMove(Transform tr, Vector3 startPosition, float duration)
            {
                Vector3 position = tr.localPosition;
                tr.localPosition = startPosition;
                tr.DOLocalMove(position, duration);
            }

            SetAnimMove(bgCharacterUpLeft.transform, bgCharacterUpLeftPoint.localPosition, bgCharacterUpLeftAnimDuration);
            SetAnimMove(bgCharacterUpRight.transform, bgCharacterUpRightPoint.localPosition, bgCharacterUpRightAnimDuration);

            bgCharacterUpLeft.DOFade(1f, bgCharacterUpLeftAnimDuration);
            bgCharacterUpRight.DOFade(1f, bgCharacterUpRightAnimDuration);
        }
        #endregion
    }
}