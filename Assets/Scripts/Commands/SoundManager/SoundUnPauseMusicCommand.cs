using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundUnPauseMusicCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundUnPauseMusicCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.UnPause(Conditions.SoundType.Music);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}