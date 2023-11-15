using Conditions;
using PFS.Assets.Scripts.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class StageItemView : BaseView
    {

        [SerializeField] private Button item;
        [SerializeField] private Image colorImage;
        [SerializeField] private TMP_Text iconLabel;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject checkmark;
        [SerializeField] private List<Colors> colors;
        public SimplifiedLevels Stage { get; private set; }
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

        public void InitItem(SimplifiedLevels stage)
        {
            this.Stage = stage;
            colorImage.color = colors.Where(c => c.stage == stage).FirstOrDefault().color;
            checkmark.SetActive(false);
        }

        public void ResetItem()
        {
            IsSelected = false;
            checkmark.SetActive(false);
        }

        private void SetLocalizedTexts()
        {
            string firstLocalizedLetter = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey).Substring(0, 1);
            string stageNumber = Stage.ToString().Substring(Stage.ToString().Length - 1, 1);
            string localizedStageLabel = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey);
            iconLabel.text = firstLocalizedLetter + stageNumber;
            label.text = localizedStageLabel + " " + stageNumber;
        }

        private void ClickOnItem()
        {
            IsSelected = !IsSelected;
            checkmark.SetActive(IsSelected);

            Dispatcher.Dispatch(EventGlobal.E_ProcessBooksSearch);
        }

        [Serializable]
        public struct Colors
        {
            public Color color;
            public Conditions.SimplifiedLevels stage;
        }
    }
}