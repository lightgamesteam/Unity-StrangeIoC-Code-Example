using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetHomeworksStatsCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.GET_HOMEWORKS_STATS.ToDescription();
        private HomeworksStatsRequestModel request;
        private HomeworksStatsModel response;

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Debug.LogError("GetHomeworksStatisticsCommand | No data");
                Fail();
                return;
            }

            request = EventData.data as HomeworksStatsRequestModel;

            if (request == null)
            {
                Debug.LogError("GetHomeworksStatisticsCommand | request - null");
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

            response.SetReceivedStars();
            request.requestTrueAction?.Invoke(response);

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<HomeworksStatsModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetHomeworksStatisticsCommand parse error. Error text: {0}", e);
            }
        }
    }
}