using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundMuteSoundCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundMuteSoundCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Mute(Conditions.SoundType.Sound);
                PlayerPrefsModel.isSound = false;
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}