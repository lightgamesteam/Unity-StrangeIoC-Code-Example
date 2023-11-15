using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PFS.Assets.Scripts.Views.Library.UIMovePanelView;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UINavigationLibraryView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        [Header("Navigation buttons")]
        [SerializeField] private ScrollRect categoriesButtonsScroll;
        [SerializeField] private UILibraryButtonView buttonPrefab;
        private List<UILibraryButtonView> categoriesButtons = new List<UILibraryButtonView>();

        [Header("Books panels")]
        [SerializeField] private Image BGBlackImage;

        [Header("Navigation buttons params")]
        [SerializeField, Range(0.1f, 10f)] private float hidePanelSpeed;

        private IEnumerator hideCategoriesButtonsIEnum;
        private ScreenState lastState = ScreenState.Down;

        private ChildModel child;

        public void LoadView()
        {
            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            InitButtons();

            Dispatcher.AddListener(EventGlobal.E_MoveLibraryPanel, HideCategoriesButtonsEvent);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_MoveLibraryPanel, HideCategoriesButtonsEvent);
        }

        private void HideCategoriesButtonsEvent(IEvent e)
        {
            ScreenState res = (ScreenState)e.data;

            if (res != lastState)
            {
                HideCategoriesButtons(res == ScreenState.Up);
                lastState = res;
            }
        }

        private void HideCategoriesButtons(bool hide)
        {
            if (hideCategoriesButtonsIEnum != null)
            {
                StopCoroutine(hideCategoriesButtonsIEnum);
            }

            hideCategoriesButtonsIEnum = HideCategoriesButtonsCoroutine(hide);
            StartCoroutine(hideCategoriesButtonsIEnum);
        }

        private IEnumerator HideCategoriesButtonsCoroutine(bool hide)
        {
            int needAlphaImage = hide ? 1 : 0;

            while (Mathf.Abs(BGBlackImage.color.a - needAlphaImage) > 0.01f)
            {
                BGBlackImage.color = new Color(BGBlackImage.color.r, BGBlackImage.color.g, BGBlackImage.color.b, Mathf.Lerp(BGBlackImage.color.a, needAlphaImage, Time.deltaTime * hidePanelSpeed));

                yield return new WaitForEndOfFrame();
            }

            BGBlackImage.color = new Color(BGBlackImage.color.r, BGBlackImage.color.g, BGBlackImage.color.b, needAlphaImage);
        }

        private void InitButtons()
        {
            categoriesButtons.Clear();
            foreach (Transform child in categoriesButtonsScroll.content)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < child.CategoriesForStrategy.Length; i++)
            {
                UILibraryButtonView button = Instantiate(buttonPrefab, categoriesButtonsScroll.content);

                button.SetButtonCategory(child.GetBookCategory(i));
                categoriesButtons.Add(button);
            }
        }

        public void InitStartCategory(BooksCategory category)
        {
            int pos = category.Position;

            if (child.CategoriesForStrategy.Length > pos)
            {
                categoriesButtons[pos].InvokeButton();
                SetButtonsScrollPosition(pos);
            }
        }

        private void SetButtonsScrollPosition(int position)
        {
            int childCount = categoriesButtonsScroll.content.childCount - 2; // -2 becouse 2 items empty space
            float oneChildScrollSize = 1f / childCount;

            categoriesButtonsScroll.horizontalNormalizedPosition = position * oneChildScrollSize;
        }
    }
}