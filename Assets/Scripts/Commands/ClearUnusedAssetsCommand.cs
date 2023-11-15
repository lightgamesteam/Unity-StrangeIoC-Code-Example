using UnityEngine;

namespace PFS.Assets.Scripts.Commands.AssetBundles
{
    public class ClearUnusedAssetsCommand : BaseCommand
    {
        public override void Execute()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}