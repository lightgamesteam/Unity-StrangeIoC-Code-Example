using BooksPlayer;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Views.Library;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.BookDetails
{
    public class UIBookDetailsItemView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Animation selectAnimation;
        [SerializeField] private Image selectImage;

        [Space(5)]
        [SerializeField] private string animName;

        [Header("Book")]
        [SerializeField] private UIBookView bookView;

        private bool selected = false;

        public void LoadView()
        {
            SetOutline(false);

            button.onClick.AddListener(ButtonClick);

            Dispatcher.AddListener(EventGlobal.E_SelectBookDetails, DisableSelectedState);
            Dispatcher.AddListener(EventGlobal.E_ChangeBookDetailsCover, ChangeBookDetailsCoverEvent);
        }

        public void RemoveView()
        {
            button.onClick.RemoveListener(ButtonClick);

            Dispatcher.RemoveListener(EventGlobal.E_SelectBookDetails, DisableSelectedState);
            Dispatcher.RemoveListener(EventGlobal.E_ChangeBookDetailsCover, ChangeBookDetailsCoverEvent);
        }

        /// <summary>
        /// Init book
        /// </summary>
        /// <param name="book"></param>
        public void SetBook(BookModel book)
        {
            bookView.SetBook(book, UIBookView.BookState.DisableButton | UIBookView.BookState.VariableSize | UIBookView.BookState.LateLoadImage);
        }

        /// <summary>
        /// Click button from script
        /// </summary>
        public void InvokeButton()
        {
            StartCoroutine(WaitInvoke());
        }

        private IEnumerator WaitInvoke()
        {
            yield return new WaitForEndOfFrame();
            button.onClick?.Invoke();
        }

        private void ButtonClick()
        {
            BooksPlayerMainContextView.DispatchStrangeEvent(ExternalEvents.CloseBooksPlayer);
            Dispatcher.Dispatch(EventGlobal.E_CancelDownloadUnityBook);
            PlayBookAnimation(true);
            button.interactable = false;
            selected = true;
            SetOutline(true);

            Dispatcher.Dispatch(EventGlobal.E_SelectBookDetails, bookView.Book);
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void DisableSelectedState(IEvent e)
        {
            var book = e.data as BookModel;
            if (book == null)
            {
                return;
            }

            if (bookView.Book.Id == book.Id || !selected)
            {
                return;
            }

            button.interactable = true;
            selected = false;
            SetOutline(false);

            PlayBookAnimation(false);
        }

        private void PlayBookAnimation(bool selected)
        {
            selectAnimation[animName].speed = selected ? 1 : -1;
            if (selectAnimation[animName].time == 0 || selectAnimation[animName].time == selectAnimation[animName].length)
            {
                selectAnimation[animName].time = selected ? 0 : selectAnimation[animName].length;
            }
            selectAnimation.Play();
        }

        private void SetOutline(bool visible)
        {
            selectImage.color = visible ? Color.white : Color.clear;
        }

        private void ChangeBookDetailsCoverEvent()
        {
            if (selected)
            {
                bookView.SetBookCover();
            }
        }
    }
}