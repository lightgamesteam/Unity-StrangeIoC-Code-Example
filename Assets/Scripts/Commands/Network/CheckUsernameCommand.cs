using System;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class CheckUsernameCommand : BaseNetworkCommand
    {
        private string apiPath;
        private CheckUsernameRequestModel request;
        private CheckUsernameResponseModel response;

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Fail();
                return;
            }

            request = EventData.data as CheckUsernameRequestModel;

            PrepareObject(request);
            if (request.isTeacher)
            {
                apiPath = APIPaths.CHECK_TEACHERNAME.ToDescription();
            }
            else
            {
                apiPath = APIPaths.CHECK_USERNAME.ToDescription();
            }

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
            request.successAction?.Invoke(response.isExist);
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<CheckUsernameResponseModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat($"{nameof(CheckUsernameCommand)} => Parse: Parse error = {e}");
                response = new CheckUsernameResponseModel();
            }
        }
    }
}