using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundClickPlayCommand : BaseCommand
    {
        [Inject]
        public SoundManagerModel SoundManager { get; set; }

        public override void Execute()
        {
            // Debug.Log("SoundClickPlayCommand");

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.Play(SoundManager.buttonClick, Conditions.SoundType.Sound);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}