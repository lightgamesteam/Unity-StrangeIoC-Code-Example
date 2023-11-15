using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Login
{
    public class UIJoinClassScreenView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI joinClassTitile;
        [SerializeField] private TextMeshProUGUI enterDataTitle;
        [SerializeField] private TextMeshProUGUI termsButtonText;
        [SerializeField] private TextMeshProUGUI policyButtonText;

        [SerializeField] private TextMeshProUGUI joinButtonText;
        [SerializeField] private TextMeshProUGUI skipText;

        [Header("UI")]
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private TMP_InputField classCodeInput;
        [SerializeField] private Image firstNameErrorOutline;
        [SerializeField] private Image classCodeErrorOutline;
        [SerializeField] private Button joinClassButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button termsButton;
        [SerializeField] private Button policyButton;
        [Space(10)]
        [SerializeField] private CanvasGroup jounClassCanvasGroup;
        [SerializeField] private CanvasGroup congratsCanvasGroup;

        [Header("Animation params")]
        [SerializeField, Range(0f, 1f)] private float createdAccAnimDuration;
        [SerializeField, Range(0f, 1f)] private float congratulationsDelay;

        public void LoadView()
        {
            Dispatcher.Dispatch(EventGlobal.E_Reinitlocalization);
            SetLocalization();
            ResetInputFields();

            congratsCanvasGroup.gameObject.SetActive(false);
            Analytics.LogEvent(EventName.NavigationSignUp,
                    new System.Collections.Generic.Dictionary<Property, object>()
                    {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                    });

            skipButton.onClick.AddListener(() =>
            {
                StartCoroutine(ProcessFinishAnimation());
                Analytics.LogEvent(EventName.ActionWithSignUp,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.ClassId, "-"},
                    { Property.Skip, "Yes"}
                });
            });
            termsButton.onClick.AddListener(() =>
            {
                Analytics.LogEvent(EventName.NavigationTermsAndConditions,
                        new System.Collections.Generic.Dictionary<Property, object>()
                        {
                                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                        });
                Application.OpenURL(LocalizationManager.GetLocalizationText(LocalizationKeys.TermsAndConditionsLinkKey));
            });
            policyButton.onClick.AddListener(() =>
            {
                Analytics.LogEvent(EventName.NavigationPrivacyPolicy,
                        new System.Collections.Generic.Dictionary<Property, object>()
                        {
                                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                        });
                Application.OpenURL(LocalizationManager.GetLocalizationText(LocalizationKeys.PrivacyPolicyLinkKey));
            });
            joinClassButton.onClick.AddListener(ProcessJoinClass);
            firstNameInput.onSelect.AddListener((string value) => ResetInputFields());
            classCodeInput.onSelect.AddListener((string value) => ResetInputFields());
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            StopAllCoroutines();
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private IEnumerator ProcessFinishAnimation()
        {
            congratsCanvasGroup.gameObject.SetActive(true);
            congratsCanvasGroup.alpha = 0.0f;

            jounClassCanvasGroup.DOFade(0.0f, createdAccAnimDuration);
            yield return new WaitForSeconds(createdAccAnimDuration);

            congratsCanvasGroup.DOFade(1.0f, createdAccAnimDuration);
            yield return new WaitForSeconds(congratulationsDelay);

            OpenMenuScreen();
        }

        private void OpenMenuScreen()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIJoinClassScreen);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UITopPanelScreen, isAddToScreensList = false });
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, UIScreens.UIMainMenu);
        }

        private void SetLocalization()
        {
            joinClassTitile.text = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinInClassKey);
            enterDataTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterClassInfoKey);
            joinButtonText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.JoinKey);
            skipText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.SkipKey);
            termsButtonText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.TermsAndConditionsKey);
            policyButtonText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.PrivacyPolicyKey);
        }

        private void ResetInputFields()
        {
            classCodeErrorOutline.gameObject.SetActive(false);
            firstNameErrorOutline.gameObject.SetActive(false);

            var firstNamePlaceholder = (firstNameInput.placeholder as TextMeshProUGUI);
            firstNamePlaceholder.color = Color.gray;
            firstNamePlaceholder.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FirstNamePlaceholderKey);

            var classCodePlaceholder = (classCodeInput.placeholder as TextMeshProUGUI);
            classCodePlaceholder.color = Color.gray;
            classCodePlaceholder.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ClassCodePlaceholderKey);
        }

        private void ProcessJoinClass()
        {
            if (firstNameInput.text == string.Empty)
            {
                ProcessInputFieldWarning(firstNameInput, firstNameErrorOutline, LocalizationKeys.EmptyFieldError);
                return;
            }

            if (classCodeInput.text == string.Empty)
            {
                ProcessInputFieldWarning(classCodeInput, classCodeErrorOutline, LocalizationKeys.EmptyFieldError);
                return;
            }

            GetChildByClassCodeRequestModel childCodeModel = new GetChildByClassCodeRequestModel(classCodeInput.text, firstNameInput.text,
            () =>
            {
                StartCoroutine(ProcessFinishAnimation());
                Analytics.LogEvent(EventName.ActionWithSignUp,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.ClassId, classCodeInput.text},
                    { Property.Skip, "No"}
                });
            },
            (string errorKeyCode) =>
            {
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                ProcessInputFieldWarning(classCodeInput, classCodeErrorOutline, errorKeyCode);
            });

            Dispatcher.Dispatch(EventGlobal.E_GetChildDataByClassCode, childCodeModel);
        }

        private void ProcessInputFieldWarning(TMP_InputField field, Image errorOutLine, string errorKeyCode)
        {
            errorOutLine.gameObject.SetActive(true);
            field.SetTextWithoutNotify(string.Empty);

            var placeholder = (field.placeholder as TextMeshProUGUI);
            placeholder.color = errorOutLine.color;
            placeholder.text = LocalizationManager.GetLocalizationText(errorKeyCode);
        }
    }
}