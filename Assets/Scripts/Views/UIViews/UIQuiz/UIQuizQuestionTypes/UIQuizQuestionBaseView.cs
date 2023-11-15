using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using System;
using TMPro;
using System.Collections.Generic;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.SoundManagerModels;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionBaseView : BaseView
    {
        [Header("UI")]
        public TextMeshProUGUI questionInfoText;

        [Header("Parts")]
        public UIQuizQuestionPartBaseView[] questionParts;

        //----------------------------------------
        [HideInInspector]
        public QuizQuestion quizQuestion;

        private const int LongText = 100;
        private const int MiddleText = 50;
        private const int BonusTimeShortText = 4;
        private const int BonusTimeMiddleText = 9;
        private const int BonusTimeLongText = 12;

        protected bool endExplanation;

        public virtual void LoadView()
        {
            SetQuestionInfo();
            SetQuestionParts();
            SetBlockerForNaration();

            Dispatcher.AddListener(EventGlobal.E_QuizPartChoice, GetPartChoice);
            Dispatcher.AddListener(EventGlobal.E_QuizRightAnswer, InitCorrectAnswerDone);
            Dispatcher.AddListener(EventGlobal.E_QuizShowCorrect, InitShowCorrectAnswer);
            Dispatcher.AddListener(EventGlobal.E_QuizPlayNaration, SetBlockerForNaration);
        }

        public virtual void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_QuizPartChoice, GetPartChoice);
            Dispatcher.RemoveListener(EventGlobal.E_QuizRightAnswer, InitCorrectAnswerDone);
            Dispatcher.RemoveListener(EventGlobal.E_QuizShowCorrect, InitShowCorrectAnswer);
            Dispatcher.RemoveListener(EventGlobal.E_QuizPlayNaration, SetBlockerForNaration);
        }

        private void SetQuestionInfo()
        {
            if (quizQuestion == null)
            {
                Debug.LogError("UIQuizQuestion => quizQuestion - null");
                return;
            }

            string questionText = quizQuestion.questionInfo.Replace("\n", " ");
            questionText = questionText.Replace("  ", " ");

            questionInfoText.text = questionText;
        }

        private void SetQuestionParts()
        {
            if (quizQuestion == null)
            {
                Debug.LogError("UIQuizQuestion => quizQuestion - null");
                return;
            }

            if (questionParts == null || questionParts.Length != QuizHelper.partsInQuestion)
            {
                Debug.LogError("UIQuizQuestion => questionParts - null");
                return;
            }

            if (quizQuestion.quizQuestionParts == null)
            {
                Debug.LogError("UIQuizQuestion => quizQuestion.quizQuestionParts - null");
                return;
            }

            if(quizQuestion.quizQuestionParts.Length == 0)
            {
                Debug.LogError($"UIQuizQuestion => quizQuestion.quizQuestionParts - array is empty - quizQuestion: {quizQuestion.questionInfo}");
                return;
            }

            for (int i = 0; i < questionParts.Length && i < quizQuestion.quizQuestionParts.Length; i++)
            {
                questionParts[i].quizQuestionPart = quizQuestion.quizQuestionParts[i];
                questionParts[i].InitPart();
            }
        }

        private void SetBlockerForNaration()
        {
            // Play narration or TTS narration for complex quizz only when it has image,
            // for other quiz types and for complex quiz with question image, narration or TTS narration should always be played
            if (quizQuestion.quizType != Conditions.QuizType.Type5
            || (quizQuestion.quizType == Conditions.QuizType.Type5 && !string.IsNullOrEmpty(quizQuestion.questionImageURL)))
            {
                StartCoroutine(SetBlockerForNarationProcess());
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
            }
        }

        private IEnumerator SetBlockerForNarationProcess()
        {
            MainContextView.DispatchStrangeEvent(EventGlobal.E_QuizBlocker, true);

            if (!quizQuestion.IsQuizSilent())
            {
                yield return StartCoroutine(ProcessQuestionNarration());
            }

            // hide blocker
            Dispatcher.Dispatch(EventGlobal.E_QuizNarationEnd);
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
        }

        private IEnumerator ProcessQuestionNarration()
        {
            float speakingSpeedChangePercents = quizQuestion.IsQuestionLevelOne() ? TextToSpeechRequestModel.SlowlSpeedSpeaking : TextToSpeechRequestModel.NormalSpeedChangeSpeaking;
            if (quizQuestion.questionLanguage == Conditions.Languages.English.ToDescription())
            {
                if (string.IsNullOrEmpty(quizQuestion.narationURL))
                {
                    yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.English, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
                }
                else
                {
                    yield return StartCoroutine(LoadEnglishNarration());
                }
            }
            else if (quizQuestion.questionLanguage == Conditions.Languages.British.ToDescription())
            {
                yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.British, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
            }
            else if (quizQuestion.questionLanguage == Conditions.Languages.Danish.ToDescription())
            {
                yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.Danish, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
            }
            else if (quizQuestion.questionLanguage == Conditions.Languages.Norwegian.ToDescription())
            {
                yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.Norwegian, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
            }
            else if (quizQuestion.questionLanguage == Conditions.Languages.NyNorsk.ToDescription())
            {
                yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.NyNorsk, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
            }
            else if (quizQuestion.questionLanguage == Conditions.Languages.Chinese.ToDescription())
            {
                yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(Conditions.Languages.Chinese, quizQuestion.questionInfo, (clip) => quizQuestion.narationClip = clip, speakingSpeedChangePercents));
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
                yield break;
            }

            if (quizQuestion.narationClip)
            {
                // play AudioClip
                Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(quizQuestion.narationClip, Conditions.SoundType.Sound, 1, false, 1));

                // wait AudioClip over
                yield return new WaitForSeconds(quizQuestion.narationClip.length);
            }
        }

        private IEnumerator LoadEnglishNarration()
        {
            if (string.IsNullOrEmpty(quizQuestion.narationURL))
            {
                Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
                yield break;
            }

            bool isLoadError = false;

            // show blocker
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);

            // load AudioClip
            QuizHelper.GetNarationClip(quizQuestion.narationURL, (AudioClip clip) =>
            {
                if (quizQuestion != null)
                    quizQuestion.narationClip = clip;
            },
            () =>
            {
                isLoadError = true;
            });

            // wait downloading AudioClip
            yield return new WaitUntil(() => quizQuestion.narationClip || isLoadError);
        }

        private void GetPartChoice(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizQuestion | GetPartChoice => data - null");
                return;
            }

            string[] partsIds;

            if (e.data is string[])
            {
                partsIds = (string[])e.data;
            }
            else
            {
                partsIds = new string[1] { e.data.ToString() };
            }

            bool isCorrect = IsCorect(partsIds);

            PlayPartChoiceSound(isCorrect);

            QuestionResult result = new QuestionResult { questionId = quizQuestion.questionId, isCorrectAnswer = isCorrect, selectedQuestionPartsIds = partsIds };
            StartCoroutine(QuestionResult(result));
        }

        private void PlayPartChoiceSound(bool isCorrect)
        {
            if (isCorrect)
            {
                MainContextView.DispatchStrangeEvent(EventGlobal.E_SoundPlay, new AudioClipToCommand(SoundManagerModel.instance.winEffect, Conditions.SoundType.Sound, 1, false, 1));
            }
            else
            {
                MainContextView.DispatchStrangeEvent(EventGlobal.E_SoundPlay, new AudioClipToCommand(SoundManagerModel.instance.loseEffect, Conditions.SoundType.Sound, 1, false, 1));
            }
        }

        protected bool IsCorect(string[] partsIds)
        {
            List<string> correctPartsIds = new List<string>();

            // find all coorect parts
            foreach (var mainPart in quizQuestion.quizQuestionParts)
            {
                if (mainPart.isCorrect)
                {
                    correctPartsIds.Add(mainPart.questionPartId);
                }
            }

            if (partsIds.Length != correctPartsIds.Count)
            {
                return false;
            }

            foreach (var choicePart in partsIds)
            {
                if (!correctPartsIds.Contains(choicePart))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator QuestionResult(QuestionResult result)
        {
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);
            yield return StartCoroutine(QuestionResultVisualDelays());
            Dispatcher.Dispatch(EventGlobal.E_QuizQuestionResult, result);
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
        }

        protected virtual IEnumerator QuestionResultVisualDelays()
        {
            yield return null;
        }

        private void InitCorrectAnswerDone(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizQuestion | InitCorrectAnswerDone => data - null");
                return;
            }

            Action action = e.data as Action;
            StartCoroutine(CorrectAnswerDoneProcess(action));
        }

        private IEnumerator CorrectAnswerDoneProcess(Action action)
        {
            yield return new WaitForEndOfFrame(); // need to wait (QuestionResult set blocker false)
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);
            yield return StartCoroutine(CorrectAnswerDone());
            if (action != null)
            {
                action();
            }
        }

        protected virtual IEnumerator CorrectAnswerDone()
        {
            yield return null;
        }

        protected virtual IEnumerator ShowCorrectAnswer()
        {
            yield return null;
        }

        protected virtual void PrepareContentFading()
        {

        }

        protected IEnumerator ShowCorrectProcess()
        {
            foreach (var part in questionParts)
            {
                if (part.quizQuestionPart != null && part.quizQuestionPart.isCorrect)
                {
                    yield return StartCoroutine(part.ShowCorrectResult());
                }
            }
        }

        private void InitShowCorrectAnswer(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizQuestion | InitShowCorrectAnswer => data - null");
                return;
            }

            Action action = e.data as Action;
            StartCoroutine(ShowCorrectAnswerProcess(action));
        }

        private IEnumerator ShowCorrectAnswerProcess(Action action)
        {
            yield return new WaitForEndOfFrame(); // need to wait (QuestionResult set blocker false)
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);

            PrepareContentFading();

            bool isQuizSilent = quizQuestion.IsQuizSilent();

            if (quizQuestion.IsTrueFalse() && quizQuestion.IsQuestionLevelFourAndAbove())
            {
                yield return new WaitForSeconds(AddDelay());
            }

            if (!isQuizSilent)
            {
                yield return StartCoroutine(LoadExplanations());

                StartCoroutine(PlayExplanations());
            }

            yield return StartCoroutine(ShowCorrectAnswer());

            yield return new WaitUntil(() => isQuizSilent || endExplanation);

            if (action != null)
            {
                action();
            }
        }

        private IEnumerator LoadExplanations()
        {
            bool isExplanationLoadingError = false;

            quizQuestion.explanationClips.Clear();
            float speakingSpeedChangePercents = quizQuestion.IsQuestionLevelOne() ? TextToSpeechRequestModel.SlowlSpeedSpeaking : TextToSpeechRequestModel.NormalSpeedChangeSpeaking;
            if (quizQuestion.IsTrueFalseQuestionWithTtsExplanation())
            {
                yield return QuizHelper.LoadTextToSpeechNarration(QuizHelper.GetQuizLanguage(quizQuestion.questionLanguage), quizQuestion.quizQuestionParts[1].partInfo, (clip) => quizQuestion.explanationClips.Add(clip), speakingSpeedChangePercents);
            }
            else if (quizQuestion.IsQuestionWithClipExplanation())
            {
                // load AudioClip
                QuizHelper.GetNarationClip(quizQuestion.explanationURL, (AudioClip clip) =>
                {
                    if (quizQuestion != null)
                        quizQuestion.explanationClips.Add(clip);
                },
                () =>
                {
                    isExplanationLoadingError = true;
                });

                // wait downloading AudioClip
                yield return new WaitUntil(() => quizQuestion.explanationClips.Count != 0 || isExplanationLoadingError);
            }
            else if (quizQuestion.IsQuestionWithTtsExplanation())
            {
                QuizQuestionPart[] correctParts = quizQuestion.GetCorrectParts();

                foreach (var correctPart in correctParts)
                {
                    if (correctPart != null)
                    {
                        yield return QuizHelper.LoadTextToSpeechNarration(QuizHelper.GetQuizLanguage(quizQuestion.questionLanguage), correctPart.partInfo, (clip) => quizQuestion.explanationClips.Add(clip), speakingSpeedChangePercents);
                    }
                }
            }
        }

        private float AddDelay()
        {
            QuizQuestionPart[] correctParts = quizQuestion.GetCorrectParts();
            int bonusTime = 0;

            foreach (var correctQuestionPartInfo in correctParts)
            {
                if (correctQuestionPartInfo != null)
                {
                    int lengthText = correctQuestionPartInfo.partInfo.Length;
                    if (lengthText > LongText)
                    {
                        bonusTime = BonusTimeLongText;
                    }
                    else if (lengthText > MiddleText)
                    {
                        bonusTime = BonusTimeMiddleText;
                    }
                    else if (lengthText > 0)
                    {
                        bonusTime = BonusTimeShortText;
                    }
                }
            }
            return bonusTime;
        }

        protected virtual IEnumerator PlayExplanations()
        {
            endExplanation = false;

            if (quizQuestion.explanationClips.Count > 0 && !quizQuestion.IsLowLevelBook())
            {
                foreach (var explanationClip in quizQuestion.explanationClips)
                {
                    if (explanationClip != null)
                    {
                        // play AudioClip
                        Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(explanationClip, Conditions.SoundType.Sound, 1, false, 1));

                        // wait AudioClip over
                        yield return new WaitForSeconds(explanationClip.length);
                    }
                    else
                    {
                        Debug.LogError($"Quiz question, explanation audio clip is null");
                    }
                }
            }
            else if (quizQuestion.IsLowLevelBook())
            {
                QuizQuestionPart[] correctParts = quizQuestion.GetCorrectParts();
                float speakingSpeedChangePercents = quizQuestion.IsQuestionLevelOne() ? TextToSpeechRequestModel.SlowlSpeedSpeaking : TextToSpeechRequestModel.NormalSpeedChangeSpeaking;
                foreach (var correctQuestionPartInfo in correctParts)
                {
                    if (correctQuestionPartInfo != null)
                    {
                        if (quizQuestion.IsAdditionalQuestionNarrations(correctQuestionPartInfo))
                        {
                            yield return StartCoroutine(QuizHelper.PlayAdditionalQuestionNarrations(correctQuestionPartInfo));
                        }
                        else
                        {
                            AudioClip correctAnswerClip = null;
                            yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(QuizHelper.GetQuizLanguage(quizQuestion.questionLanguage), correctQuestionPartInfo.partInfo, (clip) => correctAnswerClip = clip, speakingSpeedChangePercents));
                            if (correctAnswerClip != null)
                            {
                                Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(correctAnswerClip, Conditions.SoundType.Sound, 1, false, 1));
                                yield return new WaitForSeconds(correctAnswerClip.length);
                            }
                        }
                    }
                }
            }
            endExplanation = true;
        }
    }
}