using UnityEngine;
using UnityEngine.UI;
using Conditions;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;
using System.Linq;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models;

namespace PFS.Assets.Scripts.Views.Buttons
{
    public class UIChangeLanguageButtonView : BaseView
    {
        [Inject] public PoolModel Pool { get; set; }
        [Inject] public Analytics Analytics { get; private set; }

        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private GameObject body;
        [SerializeField] private GameObject arrows;
        [SerializeField] private Image languageImage;

        [Space(5)]
        [SerializeField] private GameObject downloadedIcon;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float showDwIconAnimScaleDuration;
        [SerializeField, Range(0f, 3f)] private float showDwIconAnimPunchDuration;
        [SerializeField, Range(0f, 1f)] private float showDwIconAnimPunch;
        [SerializeField, Range(0, 10)] private int showDwIconAnimPunchVibrato;
        [SerializeField, Range(0f, 1f)] private float showDwIconAnimPunchElastic;
        [Space(5)]
        [SerializeField, Range(0f, 2f)] private float hideDwIconAnimScaleUp;
        [SerializeField, Range(0f, 3f)] private float hideDwIconAnimScaleUpDuration;
        [SerializeField, Range(0f, 3f)] private float hideDwIconAnimScaleDownDuration;
        [Space(10)]
        [SerializeField, Range(0f, 3f)] private float bodyAnimScaleDownDuration;
        [SerializeField, Range(0f, 3f)] private float bodyAnimScaleUpDuration;
        [SerializeField, Range(0f, 3f)] private float bodyArrowsAnimScaleUpDuration;

        private List<Languages> availibleTranslations = new List<Languages>();

        private bool firtsClick = false;
        private BookModel book;

        private Image buttonDefaultImage;

        public void LoadView()
        {
            button.onClick.AddListener(() =>
            {
                StopAllCoroutines();

                firtsClick = true;
                ChangeLanguage();

                NoInteractableButton();
            });
        }

        public void RemoveView()
        {
        }

        public void InitButton(BookModel book)
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            if (this.book == null || this.book.Id != book.Id)
            {
                AddLanguages(book.AvailibleTranslations.ToArray());
                var curentLanguage = availibleTranslations.FirstOrDefault(x => x == book.CurrentTranslation);
                var curentLanguageIndex = availibleTranslations.FindIndex(x => x == curentLanguage);

                var firstElement = availibleTranslations[0];
                availibleTranslations[0] = curentLanguage;
                availibleTranslations[curentLanguageIndex] = firstElement;
            }

            this.book = book;

            buttonDefaultImage = GetComponent<Image>();

            SetDownloadedIcon(book.IsDownloaded);
            SetInteracButton();
            SetArrows();

            if (!firtsClick)
            {
                SetLanguageIcon(book.CurrentTranslation);
            }
            else
            {
                UnityAction iconLogic = () =>
                {
                    SetLanguageIcon(book.CurrentTranslation);
                };

                InitBodyAnimation(iconLogic);
            }

            firtsClick = false;
        }

        public void SetDownloadedIcon(bool downloaded)
        {
            InitDownloadedIconAnimation(downloaded);
        }

        private void AddLanguages(Languages[] translations)
        {
            this.availibleTranslations.Clear();

            foreach (var availibleTranslation in translations)
            {
                if (LanguagesModel.allowedTranslation.Contains(availibleTranslation))
                {
                    this.availibleTranslations.Add(availibleTranslation);
                }
            }
        }

        private void SetArrows()
        {
            arrows.SetActive(this.availibleTranslations.Count > 1);
        }

        public void SetInteracButton()
        {
            button.interactable = this.availibleTranslations.Count > 1;
        }

        public void NoInteractableButton()
        {
            button.interactable = false;
        }

        private void SetLanguageIcon(Languages language)
        {
            languageImage.sprite = Pool.GetLanguageSprite(language);
        }

        private void ShiftLanguageRight()
        {
            int len = availibleTranslations.Count; //self explanatory 
            var tmp = availibleTranslations[0]; //save first element value
            for (int i = 0; i < len - 1; i++) //starting from the end to begining
            {
                availibleTranslations[i] = availibleTranslations[i + 1]; //assign value of the next element
            }
            availibleTranslations[len - 1] = tmp; //now "first" last to last.
        }

