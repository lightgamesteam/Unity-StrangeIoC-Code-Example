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
    public class CheckDeepLinkUserTokenCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.CHECK_DEEPLINK_USER_TOKEN_EXPIRED.ToDescription();
        private BasicRequestModel request;
        private bool response;

        public override void Execute()
        {
            Retain();
            Debug.Log("<color=green>!!! CheckDeepLinkUserTokenCommand started</color> ");
            if (EventData.data != null)
            {
                request = EventData.data as BasicRequestModel;
            }
            else
            {
                Debug.LogError("GetChildDataByDeepLink: No data");
            }

            if (SwitchModeModel.Mode != Conditions.GameModes.SchoolModeForChildDeepLink)
            {
                request?.requestTrueAction?.Invoke();
                Release();
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
                DisplayError("no answer");
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                DisplayError(result.error);
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                DisplayError("jsonObject = NULL");
                Fail();
                return;
            }

            Parse(result.jsonObject);

            if (response) //if expired
            {
                Debug.LogWarning("CheckDeepLinkUserTokenCommand => lbtExpiry == " + response);
                PopupModel popupModel = new PopupModel(
                    title: LocalizationKeys.NoticeKey,
                    description: LocalizationKeys.EnterThroughtDeepLink,
                    buttonText: LocalizationKeys.OkKey,
                    isActiveCloseButton: false,
                    callback: () => { Application.Quit(); });
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false });
                Fail();
                return;
            }
            else
            {
                request?.requestTrueAction?.Invoke();
            }

            Release();
        }

        private void Parse(JObject jsonObject)
        {
            Debug.Log("<color=green>!!! CheckDeepLinkUserTokenCommand lbtExpired = </color> " + (bool)jsonObject["lbtExpired"]);
            try
            {
                response = (bool)jsonObject["lbtExpired"];
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetChildDataCommand => Parse error: {0}", e);
            }
        }
    }
}