using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Authorization
{
    public class GetChildDataByDeepLink : BaseNetworkCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        private string apiPath = APIPaths.GET_CHILD_BY_DEEPLINK.ToDescription();
        private GetChildByDeeplinkRequestModel request;
        private ChildModel response;

        public override void Execute()
        {
            Retain();
            Debug.Log("<color=green>!!! GetChildDataByDeepLink started</color> ");
            if (EventData.data != null)
            {
                request = EventData.data as GetChildByDeeplinkRequestModel;
            }
            else
            {
                Debug.LogError("GetChildDataByDeepLink: No data");
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
                DisplayError("no answer");
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => result.error = " + result.error);
                DisplayError(result.error);
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => result.jsonObject = NULL");
                DisplayError("jsonObject = NULL");
                Fail();
                return;
            }

            Parse(result.jsonObject);
            Initialize();
            //request?.requestTrueAction?.Invoke();
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<ChildModel>();
            }
            catch (Exception e)
            {
                Debug.LogError($"{GetType()} => Parse: Parse error = {e}");
                response = new ChildModel();
            }
        }

        private void Initialize()
        {
            ChildModel.ReloadCurrentChild(response);
            PlayerPrefsModel.CurrentChildId = response.Id;
            PlayerPrefsModel.CurrentChildToken = response.Token;
        }
    }

    public class GetChildByDeeplinkRequestModel : BasicRequestModel
    {
        [Newtonsoft.Json.JsonProperty("token")]
        public string deepLinkToken;
        public GetChildByDeeplinkRequestModel(string deepLinkToken, Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            this.deepLinkToken = deepLinkToken;
        }
    }
}