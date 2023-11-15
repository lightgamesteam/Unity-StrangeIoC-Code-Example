using Conditions;
using PFS.Assets.Scripts.Models.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class LanguageItemView : BaseView
    {
        [SerializeField] private Button item;
        [SerializeField] private Image flagImage;
        [SerializeField] private GameObject checkmark;

        [Inject] public PoolModel Pool { get; private set; }
        public Languages Language { get; private set; }
        public bool IsSelected { get; private set; }

        public void LoadView()
        {
            item.onClick.AddListener(ClickOnItem);
        }

        public void RemoveView()
        {
            item.onClick.RemoveAllListeners();
        }

        public void InitItem(Languages language)
        {
            this.Language = language;
            checkmark.SetActive(false);
            flagImage.sprite = PoolModel.Instance.LanguagesImages[language];
        }

        public void ResetItem()
        {
            IsSelected = false;
            checkmark.SetActive(false);
        }

        private void ClickOnItem()
        {
            IsSelected = !IsSelected;
            checkmark.SetActive(IsSelected);

            Dispatcher.Dispatch(EventGlobal.E_ProcessBooksSearch);
        }
    }
}