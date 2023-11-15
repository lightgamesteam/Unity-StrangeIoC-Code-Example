using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundMuteMusicCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundMuteMusicCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Mute(Conditions.SoundType.Music);
                PlayerPrefsModel.IsMusic = false;
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}