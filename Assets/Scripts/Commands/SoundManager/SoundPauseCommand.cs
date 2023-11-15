using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPauseCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundPauseCommand");

            AudioClip source;

            if (EventData.data != null)
            {
                source = EventData.data as AudioClip;
            }
            else
            {
                source = null;
                Debug.LogError("No SoundPause");
            }

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                if (source != null)
                {
                    soundManager.Pause(source);
                }
                else
                {
                    Debug.LogError("SoundManager Clip = NULL");
                }
            }
            else
            {
                Debug.LogError("SoundManager = NULL");
            }
        }
    }
}