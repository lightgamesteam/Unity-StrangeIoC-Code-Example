using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class RequestNetworkModel
    {
        public string cmd_id = "";
        public JObject jsonData = null;
        public Action<ResultNetworkModel> result = null;
        public RequestType requestType;
        public bool waitResponse = true;


        /// <summary>
        /// Use for POST request type
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="cmd_id"></param>
        /// <param name="jsonData"></param>
        /// <param name="result"></param>
        public RequestNetworkModel(RequestType requestType, string cmd_id, JObject jsonData, Action<ResultNetworkModel> result)
        {
            this.cmd_id = cmd_id;
            this.jsonData = jsonData;
            this.result = result;
            this.requestType = requestType;
        }

        /// <summary>
        /// Use for GET request type
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="cmd_id"></param>
        /// <param name="result"></param>
        public RequestNetworkModel(RequestType requestType, string cmd_id, Action<ResultNetworkModel> result)
        {
            this.cmd_id = cmd_id;
            this.result = result;
            this.requestType = requestType;
        }

        /// <summary>
        /// Use for GET/POST request type 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="cmd_id"></param>
        /// <param name="jsonData"></param>
        /// <param name="result"></param>
        /// <param name="waitResponse"></param>
        public RequestNetworkModel(RequestType requestType, string cmd_id, JObject jsonData, Action<ResultNetworkModel> result, bool waitResponse)
        {
            this.cmd_id = cmd_id;
            this.jsonData = jsonData;
            this.result = result;
            this.requestType = requestType;
            this.waitResponse = waitResponse;
        }
    }

    public class ResultNetworkModel
    {
        public JObject jsonObject;
        public string error;
        private string text;


        public ResultNetworkModel(UnityWebRequest request)
        {
            if (request != null)
            {
                if (request.downloadHandler.text == null)
                {
                    Debug.LogWarning($"{nameof(ResultNetworkModel)} => request.downloadHandler.text  is null");
                    text = "";
                }
                else
                {
                    text = request.downloadHandler.text;
                }
                try
                {
                    jsonObject = JObject.Parse(text);
                }
                catch (JsonReaderException)
                {
                    JArray ar = JArray.Parse(text);
                    if (ar != null)
                    {
                        jsonObject = new JObject();
                        jsonObject.Add("data", ar);
                    }
                }

                if (jsonObject == null)
                {
                    Debug.LogWarning($"{nameof(ResultNetworkModel)} => jsonObject is null");
                    jsonObject = new JObject();
                }
                return;
            }
            Debug.LogWarning($"{nameof(ResultNetworkModel)} => request is null");
        }

        public ResultNetworkModel(string error)
        {
            this.error = error;
        }
    }

    public class NetworkCommand : BaseCommand
    {
        [Inject]
        public IExecutor CoroutineExecutor { get; private set; }

        public const int Timeout = 20;
        private static int requestNumber = 0;
        private int responceNumber = 0;

        public override void Execute()
        {
            RequestNetworkModel request = EventData.data as RequestNetworkModel;
            if (request == null)
            {
                Debug.LogError("NetworkCommand - no data");
                return;
            }

            requestNumber++;
            responceNumber = requestNumber;
            Debug.LogFormat("<color=green>Client request URL #{2}: </color> {0} \n<color=blue>Data: </color>\n {1}", request.cmd_id, request.jsonData, requestNumber);
            Debug.LogFormat("<color=red>token child:</color> {0}", PlayerPrefsModel.CurrentChildToken);

            if (!MainContextInput.AppPause)
            {
                CoroutineExecutor.Execute(WaitForResponseIEnum(request.requestType, request.cmd_id, request.jsonData, request.result, request.waitResponse, responceNumber));
            }
            else
            {
                WaitForResponse(request.requestType, request.cmd_id, request.jsonData, request.result, request.waitResponse, responceNumber);
            }
        }

        private IEnumerator WaitForResponseIEnum(RequestType type, string requestUrl, JObject jsonData, Action<ResultNetworkModel> result, bool waitResponse, int responceNumber)
        {
            ResultNetworkModel reqResult = null;
            UnityWebRequest request = null;

            CreateWebRequest(ref request, type, requestUrl, jsonData);

            if (request == null)
                yield break;

            if (!waitResponse)
            {
                request.SendWebRequest();
                yield break;
            }

            yield return request.SendWebRequest();

            Response(ref request, ref reqResult, result, responceNumber);
        }

        private void WaitForResponse(RequestType type, string requestUrl, JObject jsonData, Action<ResultNetworkModel> result, bool waitResponse, int responceNumber)
        {
            ResultNetworkModel reqResult = null;
            UnityWebRequest request = null;

            CreateWebRequest(ref request, type, requestUrl, jsonData);

            if (request == null)
                return;

            request.SendWebRequest();

            if (!waitResponse)
            {
                return;
            }

            //float timer = 0;
            //freeze
            while (!request.isDone /*&& timer < Timeout*/)
            {
                //Thread.Sleep(100);
                //timer += 0.1f;
                //Debug.Log("timer " + timer);
            }

            Response(ref request, ref reqResult, result, responceNumber);
        }

        private void CreateWebRequest(ref UnityWebRequest request, RequestType type, string requestUrl, JObject jsonData)
        {
            switch (type)
            {
                case RequestType.GET:
                    request = new UnityWebRequest(requestUrl, "GET");
                    request.SetRequestHeader("token", PlayerPrefsModel.CurrentChildToken);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    break;
                case RequestType.POST:
                    string s = JsonConvert.SerializeObject(jsonData);
                    request = new UnityWebRequest(requestUrl, "POST");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(s);
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("token", PlayerPrefsModel.CurrentChildToken);
                    break;
                case RequestType.PUT:
                    request = new UnityWebRequest(requestUrl, "PUT");
                    request.SetRequestHeader("token", PlayerPrefsModel.CurrentChildToken);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    break;
                case RequestType.DELETE:
                    request = new UnityWebRequest(requestUrl, "DELETE");
                    request.SetRequestHeader("token", PlayerPrefsModel.CurrentChildToken);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    break;
                default:
                    Debug.LogError("Type undefined");
                    break;
            }

            if (request != null)
            {
                request.timeout = Timeout;
            }
        }

        private void Response(ref UnityWebRequest request, ref ResultNetworkModel reqResult, Action<ResultNetworkModel> result, int responceNumber, bool dispose = true)
        {
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogWarning($"NetworkCommand - UnityWebRequest - error {request.error}. Request URL => {request.url}"); //short error code
                                                                                                                             // if full description of error is not empty use it as error
                if (request.downloadHandler != null)
                {
                    //full error description
                    Debug.LogWarning($"NetworkCommand - UnityWebRequest - Error text: {request.downloadHandler.text}. Request URL => {request.url}");
                    reqResult = new ResultNetworkModel(request.downloadHandler.text);
                }
                else //use shor error code as error
                {
                    reqResult = new ResultNetworkModel(request.error);
                }

                result?.Invoke(reqResult);
            }
            else
            {
                Debug.Log($"NetworkCommand - UnityWebRequest - Responce Code = {request.responseCode}. Request URL => {request.url}");
                Debug.LogFormat("<color=purple> Server responce #{0}  = </color>{1}", responceNumber, request.downloadHandler.text);
                reqResult = new ResultNetworkModel(request);
                result?.Invoke(reqResult);
            }

            if (dispose)
            {
                request.Dispose();
            }
        }
    }
}