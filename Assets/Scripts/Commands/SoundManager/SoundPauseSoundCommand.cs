using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPauseSoundCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundPauseSoundCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.PauseByType(Conditions.SoundType.Sound);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}