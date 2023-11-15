using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views.Sounds;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundPlayCommand : BaseCommand
    {
        public override void Execute()
        {
            //Debug.Log("SoundPlayCommand");

            AudioClipToCommand source;

            if (EventData.data != null)
            {
                source = EventData.data as AudioClipToCommand;
            }
            else
            {
                source = null;
                Debug.LogError("No SoundPlay");
            }

            SoundManagerView soundManager = Object.FindObjectOfType<SoundManagerView>();
            if (soundManager)
            {
                if (source != null)
                {
                   soundManager.Play(source.clip, source.type, source.volume, source.loop, source.speed);
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