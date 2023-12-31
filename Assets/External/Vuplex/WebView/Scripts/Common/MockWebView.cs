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
#pragma warning disable CS0067
using System;
using System.Collections.Generic;
using UnityEngine;

#if NET_4_6 || NET_STANDARD_2_0
    using System.Threading.Tasks;
#endif

namespace Vuplex.WebView {
    /// <summary>
    /// Mock IWebView implementation used for running in the Unity editor.
    /// </summary>
    /// <remarks>
    /// MockWebView logs messages to the console to indicate when its methods are
    /// called, but it doesn't actually load or render web content.
    /// </remarks>
    class MockWebView : MonoBehaviour, IWebView {

        public event EventHandler CloseRequested;

        public event EventHandler<ConsoleMessageEventArgs> ConsoleMessageLogged;

        public event EventHandler<ProgressChangedEventArgs> LoadProgressChanged;

        public event EventHandler<EventArgs<string>> MessageEmitted;

        public event EventHandler PageLoadFailed;

        public event EventHandler<EventArgs<string>> TitleChanged;

        public event EventHandler<UrlChangedEventArgs> UrlChanged;

        public event EventHandler<EventArgs<Rect>> VideoRectChanged;

        public bool IsDisposed { get; private set; }

        public bool IsInitialized { get; private set; }

        public List<string> PageLoadScripts {
            get {
                return _pageLoadScripts;
            }
        }

        public WebPluginType PluginType {
            get {
                return WebPluginType.Mock;
            }
        }

        public float Resolution {
            get {
                return _numberOfPixelsPerUnityUnit;
            }
        }

        public Vector2 Size { get; private set; }

        public Vector2 SizeInPixels {
            get {
                return new Vector2(Size.x * _numberOfPixelsPerUnityUnit, Size.y * _numberOfPixelsPerUnityUnit);
            }
        }

        public Texture2D Texture { get; private set; }

        public string Url { get; private set; }

        public Texture2D VideoTexture { get; private set; }

        public void Init(Texture2D viewportTexture, float width, float height) {

            Init(viewportTexture, width, height, null);
        }

        public static MockWebView Instantiate() {

            return (MockWebView) new GameObject("MockWebView").AddComponent<MockWebView>();
        }

        public void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture) {

            Texture = viewportTexture;
            VideoTexture = videoTexture;
            Size = new Vector2(width, height);
            IsInitialized = true;
            DontDestroyOnLoad(gameObject);
            _log("Init() width: {0}, height: {1}", width.ToString("n4"), height.ToString("n4"));
        }

        public void Blur() {

            _log("Blur()");
        }

    #if NET_4_6 || NET_STANDARD_2_0
        public Task<bool> CanGoBack() {

            var task = new TaskCompletionSource<bool>();
            CanGoBack(task.SetResult);
            return task.Task;
        }

        public Task<bool> CanGoForward() {

            var task = new TaskCompletionSource<bool>();
            CanGoForward(task.SetResult);
            return task.Task;
        }
    #endif

        public void CanGoBack(Action<bool> callback) {

            _log("CanGoBack()");
            callback(false);
        }

        public void CanGoForward(Action<bool> callback) {

            _log("CanGoForward()");
            callback(false);
        }

    #if NET_4_6 || NET_STANDARD_2_0
        public Task<byte[]> CaptureScreenshot() {

            var task = new TaskCompletionSource<byte[]>();
            CaptureScreenshot(task.SetResult);
            return task.Task;
        }
    #endif

        public void CaptureScreenshot(Action<byte[]> callback) {

            _log("CaptureScreenshot()");
            callback(new byte[0]);
        }

        public void Click(Vector2 point) {

            _log("Click({0})", point.ToString("n4"));
        }

        public void Click(Vector2 point, bool preventStealingFocus) {

            _log("Click({0}, {1})", point.ToString("n4"), preventStealingFocus);
        }

        public void Copy() {

            _log("Copy()");
        }

        public void Cut() {

            _log("Cut()");
        }

        public void DisableViewUpdates() {

            _log("DisableViewUpdates()");
        }

