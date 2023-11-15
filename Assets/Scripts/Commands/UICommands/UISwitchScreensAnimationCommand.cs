using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using PFS.Assets.Scripts.Views.SwitchScreen;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class UISwitchScreensAnimationCommand : BaseCommand
    {
        private string screenName = UIScreens.UISwitchScreensAnimation;

        private bool show = false;

        public override void Execute()
        {
            if (EventData.data != null)
            {
                ShowScreenModel ssm = EventData.data as ShowScreenModel;
                if (ssm != null)
                {
                    show = ssm.showSwitchAnim;
                }
            }

            if (show)
            {
                LoadScreen(screenName);
            }
        }

        private void LoadScreen(string nameScreen)
        {
            GameObject obj = GameObject.Find(nameScreen);
            if (!obj)
            {
                Object prefab = Resources.Load("Prefabs/UI/" + nameScreen, typeof(GameObject));
                if (prefab)
                {
                    GameObject clone = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    clone.transform.position = Vector3.zero;
                    clone.name = nameScreen;
                }
                else
                {
                    Debug.LogError("UISwitchScreensAnimationCommand => prefab - NULL");
                }
            }
            else
            {
                obj.GetComponent<UISwitchScreensAnimationView>()?.PlayAnimation();
            }
        }
    }
}