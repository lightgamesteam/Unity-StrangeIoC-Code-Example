using UnityEngine;
using System.IO;
using PFS.Assets.Scripts.Models.AssetBundles;

namespace PFS.Assets.Scripts.Commands.AssetBundles
{
    public class DeleteAssetBundle : BaseCommand
    {
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
                Debug.LogError("DeleteAssetBundle => ShowAssetBundleModel - NULL");
                Fail();
                return;
            }

            if (!DeleteAssetbundleManifest())
            {
                Fail();
                return;
            }

            if (!DeleteAssetbundle())
            {
                Fail();
                return;
            }

            Release();
        }

        private bool DeleteAssetbundleManifest()
        {
            string pathManifest = showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName) + CheckAssetBundle.PathPart;    //path to step manifest

            if (!File.Exists(pathManifest))
            {
                Debug.LogError("DeleteAssetBundle => DeleteAssetbundleManifest - NULL");
                return false;
            }

            File.Delete(pathManifest);

            Debug.Log("Deleted manifest at path - " + pathManifest);

            return true;
        }

        private bool DeleteAssetbundle()
        {
            string path = showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName);

            if (!File.Exists(path))
            {
                Debug.LogError("DeleteAssetBundle => DeleteAssetbundle - NULL");
                return false;
            }

            File.Delete(path);

            Debug.Log("Deleted assetbundle at path - " + path);

            return true;
        }
    }
}