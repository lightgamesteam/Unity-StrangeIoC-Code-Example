using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Popups
{
    public class UIUniversalPopupView : BaseView
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button okButton;
        [SerializeField] private Button closeButton;
        private PopupModel popupModel;

        public void LoadView()
        {

            if (otherData != null)
            {
                popupModel = otherData as PopupModel;
            }

            if (popupModel != null)
            {
                if (!string.IsNullOrEmpty(popupModel.Title))
                {
                    title.text = LocalizationManager.GetLocalizationText(popupModel.Title);
                }

                if (!string.IsNullOrEmpty(popupModel.Description))
                {
                    if (popupModel.Description.Equals(LocalizationKeys.HereKey))
                    {
                        description.text = string.Format("{0}{1}{2}{3}", LocalizationManager.GetLocalizationText(LocalizationKeys.PasswordErrorHelpTeacherKey), "<link=\"https://teacher.pickatale.school/forgot-password/\"><u>", LocalizationManager.GetLocalizationText(LocalizationKeys.HereKey), "</u></link>");
                    }
                    else
                    {
                        description.text = LocalizationManager.GetLocalizationText(popupModel.Description);
                    }
                }

                if (!string.IsNullOrEmpty(popupModel.ButtonText))
                {
                    buttonText.text = LocalizationManager.GetLocalizationText(popupModel.ButtonText);
                }

                if (popupModel.IsActiveCloseButton)
                {
                    closeButton.onClick.AddListener(() => Dispatcher.Dispatch(EventGlobal.E_HideScreen, gameObject.name));
                }

                okButton.onClick.AddListener(() => OnOkClick(popupModel.Callback, popupModel.IsCloaseAfterAction));
            }
        }


        public void RemoveView()
        {
            closeButton.onClick.RemoveAllListeners();
            okButton.onClick.RemoveAllListeners();
        }

        private void OnOkClick(Action action, bool isCloaseAfterAction)
        {
            action?.Invoke();
            if (isCloaseAfterAction)
            {
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, gameObject.name);
            }
        }
    }
}