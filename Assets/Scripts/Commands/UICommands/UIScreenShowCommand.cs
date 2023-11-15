using PFS.Assets.Scripts.Models.ScreenManagerModels;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class UIScreenShowCommand : BaseCommand
    {
        [Inject]
        public ScreenManager ScreenManager { get; set; }

        static public Canvas convas = null;

        private string nameScreen = "";
        private string folderPath = "Prefabs/UI/";
        private object otherData = null;
        private bool isAddToScreensList = true; // add or not to screens list

        public override void Execute()
        {
            if (EventData.data != null)
            {
                ShowScreenModel ssm = EventData.data as ShowScreenModel;
                if (ssm == null)
                {
                    nameScreen = EventData.data.ToString();
                    isAddToScreensList = true;
                }
                else
                {
                    nameScreen = ssm.screenName;
                    otherData = ssm.data;
                    isAddToScreensList = ssm.isAddToScreensList;
                    if (!string.IsNullOrEmpty(ssm.spetialPath))
                        folderPath = ssm.spetialPath;
                }
                LoadScreen(nameScreen, otherData);
            }
            else
            {
                Debug.LogError("No Name Screen");
            }
        }

        private void LoadScreen(string nameScreen, object otherData)
        {
            Object obj = GameObject.Find(nameScreen);
            if (!obj)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////

                Object prefab = Resources.Load(folderPath + nameScreen, typeof(GameObject));
                if (!prefab)
                {
                    Debug.LogError("Resources.Load  prefab == NULL");
                    Debug.LogError("Resources name " + nameScreen);
                }
                else
                {
                    GameObject clone = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    clone.transform.position = Vector3.zero;
                    clone.name = nameScreen;
                    convas = clone.GetComponent<Canvas>();
                    if (GameObject.Find("Main Camera") != null && GameObject.Find("Main Camera").GetComponent<Camera>() != null && convas != null)
                        convas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

                    //if screen done, add this to list
                    if (isAddToScreensList)
                    {
                        ScreenManager.AddScreen(EventData.data);
                    }

                    BaseView hudV = clone.GetComponent<BaseView>();
                    if (hudV)
                    {
                        //mediationBinder.Trigger( strange.extensions.mediation.api.MediationEvent.AWAKE, hudV as IView );
                        if (otherData != null)
                        {
                            hudV.otherData = otherData;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("BaseView == NULL");
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////
            }
            else
            {
                if (convas != null)
                {
                    convas.enabled = true;
                }
                else
                {
                    Debug.LogError("convas " + nameScreen + " == NULL");
                }
                //((GameObject)obj).GetComponent<Canvas>().enabled = true;
            }
        }

    }
}
