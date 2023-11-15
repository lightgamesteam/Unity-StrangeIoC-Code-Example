using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Services.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIDoneHomeworksScrollView : BaseView
    {
        [Inject]
        public BooksLibrary BooksLibrary { get; set; }

        [Header("UI")]
        [SerializeField] private TMPro.TextMeshProUGUI homeworksTitle;
        [SerializeField] private TMPro.TextMeshProUGUI sorryTitle;

        [Header("Prefabs")]
        [SerializeField] private UIDoneHomeworkItemView homeworkItemPrefab;

        [Header("Other")]
        [SerializeField] private GameObject loader;
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Materials")]
        [SerializeField] private Material material1;
        [SerializeField] private Material material2;

        [Header("Animations params")]
        [SerializeField, Range(0f, 5f)] private float startDelay;
        [SerializeField, Range(0f, 5f)] private float buildItemDelay;

        public void LoadView()
        {
            GetDoneHomeworks();
            SetLocalization();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_UpdateHomeworks, GetDoneHomeworks);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_UpdateHomeworks, GetDoneHomeworks);
        }

        private void GetDoneHomeworks()
        {
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }

            loader.gameObject.SetActive(true);

            Dispatcher.Dispatch(EventGlobal.E_GetHomeworks, new HomeworksByStatusRequstModel(Conditions.HomeworkStatus.Checked, 10,
                (works) =>
                {
                    if (gameObject != null)
                    {
                        StartCoroutine(InitHomeworksScroll(works));
                    }
                }, () =>
                {
                    Debug.LogError("GetHomeworks server error");
                }));
        }

        private IEnumerator InitHomeworksScroll(Homework[] newHomeworks)
        {
            loader.gameObject.SetActive(false);

            yield return new WaitForSeconds(startDelay);

            bool chooseMaterial = true;

            for (int i = newHomeworks.Length - 1; i >= 0; i--)
            {
                UIDoneHomeworkItemView item = Instantiate(homeworkItemPrefab, itemsContainer);
                if (item)
                {
                    item.SetHomework(newHomeworks[i], chooseMaterial ? material1 : material2);
                    chooseMaterial = !chooseMaterial;
                }

                yield return new WaitForSeconds(buildItemDelay);
            }

            sorryTitle.gameObject.SetActive(itemsContainer.childCount == 0);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }

        private void SetLocalization()
        {
            homeworksTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FinishedHomeworksTitleKey);
            sorryTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.SorryDoneTitleKey);
        }
    }
}