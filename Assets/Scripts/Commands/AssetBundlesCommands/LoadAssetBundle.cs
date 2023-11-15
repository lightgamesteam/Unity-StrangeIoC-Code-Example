using PFS.Assets.Scripts.Models.AssetBundles;
using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.AssetBundles
{
    public class LoadAssetBundle : BaseCommand
    {
        private ShowAssetBundleModel showAssetBundleModel;

        [Inject]
        public IExecutor Courutine { get; private set; }

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

            Courutine.Execute(GetAsset());
        }

        IEnumerator GetAsset()
        {

            var myLoadedAssetBundle = AssetBundle.LoadFromFile(showAssetBundleModel.GetPathToFile(showAssetBundleModel.AssetBundleName));
            if (myLoadedAssetBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle!");
                Fail();
                yield break;
            }

            var assetBundleRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(myLoadedAssetBundle.GetAllAssetNames()[0]);
            yield return assetBundleRequest;
            if (assetBundleRequest == null)
            {
                Debug.LogError("assetBundleRequest == null");
                Fail();
                yield break;
            }

            GameObject prefab = assetBundleRequest.asset as GameObject;
            if (prefab == null)
            {
                Debug.LogError("prefab == null");
                Fail();
                yield break;
            }

            GameObject instance = null;
            if (!showAssetBundleModel.ParentGM)
            {
                instance = GameObject.Instantiate(prefab);
            }
            else
            {
                instance = GameObject.Instantiate(prefab, showAssetBundleModel.ParentGM.transform);
            }
            instance.name = prefab.name;

            myLoadedAssetBundle.Unload(false);

            showAssetBundleModel.isInstatiated = true;

            Release();
        }
    }
}