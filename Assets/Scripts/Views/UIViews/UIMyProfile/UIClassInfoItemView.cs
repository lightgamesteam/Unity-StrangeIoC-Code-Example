using UnityEngine;
using TMPro;
using DG.Tweening;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.MyProfile
{
    public class UIClassInfoItemView : BaseView
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI classTitle;
        [SerializeField] private TextMeshProUGUI studentsCount;
        [SerializeField] private TextMeshProUGUI newHomeworksCount;
        [SerializeField] private TextMeshProUGUI doneHomeworksCount;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI studentsLabel;
        [SerializeField] private TextMeshProUGUI newHomeworksLabel;
        [SerializeField] private TextMeshProUGUI doneHomeworksLabel;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float itemAnimDuration;

        private ClassModel classInfo;

        public void LoadView()
        {
            SetItemAnimation();

            if (classInfo == null)
            {
                Debug.LogError("UIClassInfoItemView => LoadView => class - null");
                return;
            }

            InitClassInfo();
            SetLocalization();

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void SetClassInfo(ClassModel classInfo)
        {
            if (classInfo == null)
            {
                Debug.LogError("UIClassInfoItemView => SetClass => class - null");
                return;
            }

            this.classInfo = classInfo;
        }

        private void InitClassInfo()
        {
            classTitle.text = classInfo.name;
            studentsCount.text = classInfo.studentsCount.ToString();
            newHomeworksCount.text = (classInfo.allHomeworksCount - classInfo.doneHomeworksCount).ToString();
            doneHomeworksCount.text = classInfo.doneHomeworksCount.ToString();
        }

        private void SetLocalization()
        {
            studentsLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.StudentsKey);
            doneHomeworksLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.FinishKey);
            newHomeworksLabel.text = LocalizationManager.GetLocalizationText(LocalizationKeys.HomeworksKey);
        }

        #region Animation
        private void SetItemAnimation()
        {
            transform.localScale = new Vector3(0.01f, 0.01f, 1f);
            transform.DOScale(Vector3.one, itemAnimDuration);
        }
        #endregion
    }
}