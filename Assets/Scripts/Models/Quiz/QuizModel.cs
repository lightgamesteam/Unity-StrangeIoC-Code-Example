using UnityEngine;
using System.Collections;

using Generic = System.Collections.Generic;
using Json = Newtonsoft.Json;
using Conditions;
using System;
using PFS.Assets.Scripts.Commands.Download;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.SoundManagerModels;

namespace PFS.Assets.Scripts.Models.Quizzes
{
    /// <summary>
    /// Get quiz from server
    /// </summary>
    public class QuizResponseModel
    {
        [Json.JsonProperty("quiz")]
        public QuizModel quiz;
    }

    /// <summary>
    /// Quiz from server
    /// </summary>
    public class QuizModel
    {
        [Json.JsonProperty("_id")]
        public string quizId;

        [Json.JsonProperty("type")]
        public string quizType;

        [Json.JsonProperty("questions")]
        public QuizQuestion[] quizQuestions;

        [Json.JsonProperty("isSilent")]
        public bool forceSilentMode = false;

        [NonSerialized]
        public Conditions.QuizType quizTypeEnum;

        [NonSerialized]
        public string homeworkId;

        [NonSerialized]
        public string language;

        public void SetQuizType()
        {
            switch (quizType)
            {
                case "Tap conveyor":
                    quizTypeEnum = Conditions.QuizType.Type1;
                    break;
                case "Drag and drop line":
                    quizTypeEnum = Conditions.QuizType.Type2;
                    break;
                case "Answer ninja":
                    quizTypeEnum = Conditions.QuizType.Type3;
                    break;
                case "Memory game":
                    quizTypeEnum = Conditions.QuizType.Type4;
                    break;
                case "Complex quiz":
                    quizTypeEnum = Conditions.QuizType.Type5;
                    break;
                default:
                    quizTypeEnum = Conditions.QuizType.Type1;
                    break;
            }
        }
    }

    /// <summary>
    /// QuizQuestion from server
    /// </summary>
    public class QuizQuestion
    {
        [Json.JsonProperty("_id")]
        public string questionId;

        [Json.JsonProperty("name")]
        public string questionName;

        [Json.JsonProperty("question")]
        public string questionInfo;

        [Json.JsonProperty("questionImage")]
        public string questionImageURL;

        [Json.JsonProperty("narration")]
        public string narationURL;

        [Json.JsonProperty("explanation")]
        public string explanationURL;

        [Json.JsonProperty("tryCount")]
        public byte tryCount;

        [Json.JsonProperty("answers")]
        public QuizQuestionPart[] quizQuestionParts;

        [Json.JsonProperty("isOriginalNarration")]
        public bool needTranslateNarrations;

        [NonSerialized]
        public UnityEngine.AudioClip narationClip;

        [NonSerialized]
        public Generic.List<AudioClip> explanationClips;

        [NonSerialized]
        public string questionLanguage;

        [NonSerialized]
        public Conditions.QuizType quizType;

        [NonSerialized]
        public Conditions.SimplifiedLevels bookLevel;

        [NonSerialized]
        public bool forceSilentMode = true;

        public QuizQuestionPart GetFirstCorrectPart()
        {
            QuizQuestionPart result = null;

            foreach (var part in quizQuestionParts)
            {
                if (part.isCorrect)
                {
                    result = part;
                    break;
                }
            }

            return result;
        }

        public QuizQuestion()
        {
            explanationClips = new Generic.List<AudioClip>();
        }

        public QuizQuestionPart[] GetCorrectParts()
        {
            QuizQuestionPart[] result = new QuizQuestionPart[4];

            int partNum = 0;

            foreach (var part in quizQuestionParts)
            {
                if (part.isCorrect)
                {
                    result[partNum] = part;
                    partNum++;
                }
            }

            return result;
        }

        public bool IsAdditionalQuestionNarrations()
        {
            return IsAdditionalQuestionNarrations(GetFirstCorrectPart());
        }

