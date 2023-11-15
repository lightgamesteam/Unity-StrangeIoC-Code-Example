using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Login
{
    public class UILoginScreenMediator : BaseMediator
    {
        [Inject]
        public UILoginScreenView View { get; set; }

        public override void PreRegister()
        {

        }

        public override void OnRegister()
        {
            View.LoadView();
        }

        public override void OnRemove()
        {
            View.RemoveView();
        }

        public override void OnAppBackButton()
        {
            //Application.Quit();        
        }
    }
}