using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class ConfirmPurchaseCommand : BaseNetworkCommand
    {
        private string apiPath;
        private ReceiptRequestModel request;
        private ReceiptResponseModel response;
        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                request = EventData.data as ReceiptRequestModel;
            }
            else
            {
                Debug.LogError("No data");
                request = new ReceiptRequestModel();
            }

            PrepareObject(request);
#if UNITY_ANDROID || UNITY_EDITOR
            apiPath = APIPaths.CHILD_GOOGLE_RECEPT_CONFIRMATION.ToDescription();
#elif UNITY_IOS
        apiPath = APIPaths.CHILD_APPLE_RECEPT_CONFIRMATION.ToDescription();
#endif
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

            if (response.receiptProcessed)
            {
                Debug.Log("ConfirmPurchaseCommand => responce.receiptProcessed: TRUE");
                request.requestTrueAction?.Invoke();
            }
            else
            {
                Debug.Log("ConfirmPurchaseCommand => responce.receiptProcessed: FALSE");
                request.requestFalseAction?.Invoke();
            }

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<ReceiptResponseModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("BuyCustomizationItemCommand parse error. Error text: {0}", e);
                response = new ReceiptResponseModel();
            }
        }
    }
}