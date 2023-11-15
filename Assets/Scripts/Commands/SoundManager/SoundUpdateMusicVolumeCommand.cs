using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundUpdateMusicVolumeCommand : BaseCommand
    {
        [Inject]
        public SoundManagerModel SoundManager { get; set; }

        private float volume = 1;

        private static float musicVolumeKofSteps = 0.17f;
        public static float MusicVolumeKofSteps { get { return musicVolumeKofSteps; } }

        public override void Execute()
        {

            if (EventData.data != null)
            {
                volume = (float)EventData.data;
            }
            else
            {
                Debug.LogError("No music volume");
            }

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.SetVolume(Conditions.SoundType.Music, volume);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}