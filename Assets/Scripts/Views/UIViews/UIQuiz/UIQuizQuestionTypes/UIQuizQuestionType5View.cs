using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using strange.extensions.dispatcher.eventdispatcher.api;
using TMPro;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.SoundManagerModels;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionType5View : UIQuizQuestionBaseView
    {
        public enum QuestionType { TrueFalse, OneAnswer, MultiAnswer }

        [Inject]
        public PoolModel Pool { get; set; }

        [Header("Type5 UI")]
        public Image questionImage;
        public RectTransform questionImageArea;
        public Button doneButton;
        public RectTransform doneButtonWithImagePos;
        public Animation explanationAnimation;
        public TextMeshProUGUI falseAnswerExplanation;
        public TextMeshProUGUI doneButtonText;
        public QuizQuestionBackground questionImageBack;
        public Image questionImageCorrect;
        public Image questionImageWrong;
        public RectTransform trueFalseImageTransform;
        public GridLayoutGroup answersGrid;
        public Image bgOverlap;

        [Header("Win effect")]
        public Transform winEffectArea;
        public GameObject winEffectGM;

        [Header("Type5 params")]
        public float waitTimeAfterQuestion;
        public float waitTimeAfterWithExplanation;
        public float waitTimePerQuestion;


        [Header("Animation params")]
        [SerializeField, Range(0.0f, 1.0f)] private float overlapFadeValue;
        [SerializeField, Range(0.0f, 1.0f)] private float overlapAnimDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float imageMarkAnimDuration;

        private QuestionType questionType;

        void OnValidate()
        {
            waitTimeAfterQuestion = Mathf.Max(waitTimeAfterQuestion, 0);
            waitTimePerQuestion = Mathf.Max(waitTimePerQuestion, 0);
            waitTimeAfterWithExplanation = Mathf.Max(waitTimeAfterWithExplanation, 0);
        }

        public override void LoadView()
        {
            questionImageCorrect.gameObject.SetActive(false);
            questionImageWrong.gameObject.SetActive(false);

            DetectQuestionType();
            base.LoadView();
            LoadQuestionImage();

            Dispatcher.AddListener(EventGlobal.E_QuizPartChoice, TryToShowAnswerExplanation);
            Dispatcher.AddListener(EventGlobal.E_QuizFadeContent, StartQuizContentFading);

            explanationAnimation.gameObject.SetActive(false);

            doneButton.onClick.AddListener(DoneButtonProcess);
            doneButtonText.text = LocalizationManager.GetLocalizationText("ui.done", quizQuestion.questionLanguage);
            AnswerPartsAddListenerHandler();

            StartCoroutine(DisableAnswersGrid());
            StartCoroutine(DisableResultButton());
        }

        public override void RemoveView()
        {
            base.RemoveView();

            Dispatcher.RemoveListener(EventGlobal.E_QuizPartChoice, TryToShowAnswerExplanation);
            Dispatcher.RemoveListener(EventGlobal.E_QuizFadeContent, StartQuizContentFading);
            doneButton.onClick.RemoveListener(DoneButtonProcess);
            AnswerPartsRemoveListenerHandler();
        }

        protected override void PrepareContentFading()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType5View castedPart = part as UIQuizQuestionPartType5View;

                if (castedPart != null && castedPart.quizQuestionPart != null && castedPart.quizQuestionPart.isCorrect)
                {
                    castedPart.SetPartSortingOrder(isTopOrder: true);
                }
            }
        }

        protected override IEnumerator CorrectAnswerDone()
        {
            StartImageMarkShowingAnim(questionImageCorrect);
            questionImageBack.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);

            if (quizQuestion.IsQuestionWithEffects())
            {
                InitWinEffect();
            }

            yield return new WaitForSeconds(IsTrueFalseQuestionWithExplanation() ? waitTimeAfterWithExplanation : waitTimeAfterQuestion);
        }

        protected override IEnumerator ShowCorrectAnswer()
        {
            StartImageMarkShowingAnim(questionImageWrong);
            questionImageBack.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Lose);

            List<UIQuizQuestionPartBaseView> correctParts = new List<UIQuizQuestionPartBaseView>();
            foreach (var part in questionParts)
            {
                if (part.quizQuestionPart != null && part.quizQuestionPart.isCorrect)
                {
                    correctParts.Add(part);
                }
            }

            for (int i = 0; i < correctParts.Count; i++)
            {
                if (i == correctParts.Count - 1)
                {
                    yield return StartCoroutine(correctParts[i].ShowCorrectResult());
                }
                else
                {
                    StartCoroutine(correctParts[i].ShowCorrectResult());
                }
            }
        }

        protected override IEnumerator QuestionResultVisualDelays()
        {
            yield return new WaitForSeconds(waitTimePerQuestion);
        }

        private void LoadQuestionImage()
        {
            questionImage.color = Color.clear;

            if (string.IsNullOrEmpty(quizQuestion.questionImageURL))
            {
                return;
            }

            //=> show load process

            QuizHelper.GetQuestionPartImage(quizQuestion.questionImageURL, (Sprite sprite) =>
            {
                SetQuestionImage(sprite);
            },
            () =>
            {
                SetQuestionImage(Pool.QuizPartDefault);
            });
        }

        private void EnableQuestionImage()
        {
            if (questionImage != null && questionImageArea != null)
            {
                questionImageArea.gameObject.SetActive(true);

                if (questionType != QuestionType.TrueFalse)
                {
                    answersGrid.constraintCount = 1;
                }
                //=> hide load process
            }
        }

        private void SetQuestionImage(Sprite sprite)
        {
            questionImage.color = Color.white;
            questionImage.sprite = sprite;
        }

        private void StartImageMarkShowingAnim(Image markImage)
        {
            StartImageFadeInAnimation(markImage);
            StartImageFadeInAnimation(markImage.transform.GetChild(0).GetComponent<Image>());
        }

        private void StartImageFadeInAnimation(Image image)
        {
            if (image)
            {
                image.gameObject.SetActive(true);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
                image.DOFade(1.0f, imageMarkAnimDuration);
            }
        }

        private void DetectQuestionType()
        {
            questionType = QuestionType.OneAnswer;

            if (quizQuestion == null)
            {
                Debug.LogError("UIQuizQuestionType5View => quizQuestion - null");
                return;
            }

            if (quizQuestion.quizQuestionParts == null)
            {
                Debug.LogError("UIQuizQuestionType5View => quizQuestion.quizQuestionParts - null");
                return;
            }

            if(quizQuestion.quizQuestionParts.Length == 0)
            {
                Debug.LogError($"UIQuizQuestionType5View => quizQuestion.quizQuestionParts - empty (question info: {quizQuestion.questionInfo})");
                return;
            }

            if (quizQuestion.quizQuestionParts.Length == 2)
            {
                questionType = QuestionType.TrueFalse;

                ((UIQuizQuestionPartType5View)questionParts[0]).SetTrueFalseSize(!string.IsNullOrEmpty(quizQuestion.questionImageURL));
                ((UIQuizQuestionPartType5View)questionParts[1]).SetTrueFalseSize(!string.IsNullOrEmpty(quizQuestion.questionImageURL));

                questionParts[2].gameObject.SetActive(false);
                questionParts[3].gameObject.SetActive(false);

                questionImageArea.GetComponent<LayoutElement>().ignoreLayout = true;
                questionImageArea.anchoredPosition = trueFalseImageTransform.anchoredPosition;
                questionImageArea.sizeDelta = trueFalseImageTransform.sizeDelta;
            }
            else
            {
                if (IsMultiAnswer())
                {
                    questionType = QuestionType.MultiAnswer;
                }
                else
                {
                    questionType = QuestionType.OneAnswer;
                }
            }

            doneButton.gameObject.SetActive(questionType == QuestionType.MultiAnswer);

            if (!string.IsNullOrEmpty(quizQuestion.questionImageURL))
            {
                EnableQuestionImage();
                doneButton.image.rectTransform.anchoredPosition = doneButtonWithImagePos.anchoredPosition;
            }

            //set types
            foreach (var part in questionParts)
            {
                if (part is UIQuizQuestionPartType5View)
                {
                    ((UIQuizQuestionPartType5View)part).questionType = questionType;
                }
            }
        }

        private bool IsMultiAnswer()
        {
            int i = 0;
            foreach (var part in quizQuestion.quizQuestionParts)
            {
                if (part.isCorrect)
                {
                    i++;
                }
            }

            return i > 1;
        }

        private void DoneButtonProcess()
        {
            string[] res = GetChoicePartsIds();
            if (res.Length > 0)
            {
                AudioClip soundEffect = null;

                if (base.IsCorect(res))
                {
                    soundEffect = SoundManagerModel.instance.winEffect;
                    ProcessMultitypeCorrectAnswer();
                }
                else
                {
                    soundEffect = SoundManagerModel.instance.loseEffect;
                    ClearPartsMultiTypeSelection();
                }

                if (quizQuestion.IsQuestionWithEffects())
                {
                    MainContextView.DispatchStrangeEvent(EventGlobal.E_SoundPlay, new AudioClipToCommand(soundEffect, Conditions.SoundType.Sound, 1, false, 1));
                }

                Dispatcher.Dispatch(EventGlobal.E_QuizPartChoice, res);
            }

            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void TryToShowAnswerExplanation(IEvent e)
        {
            if (questionParts.Length > 1)
            {
                UIQuizQuestionPartType5View falsePart = questionParts[1] as UIQuizQuestionPartType5View;

                if (IsTrueFalseQuestionWithExplanation(falsePart))
                {
                    explanationAnimation.gameObject.SetActive(true);
                    explanationAnimation.Play();

                    falseAnswerExplanation.text = falsePart.quizQuestionPart.partInfo;
                }
            }
        }

        private bool IsTrueFalseQuestionWithExplanation()
        {
            UIQuizQuestionPartType5View falsePart = questionParts[1] as UIQuizQuestionPartType5View;

            return IsTrueFalseQuestionWithExplanation(falsePart);
        }

        private bool IsTrueFalseQuestionWithExplanation(UIQuizQuestionPartType5View falsePart)
        {
            bool result = false;

            if (falsePart
             && falsePart.questionType == QuestionType.TrueFalse
             && falsePart.quizQuestionPart.isCorrect
             && !string.IsNullOrEmpty(falsePart.quizQuestionPart.partInfo))
            {
                result = true;
            }

            return result;
        }

        private string[] GetChoicePartsIds()
        {
            List<string> res = new List<string>();
            foreach (var part in questionParts)
            {
                if (part is UIQuizQuestionPartType5View)
                {
                    if (((UIQuizQuestionPartType5View)part).MultiTypeSelected)
                    {
                        res.Add(part.quizQuestionPart.questionPartId);
                    }
                }
            }

            return res.ToArray();
        }

        private void InitWinEffect()
        {
            if (winEffectGM && winEffectArea)
            {
                Instantiate(winEffectGM, winEffectArea);
            }
        }

        private void ProcessMultitypeCorrectAnswer()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType5View castedPart = part as UIQuizQuestionPartType5View;

                if (castedPart != null && castedPart.quizQuestionPart != null && castedPart.quizQuestionPart.isCorrect)
                {
                    castedPart.SetCorrectAnswerState(); ;
                }
            }
        }

        private void ClearPartsMultiTypeSelection()
        {
            foreach (var part in questionParts)
            {
                if (part is UIQuizQuestionPartType5View)
                {
                    ((UIQuizQuestionPartType5View)part).ClearMultiTypeSelection();
                }
            }
        }

        private void StartQuizContentFading()
        {
            bgOverlap.DOFade(overlapFadeValue, overlapAnimDuration);
        }

        private IEnumerator DisableAnswersGrid()
        {
            yield return new WaitForEndOfFrame();

            answersGrid.enabled = false;
        }

        private void AnswerPartsRemoveListenerHandler()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType5View castedPart = part as UIQuizQuestionPartType5View;

                if (castedPart != null)
                {
                    castedPart.AnswerSelectedEventHandler -= ShowOrHideResultButton;
                }
            }
        }

        private void AnswerPartsAddListenerHandler()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType5View castedPart = part as UIQuizQuestionPartType5View;

                if (castedPart != null)
                {
                    castedPart.AnswerSelectedEventHandler += ShowOrHideResultButton;
                }
            }
        }

        private void ShowOrHideResultButton()
        {
            string[] answerCount = GetChoicePartsIds();
            if (answerCount.Length > 0)
            {
                doneButton.interactable = true;
            }
            else
            {
                doneButton.interactable = false;
            }
        }

        private IEnumerator DisableResultButton()
        {
            yield return new WaitForEndOfFrame();
            ShowOrHideResultButton();
        }
    }
}