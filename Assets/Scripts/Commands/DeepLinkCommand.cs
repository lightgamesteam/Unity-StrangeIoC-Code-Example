using PFS.Assets.Scripts.Models.Authorization;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Authorization
{
    public class DeepLinkCommand : BaseCommand
    {
        [Inject] public DeepLinkModel DeepLinkModel { get; private set; }
        public override void Execute()
        {
            Retain();
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                OnDeepLinkActivated(Application.absoluteURL);

            }
            // Initialize DeepLink Manager global variable.
            else
            {
                DeepLinkModel.DeepLinkUrl = null;
                //OnDeepLinkActivated(Application.absoluteURL); //for test in editor
            }
            Release();
        }

        private void OnDeepLinkActivated(string url)
        {
            Debug.Log("<color=green> !!! DeepLinkCommand url = </color> " + Application.absoluteURL);
            //url = "pickatale://source/learningbooktique&token=CPI0qwzywwc="; //for test in editor
            DeepLinkModel.DeepLinkUrl = url;
            DeepLinkModel.token = url.Substring(url.LastIndexOf("token=") + 6);
            Debug.Log("DeepLinkModel.token " + DeepLinkModel.token);
        }
    }
}