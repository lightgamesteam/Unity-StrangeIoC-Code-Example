using Conditions;
using PFS.Assets.Scripts.Services.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizPausePopupView : BaseView
    {
        [Header("UI")]
        public Button buttonContinue;
        public TMPro.TextMeshProUGUI continueText;
        public TMPro.TextMeshProUGUI pauseText;

        public void LoadView()
        {
            SetLocalization();
            buttonContinue.onClick.AddListener(Unpause);

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void Unpause()
        {
            Time.timeScale = 1.0f;

            Dispatcher.Dispatch(EventGlobal.E_SoundUnPauseAll);

            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIQuizPausePopup);
        }

        private void SetLocalization()
        {
            var language = GetQuizeLanguage();
            if (language == Languages.None)
            {
                continueText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ContinueKey);
                pauseText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.PauseKey);
                Debug.Log("Localization with the default language.");
                return;
            }

            continueText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ContinueKey, language.ToString());
            pauseText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.PauseKey, language.ToString());
            Debug.Log("Localization with the custom language.");
        }

        private Languages GetQuizeLanguage()
        {
            Languages language = Languages.None;
            if (otherData is Languages)
            {
                language = (Languages)otherData;
            }
            return language;
        }
    }
}