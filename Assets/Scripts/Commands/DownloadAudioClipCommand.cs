using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace PFS.Assets.Scripts.Commands.Download
{
    public class DownloadAudioClipCommand : BaseCommand
    {
        private static bool cancelDownload = true;
        private DownloadAudioClipParams audioClipParams;

        public override void Execute()
        {
            Retain();
            Debug.Log("DownloadAudioClipCommand");

            if (!cancelDownload)
            {
                cancelDownload = true;
                Debug.Log("<color=green>Download audio clips enabled</color>");
            }

            if (EventData.data != null)
            {
                audioClipParams = EventData.data as DownloadAudioClipParams;
            }
            else
            {
                Debug.LogError("DownloadAudioClipCommand - No data");
                Fail();
                return;
            }

            DownloadAudio(audioClipParams.audionURL, audioClipParams.audioType, audioClipParams.finishSuccess, audioClipParams.finishFail);
        }

        private async void DownloadAudio(string audioURL, AudioType audioType, Action<AudioClip> finishSuccess, Action finishFail)
        {

            if (!cancelDownload)
            {
                return;
            }
            else
            {
                if (finishSuccess != null)
                {
                    finishSuccess(await LoadClip(audioURL, finishFail));
                }
            }
            Release();
        }
        private async Task<AudioClip> LoadClip(string url, Action finishFail)
        {
            AudioClip clip = null;
            if (!string.IsNullOrEmpty(url))
            {
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
                {
                    uwr.SendWebRequest();

                    while (!uwr.isDone)
                    {
                        await Task.Delay(5);
                    }

                    if (!string.IsNullOrEmpty(uwr.error))
                    {
                        Debug.LogError($"DownloadAudioClipCommand => LoadClip - path: {url} | error: {uwr.error}");

                        finishFail?.Invoke();

                        return null;
                    }
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
            }
            return clip;
        }
        public static void CancelAllDownloads()
        {
            cancelDownload = false;
        }
    }

    public class DownloadAudioClipParams
    {
        public string audionURL;
        public AudioType audioType;
        public Action<AudioClip> finishSuccess;
        public Action finishFail;

        public DownloadAudioClipParams(string audionURL, Action<AudioClip> finishSuccess) : this(audionURL, AudioType.MPEG, finishSuccess)
        {

        }

        public DownloadAudioClipParams(string audionURL, AudioType audioType, Action<AudioClip> finishSuccess) : this(audionURL, audioType, finishSuccess, null)
        {

        }

        public DownloadAudioClipParams(string audionURL, Action<AudioClip> finishSuccess, Action finishFail) : this(audionURL, AudioType.MPEG, finishSuccess, finishFail)
        {

        }

        public DownloadAudioClipParams(string audionURL, AudioType audioType, Action<AudioClip> finishSuccess, Action finishFail)
        {
            this.audionURL = audionURL;
            this.audioType = audioType;
            this.finishSuccess = finishSuccess;
            this.finishFail = finishFail;
        }
    }
}