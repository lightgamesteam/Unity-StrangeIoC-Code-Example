using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Statistics
{
    public class QuizQuittedCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.QUIZ_QUITTED.ToDescription();
        private QuizQuittedRequestModel request;
        public override void Execute()
        {
            Retain();

            if (EventData.data != null)
            {
                request = EventData.data as QuizQuittedRequestModel;
            }
            else
            {
                Debug.LogError("No data");
                request = new QuizQuittedRequestModel();
            }

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

            request.requestTrueAction?.Invoke();
            Debug.Log("Quiz quitted sent");
            Release();
        }
    }
}