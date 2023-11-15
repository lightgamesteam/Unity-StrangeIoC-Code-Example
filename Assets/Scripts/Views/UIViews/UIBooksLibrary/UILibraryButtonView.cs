using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UILibraryButtonView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [Header("UI")]
        [SerializeField] private Button libraryButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Animation animationButton;

        [Space(5)]
        [SerializeField] private string animName;

        public static BooksCategory GlobalLibraryCategory { get; private set; }
        public BooksCategory ButtonCategory { get; private set; }
        private bool selected = false;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);

            SetButtonImage();
            SetLocalization();

            libraryButton.onClick.AddListener(ButtonClick);

            Dispatcher.AddListener(EventGlobal.E_LibraryCategoriesButtonClick, DisableSelectedState);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        public void RemoveView()
        {
            libraryButton.onClick.RemoveListener(ButtonClick);

            Dispatcher.RemoveListener(EventGlobal.E_LibraryCategoriesButtonClick, DisableSelectedState);
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
        }

        /// <summary>
        /// Init button
        /// </summary>
        /// <param name="category"></param>
        public void SetButtonCategory(BooksCategory category)
        {
            ButtonCategory = category;
            //SetButtonImage();
            //SetLocalization();
        }

        /// <summary>
        /// Click button from script
        /// </summary>
        public void InvokeButton()
        {
            libraryButton?.onClick?.Invoke();
        }

        private void SetLocalization()
        {
            buttonText.text = child.GetBookCategory(ButtonCategory.Position).GetCategoryName();
        }

        private void ButtonClick()
        {
            selected = true;
            GlobalLibraryCategory = ButtonCategory;
            libraryButton.interactable = false;
            PlayButtonAnimation(true);

            Dispatcher.Dispatch(EventGlobal.E_LibraryCategoriesButtonClick);
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void DisableSelectedState()
        {
            if (ButtonCategory == GlobalLibraryCategory || !selected)
            {
                return;
            }

            libraryButton.interactable = true;
            selected = false;
            PlayButtonAnimation(false);
        }

        private void PlayButtonAnimation(bool selected)
        {
            animationButton[animName].speed = selected ? 1 : -1;
            if (animationButton[animName].time == 0 || animationButton[animName].time == animationButton[animName].length)
            {
                animationButton[animName].time = selected ? 0 : animationButton[animName].length;
            }
            animationButton.Play();
        }

        private void SetButtonImage()
        {
            child.GetBookCategory(ButtonCategory.Position).SetImageForButton(buttonImage);
        }
    }
}