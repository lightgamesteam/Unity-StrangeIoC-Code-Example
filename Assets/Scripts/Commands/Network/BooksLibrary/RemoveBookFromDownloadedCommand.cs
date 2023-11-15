using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    public class RemoveBookFromDownloadedCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.REMOVE_BOOK_FROM_DOWNLOADED.ToDescription();
        private RemoveBookFromDownloadedRequestModel request;

        public override void Execute()
        {
            Retain();
            if (EventData.data == null)
            {
                Debug.LogError("RemoveBookFromDownloaded data --- NULL");
                Fail();
            }

            request = EventData.data as RemoveBookFromDownloadedRequestModel;
            if (request == null)
            {
                request = new RemoveBookFromDownloadedRequestModel();
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
                request?.requestFalseAction.Invoke();
                DisplayError("no answer");
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                request?.requestFalseAction.Invoke();
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                request?.requestFalseAction.Invoke();
                DisplayError("jsonObject = NULL");
                Fail();
                return;
            }

            Debug.Log(result.jsonObject.ToString());
            request.requestTrueAction?.Invoke();
            Release();
        }
    }
}