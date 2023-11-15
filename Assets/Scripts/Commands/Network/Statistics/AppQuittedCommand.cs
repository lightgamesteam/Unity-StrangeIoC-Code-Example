using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Statistics
{
    public class AppQuittedCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.APP_QUITTED.ToDescription();

        public override void Execute()
        {
            Retain();
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
                Release();
            }
        }

        private void CheckResult(ResultNetworkModel result)
        {
            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                Fail();
                return;
            }

            Debug.Log("App quitted sent");
            Release();
        }
    }
}