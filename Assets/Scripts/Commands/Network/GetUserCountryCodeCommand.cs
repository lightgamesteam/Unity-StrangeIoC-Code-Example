using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetUserCountryCodeCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.GET_COUNTRY_CODE_BY_IP.ToDescription();
        private BasicRequestModel request;
        private string response;

        public override void Execute()
        {
            Retain();
            request = EventData?.data as BasicRequestModel;

            if (request == null)
            {
                Debug.LogError("GetUserCountryCodeCommand - no data");
                return;
            }

            PrepareObject(request);

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.GET, requestUrl, jsonData, CheckResult));
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
                DisplayError(result.error);
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

            PlayerPrefsModel.CountryCode = response;

            request.requestTrueAction?.Invoke();
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                response = (string)jsonObject["countryCode"];
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetUserCountryCodeCommand => Parse: Parse error = {0}", e);
                response = "GB";
            }
        }
    }
}