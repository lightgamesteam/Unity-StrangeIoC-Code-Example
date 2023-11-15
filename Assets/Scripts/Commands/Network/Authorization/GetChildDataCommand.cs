using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Authorization
{
    public class GetChildDataCommand : BaseNetworkCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        private string apiPath = APIPaths.GET_CHILD_DATA.ToDescription();
        private GetChildDataRequestModel request;
        private ChildModel response;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                request = EventData.data as GetChildDataRequestModel;
            }
            else
            {
                Debug.LogError("GetChildDataCommand: No data");
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
                CheckErrors(ParseErrorString(result.error));
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
                Debug.LogErrorFormat("GetChildDataCommand => Parse error: {0}", e);
                response = new ChildModel();
            }
        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(response.Id))
            {
                response.Id = "testChildId";
            }
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
                errorObject.TryGetValue("description", out JToken errorStringValue);

                if (errorStringValue != null)
                {
                    result = errorStringValue.Value<string>();
                }
            }

            return result;
        }
        private void CheckErrors(string error)
        {
            switch (error)
            {
                case "FEIDE_API_ERROR":
                    request.requestFalseAction?.Invoke();
                    break;
                case "FEIDE_ORGANIZATIONS_NOT_ACCESS_ERROR":
                    request.requestFalseAction?.Invoke();
                    Debug.Log("Sorry, your school does not have access to our application.Please contact your school administrator");

                    PopupModel popupModel = new PopupModel(
                        title: LocalizationKeys.NoticeKey,
                        description: LocalizationKeys.SchoolAccessDenyKey,
                        buttonText: LocalizationKeys.OkKey,
                        isActiveCloseButton: true,
                        callback: null);
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false, showSwitchAnim = false });
                    break;
                default:
                    request.requestFalseAction?.Invoke();
                    break;
            }
        }
    }
}