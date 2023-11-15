using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Statistics
{
    public class AppStartedCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.APP_STARTED.ToDescription();

        public override void Execute()
        {
            //Retain();
            bool isUserTokenExist = false;
            if (SwitchModeModel.Mode != Conditions.GameModes.None && !string.IsNullOrEmpty(PlayerPrefsModel.CurrentChildToken))
            {
                isUserTokenExist = true;
            }

            if (isUserTokenExist)
            {
                string requestUrl = ServerUrl + apiPath;

                Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult));
            }
            else
            {
                //Release();
            }
        }

        private void CheckResult(ResultNetworkModel result)
        {
            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                Debug.Log("App started not sent");
                Release();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                Debug.Log("App started not sent");
                Release();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                Debug.Log("App started not sent");
                Release();
                return;
            }
        }
    }
}