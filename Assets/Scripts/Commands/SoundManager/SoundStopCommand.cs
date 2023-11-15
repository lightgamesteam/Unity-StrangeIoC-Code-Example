using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundStopCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundStopCommand");

            AudioClip source;

            if (EventData.data != null)
            {
                source = EventData.data as AudioClip;
            }
            else
            {
                source = null;
                Debug.LogError("No SoundStop");
            }

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                if (source != null)
                {
                    soundManager.Stop(source);
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