using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class InterestItemView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [SerializeField] private Button item;
        [SerializeField] private TMP_Text name;
        [SerializeField] private GameObject checkmark;
        public BooksCategory Interest { get; private set; }
        public bool IsSelected { get; private set; }

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);

            item.onClick.AddListener(ClickOnItem);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
            SetLocalizedTexts();
        }

        public void RemoveView()
        {
            item.onClick.RemoveAllListeners();
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
        }

        public void InitItem(BooksCategory interest)
        {
            Interest = interest;
            checkmark.SetActive(false);
        }

        public void ResetItem()
        {
            IsSelected = false;
            checkmark.SetActive(false);
        }

        private void SetLocalizedTexts()
        {
            name.text = child.GetBookCategory(Interest.Position).GetCategoryName();
        }

        private void ClickOnItem()
        {
            IsSelected = !IsSelected;
            checkmark.SetActive(IsSelected);

            Dispatcher.Dispatch(EventGlobal.E_ProcessBooksSearch);
        }
    }
}