using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Popups
{
    public class UIFeedbackPopupView : BaseView
    {
        [SerializeField] private GameObject teacherFeedback;
        [SerializeField] private GameObject childFeedback;
        [SerializeField] private RadioGroup teacherGroup;
        [SerializeField] private RadioGroup childGroup;
        [SerializeField] private int feedbackRating = 6;
        [SerializeField] private Button doneButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_InputField feedbackField;


        [Space(20)]
        //Translations
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI desc;
        [SerializeField] TextMeshProUGUI pleaseWrite;
        [SerializeField] TextMeshProUGUI cancel;
        [SerializeField] TextMeshProUGUI done;
        [SerializeField] TextMeshProUGUI bad;
        [SerializeField] TextMeshProUGUI notReally;
        [SerializeField] TextMeshProUGUI good;
        [SerializeField] TextMeshProUGUI excellent;
        [SerializeField] TextMeshProUGUI happy;

        private string userType = string.Empty;
        public void OnChangeRadioFeedback(int newRating)
        {
            feedbackRating = newRating;
        }
        public void OnChangeChildRadioFeedback(int newRating)
        {
            feedbackRating = newRating * 2;
        }

        private void Send()
        {
            SendFeedbackModel model = new SendFeedbackModel(userType + ChildModel.Instance.Email, feedbackField.text, feedbackRating, () => { Debug.Log("Feedback Successful"); }, (e) => { Debug.Log(e.ToString()); });
            Dispatcher.Dispatch(EventGlobal.E_SendFeedback, model);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIFeedbackPopup);
        }

        public void LoadView()
        {
            if (SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForTeacherFeide || SwitchModeModel.Mode == Conditions.GameModes.SchoolModeForTeacherLogin)
            {
                childFeedback.SetActive(false);
                teacherFeedback.SetActive(true);
                userType = "Teacher ";
            }
            else
            {
                childFeedback.SetActive(true);
                teacherFeedback.SetActive(false);
                userType = "Child ";
            }

            teacherGroup.triggeredEvent.AddListener(OnChangeRadioFeedback);
            childGroup.triggeredEvent.AddListener(OnChangeChildRadioFeedback);
            doneButton.onClick.AddListener(Send);
            cancelButton.onClick.AddListener(() => { Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIFeedbackPopup); });

            InitTranslations();
        }

        private void InitTranslations()
        {
            title.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeedbackKey);
            desc.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeedbackDescKey);
            pleaseWrite.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FeedbackPleaseKey);
            cancel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.CancelKey);
            done.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DoneKey);
            bad.text = LocalizationManager.GetLocalizationText(LocalizationKeys.BadKey);
            notReally.text = LocalizationManager.GetLocalizationText(LocalizationKeys.NotReallyKey);
            good.text = LocalizationManager.GetLocalizationText(LocalizationKeys.GoodKey);
            excellent.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ExcellentKey);
            happy.text = LocalizationManager.GetLocalizationText(LocalizationKeys.HappyKey);
        }

        public void RemoveView()
        {
            teacherGroup.triggeredEvent.RemoveAllListeners();
            childGroup.triggeredEvent.RemoveAllListeners();
            doneButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}