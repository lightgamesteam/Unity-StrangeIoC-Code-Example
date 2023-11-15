using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using PFS.Assets.Scripts.Views.Quizzes;
using System;
using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetQuizCommand : BaseNetworkCommand
    {
        [Inject] public IExecutor Coroutine { get; private set; }
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        private string apiPath = APIPaths.GET_QUIZ_FOR_CHILD.ToDescription();
        private QuizRequestModel request;
        private QuizResponseModel response;

        public override void Execute()
        {
            Retain();
            Debug.Log("GetQuizCommand");

            if (EventData.data == null)
            {
                Debug.LogError("GetQuizCommand | No data");
                Fail();
                return;
            }

            request = EventData.data as QuizRequestModel;

            if (request == null)
            {
                Debug.LogError("GetQuizCommand | request - null");
                Fail();
                return;
            }

            ////load local file
            //TextAsset quizText = Resources.Load<TextAsset>("Quiz");
            //if (quizText == null)
            //{
            //    Debug.LogError("Quiz load error");
            //    return;
            //}
            //JObject jsonObject = JObject.Parse(quizText.text);
            //Parse(jsonObject);

            //if (response.quiz != null && response.quiz.quizId != null)
            //{
            //    if (request.requestTrueAction != null)
            //        request.requestTrueAction();

            //    Coroutine.Execute(InitQuiz());
            //}
            //else
            //{
            //    Release();
            //}
            //return;
            ////---------------

            PrepareObject(request);

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult));
        }

        private void CheckResult(ResultNetworkModel result)
        {
            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                request.requestFalseAction?.Invoke();
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                request.requestFalseAction?.Invoke();
                Fail();
                return;
            }

            Parse(result.jsonObject);
            BookModel quizBook = BooksLibrary.GetBook(request.bookId);

            if (response.quiz != null && response.quiz.quizId != null && quizBook != null)
            {
                response.quiz.SetQuizType();
                response.quiz.homeworkId = request.homeworkId;
                response.quiz.language = request.language;

                foreach (var question in response.quiz.quizQuestions)
                {
                    /* Only for test issue UIQuizQuestionPart - QuizQuestionPart - NULL */

                    if (question.quizQuestionParts == null)
                    {
                        Debug.Log($"{nameof(GetQuizCommand)} - question.quizQuestionParts is NULL (question: {question.questionName} book: {quizBook.Name} lang: {quizBook.CurrentTranslation.ToDescription()})");
                        return;
                    }

                    if(question.quizQuestionParts.Length == 0)
                    {
                        Debug.Log($"{nameof(GetQuizCommand)} - question.quizQuestionParts is empty (question: {question.questionName} book: {quizBook.Name} lang: {quizBook.CurrentTranslation.ToDescription()})");
                        return;
                    }

                    /************************************************/

                    question.questionLanguage = request.language;
                    question.forceSilentMode = response.quiz.forceSilentMode;
                    question.quizType = response.quiz.quizTypeEnum;
                    question.bookLevel = quizBook.SimplifiedLevelEnum;
                }
                request.requestTrueAction?.Invoke();
                Coroutine.Execute(InitQuiz());
            }
            else
            {
                Release();
            }
        }

        private IEnumerator InitQuiz()
        {
            yield return new WaitUntil(() => !UIQuizSplashScreenView.ShowingSplashAnimation);

            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIQuizSplashScreen);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIQuiz, data = response.quiz, isAddToScreensList = false });

            Dispatcher.Dispatch(EventGlobal.E_SoundUnPauseMusic);

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<QuizResponseModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetQuizCommand parse error. Error text: {0}", e);
            }
        }
    }
}