        public void Dispose() {

            IsDisposed = true;
            _log("Dispose()");
            if (this != null) {
                Destroy(gameObject);
            }
        }

        public void EnableViewUpdates() {

            _log("EnableViewUpdates()");
        }

    #if NET_4_6 || NET_STANDARD_2_0
        public Task<string> ExecuteJavaScript(string javaScript) {

            var task = new TaskCompletionSource<string>();
            ExecuteJavaScript(javaScript, task.SetResult);
            return task.Task;
        }
    #else
        public void ExecuteJavaScript(string javaScript) {

            _log("ExecuteJavaScript(\"{0}...\")", javaScript.Substring(0, 25));
        }
    #endif

        public void ExecuteJavaScript(string javaScript, Action<string> callback) {

            _log("ExecuteJavaScript(\"{0}...\")", javaScript.Substring(0, 25));
            callback("");
        }

        public void Focus() {

            _log("Focus()");
        }

        #if NET_4_6 || NET_STANDARD_2_0
            public Task<byte[]> GetRawTextureData() {

                var task = new TaskCompletionSource<byte[]>();
                GetRawTextureData(task.SetResult);
                return task.Task;
            }
        #endif

        public void GetRawTextureData(Action<byte[]> callback) {

            _log("GetRawTextureData()");
            callback(new byte[0]);
        }

        public void GoBack() {

            _log("GoBack()");
        }

        public void GoForward() {

            _log("GoForward()");
        }

        public void HandleKeyboardInput(string input) {

            _log("HandleKeyboardInput(\"{0}\")", input);
        }

        public virtual void LoadHtml(string html) {

            Url = html.Substring(0, 25);
            _log("LoadHtml(\"{0}...\")", Url);
            if (UrlChanged != null) {
                UrlChanged(this, new UrlChangedEventArgs(Url, "Title", UrlActionType.Load));
            }
            if (LoadProgressChanged != null) {
                LoadProgressChanged(this, new ProgressChangedEventArgs(ProgressChangeType.Finished, 1.0f));
            }
        }

        public virtual void LoadUrl(string url) {

            LoadUrl(url, null);
        }

        public virtual void LoadUrl(string url, Dictionary<string, string> additionalHttpHeaders) {

            Url = url;
            _log("LoadUrl(\"{0}\")", url);
            if (UrlChanged != null) {
                UrlChanged(this, new UrlChangedEventArgs(url, "Title", UrlActionType.Load));
            }
            if (LoadProgressChanged != null) {
                LoadProgressChanged(this, new ProgressChangedEventArgs(ProgressChangeType.Finished, 1.0f));
            }
        }

        public void Paste() {

            _log("Paste()");
        }

        public void PostMessage(string data) {

            _log("PostMessage(\"{0}\")", data);
        }

        public void Reload() {

            _log("Reload()");
        }

        public void Resize(float width, float height) {

            Size = new Vector2(width, height);
            _log("Resize({0}, {1})", width.ToString("n4"), height.ToString("n4"));
        }

        public void Scroll(Vector2 delta) {

            _log("Scroll({0})", delta.ToString("n4"));
        }

        public void Scroll(Vector2 delta, Vector2 point) {

            _log("Scroll({0}, {1})", delta.ToString("n4"), point.ToString("n4"));
        }

        public void SelectAll() {

            _log("SelectAll()");
        }

        public void SetResolution(float pixelsPerUnityUnit) {

            _numberOfPixelsPerUnityUnit = pixelsPerUnityUnit;
            _log("SetResolution({0})", pixelsPerUnityUnit);
        }

        public void ZoomIn() {

            _log("ZoomIn()");
        }

        public void ZoomOut() {

            _log("ZoomOut()");
        }

        List<string> _pageLoadScripts = new List<string>();
        float _numberOfPixelsPerUnityUnit = Config.NumberOfPixelsPerUnityUnit;

        void _log(string message, params object[] args) {

            WebViewLogger.LogFormat("[MockWebView] " + message, args);
        }
    }
}
