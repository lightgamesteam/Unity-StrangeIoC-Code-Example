using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartType5View : UIQuizQuestionPartBaseView
    {
        public UIQuizQuestionType5View.QuestionType questionType { get; set; }

        [Header("Full part")]
        public QuizQuestionBackground fullBackImage;
        public RectTransform fullPartArea;
        public Button fullPartButton;
        public GameObject correctImage;
        public GameObject wrongImage;

        [Header("TrueFalse part")]
        public RectTransform trueFalsePartArea;
        public Button trueFalsePartButton;
        public QuizQuestionBackground trueFalseBackImage;
        public RectTransform trueFalseNoImageSize;

        [Header("Animation Params")]
        [SerializeField, Range(1.0f, 2.0f)] private float animationScale;
        [SerializeField, Range(0.0f, 1.0f)] private float animationDuration;


        public delegate void AccountHandlerEvent();
        public event AccountHandlerEvent AnswerSelectedEventHandler;

        //public UnityEvent AnswerSelectedEventHandler;
        public bool MultiTypeSelected { get; private set; }

        private Transform defaultParent;

        public override void LoadView()
        {
            base.LoadView();

            fullPartButton.onClick.AddListener(PartButtonClick);
            trueFalsePartButton.onClick.AddListener(PartButtonClick);

            defaultParent = transform.parent;
        }

        public override void RemoveView()
        {
            base.RemoveView();

            fullPartButton.onClick.RemoveListener(PartButtonClick);
            trueFalsePartButton.onClick.RemoveListener(PartButtonClick);
        }

        public override void InitPart()
        {
            if (questionType == UIQuizQuestionType5View.QuestionType.TrueFalse)
            {
                fullPartArea.gameObject.SetActive(false);
                trueFalsePartArea.gameObject.SetActive(true);
            }
            else
            {
                base.InitPart();
                fullPartArea.gameObject.SetActive(true);
                trueFalsePartArea.gameObject.SetActive(false);
            }

            correctImage.SetActive(false);
            wrongImage.SetActive(false);

            SetBackImageState(QuizQuestionBackground.BackgroundImageState.Default, withAnim: false);
        }

        public void ClearMultiTypeSelection()
        {
            if (questionType == UIQuizQuestionType5View.QuestionType.MultiAnswer)
            {
                MultiTypeSelected = false;
                AnswerSelectedEventHandler?.Invoke();
                SetBackImageState(QuizQuestionBackground.BackgroundImageState.Default);
            }
        }

        public void SetTrueFalseSize(bool isQuestionImage)
        {
            if (!isQuestionImage)
            {
                trueFalsePartArea.anchoredPosition = trueFalseNoImageSize.anchoredPosition;
                trueFalsePartArea.sizeDelta = trueFalseNoImageSize.sizeDelta;
            }
        }

        public void SetCorrectAnswerState()
        {
            fullPartButton.interactable = false;
            trueFalsePartButton.interactable = false;
            correctImage.SetActive(true);

            SetPartSortingOrder(isTopOrder: true);

            SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win, withGlow: true);
        }

        protected override void CorrectAnswer()
        {

            if (questionType == UIQuizQuestionType5View.QuestionType.MultiAnswer)
            {

            }
            else
            {
                base.CorrectAnswer();
                SetCorrectAnswerState();
            }
        }

        protected override void WrongAnswer()
        {
            if (questionType == UIQuizQuestionType5View.QuestionType.MultiAnswer)
            {

            }
            else
            {
                base.WrongAnswer();

                fullPartButton.interactable = false;
                trueFalsePartButton.interactable = false;
                wrongImage.SetActive(true);

                SetBackImageState(QuizQuestionBackground.BackgroundImageState.Lose, withGlow: true);
            }
        }

        public override IEnumerator ShowCorrectResult()
        {
            SetPartSortingOrder(isTopOrder: true);

            SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);
            yield return new WaitForSeconds(animationDuration * 2.0f);

            SetBackImageState(QuizQuestionBackground.BackgroundImageState.Default);
            yield return new WaitForSeconds(animationDuration * 2.0f);

            SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);
            yield return new WaitForSeconds(animationDuration * 2.0f);
        }

        public void SetPartSortingOrder(bool isTopOrder)
        {
            transform.parent = isTopOrder ? defaultParent.parent.parent : defaultParent;
        }

        private void PartButtonClick()
        {
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);

            if (questionType == UIQuizQuestionType5View.QuestionType.MultiAnswer)
            {
                MultiTypeSelected = !MultiTypeSelected;
                AnswerSelectedEventHandler?.Invoke();
                SetBackImageState(MultiTypeSelected ? QuizQuestionBackground.BackgroundImageState.Selected : QuizQuestionBackground.BackgroundImageState.Default);

                if (quizQuestionPart.isCorrect)
                {
                    SetPartSortingOrder(isTopOrder: MultiTypeSelected);
                }
            }
            else
            {
                SelectPart();
            }
        }

        private void SetBackImageState(QuizQuestionBackground.BackgroundImageState state, bool withAnim = true, bool withGlow = false)
        {
            if (questionType == UIQuizQuestionType5View.QuestionType.TrueFalse)
            {
                trueFalseBackImage.SetBackImageState(state, withGlow);
            }
            else
            {
                fullBackImage.SetBackImageState(state, withGlow);
            }

            if (withAnim)
            {
                Sequence stateSettingSequence = DOTween.Sequence();
                stateSettingSequence.Append(transform.DOScale(animationScale, animationDuration));
                stateSettingSequence.Append(transform.DOScale(1.0f, animationDuration));

                stateSettingSequence.Play();
            }
        }
    }
}