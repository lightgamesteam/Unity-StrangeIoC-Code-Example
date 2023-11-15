using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    public class GetDownloadedBookIdsCommand : BaseNetworkCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }

        private string apiPath = APIPaths.GET_DOWNLOADED_BOOK_IDS.ToDescription();
        private BasicRequestModel request;
        private List<GetDownloadedBookIdsResponseModel> responce;

        public override void Execute()
        {
            Retain();
            if (EventData.data == null)
            {
                Debug.LogError("GetBooksByIdCommand data --- NULL");
                Fail();
                return;
            }

            request = EventData.data as BasicRequestModel;
            if (request == null)
            {
                request = new BasicRequestModel();
            }

            PrepareObject(request);

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.GET, requestUrl, jsonData, CheckResult));
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
            SetDownloadedIdsToChild();
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                JArray a = (JArray)jsonObject["data"];
                responce = a.ToObject<List<GetDownloadedBookIdsResponseModel>>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetBooksByIdCommand parse error. Error text: {0}", e);
                responce = new List<GetDownloadedBookIdsResponseModel>();
            }
        }

        private void SetDownloadedIdsToChild()
        {
            var child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            child.DownloadedBookIdsObjects = new Dictionary<string, List<Conditions.Languages>>();

            foreach (var bookIdtoChild in responce)
            {
                if (bookIdtoChild.childId == child.Id)
                {
                    // if book with this Id not downloaded yet, then create it
                    Enum.TryParse(bookIdtoChild.languageName, true, out Conditions.Languages language);
                    if (!child.DownloadedBookIdsObjects.Keys.Contains(bookIdtoChild.bookId))
                    {
                        child.DownloadedBookIdsObjects.Add(bookIdtoChild.bookId, new List<Conditions.Languages> { language });
                    }
                    else if (!child.DownloadedBookIdsObjects[bookIdtoChild.bookId].Exists(x => x == language))
                    {
                        child.DownloadedBookIdsObjects[bookIdtoChild.bookId].Add(language);
                    }
                }
            }
        }
    }
}