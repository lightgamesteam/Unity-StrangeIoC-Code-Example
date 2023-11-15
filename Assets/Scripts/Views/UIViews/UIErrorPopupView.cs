using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Popups
{
    public class UIErrorPopupView : BaseView
    {
        public GameObject popup;
        public GameObject feedbackPopup;
        public TextMeshProUGUI content;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI feedContent;
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI feedTitleText;
        public Button ok;
        public Button feedOk;
        public Button cancel;
        [SerializeField] private TextMeshProUGUI pleaseWrite;
        [SerializeField] private TMP_InputField descInput;

        [Header("Wait Time")]
        [SerializeField] private int waitBaseTime;
        private float waitCurrentTime = 0;
        private string userType = string.Empty;

        public void LoadView()
        {
            if (SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForTeacherFeide || SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForTeacherLogin)
            {
                userType = "Teacher ";
            }
            else
            {
                userType = "Child ";
            }

            GetData();
            InitContent();
            InitLocalization();
            ok.onClick.AddListener(YesProcess);
            feedOk.onClick.AddListener(YesProcess);
            cancel.onClick.AddListener(() => { Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIErrorPopup); });
        }

        public void RemoveView()
        {
            ok.onClick.RemoveAllListeners();
            feedOk.onClick.RemoveAllListeners();
            cancel.onClick.RemoveAllListeners();
        }

        private void InitLocalization()
        {
            titleText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.TechnicalProblemKey);
            feedTitleText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.TechnicalProblemKey);
            pleaseWrite.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DescribeProblemKey);
        }

        private void GetData()
        {
            if (otherData != null)
            {
                errorText.text = otherData.ToString();
            }
        }

        private void InitContent()
        {
            //visual ignore server problem
            popup.SetActive(false);

            SetText(Conditions.DisconnectEvents.ServerProblem);
        }

        private void SetText(Conditions.DisconnectEvents dEvent)
        {
            switch (dEvent)
            {
                case Conditions.DisconnectEvents.NoInternet:
                    content.text = LocalizationManager.GetLocalizationText(LocalizationKeys.NoInternetErrorKey);
                    popup.SetActive(true);
                    feedbackPopup.SetActive(false);
                    break;
                case Conditions.DisconnectEvents.SlowInternet:
                    content.text = LocalizationManager.GetLocalizationText(LocalizationKeys.SlowInternetErrorKey);
                    popup.SetActive(true);
                    feedbackPopup.SetActive(false);
                    break;
                case Conditions.DisconnectEvents.ServerProblem:
                    feedContent.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ServerProblemKey);
                    popup.SetActive(false);
                    feedbackPopup.SetActive(true);
                    break;
            }
        }

        private void YesProcess()
        {
            if (feedbackPopup.activeSelf == true)
            {
                SendFeedback();
            }
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIErrorPopup);
        }

        private void SendFeedback()
        {
            if(ChildModel.Instance == null)
            {
                Debug.LogError($"{nameof(UIErrorPopupView)} => {nameof(SendFeedback)} => {nameof(ChildModel)}.Instance is null!");
                return;
            }

            if(otherData == null)
            {
                Debug.LogError($"{nameof(UIErrorPopupView)} => {nameof(SendFeedback)} => {nameof(otherData)} is null!");
                return;
            }

            SendFeedbackModel model = new SendFeedbackModel(userType + ChildModel.Instance.Email, descInput.text + "\n" + otherData, 0, () => { Debug.Log("Feedback Successful"); }, (e) => { Debug.Log(e.ToString()); });
            Dispatcher.Dispatch(EventGlobal.E_SendFeedback, model);
        }
    }
}