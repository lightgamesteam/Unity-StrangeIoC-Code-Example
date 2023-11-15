using PFS.Assets.Scripts.Commands.Network;
using System.IO;
using UnityEngine;

namespace PFS.Assets.Scripts.Models.AssetBundles
{
    public class ShowAssetBundleModel
    {
        public bool isLoaded = false;
        public bool isInstatiated = false;
        private string assetBundleFullUrl;

        public string AssetBundleName { get; protected set; }
        public GameObject ParentGM { get; protected set; }

        public ShowAssetBundleModel(string assetBundleName, GameObject parentGM)
        {
            this.AssetBundleName = assetBundleName;
            this.ParentGM = parentGM;
        }

        public ShowAssetBundleModel(GameObject parentGM, string assetBundleFullUrl)
        {
            string[] assetBundleUrlParts = GetAssetBundleName(assetBundleFullUrl);
            this.AssetBundleName = assetBundleUrlParts[0];
            this.assetBundleFullUrl = assetBundleUrlParts[1];
            this.ParentGM = parentGM;
        }

        private string[] GetAssetBundleName(string url)
        {
            for (int i = url.Length - 1; i >= 0; i--)
            {
                if (url[i] == '/')
                {
                    string assetName = url.Remove(0, i + 1);
                    string assetBaseUrl = url.Remove(i + 1);

                    return new string[2] { assetName, assetBaseUrl };
                }
            }
            return null;
        }

        /// <summary>
        /// Get path to asset bundles folder - local
        /// </summary>
        /// <returns></returns>
        public static string GetPathToFolder()
        {
            string pathToAssetFolder;
#if UNITY_EDITOR
            //delete "Asset/" in path
            //pathToSaves =>>> "D:/_WORK_/Pickatale LP/Pickatale LP Unity Project/AssetBundles/Android";
            pathToAssetFolder = Application.dataPath.Remove(Application.dataPath.Length - 7, 7) + "/AssetBundles/Android/";
#else
        pathToAssetFolder =  Application.persistentDataPath + "/AssetBundles";
#endif
            return pathToAssetFolder;
        }

        /// <summary>
        /// Get path to asset bundles file - local
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual string GetPathToFile(string fileName)
        {
            string pathToFile = Path.Combine(GetPathToFolder(), fileName.ToLower());
            return pathToFile;
        }

        /// <summary>
        /// Get url to asset bundles file on server
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual string GetFullUrl(string fileName)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            string assetType = "android";
#elif UNITY_IOS
        string assetType = "ios";
#elif UNITY_STANDALONE || UNITY_WSA
string assetType = "windows";
#endif
            string urlToFile = (string.IsNullOrEmpty(assetBundleFullUrl) ? string.Format("{0}/{1}/{2}", BaseNetworkCommand.ServerUrlAssetBundles, PlayerPrefsModel.AssetBundleFolderName.ToLower(), assetType) : assetBundleFullUrl) + "/" + fileName.ToLower();
            return urlToFile;
        }
    }
}