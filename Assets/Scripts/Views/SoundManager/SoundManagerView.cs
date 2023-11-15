using PFS.Assets.Scripts.Models.SoundManagerModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Sounds
{
    public class SoundManagerView : BaseView
    {
        [Inject]
        public SoundManagerModel SoundManager { get; set; }

        public AudioClip buttonClick;
        public AudioClip mainTheme;
        public AudioClip winEffect;
        public AudioClip loseEffect;

        private List<AudioPack> usedAudio = new List<AudioPack>();

        public void LoadView()
        {
            InitSounds();
        }

        public void RemoveView()
        {

        }

        private void InitSounds()
        {
            SoundManager.buttonClick = buttonClick;
            SoundManager.mainTheme = mainTheme;
            SoundManager.winEffect = winEffect;
            SoundManager.loseEffect = loseEffect;
        }

        public AudioPack Play(AudioClip clip, Conditions.SoundType type, float volume = 1f, bool loop = false, float speed = 1f)
        {
            if (clip == null)
            {
                return null;
            }

            AudioSource source = null;
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources != null && listSource.usedAudioSources.clip == clip)
                {
                    source = listSource.usedAudioSources;
                }
            }

            if (source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
                source.clip = clip;
            }

            source.pitch = speed;
            if (type == Conditions.SoundType.Music)
            {
                volume *= SoundManager.MusicVolumeKof;
            }
            source.volume = volume;
            source.loop = loop;
            source.spatialBlend = 0;
            if ((type == Conditions.SoundType.Music && !SoundManager.isMusic) || (type == Conditions.SoundType.Sound && !SoundManager.isSound))
            {
                source.Stop();
            }
            else
            {
                if (!source.isPlaying || type == Conditions.SoundType.Sound)
                {
                    source.Play();
                }
            }

            AudioPack ap = new AudioPack(source, type);
            if (usedAudio.FindIndex((value) => value.usedAudioSources != null && value.usedAudioSources.clip == ap.usedAudioSources.clip) < 0)
            {
                usedAudio.Add(ap);
            }

            if (!loop)
            {
                StartCoroutine(DestroyAfterPlay(ap));
            }

            return ap;
        }

        private IEnumerator DestroyAfterPlay(AudioPack source)
        {
            while (source.usedAudioSources != null && source.usedAudioSources.time < source.usedAudioSources.clip.length - 0.05f) // 0.05 is a threshold
            {
                yield return null;
            }

            if (source.usedAudioSources != null)
            {
                Destroy(source.usedAudioSources);
            }

            usedAudio.Remove(source);
        }

        public void Stop(AudioClip clip)
        {
            if (clip == null)
                return;

            AudioPack source = new AudioPack();
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources.clip == clip)
                {
                    source.usedAudioSources = listSource.usedAudioSources;
                    break;
                }
            }

            if (source.usedAudioSources != null)
            {
                source.usedAudioSources.Stop();

                usedAudio.RemoveAt(usedAudio.FindIndex((value) => value.usedAudioSources.clip == source.usedAudioSources.clip));

                Destroy(source.usedAudioSources);
            }
        }

        public void StopAll()
        {
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources != null)
                {
                    AudioSource source = null;
                    source = listSource.usedAudioSources;

                    if (source != null)
                    {
                        source.Stop();
                        Destroy(source);
                    }
                }

            }
            usedAudio.Clear(); ;
        }

        public void StopNotAll(List<AudioPack> clips)
        {
            List<AudioPack> usedAudioBuf = new List<AudioPack>();
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources != null)
                {
                    AudioSource source = null;
                    source = listSource.usedAudioSources;

                    if (source != null)
                    {
                        if (clips.Count == 0)
                        {
                            source.Stop();
                            Destroy(source);
                        }
                        else
                            for (int i = 0; i < clips.Count; i++)
                            {
                                if (clips[i].usedAudioSources != null)
                                {
                                    if (source.clip == clips[i].usedAudioSources.clip)
                                    {
                                        if (usedAudioBuf.FindIndex((value) => value.usedAudioSources.clip == listSource.usedAudioSources.clip) < 0)
                                        {
                                            usedAudioBuf.Add(listSource);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (i == clips.Count - 1)
                                        {
                                            source.Stop();
                                            Destroy(source);
                                        }
                                    }
                                }
                            }
                    }
                }

            }
            usedAudio.Clear();
            for (int i = 0; i < usedAudioBuf.Count; i++)
            {
                usedAudio.Add(usedAudioBuf[i]);
            }
        }

        public void Mute(Conditions.SoundType type)
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                if (usedAudio[i].typeAudioSources == type)
                    usedAudio[i].usedAudioSources.Stop();
            }

            if (type == Conditions.SoundType.Music)
                SoundManager.isMusic = false;
            if (type == Conditions.SoundType.Sound)
                SoundManager.isSound = false;
        }

        public void Remute(Conditions.SoundType type)
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                if (usedAudio[i].typeAudioSources == type)
                    usedAudio[i].usedAudioSources.Play();
            }

            if (type == Conditions.SoundType.Music)
                SoundManager.isMusic = true;
            if (type == Conditions.SoundType.Sound)
                SoundManager.isSound = true;
        }

        public void SetVolume(Conditions.SoundType type, float volume)
        {
            if (type == Conditions.SoundType.Music)
            {
                volume *= SoundManager.MusicVolumeKof;
            }

            for (int i = 0; i < usedAudio.Count; i++)
            {
                if (usedAudio[i].typeAudioSources == type)
                {
                    usedAudio[i].usedAudioSources.volume = volume;
                }
            }
        }

        public void Pause(AudioClip clip)
        {
            if (clip == null)
                return;

            AudioPack source = new AudioPack();
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources.clip == clip)
                {
                    source.usedAudioSources = listSource.usedAudioSources;
                    break;
                }
            }

            if (source.usedAudioSources != null)
            {
                source.usedAudioSources.Pause();
            }
        }

        public void UnPause(AudioClip clip)
        {
            if (clip == null)
                return;

            AudioPack source = new AudioPack();
            foreach (AudioPack listSource in usedAudio)
            {
                if (listSource.usedAudioSources.clip == clip)
                {
                    source.usedAudioSources = listSource.usedAudioSources;
                    break;
                }
            }

            if (source.usedAudioSources != null)
            {
                source.usedAudioSources.UnPause();
            }
        }

        public void PauseByType(Conditions.SoundType type)
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                if (usedAudio[i].typeAudioSources == type)
                {
                    usedAudio[i].usedAudioSources.Pause();
                }
            }
        }

        public void UnPause(Conditions.SoundType type)
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                if (usedAudio[i].typeAudioSources == type)
                {
                    usedAudio[i].usedAudioSources.UnPause();
                }
            }
        }

        public void PauseAll()
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                usedAudio[i].usedAudioSources.Pause();
            }
        }

        public void UnPauseAll()
        {
            for (int i = 0; i < usedAudio.Count; i++)
            {
                usedAudio[i].usedAudioSources.UnPause();
            }
        }
    }
}