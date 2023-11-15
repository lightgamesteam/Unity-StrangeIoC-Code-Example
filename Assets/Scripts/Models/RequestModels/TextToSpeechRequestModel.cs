using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class TextToSpeechRequestModel
    {
        public string text;
        public Conditions.Languages language;
        public Action<AudioClip> successAction;
        public Action failAction;
        public float speakingSpeedChangePercents;
        public const int SlowlSpeedSpeaking = -25;
        public const int NormalSpeedChangeSpeaking = -15;

        public TextToSpeechRequestModel(string text, Conditions.Languages language, Action<AudioClip> successAction, Action failAction, float speakingSpeedChangePercents = NormalSpeedChangeSpeaking)
        {
            this.text = Regex.Replace(text, @"[_\*><]|(\s\.)|(?<=\w)\.(?=\w)", " ");
            this.text = Regex.Replace(this.text, @"\s+", " ");

            this.language = language;
            this.successAction = successAction;
            this.failAction = failAction;

            this.speakingSpeedChangePercents = speakingSpeedChangePercents;
        }
    }
}