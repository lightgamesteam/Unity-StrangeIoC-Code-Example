using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundUnPauseAllCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundUnPauseAllCommand");
            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.UnPauseAll();
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}