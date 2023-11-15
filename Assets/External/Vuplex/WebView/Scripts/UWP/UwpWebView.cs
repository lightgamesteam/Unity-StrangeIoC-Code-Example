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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
#if NET_4_6 || NET_STANDARD_2_0
    using System.Threading.Tasks;
#endif
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRSettings = UnityEngine.VR.VRSettings;
#endif

namespace Vuplex.WebView {

    /// <summary>
    /// The `IWebView` implementation for 3D WebView for UWP / Hololens.
    /// This class also includes extra methods for UWP-specific functionality.
    /// </summary>
    public class UwpWebView : BaseWebView, IWebView {

        public WebPluginType PluginType {
            get {
                return WebPluginType.UniversalWindowsPlatform;
            }
        }

        /// <summary>
        /// Overrides `BaseWebView.CaptureScreenshot()` because it leaks memory on UWP.
        /// </summary>
        public override void CaptureScreenshot(Action<byte[]> callback) {

            _invokeDataOperation(callback, WebView_captureScreenshot);
        }

        /// <summary>
        /// Like `DeleteCookies(string)`, except it uses a callback
        /// instead of a `Task` in order to be compatible with legacy .NET.
        /// </summary>
        public static void DeleteCookies(string url, Action callback) {

            WebView_deleteCookies(url);
            callback();
        }

    #if NET_4_6 || NET_STANDARD_2_0
        /// <summary>
        /// Deletes all of the cookies for the given URL.
        /// </summary>
        public static Task DeleteCookies(string url) {

            var task = new TaskCompletionSource<bool>();
            DeleteCookies(url, () => task.SetResult(true));
            return task.Task;
        }
    #endif

        public override void Dispose() {

            // Cancel the render if it has been scheduled via GL.IssuePluginEvent().
            WebView_removePointer(_nativeWebViewPtr);
            base.Dispose();
        }

        /// <summary>
        /// Overrides `BaseWebView.GetRawTextureData()` because it leaks memory on UWP.
        /// </summary>
        public override void GetRawTextureData(Action<byte[]> callback) {

            _invokeDataOperation(callback, WebView_getRawTextureData);
        }

        /// <summary>
        /// This method automatically gets called when the app changes focus.
        /// </summary>
        public static void HandleAppFocusChanged(bool focused) {

            if (XRSettings.enabled) {
                WebView_handleMixedRealityAppFocusChanged(focused);
            }
        }

        /// <summary>
        /// This method automatically gets called when the application starts.
        /// </summary>
        public static void InitializePlugin() {

            // The generic `GetFunctionPointerForDelegate<T>` is unavailable in .NET 2.0.
            var sendMessageFunction = Marshal.GetFunctionPointerForDelegate((UnitySendMessageFunction)_unitySendMessage);
            WebView_setSendMessageFunction(sendMessageFunction);

            var dataResultCallback = Marshal.GetFunctionPointerForDelegate((DataResultCallback)_dataResultCallback);
            WebView_setDataResultCallback(dataResultCallback);

            var logInfo = Marshal.GetFunctionPointerForDelegate((LogFunction)_logInfo);
            var logWarning = Marshal.GetFunctionPointerForDelegate((LogFunction)_logWarning);
            var logError = Marshal.GetFunctionPointerForDelegate((LogFunction)_logError);
            WebView_setLogFunctions(logInfo, logWarning, logError);
            WebView_logVersionInfo();
        }

        public static UwpWebView Instantiate() {

            return (UwpWebView) new GameObject().AddComponent<UwpWebView>();
        }

        public override void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture) {

            base.Init(viewportTexture, width, height, videoTexture);
            _nativeWebViewPtr = WebView_new(gameObject.name, _nativeWidth, _nativeHeight, XRSettings.enabled);
            if (_nativeWebViewPtr == IntPtr.Zero) {
                throw new WebViewUnavailableException("Failed to instantiate a new webview. This could indicate that you're using an expired trial version of 3D WebView.");
            }
        }

        public static bool ValidateGraphicsApi() {

            var isValid = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11;
            if (!isValid) {
                WebViewLogger.LogError("Unsupported graphics API: 3D WebView for UWP requires Direct3D11. Please go to Player Settings and set \"Graphics APIs\" to Direct3D11.");
            }
            return isValid;
        }

        delegate void DataResultCallback(string gameObjectName, string resultCallbackId, IntPtr imageBytes, int imageBytesLength);
        delegate void LogFunction(string message);
        Dictionary<string, Action<byte[]>> _pendingDataResultCallbacks = new Dictionary<string, Action<byte[]>>();
        delegate void UnitySendMessageFunction(string gameObjectName, string methodName, string message);
        readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

