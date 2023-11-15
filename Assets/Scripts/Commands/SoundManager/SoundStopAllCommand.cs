using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundStopAllCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundStopAllCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.StopAll();
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}