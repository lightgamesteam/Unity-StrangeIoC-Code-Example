using PFS.Assets.Scripts.Models.AssetBundles;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PFS.Assets.Scripts.Commands.AssetBundles
{
    public class DownloadAssetBundle : BaseCommand
    {
        [Inject]
        public IExecutor Courutine { get; private set; }

        private ShowAssetBundleModel showAssetBundleModel;
        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                showAssetBundleModel = EventData.data as ShowAssetBundleModel;
            }
            else
            {
                Debug.LogError("ShowAssetBundleModel - NULL");
                Release();
                return;
            }

            //#if UNITY_EDITOR
            //        showAssetBundleModel.isLoaded = true;
            //#endif

            if (!showAssetBundleModel.isLoaded)
            {
                string url = showAssetBundleModel.GetFullUrl(showAssetBundleModel.AssetBundleName);
                Courutine.Execute(DownloadFromServer(url));
            }
            else
            {
                Release();
            }
        }

        private IEnumerator DownloadFromServer(string urlPath)
        {
            Debug.Log("Asset Bundle will be loaded by URL: " + urlPath);
            UnityWebRequest www = UnityWebRequest.Get(urlPath);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                Fail();
            }
            else
            {
                byte[] bytes = www.downloadHandler.data;
                string path = showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName);
                File.WriteAllBytes(path, bytes);
                Debug.Log("Asset Bundle saved by path: " + path);
                Release();
            }

            www.Dispose();
        }
    }
}