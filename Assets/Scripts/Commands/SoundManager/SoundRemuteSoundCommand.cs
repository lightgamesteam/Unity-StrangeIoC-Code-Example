using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundRemuteSoundCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundRemuteSoundCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Remute(Conditions.SoundType.Sound);
                PlayerPrefsModel.isSound = true;
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}
