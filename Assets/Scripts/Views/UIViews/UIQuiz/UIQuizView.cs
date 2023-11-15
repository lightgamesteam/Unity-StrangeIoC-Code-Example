using UnityEngine;
using UnityEngine.UI;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views.DebugScreen;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        [Header("UI")]
        public Button exitButton;
        public Button pauseButton;
        public Button repeatButton;

        public UIQuizProgressView progress;
        public Transform quizArea;
        public GameObject blocker;

        [Header("Params"), SerializeField]
        private float waitButtonsTime;

        [Header("Sound effects")]
        public AudioClip winAdditionalSound;
        public AudioClip loseAdditionalSound;

        //---------------------------
        private const int LongText = 100;
        private const int MiddleText = 50;
        private const int BonusTimeShortText = 4;
        private const int BonusTimeMiddleText = 9;
        private const int BonusTimeLongText = 12;

        private QuizModel quiz;
        private QuizUserModel quizUser;

        private byte currentQuestion;

        private uint spendingSeconds;
        private IEnumerator timer;

        public void LoadView()
        {
            GetQuizData();
            InitQuizUserData();
            InitProgress();
            InitQuestion(currentQuestion);
            ResetAdditionalSoundsIfNeed();

            StartCoroutine(WaitButtons());

            exitButton.onClick.AddListener(() =>
            {
                Dispatcher.Dispatch(EventGlobal.E_QuizQuittedCommand, new QuizQuittedRequestModel(quiz.quizId));

                Dispatcher.Dispatch(EventGlobal.E_SoundStopAll);
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIQuiz);

                var currentBook = BooksLibrary.Instance.GetBookByGuizId(quiz.quizId);
                Analytics.LogEvent(EventName.ActionCloseQuiz,
                        new System.Collections.Generic.Dictionary<Property, object>()
                        {
                            { Property.QuizId, quiz.quizId},
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.ISBN, currentBook?.GetTranslation().Isbn},
                            { Property.Category, currentBook?.GetInterests()}
                        });

                double currentQuestionNum = Convert.ToInt32(currentQuestion) + 1;
                double allQuestions = quiz.quizQuestions.Length;
                double percentOfSeen = Math.Round(currentQuestionNum / allQuestions * 100, 3);

                Analytics.LogEvent(EventName.ActionQuizCompletePercentageSeen,
                        new System.Collections.Generic.Dictionary<Property, object>()
                        {
                            { Property.QuizId, quiz.quizId},
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.ISBN, currentBook?.GetTranslation().Isbn},
                            { Property.Category, currentBook?.GetInterests()},
                            { Property.QuizQuestionsSeenCount, currentQuestionNum.ToString()},
                            { Property.Seen, percentOfSeen.ToString() + " %"}
                        });
            });

            repeatButton.onClick.AddListener(() =>
            {
                var currentBook = BooksLibrary.Instance.GetBookByGuizId(quiz.quizId);
                Dispatcher.Dispatch(EventGlobal.E_QuizPlayNaration);
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);

                repeatButton.interactable = false;
                Analytics.LogEvent(EventName.ActionReplayQuizQuestion,
                       new System.Collections.Generic.Dictionary<Property, object>()
                       {
                            { Property.QuizId, quiz.quizId},
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.ISBN, currentBook?.GetTranslation().Isbn},
                            { Property.Category, currentBook?.GetInterests()}
                       });
            });

            pauseButton.onClick.AddListener(PauseProcess);

            Dispatcher.Dispatch(EventGlobal.E_QuizStartedCommand, new QuizStartedRequestModel(quiz.quizId));

            Dispatcher.AddListener(EventGlobal.E_QuizQuestionResult, GetQuestionResult);
            Dispatcher.AddListener(EventGlobal.E_QuizBlocker, SetBlocker);
            Dispatcher.AddListener(EventGlobal.E_QuizNarationEnd, ProcessNarationEnd);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_QuizQuestionResult, GetQuestionResult);
            Dispatcher.RemoveListener(EventGlobal.E_QuizBlocker, SetBlocker);
            Dispatcher.RemoveListener(EventGlobal.E_QuizNarationEnd, ProcessNarationEnd);
        }

        private void ResetAdditionalSoundsIfNeed()
        {
            if (quiz.language != Conditions.Languages.English.ToDescription())
            {
                winAdditionalSound = null;
                loseAdditionalSound = null;
            }
        }

        private void GetQuizData()
        {
            if (otherData != null)
            {
                quiz = otherData as QuizModel;

                if (quiz == null)
                {
                    Debug.LogError("UIQuizView - otherData NULL");
                    return;
                } 
            }
            else
            {
                Debug.LogError("UIQuizView - no otherData");
            }
        }

        private void InitProgress()
        {
            if (progress)
            {
                progress.InitProgress((byte)quiz.quizQuestions.Length);
            }
        }

        private void InitQuizUserData()
        {
            if (quiz != null)
            {
                quizUser = new QuizUserModel(quiz.quizId, quiz.quizQuestions.Length, quiz.homeworkId, quiz.language, DebugView.QuizQuestionsType);

                for (int i = 0; i < quiz.quizQuestions.Length; i++)
                {
                    quizUser.questionsResults[i].questionId = quiz.quizQuestions[i].questionId;
                }
            }
        }

        private void InitQuestion(byte questionNum)
        {
            if (quiz == null)
            {
                return;
            }

            if (questionNum < quiz.quizQuestions.Length)
            {
                repeatButton.gameObject.SetActive(false);
                Debug.Log($"Question number: {questionNum}");
                SetCurrentQuestionProgress(questionNum);
                SetQuestionByType(quiz.quizTypeEnum, quiz.quizQuestions[questionNum]);
                StartTimer();
            }
            else
            {
                SetQuizResultToServer();
            }
        }

        private void NextQuestion()
        {
            ClearReferences();

            currentQuestion++;
            InitQuestion(currentQuestion);
        }

        private void SetCurrentQuestionProgress(byte questionNum)
        {
            if (progress)
            {
                progress.SetCurrentProgress(questionNum);
            }
        }

        private void SetQuestionByType(Conditions.QuizType quizType, QuizQuestion quizQuestion)
        {
            // clear area
            foreach (Transform child in quizArea)
            {
                Destroy(child.gameObject);
            }

            GameObject questionGM = Resources.Load("Prefabs/UI/Items/Quizzes/QuizzesByType/" + quizType.ToDescription()) as GameObject;
            if (!questionGM)
            {
                Debug.LogError("UIQuizView | SetQuestionByType - load error");
                return;
            }

            GameObject questionInst = Instantiate(questionGM, quizArea);
            UIQuizQuestionBaseView quizQuestionBase = questionInst.GetComponent<UIQuizQuestionBaseView>();
            if (quizQuestionBase == null)
            {
                Debug.LogError("UIQuizView | SetQuestionByType - quizQuestionBase error");
                return;
            }
            quizQuestionBase.quizQuestion = quizQuestion;
        }

        private void SetQuizResultToServer()
        {
            quizUser.requestTrueAction = () => Debug.Log("E_SetQuizResultToServer - requestTrueAction");
            quizUser.requestFalseAction = () => Debug.Log("E_SetQuizResultToServer - requestFalseAction");
            Dispatcher.Dispatch(EventGlobal.E_SetQuizResultToServer, quizUser);
        }

        private void GetQuestionResult(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizView | GetQuestionResult => data - null");
                return;
            }

            QuestionResult result = e.data as QuestionResult;
            if (result == null)
            {
                Debug.LogError("UIQuizView | GetQuestionResult => result - null");
                return;
            }

            CalculateResult(result);
        }

        private void CalculateResult(QuestionResult result)
        {
            foreach (QuizQuestionUserModel question in quizUser.questionsResults)
            {
                if (question.questionId == result.questionId)
                {
                    if (question.spendintTryCounter >= quiz.quizQuestions[currentQuestion].tryCount)
                    {
                        return;
                    }

                    if (result.isCorrectAnswer)
                    {
                        Dispatcher.Dispatch(EventGlobal.E_QuizFadeContent);
                        StartCoroutine(NextQuestionAfterCorrectNarrations(question));

                        break;
                    }
                    else
                    {
                        question.mistakes.Add(new QuizMistakeModel { questionPartIds = result.selectedQuestionPartsIds, answerDescription = "" });
                        question.spendintTryCounter++;

                        if (question.spendintTryCounter >= quiz.quizQuestions[currentQuestion].tryCount)
                        {
                            repeatButton.interactable = false;

                            question.isCorrectAnswer = false;
                            question.progress = 0;
                            question.spendingTime = spendingSeconds;
                            Action action = () =>
                            {
                                NextQuestion();
                            };

                            Dispatcher.Dispatch(EventGlobal.E_QuizFadeContent);
                            Dispatcher.Dispatch(EventGlobal.E_QuizShowCorrect, action);
                            break;
                        }

                        StartCoroutine(NextTryAfterWrongNarrations());
                    }
                }
            }
        }

        private byte GetQuestionProgress(int tryCount, int spendintTryCount)
        {
            int res;
            if (tryCount == 3) //tryCount = 3 => 100-100-50
            {
                res = tryCount - spendintTryCount;
                res = res > 2 ? 2 : res;
                return (byte)((float)res / 2 * 100f);
            }

            // tryCount = 1 => 100 | tryCount = 2 => 100-50
            res = tryCount - spendintTryCount;
            return (byte)((float)res / tryCount * 100f);
        }

        private void SetBlocker(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizView | SetBlocker => data - null");
                return;
            }

            bool res = (bool)e.data;

            blocker.SetActive(res);
        }

        private void StartTimer()
        {
            spendingSeconds = 0;
            if (timer != null)
            {
                StopCoroutine(timer);
            }
            timer = StartTimerIenum();
            StartCoroutine(timer);
        }

        private IEnumerator StartTimerIenum()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                spendingSeconds++;
            }
        }

        private void ClearReferences()
        {
            quiz.quizQuestions[currentQuestion].narationClip = null;
            quiz.quizQuestions[currentQuestion].explanationClips.Clear();

            foreach (var part in quiz.quizQuestions[currentQuestion].quizQuestionParts)
            {
                part.partSprite = null;
            }
        }

        private IEnumerator WaitButtons()
        {
            exitButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(waitButtonsTime);
            pauseButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
        }

        private IEnumerator WaitSoundEffect(AudioClip clip)
        {
            if (clip == null)
                yield break;

            yield return new WaitForSeconds(1f);

            Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(clip, Conditions.SoundType.Sound, 1, false, 1));

            yield return new WaitForSeconds(clip.length);
        }

        private void PauseProcess()
        {
            Time.timeScale = 0.0f;

            Conditions.Languages language;
            Enum.TryParse(quiz.language, out language);
            Dispatcher.Dispatch(EventGlobal.E_SoundPauseAll);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIQuizPausePopup, data = language, isAddToScreensList = false });

            var currentBook = BooksLibrary.Instance.GetBookByGuizId(quiz.quizId);
            Analytics.LogEvent(EventName.ActionPauseQuiz,
                 new System.Collections.Generic.Dictionary<Property, object>()
                 {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.QuizId, quiz.quizId},
                    { Property.ISBN, currentBook?.GetTranslation().Isbn},
                    { Property.Category,  currentBook.GetInterests()}
                 });
        }

        private void ProcessNarationEnd()
        {
            repeatButton.gameObject.SetActive(true);
            repeatButton.interactable = true;
        }

        private IEnumerator NextTryAfterWrongNarrations()
        {
            yield return new WaitForEndOfFrame();

            repeatButton.interactable = false;

            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);
            Dispatcher.Dispatch(EventGlobal.E_QuizNextTry);

            QuizQuestion currQuestion = quiz.quizQuestions[currentQuestion];

            if (!currQuestion.IsQuizSilent() && currQuestion.IsQuestionWithEffects())
            {
                yield return StartCoroutine(PrepareAdditionalSound(false));
                yield return StartCoroutine(WaitSoundEffect(loseAdditionalSound));
            }

            repeatButton.interactable = true;
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
        }

        private IEnumerator NextQuestionAfterCorrectNarrations(QuizQuestionUserModel question)
        {
            yield return new WaitForEndOfFrame();

            repeatButton.interactable = false;
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);

            QuizQuestion currQuestion = quiz.quizQuestions[currentQuestion];

            if (!currQuestion.IsQuizSilent())
            {
                if (currQuestion.IsTrueFalseQuestionWithTtsExplanation())
                {
                    yield return PlayTrueFalseTtsExplanationSound(currQuestion.quizQuestionParts[1].partInfo);
                }
                else
                {
                    if (!IsQuestionWithAnswerExplanation())
                    {
                        if (currQuestion.IsQuestionWithEffects())
                        {
                            yield return StartCoroutine(PrepareAdditionalSound(true));
                            yield return StartCoroutine(WaitSoundEffect(winAdditionalSound));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(PlayCorrectNarrationSounds());
                    }
                }
            }
            if (quiz.quizQuestions[currentQuestion].IsTrueFalse() && quiz.quizQuestions[currentQuestion].IsQuestionLevelFourAndAbove())
            {
                yield return new WaitForSeconds(AddDelay());
            }

            question.isCorrectAnswer = true;
            question.progress = GetQuestionProgress(currQuestion.tryCount, question.spendintTryCounter);
            question.spendingTime = spendingSeconds;
            Action action = () =>
            {
                NextQuestion();
            };

            Dispatcher.Dispatch(EventGlobal.E_QuizRightAnswer, action);

        }

        private IEnumerator PlayCorrectNarrationSounds()
        {
            QuizQuestionPart[] correctParts = quiz.quizQuestions[currentQuestion].GetCorrectParts();

            float speakingSpeedChangePercents = quiz.quizQuestions[currentQuestion].IsQuestionLevelOne() ? TextToSpeechRequestModel.SlowlSpeedSpeaking : TextToSpeechRequestModel.NormalSpeedChangeSpeaking;
            foreach (var correctQuestionPartInfo in correctParts)
            {
                if (correctQuestionPartInfo != null)
                {
                    if (quiz.quizQuestions[currentQuestion].IsAdditionalQuestionNarrations(correctQuestionPartInfo))
                    {
                        yield return StartCoroutine(QuizHelper.PlayAdditionalQuestionNarrations(correctQuestionPartInfo));
                    }
                    else
                    {
                        AudioClip correctAnswerClip = null;

                        if (correctQuestionPartInfo.isCorrect && quiz.quizQuestions[currentQuestion].IsTrueFalse())
                        {
                            // if need to TTS read in correct answer, write code here
                        }
                        else
                        {
                            yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(QuizHelper.GetQuizLanguage(quiz.language), correctQuestionPartInfo.partInfo, (clip) => correctAnswerClip = clip, speakingSpeedChangePercents));
                        }

                        if (correctAnswerClip != null)
                        {
                            Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(correctAnswerClip, Conditions.SoundType.Sound, 1, false, 1));
                            yield return new WaitForSeconds(correctAnswerClip.length);
                        }
                    }
                }
            }
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
        }

        private float AddDelay()
        {
            QuizQuestionPart[] correctParts = quiz.quizQuestions[currentQuestion].GetCorrectParts();
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

        private bool IsQuestionWithAnswerExplanation()
        {
            QuizQuestion currQuestion = quiz.quizQuestions[currentQuestion];
            return currQuestion.IsQuestionWithTtsExplanation() || currQuestion.IsAdditionalQuestionNarrations();
        }

        private IEnumerator PlayTrueFalseTtsExplanationSound(string explanation)
        {
            AudioClip trueFalseClip = null;
            float speakingSpeedChangePercents = quiz.quizQuestions[currentQuestion].IsQuestionLevelOne() ? TextToSpeechRequestModel.SlowlSpeedSpeaking : TextToSpeechRequestModel.NormalSpeedChangeSpeaking;
            yield return StartCoroutine(QuizHelper.LoadTextToSpeechNarration(QuizHelper.GetQuizLanguage(quiz.language), explanation, (clip) => trueFalseClip = clip, speakingSpeedChangePercents));

            if (trueFalseClip != null)
            {
                Dispatcher.Dispatch(EventGlobal.E_SoundPlay, new AudioClipToCommand(trueFalseClip, Conditions.SoundType.Sound, 1, false, 1));
                yield return new WaitForSeconds(trueFalseClip.length);
            }
            else
            {
                Debug.LogError($"True false TTS explanation sound clip is null");
            }
        }

        private IEnumerator PrepareAdditionalSound(bool isWinSound)
        {
            if (quiz.language == Conditions.Languages.English.ToDescription())
            {
                yield break;
            }

            if ((isWinSound && winAdditionalSound) || (!isWinSound && loseAdditionalSound))
            {
                yield break;
            }
            else if (quiz.language == Conditions.Languages.Norwegian.ToDescription())
            {
                yield return StartCoroutine(LoadAdditionalTextToSpeechSound(isWinSound ? "Det er riktig" : "Prøv igjen", Conditions.Languages.Norwegian, isWinSound));
            }
        }

        private IEnumerator LoadAdditionalTextToSpeechSound(string text, Conditions.Languages language, bool isWinSound)
        {
            bool isError = false;

            Dispatcher.Dispatch(EventGlobal.E_TextToSpeech, new TextToSpeechRequestModel(text, language,
                (AudioClip clip) =>
                {
                    if (isWinSound)
                    {
                        winAdditionalSound = clip;
                    }
                    else
                    {
                        loseAdditionalSound = clip;
                    }
                },
                () =>
                {
                    isError = true;
                }
                ));

            yield return new WaitUntil(() => (isWinSound ? winAdditionalSound : loseAdditionalSound) || isError);

        }
    }
}