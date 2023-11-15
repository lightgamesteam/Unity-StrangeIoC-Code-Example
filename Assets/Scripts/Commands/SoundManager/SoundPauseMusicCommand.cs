using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPauseMusicCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundPauseMusicCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.PauseByType(Conditions.SoundType.Music);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}