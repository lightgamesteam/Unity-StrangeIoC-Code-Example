using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Statistics
{
    public class FlipPageBookCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.PAGE_FLIPPED.ToDescription();
        private FlipPageBookRequestModel request;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                request = EventData.data as FlipPageBookRequestModel;
            }
            else
            {
                Debug.LogError("No data");
                request = new FlipPageBookRequestModel();
            }

            PrepareObject(request);
            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult, request.waitResponse));
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
            Release();
        }
    }
}