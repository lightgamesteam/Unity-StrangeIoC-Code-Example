using BooksPlayer;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundUpdateSoundVolumeCommand : BaseCommand
    {
        private float volume = 1;

        public override void Execute()
        {
            Debug.Log("SoundUpdateSoundVolumeCommand");

            if (EventData.data != null)
            {
                volume = (float)EventData.data;
            }
            else
            {
                Debug.LogError("No sound volume");
            }

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                soundManager.SetVolume(SoundType.Sound, volume);
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    } 
}
