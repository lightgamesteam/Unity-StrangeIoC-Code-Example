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
    public class EditChildCommand : BaseNetworkCommand
    {
        [Inject]
        public ChildModel ChildModel { get; set; }

        private string apiPath = APIPaths.UPDATE_CHILD.ToDescription();
        private ChildEditRequestModel request;
        private ChildModel response;

        public override void Execute()
        {
            Retain();
            Debug.Log("EditChildCommand");

            if (EventData.data == null)
            {
                Debug.LogError("Edit Child data --- error");
                Fail();
                return;
            }

            request = EventData.data as ChildEditRequestModel;
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
            ChildModel child = ChildModel.GetChild(request.id);
            child.SetChildInfo(response);
            request.requestTrueAction?.Invoke();
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
                Debug.LogErrorFormat("EditChildCommand parse error. Error text: {0}", e);
                response = new ChildModel();
            }
        }
    }
}