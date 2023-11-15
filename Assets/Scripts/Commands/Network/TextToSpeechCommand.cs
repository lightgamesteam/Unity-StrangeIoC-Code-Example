using Conditions;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using PFS.Assets.Scripts.Models.Requests;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class TextToSpeechCommand : BaseCommand
    {
        [Inject]
        public IExecutor CoroutineExecutor { get; private set; }

        private static string ttsRequestToken = string.Empty;
        private static float getRequestTokenTime;

        private const int Timeout = 15;
        private const float requestTokenValidTime = 595.0f; // tts request token valid lifetime is 10 minutes
        private const string SubscriptionKey = "//////////////";

        private TextToSpeechRequestModel requestModel;

        public override void Execute()
        {
            requestModel = EventData.data as TextToSpeechRequestModel;

            if (requestModel == null)
            {
                Debug.LogError("TextToSpeechCommand - no data");
                return;
            }

            if (!MainContextInput.AppPause)
            {
                CoroutineExecutor.Execute(WaitForResponseIEnum());
            }
            else
            {
                WaitForResponse();
            }
        }

        private IEnumerator WaitForResponseIEnum()
        {
            if (string.IsNullOrEmpty(ttsRequestToken) || (Time.time - getRequestTokenTime >= requestTokenValidTime))
            {
                Debug.Log("Creating new TTS token");

                UnityWebRequest tokenRequest = null;

                CreateTTSTokenRequest(ref tokenRequest);

                yield return tokenRequest.SendWebRequest();

                ResponseTtsToken(ref tokenRequest);
            }

            if (string.IsNullOrEmpty(ttsRequestToken))
            {
                yield break;
            }

            UnityWebRequest ttsRequest = null;
            CreateTtsRequest(ref ttsRequest);

            yield return ttsRequest.SendWebRequest();

            if (ttsRequest.responseCode == 401)
            {
                Debug.Log("Creating new TTS token when responseCode = 401");

                UnityWebRequest tokenRequest = null;

                CreateTTSTokenRequest(ref tokenRequest);

                yield return tokenRequest.SendWebRequest();

                ResponseTtsToken(ref tokenRequest);

                CreateTtsRequest(ref ttsRequest);

                yield return ttsRequest.SendWebRequest();
            }

            Response(ref ttsRequest);
        }

        private void WaitForResponse()
        {
            if (string.IsNullOrEmpty(ttsRequestToken) || (Time.time - getRequestTokenTime >= 600.0f))
            {
                UnityWebRequest tokenRequest = null;

                CreateTTSTokenRequest(ref tokenRequest);

                tokenRequest.SendWebRequest();

                while (!tokenRequest.isDone)
                {

                }

                ResponseTtsToken(ref tokenRequest);
            }

            if (string.IsNullOrEmpty(ttsRequestToken))
            {
                return;
            }

            UnityWebRequest ttsRequest = null;
            CreateTtsRequest(ref ttsRequest);

            ttsRequest.SendWebRequest();

            while (!ttsRequest.isDone)
            {

            }

            Response(ref ttsRequest);
        }

        private void CreateTTSTokenRequest(ref UnityWebRequest request)
        {
            string requestUrl = "https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issuetoken";

            request = new UnityWebRequest(requestUrl, "POST");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);

            if (request != null)
            {
                request.timeout = Timeout;
            }
        }

        private void CreateTtsRequest(ref UnityWebRequest request)
        {
            string requestUrl = "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1";

            string ssmlString = CreateSsmlString();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(ssmlString);

            request = new UnityWebRequest(requestUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Authorization", "Bearer " + ttsRequestToken);
            request.SetRequestHeader("Content-Type", "application/ssml+xml");
            request.SetRequestHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
            request.SetRequestHeader("User-Agent", "PFS");

            if (request != null)
            {
                request.timeout = Timeout;
            }
        }

        private void Response(ref UnityWebRequest request)
        {
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("TextToSpeechCommand - UnityWebRequest - error " + request.error + " - ttsRequestToken: " + ttsRequestToken);

                if (request.downloadHandler != null)
                {
                    Debug.LogError("TextToSpeechCommand - UnityWebRequest - Error text: " + request.downloadHandler.text);
                }

                requestModel?.failAction();
            }
            else
            {
                Debug.Log("TextToSpeechCommand - UnityWebRequest - Responce Code = " + request.responseCode);
                Debug.Log("<color=purple> Server responce = </color>" + request.downloadHandler.data);

                AudioClip clip = WavUtility.ToAudioClip(request.downloadHandler.data);
                requestModel?.successAction(clip);
            }

            request.Dispose();
        }

        private void ResponseTtsToken(ref UnityWebRequest request)
        {
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("TextToSpeechCommand - Get TTS token UnityWebRequest - error " + request.error);

                if (request.downloadHandler != null)
                {
                    Debug.LogError("TextToSpeechCommand - Get TTS token UnityWebRequest - error: " + request.downloadHandler.text);
                }

                requestModel?.failAction();

                ttsRequestToken = string.Empty;
            }
            else
            {
                getRequestTokenTime = Time.time;
                ttsRequestToken = request.downloadHandler.text;
            }

            request.Dispose();
        }

        private string CreateSsmlString()
        {
            return string.Format("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"{0}\"><voice name=\"{1}\"><prosody rate=\"{2}%\">{3}</prosody></voice></speak>",
                                            GetLanguageCode(),
                                            GetVoiceCode(),
                                            requestModel.speakingSpeedChangePercents.ToString(),
                                            requestModel.text);
        }

        private string GetLanguageCode()
        {
            string result = string.Empty;

            switch (requestModel.language)
            {
                case Languages.British:
                    result = "en-GB";
                    break;

                case Languages.Norwegian:
                    result = "nb-NO";
                    break;

                case Languages.Danish:
                    result = "da-DK";
                    break;

                case Languages.English:
                    result = "en-US";
                    break;

                case Languages.Chinese:
                    result = "zh-CN";
                    break;

                default:
                    result = "en-US";
                    break;
            }

            return result;
        }

        private string GetVoiceCode()
        {
            string result = string.Empty;

            switch (requestModel.language)
            {
                case Languages.British:
                    result = "en-GB-HazelRUS";
                    break;

                case Languages.Norwegian:
                    result = "nb-NO-HuldaRUS";
                    break;
                case Languages.NyNorsk:
                    result = "nb-NO-HuldaRUS";
                    break;

                case Languages.Danish:
                    result = "da-DK-ChristelNeural";
                    break;

                case Languages.English:
                    result = "en-US-ZiraRUS";
                    break;

                case Languages.Chinese:
                    result = "zh-CN-HuihuiRUS";
                    break;

                case Languages.KeyLanguage:
                    result = "";
                    break;

                default:
                    result = "en-US-ZiraRUS";
                    break;
            }

            return result;
        }
    }
}