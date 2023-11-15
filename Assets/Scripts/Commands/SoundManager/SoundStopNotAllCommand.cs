using UnityEngine;
using System.Collections.Generic;
using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views.Sounds;

namespace PFS.Assets.Scripts.Commands.SoundManagerCommands
{
    public class SoundStopNotAllCommand : BaseCommand
    {
        public override void Execute()
        {
            Debug.Log("SoundStopNotAllCommand");

            List<AudioClipToCommand> source;

            if (EventData.data != null)
            {
                source = EventData.data as List<AudioClipToCommand>;
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
                    List<AudioPack> clips = new List<AudioPack>();
                    for (int i = 0; i < source.Count; i++)
                    {
                        clips.Add(soundManager.Play(source[i].clip, source[i].type, source[i].volume, source[i].loop, source[i].speed));
                    }
                    soundManager.StopNotAll(clips);
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