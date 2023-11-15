using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Purchase
{
    public class UISubscriptionView : BaseView
    {
        [Inject] public Analytics Analytics { get; set; }

        [Header("Localization Titles")]
        [SerializeField] private TextMeshProUGUI joinClassTitle;
        [SerializeField] private TextMeshProUGUI inviteCodeDesctiptionTitle;
        [SerializeField] private TextMeshProUGUI doneButtonTitle;
        [SerializeField] private TextMeshProUGUI termsAndConditionsTitle;
        [SerializeField] private TextMeshProUGUI privacyPolicyTitle;
        [SerializeField] private TextMeshProUGUI orTitle;

        [Space(10)]
        [SerializeField] private string privacyPolicyLink = "";
        [SerializeField] private string termAndServicesLink = "";
        [SerializeField] private Button back;
        [SerializeField] private Button termAndServices;
        [SerializeField] private Button privacyPolicy;

        [SerializeField] private Button done;
        [SerializeField] private TMP_InputField codeInput;
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private Image firstNameErrorOutline;
        [SerializeField] private Image classCodeErrorOutline;

        [SerializeField] private GameObject middleLine;
        [SerializeField] private Action previousAction;
        private string isoCurrencyCode;

        [Inject] public ScreenManager ScreenManager { get; set; }
        [Inject] public ChildModel ChildModel { get; private set; }

        public void LoadView()
        {
            SetLocalization();
            InitScreenState();
            ResetInputFields();

            firstNameInput.onSelect.AddListener((string value) => ResetInputFields());
            codeInput.onSelect.AddListener((string value) => ResetInputFields());
            back.onClick.AddListener(Back);
            termAndServices.onClick.AddListener(OpenTermAndServices);
            privacyPolicy.onClick.AddListener(OpenPrivacyPolicy);
            done.onClick.AddListener(OnDone);
            if (otherData != null)
            {
                previousAction = otherData as Action;
            }

            if (Dispatcher == null)
            {
                Debug.LogError($"{nameof(UISubscriptionView)} => {nameof(LoadView)} => Dispatcher is NULL");
                return;
            }

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);

            if (Analytics == null)
            {
                Debug.LogError($"{nameof(UISubscriptionView)} => {nameof(LoadView)} => Analytics is NULL");
                return;
            }

            Analytics.LogEvent(EventName.NavigationSubscribeOrJoinAClass,
              new Dictionary<Property, object>()
              {
                      { Property.Uuid, PlayerPrefsModel.CurrentChildId}
              });
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }


        private void InitScreenState()
        {
            Debug.Log("PlayerPrefsModel.CountryCode =" + PlayerPrefsModel.CountryCode);
            bool isLoginPasswordMode = SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForChildLogin;
            bool isBritain = PlayerPrefsModel.CountryCode == Conditions.CountryCodes.GreatBritain.ToDescription();
            bool isHide = isLoginPasswordMode && isBritain;
            Debug.Log("isLoginPasswordMode = " + isLoginPasswordMode);
            Debug.Log("isBritain = " + isBritain);
            Debug.Log("isHide = " + isHide);

            // Hide on PS player
            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.LinuxPlayer ||
                Application.platform == RuntimePlatform.WSAPlayerARM ||
                 Application.platform == RuntimePlatform.WSAPlayerX64 ||
                 Application.platform == RuntimePlatform.WSAPlayerX86)

            {
                isHide = true;
            }

            //middleLine.gameObject.SetActive(!isHide);
        }

        private void OnDone()
        {
            if (firstNameInput.text == string.Empty)
            {
                ProcessInputFieldWarning(firstNameInput, firstNameErrorOutline, LocalizationKeys.EmptyFieldError);
                return;
            }

            if (codeInput.text == string.Empty)
            {
                ProcessInputFieldWarning(codeInput, classCodeErrorOutline, LocalizationKeys.EmptyFieldError);
                return;
            }
            GetChildByClassCodeRequestModel childCodeModel = new GetChildByClassCodeRequestModel(codeInput.text, firstNameInput.text,
            () =>
            {
                previousAction?.Invoke();
            //PlayerPrefsModel.CurrectChildSchoolCode = codeInput.text;
            Analytics.LogEvent(EventName.ActionJoinClass,
                     new System.Collections.Generic.Dictionary<Property, object>()
                     {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.ClassId, codeInput.text}
                     });
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.SubscriptionScreen);

            },
            (string errorKeyCode) =>
            {
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                ProcessInputFieldWarning(codeInput, classCodeErrorOutline, errorKeyCode);
            });

            Dispatcher.Dispatch(EventGlobal.E_GetChildDataByClassCode, childCodeModel);
        }

        private void ResetInputFields()
        {
            classCodeErrorOutline.gameObject.SetActive(false);
            firstNameErrorOutline.gameObject.SetActive(false);

            var firstNamePlaceholder = (firstNameInput.placeholder as TextMeshProUGUI);
            firstNamePlaceholder.color = Color.white;
            firstNamePlaceholder.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FirstNameKey);

            var classCodePlaceholder = (codeInput.placeholder as TextMeshProUGUI);
            classCodePlaceholder.color = Color.white;
            classCodePlaceholder.text = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterCodeKey);
        }

        private void ProcessInputFieldWarning(TMP_InputField field, Image errorOutLine, string errorKeyCode)
        {
            errorOutLine.gameObject.SetActive(true);
            field.SetTextWithoutNotify(string.Empty);

            var placeholder = (field.placeholder as TextMeshProUGUI);
            placeholder.color = errorOutLine.color;
            placeholder.text = LocalizationManager.GetLocalizationText(errorKeyCode);
        }

        private void Back()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.SubscriptionScreen);
        }

        private void OpenTermAndServices()
        {
            Application.OpenURL(LocalizationManager.GetLocalizationText(LocalizationKeys.TermsAndConditionsLinkKey));
        }

        private void OpenPrivacyPolicy()
        {
            Application.OpenURL(LocalizationManager.GetLocalizationText(LocalizationKeys.PrivacyPolicyLinkKey));
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.SubscriptionScreen);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, "SubscriptionDiscountScreen");
        }

        private void SetLocalization()
        {
            joinClassTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinClassKey);

            inviteCodeDesctiptionTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.InviteCodeDesctiptionKey);
            doneButtonTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DoneKey);
            termsAndConditionsTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.TermsAndConditionsKey);
            privacyPolicyTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.PrivacyPolicyKey);
            orTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.OrKey);
        }
    }

}