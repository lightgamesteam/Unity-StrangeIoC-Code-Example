using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace PFS.Assets.Scripts.Commands.Download
{
    public class DownloadImageCommand : BaseCommand
    {
        private static bool cancelDownload = true;
        private DownloadImageParams imageParams;

        public override void Execute()
        {
            Retain();
            //Debug.Log("DownloadImageCommand");

            if (!cancelDownload)
            {
                cancelDownload = true;
                Debug.Log("<color=green>Download images enabled</color>");
            }

            if (EventData.data != null)
            {
                imageParams = EventData.data as DownloadImageParams;
            }
            else
            {
                Debug.LogError("DownloadImageCommand - No data");
                Fail();
                return;
            }

            DownloadImage(imageParams.imageURL, imageParams.finishSuccess, imageParams.finishFail);
        }

        private async void DownloadImage(string imageURL, Action<Sprite> finishSuccess, Action finishFail)
        {
            if (string.IsNullOrEmpty(imageURL))
            {
                Debug.LogError("DownloadImageCommand => imageURL null or empty");
                finishFail?.Invoke();
                return;
            }
            else if (!cancelDownload)
            {
                return;
            }

            Texture2D myTexture = await LoadTexture(imageURL);

            if (myTexture == null)
            {
                Debug.LogWarning("DownloadImageCommand => cant download texture from server");
                finishFail?.Invoke();
                return;
            }
            myTexture.wrapMode = TextureWrapMode.Clamp;
            myTexture.Apply();

            Sprite sprite = CreateSprite(myTexture);

            if (sprite)
            {
                finishSuccess?.Invoke(sprite);
            }
            else
            {
                Debug.LogError("DownloadImageCommand => cant create sprite");
                finishFail?.Invoke();
            }

            Release();
        }

        public static void CancelAllDownloads()
        {
            cancelDownload = false;
        }

        private Sprite CreateSprite(Texture2D spriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (spriteTexture == null)
            {
                return null;
            }
            return Sprite.Create(spriteTexture, new Rect(0f, 0f, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
        }
        private async Task<Texture2D> LoadTexture(string url)
        {
            Texture2D texture = null;
            if (!string.IsNullOrEmpty(url))
            {
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
                {
                    uwr.SendWebRequest();

                    while (!uwr.isDone)
                    {
                        await Task.Delay(5);
                    }
                    if (!string.IsNullOrEmpty(uwr.error))
                    {
                        Debug.LogError($"DownloadImageCommand => LoadTexture - url: {url} | error: {uwr.error}");
                    }
                    else
                    {
                        texture = DownloadHandlerTexture.GetContent(uwr);
                    }
                }
            }
            return texture;
        }
    }

    public class DownloadImageParams
    {
        public string imageURL;
        public Action<Sprite> finishSuccess;
        public Action finishFail;

        public DownloadImageParams(string imageURL, Action<Sprite> finishSuccess) : this(imageURL, finishSuccess, null)
        {

        }

        public DownloadImageParams(string imageURL, Action<Sprite> finishSuccess, Action finishFail)
        {
            this.imageURL = imageURL;
            this.finishSuccess = finishSuccess;
            this.finishFail = finishFail;
        }
    }
}