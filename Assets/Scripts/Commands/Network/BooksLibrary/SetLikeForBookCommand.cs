using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    public class SetLikeForBookCommand : BaseNetworkCommand
    {
        private string apiPath = APIPaths.BOOK_LIKED.ToDescription();
        private SetLikeForBookRequestModel request;
        private SetLikeForBookResponseModel response;

        public override void Execute()
        {
            Retain();
            request = EventData.data as SetLikeForBookRequestModel;
            if (request == null)
            {
                Debug.LogError("SetLikeForBookCommand : request --- NULL");
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
            request.requestTrueAction?.Invoke(response.action == "bookLiked");
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<SetLikeForBookResponseModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("SetLikeForBookCommand parse error. Error text: {0}", e);
                response = new SetLikeForBookResponseModel();
            }
        }
    }

    public class SetLikeForBookRequestModel : BasicRequestModel
    {
        public string bookId;

        [NonSerialized]
        public new Action<bool> requestTrueAction;
        public SetLikeForBookRequestModel(string bookId, Action<bool> successAction, Action failAction)
        {
            this.bookId = bookId;
            requestTrueAction = successAction;
            requestFalseAction = failAction;
        }
    }

    public class SetLikeForBookResponseModel
    {
        public string action;
    }
}