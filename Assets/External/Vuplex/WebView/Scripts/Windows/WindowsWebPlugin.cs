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
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The Windows `IWebPlugin` implementation.
    /// </summary>
    class WindowsWebPlugin : StandaloneWebPlugin, IWebPlugin {

        public static WindowsWebPlugin Instance {
            get {
                if (_instance == null) {
                    _instance = (WindowsWebPlugin) new GameObject("WindowsWebPlugin").AddComponent<WindowsWebPlugin>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public virtual IWebView CreateWebView() {

            return WindowsWebView.Instantiate();
        }

        static WindowsWebPlugin _instance;

        void OnValidate() {

            WindowsWebView.ValidateGraphicsApi();
        }

        public void EnableRemoteDebugging()
        {

        }
    }
}
#endif
