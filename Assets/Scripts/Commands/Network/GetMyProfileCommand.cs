using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using System;
using System.Linq;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetMyProfileCommand : BaseNetworkCommand
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        [Inject] public ChildStatsModel ChildStatsModel { get; set; }

        private string apiPath = APIPaths.GET_CHILD_STATS.ToDescription();
        private BasicRequestModel request;
        private ChildStatsModel response;

        public override void Execute()
        {
            Retain();
            Debug.Log("GetMyProfileCommand");

            if (EventData.data == null)
            {
                Debug.LogError("Get MyProfile data --- error");
                Fail();
                return;
            }

            request = EventData.data as BasicRequestModel;
            if (request == null)
            {
                Debug.LogError("Get MyProfile request --- error");
                Fail();
                return;
            }

            string requestUrl = ServerUrl + apiPath;

            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.GET, requestUrl, CheckResult));
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

            ChildStatsModel.Stats = response.Stats;

            BooksLibrary.statsMostReadBooks.books = response.Stats.MostRead.ToList();
            foreach (var book in BooksLibrary.statsMostReadBooks.books)
            {
                book.InitializeBook();
                book.SetLibraryBooksLink(BooksLibrary.statsMostReadBooks);
            }

            request.requestTrueAction?.Invoke();

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<ChildStatsModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetMyProfileCommand parse error. Error text: {0}", e);
                response = new ChildStatsModel();
            }
        }
    }
}