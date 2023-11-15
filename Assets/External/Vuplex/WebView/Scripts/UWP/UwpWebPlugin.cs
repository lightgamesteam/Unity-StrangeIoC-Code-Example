/**
* Copyright (c) 2021 Vuplex Inc. All rights reserved.
*
* Licensed under the Vuplex Commercial Software Library License, you may
* not use this file except in compliance with the License. You may obtain
* a copy of the License at
*
*     https://vuplex.com/commercial-library-license
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#if UNITY_WSA && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The `IWebPlugin` implementation for Universal Windows Platform.
    /// </summary>
    class UwpWebPlugin : MonoBehaviour, IWebPlugin {

        static UwpWebPlugin() {

            UwpWebView.InitializePlugin();
        }

        public static UwpWebPlugin Instance {
            get {
                if (_instance == null) {
                    _instance = (UwpWebPlugin) new GameObject("UwpWebPlugin").AddComponent<UwpWebPlugin>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public void ClearAllData() {

            UwpWebView.ClearAllData();
        }

        public void CreateTexture(float width, float height, Action<Texture2D> callback) {

            UwpWebView.CreateTexture(width, height, callback);
        }

        public void CreateMaterial(Action<Material> callback) {

            CreateTexture(1, 1, texture => {
                // Construct a new material, because Resources.Load<T>() returns a singleton.
                var material = new Material(Resources.Load<Material>("DefaultViewportMaterial"));
                material.mainTexture = texture;
                callback(material);
            });
        }

        public void CreateVideoMaterial(Action<Material> callback) {

            callback(null);
        }

        public virtual IWebView CreateWebView() {

            return UwpWebView.Instantiate();
        }

        public void EnableRemoteDebugging() {

            WebViewLogger.Log("Remote debugging is enabled for UWP. For instructions, please see https://support.vuplex.com/articles/how-to-debug-web-content#uwp.");
        }

        public void SetIgnoreCertificateErrors(bool ignore) {

            WebViewLogger.LogWarning("3D WebView for UWP does not support ignoring certificate errors through Web.SetIgnoreCertificateErrors(). To ignore certificate errors on UWP, please whitelist the certificate in Package.appxmanifest/Declarations/Certificates (https://www.suchan.cz/2015/10/displaying-https-page-with-invalid-certificate-in-uwp-webview/).");
        }

        public void SetStorageEnabled(bool enabled) {

            UwpWebView.SetStorageEnabled(enabled);
        }

        public void SetUserAgent(bool mobile) {

            UwpWebView.GloballySetUserAgent(mobile);
        }

        public void SetUserAgent(string userAgent) {

            UwpWebView.GloballySetUserAgent(userAgent);
        }

        static UwpWebPlugin _instance;

        void OnApplicationFocus(bool focused) {

            UwpWebView.HandleAppFocusChanged(focused);
        }

        void OnValidate() {

            UwpWebView.ValidateGraphicsApi();
        }
    }
}
#endif
