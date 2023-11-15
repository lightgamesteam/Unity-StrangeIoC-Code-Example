/**
* Copyright (c) 2020 Vuplex Inc. All rights reserved.
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
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
using System;
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The base class for `WindowsWebPlugin` and `MacWebPlugin`.
    /// </summary>
    class StandaloneWebPlugin : MonoBehaviour {

        public void ClearAllData() {

            StandaloneWebView.ClearAllData();
        }

        public void CreateTexture(float width, float height, Action<Texture2D> callback) {

            StandaloneWebView.CreateTexture(width, height, callback);
        }

        public void CreateMaterial(Action<Material> callback) {

            CreateTexture(1, 1, texture => {
                var material = Utils.CreateDefaultMaterial();
                material.mainTexture = texture;
                callback(material);
            });
        }

        public void CreateVideoMaterial(Action<Material> callback) {

            callback(null);
        }

        public void SetIgnoreCertificateErrors(bool ignore) {

            StandaloneWebView.SetIgnoreCertificateErrors(ignore);
        }

        public void SetStorageEnabled(bool enabled) {

            StandaloneWebView.SetStorageEnabled(enabled);
        }

        public void SetUserAgent(bool mobile) {

            StandaloneWebView.GloballySetUserAgent(mobile);
        }

        public void SetUserAgent(string userAgent) {

            StandaloneWebView.GloballySetUserAgent(userAgent);
        }

    #if UNITY_2018_1_OR_NEWER
        void Start() {
            // In newer versions of Unity, use Application.quitting instead
            // of OnApplicationQuit(), because the latter is called even if
            // the quit is cancelled by the application returning false from
            // Application.wantsToQuit, and the former is called only when the
            // application really quits.
            Application.quitting += () => StandaloneWebView.TerminatePlugin();
        }
    #else
        // Prior to Unity 2018.1, Application.quitting and Application.wantsToQuit
        // don't exist.
        void OnApplicationQuit() {

            StandaloneWebView.TerminatePlugin();
        }
    #endif
    }
}
#endif