        [AOT.MonoPInvokeCallback(typeof(DataResultCallback))]
        static void _dataResultCallback(string gameObjectName, string resultCallbackId, IntPtr unmanagedBytes, int bytesLength) {

            // Load the results into a managed array.
            var managedBytes = new byte[bytesLength];
            Marshal.Copy(unmanagedBytes, managedBytes, 0, bytesLength);

            Dispatcher.RunOnMainThread(() => {
                var gameObj = GameObject.Find(gameObjectName);
                if (gameObj == null) {
                    WebViewLogger.LogErrorFormat("Unable to process the data result, because there is no GameObject named '{0}'", gameObjectName);
                    return;
                }
                var webView = gameObj.GetComponent<UwpWebView>();
                webView._handleDataResult(resultCallbackId, managedBytes);
            });
        }

        void _handleDataResult(string resultCallbackId, byte[] bytes) {

            var callback = _pendingDataResultCallbacks[resultCallbackId];
            _pendingDataResultCallbacks.Remove(resultCallbackId);
            callback(bytes);
        }

        void _invokeDataOperation(Action<byte[]> callback, Action<IntPtr, string> nativeDataMethod) {

            _assertValidState();
            try {
                string resultCallbackId = null;
                if (callback != null) {
                    resultCallbackId = Guid.NewGuid().ToString();
                    _pendingDataResultCallbacks[resultCallbackId] = callback;
                }
                nativeDataMethod(_nativeWebViewPtr, resultCallbackId);
            } catch (Exception e) {
                WebViewLogger.LogError("An exception occurred in while getting the webview data: " + e);
                callback(new byte[0]);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logInfo(string message) {

            WebViewLogger.Log(message, false);
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logWarning(string message) {

            WebViewLogger.LogWarning(message, false);
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logError(string message) {

            WebViewLogger.LogError(message, false);
        }

        void OnEnable() {

            // Start the coroutine from OnEnable so that the coroutine
            // is restarted if the object is deactivated and then reactivated.
            StartCoroutine(_renderPluginOncePerFrame());
        }

        IEnumerator _renderPluginOncePerFrame() {

            while (true) {
                yield return _waitForEndOfFrame;

                if (_nativeWebViewPtr != IntPtr.Zero && !IsDisposed) {
                    int pointerId = WebView_depositPointer(_nativeWebViewPtr);
                    GL.IssuePluginEvent(WebView_getRenderFunction(), pointerId);
                }
            }
        }

        [AOT.MonoPInvokeCallback(typeof(UnitySendMessageFunction))]
        static void _unitySendMessage(string gameObjectName, string methodName, string message) {

            Dispatcher.RunOnMainThread(() => {
                var gameObj = GameObject.Find(gameObjectName);
                if (gameObj == null) {
                    WebViewLogger.LogErrorFormat("Unable to send the message, because there is no GameObject named '{0}'", gameObjectName);
                    return;
                }
                gameObj.SendMessage(methodName, message);
            });
        }

        [DllImport(_dllName)]
        static extern void WebView_captureScreenshot(IntPtr webViewPtr, string resultCallbackId);

        [DllImport(_dllName)]
        static extern void WebView_deleteCookies(string url);

        [DllImport(_dllName)]
        static extern int WebView_depositPointer(IntPtr pointer);

        [DllImport(_dllName)]
        static extern void WebView_getRawTextureData(IntPtr webViewPtr, string resultCallbackId);

        [DllImport(_dllName)]
        static extern IntPtr WebView_getRenderFunction();

        [DllImport(_dllName)]
        static extern void WebView_handleMixedRealityAppFocusChanged(bool focused);

        [DllImport(_dllName)]
        static extern IntPtr WebView_logVersionInfo();

        [DllImport(_dllName)]
        static extern IntPtr WebView_new(string gameObjectName, int width, int height, bool mixedRealityEnabled);

        [DllImport(_dllName)]
        static extern void WebView_removePointer(IntPtr pointer);

        [DllImport(_dllName)]
        static extern int WebView_setDataResultCallback(IntPtr callback);

        [DllImport(_dllName)]
        static extern int WebView_setLogFunctions(
            IntPtr logInfoFunction,
            IntPtr logWarningFunction,
            IntPtr logErrorFunction
        );

        [DllImport(_dllName)]
        static extern int WebView_setSendMessageFunction(IntPtr sendMessageFunction);
    }
}
#endif
