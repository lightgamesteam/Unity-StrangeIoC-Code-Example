using Assets.Scripts.Services.Analytics;
using ImaginationOverflow.UniversalDeepLinking;
using System;
using UnityEngine;
using Vuplex.WebView;
using Vuplex.WebView.Demos;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Authorization;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

namespace PFS.Assets.Scripts.Commands.Authorization
{
    public class StartAutorizationByFeideCommand : BaseCommand
    {
        [Inject] public Analytics Analytics { get; private set; }

        private const string PickataleForSchoolScheme = "pickataleforschool";
        private StartAutorizationByFeideModel startAutorizationByFeideModel;
        private UniWebView uniWebView;

        private BaseCanvasWebViewPrefab baseWebView3D;
        private CanvasWebViewPrefab webView3D;
        private HardwareKeyboardListener hardwareKeyboardListener;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                startAutorizationByFeideModel = EventData.data as StartAutorizationByFeideModel;
                if (startAutorizationByFeideModel != null)
                {
#if UNITY_EDITOR_WIN
                    Start3DWebViewLoginProcess();
#elif UNITY_EDITOR_OSX
                StartUniWebViewLoginProcess();
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WSA
                StartBrowserLoginProcess();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                StartLoginWitchDeepLincPlugin();
#endif
                }
                else
                {
                    Release();
                }
            }
            else
            {
                Debug.LogError("StartAutorizationByFeideCommand => EventData.data - null");
                Fail();
            }
        }

        #region 3DWebView (Windows)
        private void Start3DWebViewLoginProcess()
        {
            baseWebView3D = GameObject.Instantiate(startAutorizationByFeideModel.WebView3DPrefab, startAutorizationByFeideModel.LoginScreenView.transform);
            baseWebView3D.name = startAutorizationByFeideModel.WebView3DPrefab.name;
            webView3D = baseWebView3D.canvasWebViewPrefab;
            webView3D.InitialUrl = startAutorizationByFeideModel.FeideLink;
            webView3D.InitialResolution = 0.7f;

            webView3D.Init();

            webView3D.Initialized += (s, e) =>
            {
                Release();

                webView3D.WebView.UrlChanged += (sender, eventArgs) =>
                {
                    Debug.Log($"webView3D => UrlChanged: {eventArgs.Title} | {eventArgs.Type} | {eventArgs.Url}");

                    if (eventArgs.Url.StartsWith($"{PickataleForSchoolScheme}://"))
                    {
                        string code = eventArgs.Url.Replace($"{PickataleForSchoolScheme}://?code=", string.Empty);
                        code = code.Remove(code.IndexOf('&'));

                        Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(code, ProcessAuthorizationFinished, ProcessAuthorizationFailed));
                    }
                };

#if UNITY_EDITOR_WIN || UNITY_WSA || UNITY_WSA_10_0
            Application.deepLinkActivated += (url) =>
                {
                    Debug.Log($"webView3D => deepLinkActivated: {url}");

                    if (url.StartsWith($"{PickataleForSchoolScheme}:///"))
                    {
                        string code = url.Replace($"{PickataleForSchoolScheme}:///?code=", string.Empty);
                        code = code.Remove(code.IndexOf('&'));

                        Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(code, ProcessAuthorizationFinished, ProcessAuthorizationFailed));
                    }
                };
