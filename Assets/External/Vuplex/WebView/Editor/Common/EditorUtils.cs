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
#pragma warning disable CS0618
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRSettings = UnityEngine.VR.VRSettings;
#endif

namespace Vuplex.WebView {

    static class EditorUtils {

        public static void AssertThatSrpBatcherIsDisabled() {

            #if UNITY_2018_2_OR_NEWER
                if (UnityEngine.Rendering.GraphicsSettings.useScriptableRenderPipelineBatching) {
                    throw new BuildFailedException("URP settings error: 3D WebView for Android requires that \"SRP Batcher\" be disabled in URP settings. Please go to \"UniversalRenderPipelineAsset\" -> \"Advanced\" and disable SRP Batcher.");
                }
            #endif
        }


        public static void AssertThatOculusLowOverheadModeIsDisabled() {

            // VROculus.lowOverheadMode is only supported from Unity 2019.2 - 2019.4
            #if UNITY_2019_2_OR_NEWER && !UNITY_2020_1_OR_NEWER
                var oculusSdkIsEnabled = Array.IndexOf(XRSettings.supportedDevices, "Oculus") != -1;
                if (oculusSdkIsEnabled && PlayerSettings.VROculus.lowOverheadMode) {
                    throw new BuildFailedException("XR settings error: Vuplex 3D WebView requires that \"Low Overhead Mode\" be disabled in Oculus XR settings. Please disable Low Overhead Mode in Oculus XR settings.");
                }
            #endif
        }

        public static void CopyAndReplaceDirectory(string srcPath, string dstPath, bool ignoreMetaFiles = true) {

            if (Directory.Exists(dstPath)) {
                Directory.Delete(dstPath, true);
            }
            if (File.Exists(dstPath)) {
                File.Delete(dstPath);
            }
            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath)) {
                if (!ignoreMetaFiles || Path.GetExtension(file) != ".meta") {
                    File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
                }
            }
            foreach (var dir in Directory.GetDirectories(srcPath)) {
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)), ignoreMetaFiles);
            }
        }

        public static void DrawLink(string linkText, string url, int underlineLength) {

            var linkStyle = new GUIStyle {
                richText = true,
                padding = new RectOffset {
                    top = 10,
                    bottom = 10
                }
            };
            var linkClicked = GUILayout.Button(
                EditorUtils.TextWithColor(linkText, EditorUtils.GetLinkColor()),
                linkStyle
            );
            var linkRect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(linkRect, MouseCursor.Link);

            // Unity's editor GUI doesn't support underlines, so fake it.
            var underscores = new string[underlineLength];
            for (var i = 0; i < underlineLength; i++) {
                underscores[i] = "_";
            }
            var underline = String.Join("", underscores);

            GUI.Label(
                linkRect,
                EditorUtils.TextWithColor(underline, EditorUtils.GetLinkColor()),
                new GUIStyle {
                    richText = true,
                    padding = new RectOffset {
                        top = 12,
                        bottom = 10
                }
            });
            if (linkClicked) {
                Application.OpenURL(url);
            }
        }

        /// <summary>
        /// Returns the path to a given directory, searching for it if needed.
        /// If `directoryToSearch` isn't provided, `Application.dataPath` is used.
        /// </summary>
        public static string FindDirectory(string expectedPath, string directoryToSearch = null) {

            if (Directory.Exists(expectedPath)) {
                return expectedPath;
            }
            // The directory isn't in the expected location, so fall back to finding it.
            var directoryName = Path.GetFileName(expectedPath);
            if (directoryToSearch == null) {
                directoryToSearch = Application.dataPath;
            }
            var directories = Directory.GetDirectories(directoryToSearch, directoryName, SearchOption.AllDirectories);
            if (directories.Length == 1) {
                return directories[0];
            }
            var errorMessage = String.Format("Unable to locate the directory {0}. It's not in the expected location ({1}), and {2} instances of the directory were found in the directory {3}.", directoryName, expectedPath, directories.Length, directoryToSearch);
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Returns the path to a given file, searching for it if needed.
        /// If `directoryToSearch` isn't provided, `Application.dataPath` is used.
        /// </summary>
        public static string FindFile(string expectedPath, string directoryToSearch = null) {

            if (File.Exists(expectedPath)) {
                return expectedPath;
            }
            // The file isn't in the expected location, so fall back to finding it.
            var fileName = Path.GetFileName(expectedPath);
            if (directoryToSearch == null) {
                directoryToSearch = Application.dataPath;
            }
            var files = Directory.GetFiles(directoryToSearch, fileName, SearchOption.AllDirectories);
            if (files.Length == 1) {
                return files[0];
            }
            var errorMessage = String.Format("Unable to locate the file {0}. It's not in the expected location ({1}), and {2} instances of the directory were found in the directory {3}.", fileName, expectedPath, files.Length, directoryToSearch);
            throw new Exception(errorMessage);
        }

        public static string GetLinkColor() {

            return EditorGUIUtility.isProSkin ? "#7faef0ff" : "#11468aff";
        }

        /// <summary>
        /// A polyfill for `Path.Combine(string[])`, which isn't present in .NET 2.0.
        /// </summary>
        public static string PathCombine(string[] pathComponents) {

            if (pathComponents.Length == 0) {
                return "";
            }
            if (pathComponents.Length == 1) {
                return pathComponents[0];
            }
            var path = pathComponents[0];
            for (var i = 1; i < pathComponents.Length; i++) {
                path = System.IO.Path.Combine(path, pathComponents[i]);
            }
            return path;
        }

        public static string TextWithColor(string text, string color) {

            return String.Format("<color={0}>{1}</color>", color, text);
        }
    }
}
