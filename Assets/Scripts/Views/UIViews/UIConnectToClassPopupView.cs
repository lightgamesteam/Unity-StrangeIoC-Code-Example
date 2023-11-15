using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Popups
{
    public class UIConnectToClassPopupView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI subscribeTitle;
        [SerializeField] private TextMeshProUGUI enterClassCodeTitle;

        [Header("UI")]
        [SerializeField] private Button close;
        [SerializeField] private Button subscribe;
        [SerializeField] private TMP_InputField codeInput;
        [Space(10)]
        [SerializeField] private Image errorOutline;
        [SerializeField] private TextMeshProUGUI errorTitle;

        public void LoadView()
        {
            SetLocalization();
            ResetInputField();

            close.onClick.AddListener(Close);
            subscribe.onClick.AddListener(Subscribe);
            codeInput.onSelect.AddListener((string value) => ResetInputField());

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void Close()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UIConnectToClassPopup, showSwitchAnim = false });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void Subscribe()
        {
            if (codeInput.text == string.Empty)
            {
                ProcessError(LocalizationKeys.EmptyFieldError);
                return;
            }

            GetChildByClassCodeRequestModel childCodeModel = new GetChildByClassCodeRequestModel(codeInput.text, null,
                () =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UIConnectToClassPopup, showSwitchAnim = false });
                    Dispatcher.Dispatch(EventGlobal.E_ResetChildClasses);
                    Dispatcher.Dispatch(EventGlobal.E_UpdateChildClasses);

                    Analytics.LogEvent(EventName.NavigationConnectToNewClass,
                          new System.Collections.Generic.Dictionary<Property, object>()
                          {
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.ClassId, codeInput.text}
                          });
                },
                (string errorKeyCode) =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                    ProcessError(errorKeyCode);
                });

            Dispatcher.Dispatch(EventGlobal.E_GetChildDataByClassCode, childCodeModel);
        }

        private void SetLocalization()
        {
            subscribeTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.OkKey);
            enterClassCodeTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.EnterClassCodeKey);

            (codeInput.placeholder as TextMeshProUGUI).text = LocalizationManager.GetLocalizationText(LocalizationKeys.CodeKey);
        }

        private void ResetInputField()
        {
            errorTitle.gameObject.SetActive(false);
            errorOutline.gameObject.SetActive(false);
        }

        private void ProcessError(string errorKeyCode)
        {
            errorOutline.gameObject.SetActive(true);

            if (!string.IsNullOrEmpty(errorKeyCode))
            {
                errorTitle.gameObject.SetActive(true);
                errorTitle.text = LocalizationManager.GetLocalizationText(errorKeyCode);
            }
        }
    }
}