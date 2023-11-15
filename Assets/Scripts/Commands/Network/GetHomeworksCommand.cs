using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Conditions;
using UnityEngine;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Models.NetworkPaths;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class GetHomeworksCommand : BaseNetworkCommand
    {
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        private string apiPath = APIPaths.GET_HOMEWORK_BY_STATUS.ToDescription();
        private HomeworksByStatusRequstModel request;
        private HomeworksByStatusResponceModel response;

        public override void Execute()
        {
            Debug.Log("GetHomeworksCommand");
            Retain();

            if (EventData == null || EventData.data == null)
            {
                Debug.LogError("GetHomeworksCommand | No data");
                Fail();
                return;
            }

            request = EventData.data as HomeworksByStatusRequstModel;

            if (request == null)
            {
                Debug.LogError("GetHomeworksCommand | request - null");
                Fail();
                return;
            }

            PrepareObject(request);

            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.POST, requestUrl, jsonData, CheckResult));
        }

        private void CheckResult(ResultNetworkModel result)
        {
            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
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
                Fail();
                return;
            }

            Parse(result.jsonObject);
            List<BookModel> books = new List<BookModel>();
            bool isNewHomworks = false;
            if (response != null && response.homeworks != null)
            {
                foreach (var work in response.homeworks)
                {
                    if (work != null && work.homeworkBook != null)
                    {
                        work.SetStatusEnum();
                        work.SetLanguageEnum();
                        work.ParseDateTimes();
                        work.homeworkBook.HomeworkId = work.workId;
                        work.homeworkBook.ReadOnly = work.readOnly;
                        work.homeworkBook.HideQuiz = !work.withQuiz;
                        work.homeworkBook.InitializeBook();


                        books.Add(work.homeworkBook);
                        //if one of the homeworks has New status - it means all homeworks - New
                        if (work.statusEnum == HomeworkStatus.New)
                        {
                            isNewHomworks = true;
                        }
                    }
                }

                response.SortHomeworks();
            }

            if (books != null && books.Count != 0 && isNewHomworks)
            {
                BooksLibrary.AddHomeworkBooks(books);
            }
            else
            {
                Debug.LogWarning("Homework Books is null or empty");
            }

            request.requestTrueAction?.Invoke(response.homeworks);

            Release();
        }

        public void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<HomeworksByStatusResponceModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetHomeworksCommand parse error. Error text: {0}", e);
            }
        }
    }
}