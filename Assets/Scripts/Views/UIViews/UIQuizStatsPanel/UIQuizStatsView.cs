using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Statistics;
using PFS.Assets.Scripts.Views.Components;

namespace PFS.Assets.Scripts.Views.QuizStats
{
    public class UIQuizStatsView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; set; }
        [Inject] public QuizStatisticsModel QuizStatModel { get; set; }

        [Header("QUIZ Statistic Area")]
        [SerializeField] private GameObject dropdownContent;
        [SerializeField] private GameObject dropdownItem;
        [SerializeField] private TextMeshProUGUI quizStatLable;
        [SerializeField] private TextMeshProUGUI selectWeekLable;
        [SerializeField] private GameObject diagramContent;
        [SerializeField] private GameObject diagramItem;

        private List<GameObject> columns = new List<GameObject>();
        private int currentWeek = 0;

        public void LoadView()
        {
            SetLocalization();
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_SetQuizStatsChild, UpdateTable);
            //QuizStatModel.SetTimeSlice(currentWeek);

            // Set default week
            if (QuizStatModel.TimeSlice == -1)
            {
                UIQuizStatsView currentChoise = GameObject.Find("QUIZStatisticsArea").GetComponent<UIQuizStatsView>();
                currentChoise.SetDropDownVariants(0);
                Dispatcher.Dispatch(EventGlobal.E_GetQuizStatsChild, new QuizStatRequestModel(1,
                () =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_SetQuizStatsChild);
                    QuizStatModel.TimeSlice = 0;
                },
                () =>
                {
                    Debug.Log("Request false action");

                }));
            }
            else
            {
                UIQuizStatsView currentChoise = GameObject.Find("QUIZStatisticsArea").GetComponent<UIQuizStatsView>();
                currentChoise.SetDropDownVariants(QuizStatModel.TimeSlice - 1);
                Dispatcher.Dispatch(EventGlobal.E_GetQuizStatsChild, new QuizStatRequestModel(QuizStatModel.TimeSlice,
                () =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_SetQuizStatsChild);
                },
                () =>
                {
                    Debug.Log("Request false action");

                }));
            }
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_SetQuizStatsChild, UpdateTable);
        }

        public void InitializePanel()
        {
            // Create new points
            for (int i = 0; i < TimeSlicesKeys.Length; i++)
            {
                var item = Instantiate(dropdownItem, dropdownContent.transform);
                columns.Add(item);
                item.GetComponent<UIDropdownItemView>().number = i;
            }
            //SetDropDownVariants(0);
        }

        private void SetLocalization()
        {
            quizStatLable.text = LocalizationManager.GetLocalizationText(QUIZStatisticsKey);

            if (TimeSlicesKeys.Length > currentWeek && currentWeek > -1)
            {
                selectWeekLable.text = LocalizationManager.GetLocalizationText(TimeSlicesKeys[currentWeek]);
            }
        }

        public void SetDropDownVariants(int id)
        {
            UIDropdownItemView[] choiseObj = dropdownContent.GetComponentsInChildren<UIDropdownItemView>();

            for (int i = 0; i < choiseObj.Length; i++)
            {
                if (id == i)
                {
                    choiseObj[i].GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
                    choiseObj[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color32(63, 63, 63, 255);
                }
                else
                {
                    choiseObj[i].GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                    choiseObj[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color32(171, 171, 171, 255);
                }
            }

            if (TimeSlicesKeys.Length > id && id > -1)
            {
                selectWeekLable.text = LocalizationManager.GetLocalizationText(TimeSlicesKeys[id]);
            }
            currentWeek = id;
        }

        public void UpdateTable()
        {
            // Destroy old columns
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in diagramContent.transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));

            // Create new columns
            for (int i = 0; i < QuizStatModel.Week.Length; i++)
            {
                GameObject point = Instantiate(diagramItem) as GameObject;
                point.transform.SetParent(diagramContent.transform, false);
                point.GetComponent<StatGraphColumn>().Initialize(QuizStatModel.Week[i].Value, QuizStatModel.Week[i].Day);
            }
        }
    }
}