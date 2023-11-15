using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.Authorization;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Responses;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.Authorization
{
    public class GetFeideLinkCommand : BaseNetworkCommand
    {
        private StartAutorizationByFeideModel StartAutorizationByFeideModel { get; set; }

        private string apiPath = APIPaths.GET_FEIDE_LINK.ToDescription();
        private BasicRequestModel request;
        private FeideLinkResponseModel response;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                StartAutorizationByFeideModel = EventData.data as StartAutorizationByFeideModel;
            }
            PrepareObject(request);
            string requestUrl = ServerUrl + apiPath;
            Dispatcher.Dispatch(EventGlobal.E_NetworkCommand, new RequestNetworkModel(RequestType.GET, requestUrl, jsonData, CheckResult));
        }

        private void CheckResult(ResultNetworkModel result)
        {
            if (result == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result == NULL");
                request.requestFalseAction?.Invoke();
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                Fail();
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => result.error = " + result.error);
                request.requestFalseAction?.Invoke();
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                Fail();
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => result.jsonObject = NULL");
                request.requestFalseAction?.Invoke();
                Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                Fail();
                return;
            }

            Parse(result.jsonObject);
            StartAutorizationByFeideModel.FeideLink = response.link;
            //request.successAction(response.link);
            Release();
        }


        private void Parse(JObject jsonObject)
        {
            try
            {
                response = jsonObject.ToObject<FeideLinkResponseModel>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetFeideLinkCommand => Parse: Parse =  {0}", e);
                response = new FeideLinkResponseModel();
            }
        }
    }
}