        private void ChangeLanguage()
        {
            if (availibleTranslations.Count <= 1)
            {
                return;
            }

            if (book == null)
            {
                Debug.LogError("book == NULL!!!");
                return;
            }

            ShiftLanguageRight();
            book.CurrentTranslation = availibleTranslations[0];

            //if (book.CurrentTranslation == LanguagesModel.DefaultLanguage)
            //{
            //    book.CurrentTranslation = LanguagesModel.AdditionalLanguage;
            //}
            //else if (book.CurrentTranslation == LanguagesModel.AdditionalLanguage)
            //{
            //    book.CurrentTranslation = LanguagesModel.DefaultLanguage;
            //}

            Dispatcher.Dispatch(EventGlobal.E_ChangeBookDetailsLanguage, book);

            Analytics.LogEvent(EventName.ActionSwitchBookLanguage,
                  new Dictionary<Property, object>()
                  {
                                    { Property.ISBN, book.GetTranslation().Isbn},
                                    { Property.Category, book.GetInterests()},
                                    { Property.Translation, book.CurrentTranslation.ToDescription()},
                                    { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                  });
        }

        #region Animation
        private void InitBodyAnimation(UnityAction unityAction)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimationBodyCoroutine(unityAction));
            }
        }

        private IEnumerator AnimationBodyCoroutine(UnityAction unityAction)
        {
            buttonDefaultImage.DOFade(0, 0);

            body.transform.DOScaleY(0f, bodyAnimScaleDownDuration);

            yield return new WaitForSeconds(bodyAnimScaleDownDuration);

            unityAction?.Invoke();

            body.transform.DOScaleY(1f, bodyAnimScaleUpDuration);

            arrows.transform.localScale = new Vector3(1f, 0f, 1f);
            arrows.transform.DOScaleY(1f, bodyArrowsAnimScaleUpDuration);

            yield return new WaitForSeconds(bodyAnimScaleUpDuration);

            buttonDefaultImage.DOFade(1, 0);

            SetInteracButton();
        }

        private void InitDownloadedIconAnimation(bool downloaded)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimationDownloadedIconCoroutine(downloaded));
            }
        }

        private IEnumerator AnimationDownloadedIconCoroutine(bool downloaded)
        {
            if (downloadedIcon.activeSelf == downloaded && !downloaded)
            {
                yield break;
            }

            StopCoroutine("ShowDownloadedIconAnim");
            StopCoroutine("HideDownloadedIconAnim");

            if (downloaded)
            {
                yield return StartCoroutine("ShowDownloadedIconAnim");
            }
            else
            {
                yield return StartCoroutine("HideDownloadedIconAnim");
            }
        }

        private IEnumerator ShowDownloadedIconAnim()
        {
            downloadedIcon.SetActive(true);

            var tr = downloadedIcon.transform;
            DOTween.Pause(tr);
            tr.localScale = Vector3.zero;
            tr.DOScale(Vector3.one, showDwIconAnimScaleDuration);

            yield return new WaitForSeconds(showDwIconAnimScaleDuration);

            tr.DOPunchScale(new Vector3(showDwIconAnimPunch, showDwIconAnimPunch, 1f), showDwIconAnimPunchDuration, showDwIconAnimPunchVibrato, showDwIconAnimPunchElastic);

            yield return new WaitForSeconds(showDwIconAnimPunchDuration);
        }

        private IEnumerator HideDownloadedIconAnim()
        {
            downloadedIcon.SetActive(true);

            var tr = downloadedIcon.transform;
            DOTween.Pause(tr);
            tr.localScale = Vector3.one;
            tr.DOScale(new Vector3(hideDwIconAnimScaleUp, hideDwIconAnimScaleUp, 1f), hideDwIconAnimScaleUpDuration);

            yield return new WaitForSeconds(hideDwIconAnimScaleUpDuration);

            tr.DOScale(Vector3.zero, hideDwIconAnimScaleDownDuration);

            yield return new WaitForSeconds(hideDwIconAnimScaleDownDuration);

            downloadedIcon.SetActive(false);
        }
        #endregion
    }
}