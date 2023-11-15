using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundRemuteMusicCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundRemuteMusicCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Remute(Conditions.SoundType.Music);
                PlayerPrefsModel.IsMusic = true;
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}