using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Services.Localization;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UINewHomeworksScrollView : BaseView
    {
        [Inject]
        public BooksLibrary BooksLibrary { get; set; }

        [Inject] public ChildStatsModel ChildStatsModel { get; set; }

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI homeworksTitle;
        [SerializeField] private TextMeshProUGUI sorryTitle;

        [Header("Prefabs")]
        [SerializeField] private UINewHomeworkItemView homeworkItemPrefab;

        [Header("Other")]
        [SerializeField] private GameObject loader;
        [SerializeField] private Transform itemsContainer;

        [Header("Materials")]
        [SerializeField] private Material material1;
        [SerializeField] private Material material2;

        [Header("Animations params")]
        [SerializeField, Range(0f, 5f)] private float startDelay;
        [SerializeField, Range(0f, 5f)] private float buildItemDelay;



        public void LoadView()
        {
            if (PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherFeide || PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherLogin)
            {
                this.gameObject.SetActive(false);
                return;
            }
            SetLocalization();
            GetNewHomeworks();
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_UpdateHomeworks, GetNewHomeworks);

        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_UpdateHomeworks, GetNewHomeworks);

            StopAllCoroutines();
        }

        private void GetNewHomeworks()
        {
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }

            loader.gameObject.SetActive(true);

            Dispatcher.Dispatch(EventGlobal.E_GetHomeworks, new HomeworksByStatusRequstModel(Conditions.HomeworkStatus.New, 10,
                (works) =>
                {
                    if (this == null)
                    {
                        return;
                    }
                    StartCoroutine(InitHomeworksScroll(works));
                }, () =>
                {
                    Debug.LogError("GetHomeworks server error");
                }));
        }

        private IEnumerator InitHomeworksScroll(Homework[] newHomeworks)
        {
            loader.gameObject.SetActive(false);
            sorryTitle.gameObject.SetActive(false);
            yield return new WaitForSeconds(startDelay);

            bool chooseMaterial = true;

            for (int i = 0; i < newHomeworks.Length; i++)
            {
                UINewHomeworkItemView item = Instantiate(homeworkItemPrefab, itemsContainer);
                if (item)
                {
                    item.SetHomework(newHomeworks[i], chooseMaterial ? material1 : material2);
                    chooseMaterial = !chooseMaterial;
                }

                yield return new WaitForSeconds(buildItemDelay);
            }
            if (itemsContainer.childCount == 0)
            {
                sorryTitle.gameObject.SetActive(true);
                Dispatcher.Dispatch(EventGlobal.E_GetStatsChild, new BasicRequestModel(() =>
                {
                    sorryTitle.text = GetSorryTitleText();
                }, null));
            }
        }

        private string GetSorryTitleText()
        {
            if(ChildStatsModel == null || ChildStatsModel.Stats == null)
            {
                return string.Empty;
            }

            if (ChildStatsModel.Stats.ReadingTime > 0)
            {
                if (ChildStatsModel.Stats.HomeworksCount == 0)
                {
                    return LocalizationManager.GetLocalizationText(LocalizationKeys.NoHomeworkYetKey);
                }
                return LocalizationManager.GetLocalizationText(LocalizationKeys.SorryActiveTitleKey);
            }
            return LocalizationManager.GetLocalizationText(LocalizationKeys.PickataleWelcomeKey);
        }

        private void SetLocalization()
        {
            homeworksTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.MyHomeworksTitleKey);
            sorryTitle.text = GetSorryTitleText();
        }
    }
}