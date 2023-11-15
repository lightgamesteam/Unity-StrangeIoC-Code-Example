using PFS.Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.MainMenu
{
    public class UIBooksCategoryScrollsView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [Header("UI")]
        [SerializeField] private ScrollRect mainScroll;

        [Header("Prefab")]
        [SerializeField] private UIBooksCategoryScrollView scrollPrefab;

        [Header("Params")]
        [SerializeField, Range(0f, 1f)] private float verticalNormalizedPositionDelta;

        private int categoryPos = 0;
        private int categoriesCount;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            categoriesCount = child.CategoriesForStrategy.Length;
        }

        public void RemoveView()
        {

        }

        private void Update()
        {
            if (mainScroll.verticalNormalizedPosition <= verticalNormalizedPositionDelta)
            {
                if (categoryPos < categoriesCount)
                {
                    UIBooksCategoryScrollView scrollItem = Instantiate(scrollPrefab, mainScroll.content);
                    scrollItem.ScrollCategory = child.GetBookCategory(categoryPos);

                    categoryPos++;
                }
            }
        }
    }
}