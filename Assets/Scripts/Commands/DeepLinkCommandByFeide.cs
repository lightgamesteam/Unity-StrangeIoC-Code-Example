using ImaginationOverflow.UniversalDeepLinking;
using UnityEngine;
using System;
using Vuplex.WebView;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

namespace PFS.Assets.Scripts.Commands.Authorization
{
    public class DeepLinkCommandByFeide : BaseCommand
    {
        public static bool isFeidDeepLink = false;
        [Inject] public Analytics Analytics { get; private set; }


        private UniWebView uniWebView;
        private BaseCanvasWebViewPrefab baseWebView3D;

        private const string PickataleForSchoolScheme = "pickataleforschool";

        public override void Execute()
        {
            Retain();
            Debug.Log("<color=green>!!! DeepLinkCommandByFeide started</color> ");
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        DeepLinkManager.Instance.LinkActivated += OnDeepLinkByFeideActivated;
#endif
            Release();
        }

        private void OnDeepLinkByFeideActivated(LinkActivation s)
        {
            if (s.Uri.StartsWith($"{PickataleForSchoolScheme}://"))
            {
                string code = s.Uri.Replace($"{PickataleForSchoolScheme}://?code=", string.Empty);
                code = code.Remove(code.IndexOf("&state="));
                isFeidDeepLink = true;
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(
                code,
                ProcessAuthorizationFinished,
                ProcessAuthorizationFailed));
            }
            if (s.Uri.StartsWith($"{PickataleForSchoolScheme}:///"))
            {
                string code = s.Uri.Replace($"{PickataleForSchoolScheme}:///?code=", string.Empty);
                code = code.Remove(code.IndexOf("&state="));
                isFeidDeepLink = true;
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(
                code,
                ProcessAuthorizationFinished,
                ProcessAuthorizationFailed));
            }
        }

        #region Additional methods
        // copy from StartAutorizationByFeideCommand.cs
        private void ProcessAuthorizationFinished()
        {
            DestroyCurrentWebView();
            Analytics.LogEvent(EventName.ActionSignIn,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                    { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                    { Property.LoginType, "Feide"},
                    { Property.Region, PlayerPrefsModel.CountryCode}
            });

            Analytics.LogEvent(EventName.ActionOnSignIn,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                    { Property.SignInTime_s, DateTime.Now.ToString()}
            });
            OpenMenuScreen();

        }
        private void OpenMenuScreen()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UILoginScreen);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UITopPanelScreen, isAddToScreensList = false });
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, UIScreens.UIMainMenu);
            Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
            DeepLinkCommandByFeide.isFeidDeepLink = false;
        }
        private void ProcessAuthorizationFailed()
        {
            DestroyCurrentWebView();
        }
        private void DestroyCurrentWebView()
        {
            Release();

            if (uniWebView)
            {
                uniWebView.Hide();
                GameObject.Destroy(uniWebView);
                uniWebView = null;
            }

            if (baseWebView3D)
            {
                baseWebView3D.Destroy();
            }
        }
        #endregion
    }
}