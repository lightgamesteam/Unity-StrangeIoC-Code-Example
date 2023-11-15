using UnityEngine;

namespace PFS.Assets.Scripts.Models.SoundManagerModels
{
    public class SoundManagerModel
    {
        static public SoundManagerModel instance = null;

        public bool isMusic = true;
        public bool isSound = true;
        public float musicVolume = 1;
        public float soundVolume = 1;

        private float musicVolumeKof = 0.35f;
        public float MusicVolumeKof { get { return musicVolumeKof; } }

        public SoundManagerModel()
        {
            if (instance == null)
                instance = this;
        }

        public AudioClip buttonClick;
        public AudioClip mainTheme;
        public AudioClip winEffect;
        public AudioClip loseEffect;
    }

    public class AudioPack
    {
        public AudioSource usedAudioSources = null;
        public Conditions.SoundType typeAudioSources = Conditions.SoundType.Music;

        public AudioPack()
        {
            usedAudioSources = null;
            typeAudioSources = Conditions.SoundType.Music;
        }

        public AudioPack(AudioSource _usedAudioSources, Conditions.SoundType _typeAudioSources)
        {
            usedAudioSources = _usedAudioSources;
            typeAudioSources = _typeAudioSources;
        }
    }

    public class AudioClipToCommand
    {
        public AudioClip clip;
        public Conditions.SoundType type;
        public float volume = 1f;
        public bool loop = false;
        public float speed = 1f;

        public AudioClipToCommand()
        {
            clip = null;
            type = Conditions.SoundType.Music;
            volume = 1f;
            loop = false;
            speed = 1f;
        }

        public AudioClipToCommand(AudioClip clip, Conditions.SoundType type, float volume, bool loop, float speed)
        {
            this.clip = clip;
            this.type = type;
            this.volume = volume;
            this.loop = loop;
            this.speed = speed;
        }

        public AudioClipToCommand(AudioClip clip, Conditions.SoundType type)
        {
            this.clip = clip;
            this.type = type;
            volume = 1f;
            loop = false;
            speed = 1f;
        }
    }
}