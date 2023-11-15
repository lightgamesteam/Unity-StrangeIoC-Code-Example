using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Statistics;
using PFS.Assets.Scripts.Views.QuizStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;

namespace PFS.Assets.Scripts.Views.Components
{
    public class UIDropdownItemView : BaseView
    {
        [SerializeField] private Button item;
        [SerializeField] private TextMeshProUGUI name;
        [HideInInspector] public int number;
        [Inject] public QuizStatisticsModel QuizStatModel { get; set; }
        public bool IsSelected { get; private set; }

        public void LoadView()
        {
            item.onClick.AddListener(ClickOnItem);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            SetLocalizedTexts();
        }

        public void RemoveView()
        {
            item.onClick.RemoveAllListeners();
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
        }

        private void SetLocalizedTexts()
        {
            if (TimeSlicesKeys.Length > number && number > -1)
            {
                name.text = LocalizationManager.GetLocalizationText(TimeSlicesKeys[number]);
            }
        }


        private void ClickOnItem()
        {
            UIQuizStatsView currentChoise = GameObject.Find("QUIZStatisticsArea").GetComponent<UIQuizStatsView>();
            currentChoise.SetDropDownVariants(number);
            Dispatcher.Dispatch(EventGlobal.E_GetQuizStatsChild, new QuizStatRequestModel(number + 1,
            () =>
            {
                Dispatcher.Dispatch(EventGlobal.E_SetQuizStatsChild);
                QuizStatModel.TimeSlice = number + 1;
            },
            () =>
            {
                Debug.Log("Request false action");

            }));
        }
    }
}