using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class InitManagerCommand : BaseCommand
    {
        static public Canvas convas = null;

        private string nameScreen = "";

        public override void Execute()
        {
            //Debug.Log("InitManagerCommand");

            if (EventData.data != null)
            {
                nameScreen = EventData.data.ToString();
            }
            else
            {
                Debug.LogError("No Name Screen manager");
            }

            LoadScreen(nameScreen);
        }

        private void LoadScreen(string nameScreen)
        {
            Object obj = GameObject.Find(nameScreen);
            if (!obj)
            {
                Object prefab = Resources.Load("Prefabs/Managers/" + nameScreen, typeof(GameObject));
                if (!prefab)
                {
                    Debug.LogError("Resources.Load  prefab == NULL");
                    Debug.LogError("Resources name" + nameScreen);
                }
                else
                {
                    GameObject clone = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    clone.transform.position = Vector3.one;
                    clone.name = nameScreen;
                    convas = clone.GetComponent<Canvas>();
                    if (GameObject.Find("Main Camera") != null && GameObject.Find("Main Camera").GetComponent<Camera>() != null && convas != null)
                        convas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                }
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
            }
        }
    }
}
