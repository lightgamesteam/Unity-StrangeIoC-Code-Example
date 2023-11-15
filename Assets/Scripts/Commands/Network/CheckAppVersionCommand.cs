using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models.NetworkPaths;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class CheckAppVersionCommand : BaseNetworkCommand
    {
        [Inject] public AppVersionsModel AppVersionsModel { get; private set; }

        private string apiPath = APIPaths.APPVERSION.ToDescription();
        private BasicRequestModel request;
        private List<AppVersionModel> responce;
        private AppVersionModel currentAppVersion;

        public override void Execute()
        {
            Retain();
            GetCurrentVersion();
            request = new BasicRequestModel();

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
                AppVersionsModel.latestVersion = new AppVersionModel { version = "Failed" };
                return;
            }
            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.error = " + result.error);
                request.requestFalseAction?.Invoke();
                Fail();
                AppVersionsModel.latestVersion = new AppVersionModel { version = "Failed" };
                return;
            }
            if (result.jsonObject == null)
            {
                Debug.LogError($"{GetType()} => ResultNetworkModel result.jsonObject = NULL");
                request.requestFalseAction?.Invoke();
                DisplayError("jsonObject = NULL");
                Fail();
                AppVersionsModel.latestVersion = new AppVersionModel { version = "Failed" };
                return;
            }

            Parse(result.jsonObject);
            GetHighestversion(responce);
            Conclude();
            Release();
        }

        private void Parse(JObject jsonObject)
        {
            try
            {
                responce = jsonObject["list"].ToObject<List<AppVersionModel>>();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(". Error text: {0}", e);

            }
        }

        /// <summary>
        /// Get Current version of the app
        /// </summary>
        private void GetCurrentVersion()
        {
            currentAppVersion = new AppVersionModel();
            var textAsset = Resources.Load("Version") as TextAsset;
            currentAppVersion.version = textAsset.ToString();
            AppVersionsModel.currentVersion = currentAppVersion;
        }

        private List<AppVersionModel> GetHigherVersions(List<AppVersionModel> allVersions)
        {
            List<AppVersionModel> higherVersions = new List<AppVersionModel>();
            foreach (var version in allVersions)
            {
                if (version > currentAppVersion)
                {
                    higherVersions.Add(version);
                }
            }
            return higherVersions;
        }

        /// <summary>
        /// Make some decision what popup we should show
        /// </summary>
        private void Conclude()
        {
            //Show the force update popup
            if (CheckTheForceUpdate())
            {
                PopupModel popupModel = new PopupModel(
                    title: LocalizationKeys.UpdateRequiredTitleKey,
                    description: LocalizationKeys.UpdateRequiredDescriptionKey,
                    buttonText: LocalizationKeys.OkKey,
                    isActiveCloseButton: true,
                    callback: null);

                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false });
                Release();
                return;
            }

            //var date = DateTime.UtcNow;
            //if (CheckTheExpireDate(ref date))
            //{
            //    if (DateTime.UtcNow > date)
            //    {
            //        //Show the force update popup
            //        PopupModel popupModel = new PopupModel(
            //        title: LocalizationKeys.UpdateRequiredTitleKey,
            //        description: LocalizationKeys.UpdateRequiredDescriptionKey,
            //        buttonText: LocalizationKeys.UpdateKey,
            //        isActiveCloseButton: false,
            //        callback: OpenStore,
            //        isCloseAfterAction: false);

            //        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false });
            //        Release();
            //        return;
            //    }
            //    else
            //    {
            //        //Show nitice popup to inform user that form some date this version will not be supported
            //        PopupModel popupModel = new PopupModel(
            //        title: LocalizationKeys.NoticeKey,
            //        description: LocalizationKeys.OutOfSupportDescriptionKey,
            //        buttonText: LocalizationKeys.UpdateKey,
            //        isActiveCloseButton: true,
            //        callback: OpenStore);

            //        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false });
            //        Release();
            //        return;
            //    }
            //}

            //Show the recommendet update popup
            //if (GetHigherVersions(responce).Count > 0)
            //{
            //    PopupModel popupModel = new PopupModel(
            //        title: LocalizationKeys.NoticeKey,
            //        description: LocalizationKeys.UpdateRecommendDescriptionKey,
            //        buttonText: LocalizationKeys.UpdateKey,
            //        isActiveCloseButton: true,
            //        callback: OpenStore);

            //    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false });
            //    Release();
            //    return;
            //}
        }

        /// <summary>
        /// Check if some of higher version contain ForceUpdate flag
        /// </summary>
        /// <returns></returns>
        private bool CheckTheForceUpdate()
        {
            foreach (var version in GetHigherVersions(responce))
            {
                if (version.isForceUpdate)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check our current version has expireDate and return this date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //private bool CheckTheExpireDate(ref DateTime date)
        //{
        //    foreach (var version in responce)
        //    {
        //        if (version == currentAppVersion && !string.IsNullOrEmpty(version.expireDate))
        //        {
        //            date = DateTime.Parse(version.expireDate);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        private void OpenStore()
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.Pickatale.PFS");
#elif UNITY_IPHONE
                    Application.OpenURL("itms-apps://itunes.apple.com/app/id1533803381");
#elif UNITY_WSA
        Application.OpenURL("https://www.microsoft.com/store/productId/9PFVXL9H7J76");
#endif
        }

        private void GetHighestversion(List<AppVersionModel> allVersions)
        {
            if (allVersions.Count > 0)
            {
                AppVersionModel maxAppVersion = allVersions[0];
                AppVersionsModel.latestVersion = maxAppVersion;
            }
            else
            {
                AppVersionsModel.latestVersion = new AppVersionModel();
            }
        }
    }
}