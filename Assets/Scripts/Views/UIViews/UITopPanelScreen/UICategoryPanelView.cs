using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class UICategoryPanelView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [SerializeField] private Image background;
        [SerializeField] private TMP_Text title;
        private BooksCategory category;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalizedTexts);
        }

        public void InitializePanel()
        {
            Debug.Log("Nothing to initialize in Category Panel");
        }

        private void SetLocalizedTexts()
        {
            if (category != null)
            {
                title.text = child.GetBookCategory(category.Position).GetCategoryName();
            }
        }

        public void OpenPanel(BooksCategory category)
        {
            this.category = category;
            gameObject.SetActive(true);
            child.GetBookCategory(category.Position).SetImageForTitle(background);
            SetLocalizedTexts();
        }

        public void ClosePanel()
        {
            this.gameObject.SetActive(false);
        }
    }
}