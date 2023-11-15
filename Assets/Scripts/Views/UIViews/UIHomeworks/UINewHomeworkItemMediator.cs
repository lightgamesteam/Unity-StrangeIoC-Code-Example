using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UINewHomeworkItemMediator : BaseMediator
    {
        [Inject]
        public UINewHomeworkItemView View { get; set; }

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