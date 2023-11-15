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
    public class GetChildClassesCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.GET_CHILD_CLASSES.ToDescription();
        private GetChildClassesRequestModel request;
        private ClassModel[] response;

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Debug.LogError("GetChildClassesCommand data --- error");
                Fail();
                return;
            }

            request = EventData.data as GetChildClassesRequestModel;
            if (request == null)
            {
                Debug.LogError("GetChildClassesCommand request --- error");
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
            request.successAction?.Invoke(response);
            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                JArray jsonArray = jsonObject.Value<JArray>("data");
                response = jsonArray.ToObject<ClassModel[]>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetMyProfileCommand parse error. Error text: {0}", e);
                response = new ClassModel[1];
            }
        }
    }
}