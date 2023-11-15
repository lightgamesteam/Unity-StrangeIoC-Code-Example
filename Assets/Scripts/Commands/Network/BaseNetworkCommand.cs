using Newtonsoft.Json.Linq;
using PFS.Assets.Scripts.Models;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network
{
    abstract public class BaseNetworkCommand : BaseCommand
    {
        //Product Amazon server
        private const string ProdServerUrl = "//////////////////////////////";
        private const string ProdAssetBundlesServerUrl = "////////////////////////////////";

        //Test Amazon server
        private const string TestServerUrl = "//////////////////////////////////////";
        private const string TestAssetBundlesServerUrl = "/////////////////////////////////////";

        //Local server
        private const string LocalServerUrl = "/////////////////////////////";
        private const string LocalAssetBundlesServerUrl = "/////////////////////////////////";


        public JObject jsonData = new JObject();

        public static string ServerUrl
        {
            get
            {
#if DEVELOP
                if (PlayerPrefsModel.ServerTypeName == "Test")
                {
                    return TestServerUrl;
                }
                else if (PlayerPrefsModel.ServerTypeName == "Prod")
                {
                    return ProdServerUrl;
                }
                else
                {
                    return LocalServerUrl;
                }
#else
                return ProdServerUrl;
#endif
            }
        }

        public static string ServerUrlAssetBundles
        {
            get
            {
#if DEVELOP
                if (PlayerPrefsModel.ServerTypeName == "Test")
                {
                    return TestAssetBundlesServerUrl;
                }
                else if (PlayerPrefsModel.ServerTypeName == "Prod")
                {
                    return ProdAssetBundlesServerUrl;
                }
                else
                {
                    return LocalAssetBundlesServerUrl;
                }
#else
                return ProdAssetBundlesServerUrl;
#endif
            }
        }

        public void PrepareObject(object requestObject)
        {
            if (requestObject == null)
            {
                Debug.LogWarning("BaseNetworkCommand -> PrepareObject -> requestObject --- NULL");
                jsonData = new JObject();
            }
            else
            {
                //Debug.Log("Prepare object = " + requestObject.ToString());
                jsonData = JObject.FromObject(requestObject);
            }
        }
    }
}