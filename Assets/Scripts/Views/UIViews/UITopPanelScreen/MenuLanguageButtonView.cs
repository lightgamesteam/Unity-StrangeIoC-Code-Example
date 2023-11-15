using Assets.Scripts.Services.Analytics;
using Conditions;
using DG.Tweening;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class MenuLanguageButtonView : BaseView
    {
        [SerializeField] private Button item;
        [SerializeField] private Image flagImage;
        [SerializeField] private GameObject outline;

        [Header("Animations params")]
        [SerializeField, Range(0f, 5f)] private float languagesButtonsAnimDuration;
        [SerializeField] private Vector2 languagesButtonsSelectedScale;
        [SerializeField] private Vector2 languagesButtonsUnselectedScale;

        public Languages Language { get; private set; }
        public bool IsSelected { get; private set; }

        public void LoadView()
        {
            item.onClick.AddListener(OnItemClick);
        }

        public void RemoveView()
        {
            item.onClick.RemoveAllListeners();
        }

        public void InitItem(Languages language)
        {
            this.Language = language;
            name = Language + "Button";
            flagImage.sprite = PoolModel.Instance.LanguagesImages[language];
            ResetItem();
            if (LanguagesModel.SelectedLanguage == language)
            {
                SelectItem();
            }
        }

        public void ResetItem()
        {
            IsSelected = false;
            ScaleDown();
        }

        private void OnItemClick()
        {
            SelectItem();
            Dispatcher.Dispatch(EventGlobal.E_LocalizationSelected, this);
        }

        public void SelectItem()
        {
            if (IsSelected)
            {
                return;
            }

            IsSelected = !IsSelected;
            ScaleUp();
            LanguagesModel.SelectedLanguage = Language;

            Analytics.Instance.LogEvent(EventName.ActionSwitchUILanguage,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                    { Property.Translation, LanguagesModel.SelectedLanguage.ToDescription()},
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.Country, PlayerPrefsModel.CountryCode},
                });
        }

        private void ScaleUp()
        {
            item.transform.DOScale(languagesButtonsSelectedScale, languagesButtonsAnimDuration);
            outline.SetActive(true);
        }

        private void ScaleDown()
        {
            item.transform.DOScale(languagesButtonsUnselectedScale, languagesButtonsAnimDuration);
            outline.SetActive(false);
        }
    }
}