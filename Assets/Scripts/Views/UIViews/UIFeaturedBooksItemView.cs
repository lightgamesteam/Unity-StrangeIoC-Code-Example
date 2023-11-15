using Assets.Scripts.Services.Analytics;
using DG.Tweening;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views.Buttons;
using PFS.Assets.Scripts.Views.Components;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UIFeaturedBooksItemView : BaseView
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }
        [Inject] public Analytics Analytics { get; private set; }

        [Inject] public PoolModel Pool { get; set; }

        private const int MaxFeaturedBooksCount = 5;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI stageTitle;
        [SerializeField] private TextMeshProUGUI categoriesTitle;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI categoriesText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Images")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private Image background;
        [SerializeField] private Image initializingBlocker;

        [Header("Buttons")]
        [SerializeField] private UILikeButtonView likeButton;
        [SerializeField] private RectTransform startBookButton;
        [SerializeField] private RectTransform changeLanguageButton;

        [Header("Prefabs")]
        [SerializeField] private ScrollPageItem scrollPageItemPrefab;

        [Header("Other")]
        [SerializeField] private Transform pageItemsContainer;
        [SerializeField] private FeaturedBooksScroll booksScroll;
        [SerializeField] private GameObject loader;

        [Header("Options")]
        [SerializeField, Range(0f, 3f)] private float bookSwipingAnimLength = 1.0f;
        [SerializeField, Range(0f, 3f)] private float blockAnimDuration;
        [Space(3)]
        [SerializeField, Range(0f, 1f)] private float fadeAnimationLength = 0.2f;
        [SerializeField, Range(0f, 1f)] private float waitAfterfaideAnimation = 0.2f;
        [SerializeField, Range(0f, 1f)] private float fadeMoveDistance = 0.3f;
        [SerializeField, Range(0f, 2f)] private float waitBeforeShowImages = 0.1f;

        private float descriptionTextNormalPositionX;
        private float categoriesTextNormalPositionX;
        private float categoriesTitleNormalPositionX;
        private float stageTextNormalPositionX;
        private float stageTitleNormalPositionX;
        private float titleTextNormalPositionX;
        private float bookContainerNormalPositionX;
        private float bookContainerNormalRectPositionY;

        [Header("Book logo")]
        [SerializeField] private RectTransform bookContainer;
        [SerializeField] private Image bookImage;
        [SerializeField] private Image bookOutline;
        [SerializeField] private Image simplifiedLevel;
        [SerializeField] private Image simplifiedFill;
        [SerializeField] private TextMeshProUGUI imageSimplifiedLevelText;

        [Space(5)]
        [SerializeField] private UIChangeLanguageButtonView changeLanguage;
        [SerializeField] private UIDeleteButtonView deleteBookButton;

        [SerializeField] private StartBookButtonView startBookButtonView;

        private readonly List<ScrollPageItem> pageItems = new List<ScrollPageItem>();

        private int selectedBookNum = 0;

        public BookModel SelectedBook
        {
            get
            {
                if (BooksLibrary.featuredBooks.books.Count > selectedBookNum)
                {
                    return BooksLibrary.featuredBooks.books[selectedBookNum];
                }
                else
                {
                    Debug.LogWarning($"Feautured books Count ({BooksLibrary.featuredBooks.books.Count})  <= selectedBookIndex ({selectedBookNum})");
                    return null;
                }
            }
        }

        public void LoadView()
        {
            Debug.Log("GetDownloadedBookIds - Done");
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);
            Dispatcher.AddListener(EventGlobal.E_ChangeBookDetailsLanguage, ReinitBookInfoEvent);
            Dispatcher.AddListener(EventGlobal.E_BookDownloadStart, StartDownloadBookEvent);
            Dispatcher.AddListener(EventGlobal.E_BookDownloadEnd, FinishDownloadedBookEvent);
            Dispatcher.AddListener(EventGlobal.E_BookLoadProcessEnd, FinishLoadBookProcessEvent);
            //background.color = Color.black;

            likeButton.AddListener(ProccessBookLike);

            //booksScroll.processDrag += ProcessScrollDrag;
            booksScroll.processSwipe += ProcessScrollSwipe;
            booksScroll.processDragEnd += ProcessScrollEndDrag;

            SetItemActive(false);

            descriptionTextNormalPositionX = descriptionText.rectTransform.position.x;
            categoriesTextNormalPositionX = categoriesText.rectTransform.position.x;
            categoriesTitleNormalPositionX = categoriesTitle.rectTransform.position.x;
            stageTextNormalPositionX = stageText.rectTransform.position.x;
            stageTitleNormalPositionX = stageTitle.rectTransform.position.x;
            titleTextNormalPositionX = titleText.rectTransform.position.x;
            bookContainerNormalPositionX = bookContainer.position.x;
            bookContainerNormalRectPositionY = background.rectTransform.position.y;
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);
            Dispatcher.RemoveListener(EventGlobal.E_ChangeBookDetailsLanguage, ReinitBookInfoEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookDownloadStart, StartDownloadBookEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookDownloadEnd, FinishDownloadedBookEvent);
            Dispatcher.RemoveListener(EventGlobal.E_BookLoadProcessEnd, FinishLoadBookProcessEvent);

            //booksScroll.processDrag -= ProcessScrollDrag;
            booksScroll.processSwipe -= ProcessScrollSwipe;
            booksScroll.processDragEnd -= ProcessScrollEndDrag;
        }

        public void Init()
        {
            StartCoroutine(SelecterBookAnimHide(RunAfrerHide));
        }

        private void RunAfrerHide()
        {
            if(SelectedBook == null)
            {
                return;
            }
            SetLocalization();
            InitPageItems();
            UpdateLikeButtonState();
            SwitchToShowAnimation();
        }

        private void SetLocalization()
        {
            stageTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey);
            categoriesTitle.text = LocalizationManager.GetLocalizationText(LocalizationKeys.InterestsTitleKey);
        }

        private void InitPageItems()
        {
            ClearItems();

            if (BooksLibrary.featuredBooks.books.Count == 0)
            {
                return;
            }

            SetItemActive(true);

            for (int i = 0; i < BooksLibrary.featuredBooks.books.Count && i < MaxFeaturedBooksCount; i++)
            {
                int pageNum = i;

                ScrollPageItem pageItem = Instantiate(scrollPageItemPrefab, pageItemsContainer);
                pageItem.Init(() => ProcessBookSelected(pageNum));

                pageItems.Add(pageItem);
            }

            pageItems[selectedBookNum].SetSelectedState(true);

            startBookButtonView.InitializeStartBookButton(SelectedBook);
        }

        private void ProcessBookSelected(int newSelectedBookNum)
        {
            if (selectedBookNum != newSelectedBookNum)
            {
                pageItems[selectedBookNum].SetSelectedState(false);
                pageItems[newSelectedBookNum].SetSelectedState(true);

                selectedBookNum = newSelectedBookNum;

                StartCoroutine(SelecterBookAnimHide());

                startBookButtonView.InitializeStartBookButton(SelectedBook);
            }
        }

        #region Hide/Show Animations
        private IEnumerator SelecterBookAnimHide(Action actionAfterAnimation = null)
        {
            fadeImage.raycastTarget = true;

            float newPos = 0;
            Color color = new Color(0, 0, 0, 0);

            newPos = descriptionTextNormalPositionX - fadeMoveDistance;
            descriptionText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            descriptionText.DOFade(0, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = categoriesTextNormalPositionX - fadeMoveDistance;
            categoriesText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            categoriesText.DOFade(0, fadeAnimationLength);

            newPos = categoriesTitleNormalPositionX - fadeMoveDistance;
            categoriesTitle.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            categoriesTitle.DOFade(0, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = stageTextNormalPositionX - fadeMoveDistance;
            stageText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            stageText.DOFade(0, fadeAnimationLength);

            newPos = stageTitleNormalPositionX - fadeMoveDistance;
            stageTitle.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            stageTitle.DOFade(0, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = titleTextNormalPositionX - fadeMoveDistance;
            titleText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            titleText.DOFade(0, fadeAnimationLength);

            bookImage.DOFade(0f, fadeAnimationLength);
            bookOutline.DOFade(0f, fadeAnimationLength);
            simplifiedLevel.DOFade(0f, fadeAnimationLength);
            simplifiedFill.DOFade(0f, fadeAnimationLength);
            imageSimplifiedLevelText.DOFade(0, fadeAnimationLength);

            newPos = bookContainerNormalPositionX - fadeMoveDistance;
            bookContainer.DOMoveX(newPos, fadeAnimationLength);

            color = Color.white;
            color.a = 0;
            background.DOColor(color, fadeAnimationLength);
            newPos = bookContainerNormalRectPositionY - fadeMoveDistance;
            background.rectTransform.DOMoveY(newPos, fadeAnimationLength);

            Image[] startBookChildrenImages = startBookButton.gameObject.GetComponentsInChildren<Image>();
            foreach (var img in startBookChildrenImages)
            {
                color = img.color;
                color.a = 0;
                img.DOColor(color, fadeAnimationLength);
            }

            Image[] changeLanguageButtonChildren = changeLanguageButton.gameObject.GetComponentsInChildren<Image>();
            foreach (var img in changeLanguageButtonChildren)
            {
                color = img.color;
                color.a = 0;
                img.DOColor(color, fadeAnimationLength);
            }

            yield return new WaitForSeconds(waitBeforeShowImages);

            if (actionAfterAnimation != null)
            {
                actionAfterAnimation.Invoke();
            }
            else
            {
                SwitchToShowAnimation();
            }
        }

        private void SwitchToShowAnimation()
        {
            bool bookCover = false;
            bool backGroundCover = false;
            SetBookBackgroundImage(SelectedBook, () =>
            {
                backGroundCover = true;
                CheckedAvailableImages(bookCover, backGroundCover);
            });
            SetBookImage(SelectedBook, () =>
            {
                bookCover = true;
                CheckedAvailableImages(bookCover, backGroundCover);
            });
        }

        private void CheckedAvailableImages(bool bookCoverReady, bool backGroundCoverReady)
        {
            if (this != null && SelectedBook != null && bookCoverReady && backGroundCoverReady)
            {
                UpdateBookDescriptionPanel();
                StartCoroutine(SelecterBookAnimShow());
            }
        }

        private IEnumerator SelecterBookAnimShow()
        {
            fadeImage.raycastTarget = true;
            float newPos = 0;
            Color color = new Color(0, 0, 0, 0);

            color = Color.white;
            color.a = 1;
            background.DOColor(color, fadeAnimationLength);
            newPos = bookContainerNormalRectPositionY + fadeMoveDistance;
            background.rectTransform.DOMoveY(newPos, fadeAnimationLength);

            Image[] choldrenImages = startBookButton.gameObject.GetComponentsInChildren<Image>();
            foreach (var img in choldrenImages)
            {
                color = img.color;
                color.a = 1;
                img.DOColor(color, fadeAnimationLength);
            }

            Image[] changeLanguageButtonChildren = changeLanguageButton.gameObject.GetComponentsInChildren<Image>();
            foreach (var img in changeLanguageButtonChildren)
            {
                color = img.color;
                color.a = 1;
                img.DOColor(color, fadeAnimationLength);
            }

            newPos = descriptionTextNormalPositionX + fadeMoveDistance;
            descriptionText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            descriptionText.DOFade(1f, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = categoriesTextNormalPositionX + fadeMoveDistance;
            categoriesText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            categoriesText.DOFade(1f, fadeAnimationLength);

            newPos = categoriesTitleNormalPositionX + fadeMoveDistance;
            categoriesTitle.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            categoriesTitle.DOFade(1f, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = stageTextNormalPositionX + fadeMoveDistance;
            stageText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            stageText.DOFade(1f, fadeAnimationLength);

            newPos = stageTitleNormalPositionX + fadeMoveDistance;
            stageTitle.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            stageTitle.DOFade(1f, fadeAnimationLength);

            yield return new WaitForSeconds(waitAfterfaideAnimation);

            newPos = titleTextNormalPositionX + fadeMoveDistance;
            titleText.rectTransform.DOMoveX(newPos, fadeAnimationLength);
            titleText.DOFade(1f, fadeAnimationLength);

            bookImage.DOFade(1f, fadeAnimationLength);
            bookOutline.DOFade(1f, fadeAnimationLength);
            simplifiedLevel.DOFade(1f, fadeAnimationLength);
            simplifiedFill.DOFade(1f, fadeAnimationLength);
            imageSimplifiedLevelText.DOFade(1f, fadeAnimationLength);

            newPos = bookContainerNormalPositionX + fadeMoveDistance;
            bookContainer.DOMoveX(newPos, fadeAnimationLength);

            SetSimplifiedInfo();

            fadeImage.raycastTarget = false;
            booksScroll.isEnable = true;
        }
        #endregion

        private void UpdateBookDescriptionPanel()
        {
            titleText.text = SelectedBook.GetTranslation().BookName;
            descriptionText.text = SelectedBook.GetTranslation().DescriptionShort;

            stageText.text = ((int)SelectedBook.SimplifiedLevelEnum + 1).ToString();
            categoriesText.text = SelectedBook?.GetInterests();

            changeLanguage.InitButton(SelectedBook);
            deleteBookButton.InitButton(SelectedBook);
        }

        public void SetBookImage(BookModel book, Action callBack = null)
        {
            // Logic before loading
            bool needLoadImage = book?.GetTranslation().coverSpritePool == null;
            //-----------

            // Logic after loading
            Action<Sprite> setImageAction = resultSprite =>
            {
                if (bookImage != null)
                {
                    bookImage.sprite = resultSprite;
                }
                callBack?.Invoke();
            };

            Action setImageActionFail = () =>
            {
                if (bookImage != null)
                {
                    book.GetTranslation().coverSpritePool = Pool.BookCoverDefault;
                    bookImage.sprite = Pool.BookCoverDefault;
                }
                callBack?.Invoke();
            };

            // Init loading
            book?.LoadCoverImage(setImageAction, setImageActionFail);
        }

        private void SetSimplifiedInfo()
        {
            string bookLevel = LocalizationManager.GetLocalizationText(LocalizationKeys.LevelTitleKey);
            if (!string.IsNullOrEmpty(bookLevel))
            {
                bookLevel = bookLevel.Substring(0, 1);
            }

            if (SelectedBook != null)
            {
                if (SelectedBook.SimplifiedLevelEnum != Conditions.SimplifiedLevels.None)
                {
                    imageSimplifiedLevelText.text = bookLevel + ((int)SelectedBook.SimplifiedLevelEnum + 1);

                    if ((int)SelectedBook.SimplifiedLevelEnum >= 0 && (int)SelectedBook.SimplifiedLevelEnum < Pool.SimplifiedColors.Length)
                    {
                        simplifiedFill.color = Pool.SimplifiedColors[(int)SelectedBook.SimplifiedLevelEnum];
                    }
                    else
                    {
                        simplifiedFill.color = Color.white;
                    }
                }
            }
            else
            {
                Debug.LogWarning("UIFeaturedBooksItemView => SetSimplifiedInfo: SelectedBook == null");
            }
        }

        private void SetBookBackgroundImage(BookModel book, Action callBack = null)
        {
            Action<Sprite, string> setImageAction = (sprite, id) =>
            {
                if (background != null && id == this.SelectedBook?.Id)
                {
                    background.sprite = sprite;
                }
                callBack?.Invoke();
            };

            Action setImageActionFail = () =>
            {
                if (background != null && book.Id == this.SelectedBook.Id)
                {
                    background.sprite = Pool.BookBackgroundDefault;
                }
                callBack?.Invoke();
            };

            book?.LoadBackgroundImage(setImageAction, setImageActionFail);
        }

        private void UpdateLikeButtonState()
        {
            likeButton.SetLike(SelectedBook.IsLiked);
        }

        private void ProccessBookLike()
        {
            likeButton.SetInteractable(false);

            MainContextView.DispatchStrangeEvent(EventGlobal.E_SetLikeForBookToServer, new SetLikeForBookRequestModel(SelectedBook.Id,
                (like) =>
                {
                    if (this != null)
                    {
                        likeButton.SetInteractable(true);
                        SelectedBook.IsLiked = like;
                        UpdateLikeButtonState();
                        LoadBooksHelper.UpdateBookLikeStatus(SelectedBook);
                        MainContextView.DispatchStrangeEvent(EventGlobal.E_UpdateBooksLikeStatus);
                    }
                },
                () =>
                {
                    if (this != null)
                    {
                        Debug.LogError("Set like from server response error");
                        likeButton.SetInteractable(true);
                    }
                }));

            Analytics.LogEvent(EventName.ActionBookAddToFavourites,
              new System.Collections.Generic.Dictionary<Property, object>()
              {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.ISBN, SelectedBook.GetTranslation().Isbn},
                    { Property.Category, SelectedBook.GetInterests()}
              });
        }

        private void ProcessScrollDrag(float swipeValue)
        {
            if (fadeImage)
            {
                Color fadeColor = fadeImage.color;
                fadeColor.a = swipeValue;

                fadeImage.color = fadeColor;
            }
        }

        private void ProcessScrollEndDrag(float swipeValue)
        {
            StartCoroutine(SetBlockerForReverseAnim(swipeValue));
        }

        private void SetItemActive(bool isActive)
        {
            StartCoroutine(SetBlockerActive(!isActive));

            initializingBlocker.DOFade(isActive ? 0.0f : 1.0f, blockAnimDuration);
            loader.SetActive(!isActive);
        }

        private IEnumerator SetBlockerActive(bool isActive)
        {
            yield return new WaitForSeconds(isActive ? 0.0f : blockAnimDuration);

            initializingBlocker.gameObject.SetActive(isActive);
        }

        private void ProcessScrollSwipe(bool isNextDirection)
        {
            if (SelectedBook.IsUnityBook)
            {
                Dispatcher.Dispatch(EventGlobal.E_CancelDownloadUnityBook);
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_CancelDownloadNativeBook);
            }

            booksScroll.isEnable = false;

            int nextBookNum = isNextDirection ? selectedBookNum + 1 : selectedBookNum - 1;
            int featuredBooksCount = BooksLibrary.featuredBooks.books.Count > MaxFeaturedBooksCount ? MaxFeaturedBooksCount : BooksLibrary.featuredBooks.books.Count;

            if (nextBookNum >= featuredBooksCount)
            {
                nextBookNum = 0;
            }
            else if (nextBookNum < 0)
            {
                nextBookNum = featuredBooksCount - 1;
            }
            ProcessBookSelected(nextBookNum);
            StartCoroutine(SetBlockerForBookChanging());
        }

        private IEnumerator SetBlockerForBookChanging()
        {
            fadeImage.raycastTarget = true;

            yield return new WaitForSeconds(bookSwipingAnimLength);

            fadeImage.raycastTarget = false;
            booksScroll.isEnable = true;
        }

        private IEnumerator SetBlockerForReverseAnim(float endSwipeValue)
        {
            fadeImage.raycastTarget = true;

            float elapsedTime = 0.0f;
            float reverseAnimLength = bookSwipingAnimLength * endSwipeValue;

            while (elapsedTime < reverseAnimLength)
            {
                SetFadeImageAlpha(endSwipeValue - (endSwipeValue * (elapsedTime / reverseAnimLength)));

                yield return new WaitForEndOfFrame();

                elapsedTime += Time.deltaTime;
            }

            SetFadeImageAlpha(0.0f);
            fadeImage.raycastTarget = false;
        }

        private void SetFadeImageAlpha(float alpha)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
        }

        private void ClearItems()
        {
            foreach (var item in pageItems)
            {
                Destroy(item.gameObject);
            }

            pageItems.Clear();
        }

        private void StartDownloadBookEvent()
        {
            changeLanguage.NoInteractableButton();
            deleteBookButton.NoInteractableButton();
        }

        private void FinishDownloadedBookEvent()
        {
            if (SelectedBook != null)
            {
                changeLanguage.SetDownloadedIcon(SelectedBook.IsDownloaded);
                deleteBookButton.SetInteracButton();
            }
        }

        private void FinishLoadBookProcessEvent()
        {
            if (SelectedBook != null)
            {
                changeLanguage.SetInteracButton();
            }
        }

        private void ReinitBookInfoEvent(IEvent e)
        {
            var book = (BookModel)e.data;

            if (SelectedBook != null && SelectedBook == book)
            {
                UpdateBookDescriptionPanel();
            }
        }

        private void ChangeGlobalLanguageEvent()
        {
            if (SelectedBook != null)
            {
                SelectedBook.InitializeCurrentTranslationLanguage();
                UpdateBookDescriptionPanel();
            }

            SetLocalization();
        }
    }
}