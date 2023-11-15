using Assets.Scripts.Services.Analytics;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Quizzes;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;


namespace PFS.Assets.Scripts.Commands.Network
{
    public class SetQuizResultCommand : BaseNetworkCommand
    {
        [Inject] public Analytics Analytics { get; private set; }

        private string apiPath = APIPaths.SET_QUIZ_CHILD_RESULT.ToDescription();
        private QuizUserModel request;
        private QuizResultFromServer response;

        public override void Execute()
        {
            Retain();
            Debug.Log("SetQuizResultCommand");

            if (EventData.data == null)
            {
                Debug.LogError("SetQuizResultCommand | No data");
                Fail();
                return;
            }

            request = EventData.data as QuizUserModel;

            if (request == null)
            {
                Debug.LogError("SetQuizResultCommand | request - null");
                Fail();
                return;
            }

            PrepareObject(request);

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult));
        }

        private void CheckResult(ResultNetworkModel result)
        {
            void DisplayError(string error)
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel
                {
                    screenName = UIScreens.UIErrorPopup,
                    data = $"Technical info: {apiPath}, Error: {error}",
                    isAddToScreensList = false
                });
            }

            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
                DisplayError("no answer");
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
                DisplayError("jsonObject = NULL");
                Fail();
                return;
            }

            Parse(result.jsonObject);

            Conditions.Languages language;
            Enum.TryParse(request.language, out language);

            if (response != null)
            {
                response.isHomework = !string.IsNullOrEmpty(request.homeworkId);
                response.QuizLanguage = language;
            }

            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIFinalQuizPopup, data = response, isAddToScreensList = false, showSwitchAnim = false });

            Analytics.LogEvent(EventName.NavigationQuizScore,
                     new System.Collections.Generic.Dictionary<Property, object>()
                     {
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.QuizId, request.quizId},
                            { Property.QuizScoreCoins, response.coins.ToString()},
                            { Property.Translation, response.QuizLanguage.ToDescription()}
                     });

            var quizCorrectAnswer = request.questionsResults.Where(x => x.isCorrectAnswer == true).ToList().Count;
            var quizIncorrectAnswer = request.questionsResults.Length - quizCorrectAnswer;

            var currentBook = BooksLibrary.Instance.GetBookByGuizId(request.quizId);
            Analytics.LogEvent(EventName.ActionOnQuizQuestionAnswer,
                new System.Collections.Generic.Dictionary<Property, object>()
                {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                        { Property.QuizId, request.quizId},
                        { Property.ISBN, currentBook?.GetTranslation().Isbn},
                        { Property.Category, currentBook?.GetInterests()},
                        { Property.QuizCorrectAnswer, quizCorrectAnswer.ToString()},
                        { Property.QuizIncorrectAnswer, quizIncorrectAnswer.ToString()}
                });
            request.requestTrueAction?.Invoke();

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<QuizResultFromServer>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("SetQuizResultCommand parse error. Error text: {0}", e);
            }
        }
    }
}