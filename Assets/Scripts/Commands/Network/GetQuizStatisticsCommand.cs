using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Statistics;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetQuizStatisticsCommand : BaseNetworkCommand
    {
        [Inject]
        public ChildModel ChildModel { get; set; }

        [Inject]
        public QuizStatisticsModel QuizStatResponseModel { get; set; }

        private string apiPath = APIPaths.GET_STATISTIC_ABOUT_QUIZ_FOR_CHILD.ToDescription();
        private QuizStatRequestModel request;
        private QuizStatisticsModel response;

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Debug.LogError("Get MyProfile data --- error");
                Fail();
                return;
            }

            request = EventData.data as QuizStatRequestModel;

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

            QuizStatResponseModel.Week = response.Week;

            request.requestTrueAction?.Invoke();

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<QuizStatisticsModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("EditChildCommand parse error. Error text: {0}", e);
                response = new QuizStatisticsModel();
            }
        }
    }
}