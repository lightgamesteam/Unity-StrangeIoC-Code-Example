using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views;
using System;
using System.Linq;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Authorization
{
    public class GetChildDataByClassCode : BaseNetworkCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        private string apiPath = APIPaths.GET_CHILD_BY_SCHOOL_CODE.ToDescription();
        private GetChildByClassCodeRequestModel request;
        private ChildModel response;

        private string localizationErrorPrefix = "ui.Error.ConnectClass.";

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                request = EventData.data as GetChildByClassCodeRequestModel;
            }
            else
            {
                Debug.LogError("GetChildByCodeCommand: No data");
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
                Debug.LogWarning($"{GetType()} => result.error = " + result.error);
                request.failAction?.Invoke(GetLocalizedKey(ParseErrorString(result.error)));
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
            request?.requestTrueAction?.Invoke();
            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<ChildModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetChildByCodeCommand parse error. Error text: {0}", e);
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
                errorObject.TryGetValue("description", out JToken errorStringValue);

                if (errorStringValue != null)
                {
                    result = errorStringValue.Value<string>();
                }
            }

            return result;
        }

        private string GetLocalizedKey(string error)
        {
            string combinedKey = localizationErrorPrefix + error;
            string localizationKey = LocalizationKeys.ConnectToClassErrors.Where(c => c == combinedKey).FirstOrDefault();

            if (string.IsNullOrEmpty(localizationKey))
            {
                localizationKey = LocalizationKeys.ConnectToClassErrors.First();
            }

            return localizationKey;
        }
    }
}