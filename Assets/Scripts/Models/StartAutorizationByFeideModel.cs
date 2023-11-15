using PFS.Assets.Scripts.Views.Login;
using Vuplex.WebView;

namespace PFS.Assets.Scripts.Models.Authorization
{
    public class StartAutorizationByFeideModel
    {
        public string FeideLink { get; set; }
        public UILoginScreenView LoginScreenView { get; private set; }
        public UniWebView UniWebViewPrefab { get; private set; }
        public BaseCanvasWebViewPrefab WebView3DPrefab { get; private set; }

        public StartAutorizationByFeideModel(UILoginScreenView loginScreenView, UniWebView uniWebViewPrefab, BaseCanvasWebViewPrefab webView3DPrefab)
        {
            this.LoginScreenView = loginScreenView;
            this.UniWebViewPrefab = uniWebViewPrefab;
            this.WebView3DPrefab = webView3DPrefab;
        }
    }
}