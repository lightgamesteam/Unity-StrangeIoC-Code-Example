using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundUnPauseSoundCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundUnPauseSoundCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.UnPause(Conditions.SoundType.Sound);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}