        public bool IsAdditionalQuestionNarrations(QuizQuestionPart questionPart)
        {
            bool result = false;

            if (questionLanguage == Conditions.Languages.English.ToDescription())
            {
                if (questionPart.correctNarrations != null)
                {
                    foreach (string narrationUrl in questionPart.correctNarrations)
                    {
                        if (!string.IsNullOrEmpty(narrationUrl))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public bool IsTrueFalse()
        {
            return quizQuestionParts.Length == 2;
        }

        public bool IsTrueFalseQuestionWithTtsExplanation()
        {
            bool result = false;

            QuizQuestionPart falsePart = quizQuestionParts[1];

            if (quizType == Conditions.QuizType.Type5
            && IsTrueFalse()
            && falsePart.isCorrect
            && !IsAdditionalQuestionNarrations(falsePart)
            && string.IsNullOrEmpty(explanationURL)
            && !string.IsNullOrEmpty(falsePart.partInfo)
            && !string.IsNullOrEmpty(questionImageURL))
            {
                result = true;
            }

            return result;
        }

        public bool IsQuestionWithTtsExplanation()
        {
            bool result = false;

            if (quizType == Conditions.QuizType.Type5
            && !string.IsNullOrEmpty(questionImageURL))
            {
                result = true;
            }
            else if (quizType != Conditions.QuizType.Type5)
            {
                result = true;
            }

            return result;
        }

        public bool IsQuestionWithClipExplanation()
        {
            return questionLanguage == Conditions.Languages.English.ToDescription() && !string.IsNullOrEmpty(explanationURL);
        }

        public bool IsQuizSilent()
        {
            bool result = false;

            if (forceSilentMode ||
               (quizType == Conditions.QuizType.Type5 && string.IsNullOrEmpty(questionImageURL)))
            {
                result = true;
            }

            return result;
        }

        public bool IsQuestionWithEffects()
        {
            return bookLevel < Conditions.SimplifiedLevels.S4;
        }

        public bool IsQuestionLevelFourAndAbove()
        {
            return bookLevel >= Conditions.SimplifiedLevels.S4;
        }

        public bool IsQuestionLevelOne()
        {
            return bookLevel < Conditions.SimplifiedLevels.S2;
        }

        public bool IsLowLevelBook()
        { 
            return bookLevel < Conditions.SimplifiedLevels.S3;
        }
    }

    /// <summary>
    /// QuizQuestionPart from server
    /// </summary>
    public class QuizQuestionPart
    {
        [Json.JsonProperty("_id")]
        public string questionPartId;

        [Json.JsonProperty("audioFiles")]
        public string[] correctNarrations;

        [Json.JsonProperty("answer")]
        public string partInfo;

        [Json.JsonProperty("image")]
        public string imageURL;

        [Json.JsonProperty("correct")]
        public bool isCorrect;

        [NonSerialized]
        public UnityEngine.Sprite partSprite;
    }


    /// <summary>
    /// Quiz result from server
    /// </summary>
    public class QuizResultFromServer
    {
        [Json.JsonProperty("stars")]
        public byte stars;

        [Json.JsonProperty("coins")]
        public ushort coins;

        [NonSerialized]
        public bool isHomework;

        [NonSerialized]
        public Languages QuizLanguage;
    }

    //---------------------------------------------------------

    /// <summary>
    /// QuizUserModel to server
    /// </summary>
    public class QuizUserModel : BasicRequestModel
    {
        [Json.JsonProperty("quizId")]
        public string quizId;

        [Json.JsonProperty("results")]
        public QuizQuestionUserModel[] questionsResults;

        [Json.JsonProperty("homeworkId")]
        public string homeworkId;

        [Json.JsonProperty("language")]
        public string language;

        [Json.JsonProperty("questionsType")]
        public string questionsType;

        public QuizUserModel(string quizId, int questionsCount, string homeworkId, string language, Conditions.QuizQuestionsType questionsType) : base()
        {
            this.quizId = quizId;
            this.homeworkId = homeworkId;
            this.questionsType = questionsType.ToDescription();
            this.language = language;

            questionsResults = new QuizQuestionUserModel[questionsCount];
            for (int i = 0; i < questionsCount; i++)
            {
                questionsResults[i] = new QuizQuestionUserModel();
            }
        }
    }

    /// <summary>
    /// QuizQuestionUserModel to server
    /// </summary>
    public class QuizQuestionUserModel
    {
        [Json.JsonProperty("questionId")]
        public string questionId;

        [Json.JsonProperty("isCorrectAnswer")]
        public bool isCorrectAnswer;

        [Json.JsonProperty("progress")]
        public byte progress;

        [Json.JsonProperty("mistakes")]
        public Generic.List<QuizMistakeModel> mistakes = new Generic.List<QuizMistakeModel>();

        [Json.JsonProperty("spendingTime")]
        public uint spendingTime;

        [NonSerialized]
        public byte spendintTryCounter;
    }

    /// <summary>
    /// QuizMistakeModel to server
    /// </summary>
    public class QuizMistakeModel
    {
        [Json.JsonProperty("answerIds")]
        public string[] questionPartIds;

        [Json.JsonProperty("answerDescription")]
        public string answerDescription;
    }

    //---------------------------------------------------------

    /// <summary>
    /// Main static methods for quizzes
    /// </summary>
    public static class QuizHelper
    {
        public const byte partsInQuestion = 4;

        public static void GetNarationClip(string narationURL, Action<UnityEngine.AudioClip> audioClipTrueAction, Action audioClipFalseAction)
        {
            MainContextView.DispatchStrangeEvent(EventGlobal.E_DownloadAudioClip, new DownloadAudioClipParams(narationURL, audioClipTrueAction, audioClipFalseAction));
        }

        public static void GetQuestionPartImage(string imageURL, Action<UnityEngine.Sprite> imageAction, Action failAction)
        {
            MainContextView.DispatchStrangeEvent(EventGlobal.E_DownloadImage, new DownloadImageParams(imageURL, imageAction, failAction));
        }

        public static IEnumerator LoadTextToSpeechNarration(Conditions.Languages language, string narrationText, Action<AudioClip> action, float speakingSpeedChangePercents)
        {
            bool isError = false;
            bool isLoaded = false;

            if (!string.IsNullOrEmpty(narrationText))
            {
                MainContextView.DispatchStrangeEvent(EventGlobal.E_TextToSpeech, new TextToSpeechRequestModel(narrationText, language,
                                                    (AudioClip clip) =>
                                                    {
                                                        isLoaded = true;

                                                        action(clip);
                                                    },
                                                    () =>
                                                    {
                                                        isError = true;
                                                    },
                                                    speakingSpeedChangePercents
                                                    ));

                yield return new WaitUntil(() => isLoaded || isError);
            }
        }

        public static Conditions.Languages GetQuizLanguage(string language)
        {
            Conditions.Languages result = Conditions.Languages.English;

            foreach (Conditions.Languages languageEnum in Enum.GetValues(typeof(Conditions.Languages)))
            {
                if (languageEnum.ToDescription() == language)
                {
                    result = languageEnum;
                    break;
                }
            }

            return result;
        }

        public static IEnumerator PlayAdditionalQuestionNarrations(QuizQuestionPart questionPartInfo)
        {
            foreach (string narrationUrl in questionPartInfo.correctNarrations)
            {
                bool clipLoadingFailed = false;
                AudioClip narrationClip = null;

                QuizHelper.GetNarationClip(narrationUrl, (AudioClip clip) =>
                {
                    narrationClip = clip;
                },
                () =>
                {
                    clipLoadingFailed = true;
                });

                yield return new WaitUntil(() => narrationClip || clipLoadingFailed);

                if (!clipLoadingFailed)
                {
                    MainContextView.DispatchStrangeEvent(EventGlobal.E_SoundPlay, new AudioClipToCommand(narrationClip, Conditions.SoundType.Sound, 1, false, 1));

                    yield return new WaitForSeconds(narrationClip.length);
                }
            }
        }

    }

    //---------------------------------------------------------

    /// <summary>
    /// Question result to main user quiz
    /// </summary>
    public class QuestionResult
    {
        public string questionId;
        public bool isCorrectAnswer;
        public string[] selectedQuestionPartsIds;
    }

    /// <summary>
    /// for type 4
    /// </summary>
    public class QuizResultType4
    {
        public string partId;
        public Action action;
    }
}