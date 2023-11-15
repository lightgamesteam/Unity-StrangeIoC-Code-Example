using System;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class ValidateChildAppleSubscribtionCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.VALIDATE_CHILD_SUBSCRIBTION.ToDescription();
        private BasicRequestModel request;
        private bool isValid = false;

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Fail();
                return;
            }

            request = EventData.data as BasicRequestModel;

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

            if (isValid)
            {
                request.requestTrueAction?.Invoke();
            }
            else
            {
                request.requestFalseAction?.Invoke();
            }

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                isValid = (bool)jsonObject["valid"];
                Debug.Log("isValid =" + isValid);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(". Error text: {0}", e);
            }
        }
    }
}