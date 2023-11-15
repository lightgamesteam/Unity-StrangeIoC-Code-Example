using Newtonsoft.Json;
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
    public class GetChildDataByPasswordCommand : BaseNetworkCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        private string apiPath = APIPaths.GET_CHILD_BY_PASSWORD.ToDescription();
        private GetChildByPasswordRequestModel request;
        private ChildModel response;

        public override void Execute()
        {
            Retain();
            request = EventData.data as GetChildByPasswordRequestModel;
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
                if (result.error.Contains("INCORRECT_PASSWORD") == false)
                {
                    Debug.LogError($"{GetType()} => result.error = " + result.error);
                }
                request.failAction?.Invoke(ParseErrorString(result.error));
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => result.jsonObject = NULL");
                request.requestFalseAction?.Invoke();
                DisplayError("jsonObject = NULL");
                Fail();
                return;
            }

            Parse(result.jsonObject);
            Initialize();
            request?.requestTrueAction?.Invoke();
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
                Debug.LogErrorFormat("GetChildDataByPasswordCommand parse error. Error text: {0}", e);
                response = new ChildModel();
            }
        }

        private void Initialize()
        {
            ChildModel.ReloadCurrentChild(response);
            PlayerPrefsModel.CurrentChildId = response.Id;
            PlayerPrefsModel.CurrentChildToken = response.Token;
        }

        private string ParseErrorString(string unparsedString)
        {
            string result = string.Empty;
            JObject errorObject = null;

            try
            {
                errorObject = JObject.Parse(unparsedString);
            }
            catch (JsonReaderException)
            {

            }

            if (errorObject != null)
            {
                errorObject.TryGetValue("error", out JToken errorObjectValue);

                if (errorObjectValue != null)
                {
                    errorObjectValue.Value<JObject>().TryGetValue("code", out JToken errorStringValue);

                    if (errorStringValue != null)
                    {
                        result = errorStringValue.Value<string>();
                    }
                }
            }

            return result;
        }
    }
}