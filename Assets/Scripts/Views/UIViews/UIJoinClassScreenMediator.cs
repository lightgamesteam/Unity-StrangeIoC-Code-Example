using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Login
{
    public class UIJoinClassScreenMediator : BaseMediator
    {
        [Inject]
        public UIJoinClassScreenView View { get; set; }

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