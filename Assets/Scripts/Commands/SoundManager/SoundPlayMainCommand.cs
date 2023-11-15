using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPlayMainCommand : BaseCommand
    {
        [Inject]
        public SoundManagerModel SoundManager { get; set; }

        public override void Execute()
        {
            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Play(SoundManager.mainTheme, Conditions.SoundType.Music, SoundManager.musicVolume, true, 1);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}