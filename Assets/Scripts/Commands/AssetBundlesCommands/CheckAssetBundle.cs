using PFS.Assets.Scripts.Models.AssetBundles;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PFS.Assets.Scripts.Commands.AssetBundles
{

    /// <summary>
    /// Check if Asset bundle for selected step alredy in the device storage. 
    /// And compare local manifest with serve manifest. 
    /// If server manifest is different - we should load step file from server.
    /// </summary>
    public class CheckAssetBundle : BaseCommand
    {
        public const string PathPart = ".manifest";

        [Inject]
        public IExecutor Courutine { get; private set; }

        private ShowAssetBundleModel showAssetBundleModel;
        private string pathManifest;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                showAssetBundleModel = EventData.data as ShowAssetBundleModel;
            }
            else
            {
                Debug.LogError("Can't check asset bundle: ShowAssetBundleModel - NULL");
                Release();
                return;
            }
            Courutine.Execute(CheckAssetExist());
        }

        private IEnumerator CheckAssetExist()
        {
            string urlManifest = showAssetBundleModel.GetFullUrl(showAssetBundleModel.AssetBundleName) + PathPart; //url  to step manifest
            pathManifest = showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName) + PathPart;    //path to step manifest
            string path = showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName);                //path to step file

            if (Directory.Exists(ShowAssetBundleModel.GetPathToFolder()))
            {
                if (File.Exists(path))
                {
                    Debug.Log("Asset Bundle File is existed by path: " + path);

                    if (!File.Exists(pathManifest))
                    {
                        showAssetBundleModel.isLoaded = true;
                        Release();
                        yield break;
                    }
                    string oldHash = GetAssetBundleHash(pathManifest);
                    yield return Courutine.Execute(DownloadManifestFromServer(urlManifest));
                    string newHash = GetAssetBundleHash(pathManifest);
                    Debug.Log("Comapare Manifests");
                    if (!string.IsNullOrEmpty(oldHash) && oldHash == newHash)
                    {
                        Debug.Log("Don't need to load Asset Bundle");
                        showAssetBundleModel.isLoaded = true;
                    }
                    else
                    {
                        Debug.Log("Need load new Asset Bundle");
                    }
                }
                else
                {
                    Debug.Log("Asset Bundle file don't existed by path: " + path);
                    yield return Courutine.Execute(DownloadManifestFromServer(urlManifest));
                }
            }
            else
            {
                Debug.Log("Folder don't existed, folder will be created by same path: " + ShowAssetBundleModel.GetPathToFolder());
                Directory.CreateDirectory(ShowAssetBundleModel.GetPathToFolder());
                yield return Courutine.Execute(DownloadManifestFromServer(urlManifest));
            }
            Release();
        }

        private IEnumerator DownloadManifestFromServer(string urlPath)
        {
            UnityWebRequest www = UnityWebRequest.Get(urlPath);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                Debug.LogError("Error by URL:" + urlPath);
            }
            else
            {
                byte[] bytes = www.downloadHandler.data;
                File.WriteAllBytes(pathManifest, bytes);
                Debug.Log("Downloaded and Saved .manifest by path: " + pathManifest);
            }
            www.Dispose();
        }

        private string GetAssetBundleHash(string path)
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines == null || lines.Length < 6 || lines[5].Length < 10)
            {
                Debug.LogError("Cant get hash");
                return "";
            }
            return lines[5].Remove(0, 10);
        }
    }
}