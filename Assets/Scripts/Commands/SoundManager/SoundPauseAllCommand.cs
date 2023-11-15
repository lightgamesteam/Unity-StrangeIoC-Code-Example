using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPauseAllCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundPauseAllCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.PauseAll();
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}