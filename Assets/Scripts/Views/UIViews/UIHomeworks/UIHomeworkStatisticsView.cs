using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.MyProfile
{
    public class UIHomeworkStatisticsView : BaseView
    {
        [Header("Titles")]
        [SerializeField] private TextMeshProUGUI statisticsTitle;
        [SerializeField] private TextMeshProUGUI doneHomeworksTitle;
        [SerializeField] private TextMeshProUGUI timeSpentTitle;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI doneHomeworksText;
        [SerializeField] private TextMeshProUGUI spentTimeText;
        [SerializeField] private List<TextMeshProUGUI> receivedStarsTexts;

        [Header("Other")]
        [SerializeField] private GameObject loader;
        [SerializeField] private GameObject body;

        private HomeworksStatsModel homeworksStats;

        public void LoadView()
        {
            SetLocalization();

            body.gameObject.SetActive(false);
            loader.gameObject.SetActive(true);

            Dispatcher.Dispatch(EventGlobal.E_GetHomeworksStatsCommand, new HomeworksStatsRequestModel(Init, null));
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        private void Init(HomeworksStatsModel stats)
        {
            if (gameObject == null)
            {
                return;
            }

            body.gameObject.SetActive(true);
            loader.gameObject.SetActive(false);

            homeworksStats = stats;

            if (homeworksStats == null)
            {
                Debug.LogError("UIHomeworks -> StatisticsView -> HomeworkStats = null");
                return;
            }

            doneHomeworksText.text = homeworksStats.homeworksDone.ToString();
            InitReceivedStarsTexts();
            InitSpentTime();
        }

        private void SetLocalization()
        {
            statisticsTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.StatisticsKey);
            doneHomeworksTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.DoneHomeworksKey) + ":";
            timeSpentTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.SpentTimeKey) + ":";
        }

        private void InitReceivedStarsTexts()
        {
            if (homeworksStats.receivedStarsCounts.Length == receivedStarsTexts.Count)
            {
                for (int i = 0; i < receivedStarsTexts.Count; i++)
                {
                    receivedStarsTexts[i].text = homeworksStats.receivedStarsCounts[i].ToString();
                }
            }
            else
            {
                for (int i = 0; i < receivedStarsTexts.Count; i++)
                {
                    receivedStarsTexts[i].text = "0";
                }
            }
        }

        private void InitSpentTime()
        {
            System.TimeSpan spentTime = System.TimeSpan.FromSeconds(homeworksStats.totalSpentTime);

            if (spentTime.Days == 0)
            {
                spentTimeText.text = string.Format("{0}h {1}min", spentTime.Hours, spentTime.Minutes);
            }
            else
            {
                spentTimeText.text = string.Format("{0}d {1}h {2}min", spentTime.Days, spentTime.Hours, spentTime.Minutes);
            }
        }
    }
}