#endif
            webView3D.WebView.MessageEmitted += (webView, eventArgs) =>
                {
                    Debug.Log($"webView3D => MessageEmitted: {eventArgs.Value}");
                };

                webView3D.WebView.CloseRequested += (webView, eventArgs) =>
                {
                    Debug.Log($"webView3D => CloseRequested");

                    DestroyCurrentWebView();
                };
            };

            SetUpHardwareKeyboard();
        }

        private void SetUpHardwareKeyboard()
        {
            hardwareKeyboardListener = HardwareKeyboardListener.Instantiate(webView3D.transform);
            hardwareKeyboardListener.KeyDownReceived += (sender, eventArgs) =>
            {
                var webViewWithKeyDown = webView3D.WebView as IWithKeyDownAndUp;
                if (webViewWithKeyDown == null)
                {
                    webView3D.WebView.HandleKeyboardInput(eventArgs.Value);
                }
                else
                {
                    webViewWithKeyDown.KeyDown(eventArgs.Value, eventArgs.Modifiers);
                }
            };
            hardwareKeyboardListener.KeyUpReceived += (sender, eventArgs) =>
            {
                var webViewWithKeyUp = webView3D.WebView as IWithKeyDownAndUp;
                if (webViewWithKeyUp != null)
                {
                    webViewWithKeyUp.KeyUp(eventArgs.Value, eventArgs.Modifiers);
                }
            };
        }
        #endregion

        #region UniWebView (Android, iOS, macOS)
        private void StartUniWebViewLoginProcess()
        {
            uniWebView = GameObject.Instantiate(startAutorizationByFeideModel.UniWebViewPrefab, startAutorizationByFeideModel.LoginScreenView.transform);
            uniWebView.ReferenceRectTransform = startAutorizationByFeideModel.LoginScreenView.GetComponent<RectTransform>();
            uniWebView.AddUrlScheme(PickataleForSchoolScheme);
            uniWebView.OnMessageReceived += ProcessUniWebViewMessage;
            uniWebView.OnShouldClose += ProcessWebViewClosing;
            uniWebView.Hide();

            if (!string.IsNullOrEmpty(startAutorizationByFeideModel.FeideLink))
            {
                ShowLoginWebView(startAutorizationByFeideModel.FeideLink);
            }

            // Dispatcher.Dispatch(EventGlobal.E_GetFeideLinkCommand, new GetFeideLinkRequestModel(ShowLoginWebView, () => Debug.LogError("FEIDE Link Get ERROR!")));
        }

        private void ProcessUniWebViewMessage(UniWebView view, UniWebViewMessage message)
        {
            Debug.LogError(message.RawMessage);
            if (message.Scheme == PickataleForSchoolScheme)
            {
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(message.Args["code"], ProcessAuthorizationFinished, ProcessAuthorizationFailed));
            }
        }

        private bool ProcessWebViewClosing(UniWebView webView)
        {
            Release();
            if (webView == uniWebView)
            {
                startAutorizationByFeideModel.LoginScreenView.StopAllCoroutines();

                uniWebView = null;
            }

            return true;
        }

        private void ShowLoginWebView(string link)
        {
            uniWebView.Load(link);
            uniWebView.Show();
        }
        #endregion

        #region Browser
        private void StartBrowserLoginProcess()
        {
            Application.deepLinkActivated += DeeplinkHendler;
            Application.OpenURL(startAutorizationByFeideModel.FeideLink);
        }

        private void DeeplinkHendler(string url)
        {
            Debug.Log($"webView3D => deepLinkActivated: {url}");

            //#if UNITY_WSA

            if (url.StartsWith($"1{PickataleForSchoolScheme}:///"))
            {
                Debug.Log("0000");
                string code = url.Replace($"1{PickataleForSchoolScheme}:///?code=", string.Empty);
                code = code.Remove(code.IndexOf('&'));
                Application.deepLinkActivated -= DeeplinkHendler;
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(code, ProcessAuthorizationFinished, ProcessAuthorizationFailed));
            }

            if (url.StartsWith($"{PickataleForSchoolScheme}:///"))
            {
                Debug.Log("1111");
                string code = url.Replace($"{PickataleForSchoolScheme}:///?code=", string.Empty);
                code = code.Remove(code.IndexOf('&'));
                Application.deepLinkActivated -= DeeplinkHendler;
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(code, ProcessAuthorizationFinished, ProcessAuthorizationFailed));
            }
            //#else
            if (url.StartsWith($"{PickataleForSchoolScheme}://"))
            {
                Debug.Log("22222");
                string code = url.Replace($"{PickataleForSchoolScheme}://?code=", string.Empty);
                code = code.Remove(code.IndexOf('&'));
                Application.deepLinkActivated -= DeeplinkHendler;
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByFeide, new CheckFeideAuthorazitionRequestModel(code, ProcessAuthorizationFinished, ProcessAuthorizationFailed));
            }
            //#endif
        }


        #endregion


        #region Windows macOS
        private void StartLoginWitchDeepLincPlugin()
        {
            Application.OpenURL(startAutorizationByFeideModel.FeideLink);
            Application.Quit();
        }
        #endregion


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
    }
}