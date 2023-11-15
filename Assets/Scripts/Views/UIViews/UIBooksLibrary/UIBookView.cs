using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using DG.Tweening;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UIBookView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }
        [Flags] public enum BookState { Standard = 1, LateLoadImage = Standard << 2, DisableButton = Standard << 3, SetOutline = Standard << 4, VariableSize = Standard << 5, IgnoreGlobalLanguage = Standard << 6 }

        [Inject] public PoolModel Pool { get; set; }

        [Header("UI")]
        [SerializeField] private Image bookCover;
        [SerializeField] private Button bookButton;
        [SerializeField] private GameObject rightPanel;

        [Space(5)]
        [SerializeField] private VerticalLayoutGroup rightPanelLayout;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        [SerializeField] private Mask bookMask;
        [SerializeField] private Image bookMaskImage;

        [Space(5)]
        [SerializeField] private GameObject simplifiedBook;
        [SerializeField] private Image simplifiedBookImage;
        [SerializeField] private TextMeshProUGUI simplifiedBookText;

        [Header("Collider")]
        [SerializeField] private BoxCollider2D bookCollider;
        [SerializeField] private Rigidbody2D bookRgbd2D;

        [Header("Animation params")]
        [SerializeField, Range(0f, 3f)] private float imageFadeAnimDuration;
        [SerializeField, Range(0f, 3f)] private float imageFadeNewImageAnimDuration;

        public BookModel Book { get; private set; }

        private GameObject bookOutline;
        private GameObject bookTypeIcon;

        private BookState bookStates;
        private bool bookReady = false;
        private string bookLevel;
        private bool ignoreGlobalLanguage;

        public void LoadView()
        {
            SetLocalization();

            bookReady = true;

            if (Book != null)
            {
                InitBook(bookStates);
            }

            bookButton?.onClick.AddListener(OpenBook);
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);
        }

        public void RemoveView()
        {
            bookButton?.onClick.RemoveListener(OpenBook);
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, ChangeGlobalLanguageEvent);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "LoadBooksPanel")
            {
                EnableBook(true);
                SetBookCover();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "LoadBooksPanel")
            {
                EnableBook(false);
                DisableBookImage();
            }
        }

        /// <summary>
        /// If need more that one bookStates -> BookState.DisableButton | BookState.SetOutline
        /// </summary>
        /// <param name="book"></param>
        /// <param name="bookStates"></param>
        public void SetBook(BookModel book, BookState bookStates = BookState.Standard)
        {
            this.Book = book;

            this.bookStates = bookStates;

            if (this.Book != null && bookReady)
            {
                InitBook(bookStates);
            }
        }

        private void InitBook(BookState state)
        {
            BuildInfo();

            if (bookButton)
            {
                bookButton.enabled = true;
            }

            if (bookCollider)
            {
                bookCollider.enabled = false;
            }

            ignoreGlobalLanguage = false;

            void CheckObjects()
            {
                if (!bookCollider || !bookCollider.enabled)
                {
                    DisableBookImage();
                    SetBookCover();
                    EnableBook(true);

                    Destroy(bookCollider);
                    Destroy(bookRgbd2D);
                }

                if ((state & BookState.VariableSize) != BookState.VariableSize)
                {
                    StartCoroutine(DestroyLayout());
                }
            }

            if ((state & BookState.Standard) == BookState.Standard)
            {
                SetBookCover();
                CheckObjects();
                return;
            }

            if ((state & BookState.LateLoadImage) == BookState.LateLoadImage)
            {
                bookCollider.enabled = true;
                SetBookColliderSize();
            }

            if ((state & BookState.DisableButton) == BookState.DisableButton)
            {
                Destroy(bookButton);
            }

            if ((state & BookState.SetOutline) == BookState.SetOutline)
            {
                if (bookOutline == null)
                {
                    bookOutline = Instantiate(Pool.BookOutline, transform);
                }
            }

            if ((state & BookState.IgnoreGlobalLanguage) == BookState.IgnoreGlobalLanguage)
            {
                ignoreGlobalLanguage = true;
            }

            CheckObjects();
        }

        private IEnumerator DestroyLayout()
        {
            yield return new WaitForEndOfFrame();

            if (rightPanelLayout)
            {
                Destroy(rightPanelLayout);
            }

            if (aspectRatioFitter)
            {
                Destroy(aspectRatioFitter);
            }
        }

        private void BuildInfo()
        {
            CheckRightPanelContent();

            if (Book.TypeEnum == Conditions.BookContentType.Animated)
            {
                bookTypeIcon = Instantiate(Pool.BookAnimatedIcon, rightPanel.transform);
                bookTypeIcon.transform.SetSiblingIndex(0);
            }
            else if (Book.TypeEnum == Conditions.BookContentType.Song)
            {
                bookTypeIcon = Instantiate(Pool.BookSongIcon, rightPanel.transform);
                bookTypeIcon.transform.SetSiblingIndex(0);
            }

            SetSimplifiedInfo();
        }

        private void CheckRightPanelContent()
        {
            if (bookTypeIcon)
            {
                Destroy(bookTypeIcon);
                bookTypeIcon = null;
            }
        }

        private void EnableBook(bool enable)
        {
            bookCover.gameObject.SetActive(enable);
            rightPanel.SetActive(enable);
            bookMask.enabled = enable;
            bookMaskImage.enabled = enable;

            if (bookOutline)
            {
                bookOutline.SetActive(enable);
            }

            if (bookButton)
            {
                bookButton.enabled = enable;
            }
        }

        public void SetBookCover()
        {
            // Logic before loading
            bool needLoadImage = Book?.GetTranslation().coverSpritePool == null;
            //-----------

            // Logic after loading
            Action<Sprite> setImageAction = resultSprite =>
            {
                if (bookCover != null)
                {
                    Action action = () =>
                    {
                        bookCover.sprite = resultSprite;
                    };

                    if (this != null && needLoadImage)
                    {
                        StartCoroutine(Animation(action));
                    }
                    else
                    {
                        action();

                        bookCover.color = new Color(bookCover.color.r, bookCover.color.g, bookCover.color.b, 0f);
                        bookCover.DOFade(1f, imageFadeAnimDuration);
                    }
                }
            };

            Action setImageActionFail = () =>
            {
                if (bookCover != null)
                {
                    Action action = () =>
                    {
                        Book.GetTranslation().coverSpritePool = Pool.BookCoverDefault;
                        bookCover.sprite = Pool.BookCoverDefault;
                    };

                    if (this != null && needLoadImage)
                    {
                        StartCoroutine(Animation(action));
                    }
                    else
                    {
                        action();

                        bookCover.color = new Color(bookCover.color.r, bookCover.color.g, bookCover.color.b, 0f);
                        bookCover.DOFade(1f, imageFadeAnimDuration);
                    }
                }
            };

            // Init loading
            Book?.LoadCoverImage(setImageAction, setImageActionFail);
        }

        private IEnumerator Animation(Action action)
        {
            bookCover.DOFade(0f, imageFadeNewImageAnimDuration);

            yield return new WaitForSeconds(imageFadeNewImageAnimDuration);

            action?.Invoke();
            bookCover.DOFade(1f, imageFadeNewImageAnimDuration);
        }

        private void DisableBookImage()
        {
            bookCover.sprite = Pool.BookCoverDefault;
            //bookCover.color = new Color(bookCover.color.r, bookCover.color.g, bookCover.color.b, 0f);
        }

        private void OpenBook()
        {
            //Dispatcher.Dispatch(EventGlobal.E_ResetTopPanel);
            if (Book.IsBookFromSearch)
            {
                Analytics.LogEvent(EventName.ActionSearch,
                 new System.Collections.Generic.Dictionary<Property, object>()
                 {
                        { Property.ISBN, this.Book.GetTranslation().Isbn},
                        { Property.Translation, Book.CurrentTranslation.ToDescription()},
                        { Property.Category, this.Book.GetInterests()},
                        { Property.BookLevel, this.Book.SimplifiedLevelEnum.ToDescription()}
                 });
            }
            if (Book.IsBookFromMostRead)
            {
                Analytics.LogEvent(EventName.NavigationMostReadBooks,
                  new System.Collections.Generic.Dictionary<Property, object>()
                  {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.ISBN, this.Book.GetTranslation().Isbn},
                        { Property.Category, this.Book.GetInterests()}
                  });
            }

            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBookDetails, data = Book, isAddToScreensList = false });
        }

        private void SetSimplifiedInfo()
        {
            if (Book.SimplifiedLevelEnum == Conditions.SimplifiedLevels.None)
            {
                simplifiedBook.SetActive(false);
            }
            else
            {
                simplifiedBook.SetActive(true);
                simplifiedBookText.text = bookLevel + ((int)Book.SimplifiedLevelEnum + 1);

                if ((int)Book.SimplifiedLevelEnum >= 0 && (int)Book.SimplifiedLevelEnum < Pool.SimplifiedColors.Length)
                {
                    simplifiedBookImage.color = Pool.SimplifiedColors[(int)Book.SimplifiedLevelEnum];
                }
                else
                {
                    simplifiedBookImage.color = Color.white;
                }
            }
        }

        private void SetBookColliderSize()
        {
            StartCoroutine(WaitBookInit());
        }

        private IEnumerator WaitBookInit()
        {
            yield return new WaitForEndOfFrame();

            var size = GetComponent<RectTransform>()?.rect.size;
            bookCollider.size = size ?? Vector2.zero;
        }

        private void ChangeGlobalLanguageEvent()
        {
            if (!ignoreGlobalLanguage)
            {
                Book?.InitializeCurrentTranslationLanguage();
                if (bookCover.sprite != null)
                {
                    SetBookCover();
                }
            }

            SetLocalization();
        }

        private void SetLocalization()
        {
            bookLevel = LocalizationManager.GetLocalizationText(LevelTitleKey);
            if (!string.IsNullOrEmpty(bookLevel))
            {
                bookLevel = bookLevel.Substring(0, 1);
            }

            if (this.Book != null && bookReady)
            {
                SetSimplifiedInfo();
            }
        }
    }
}