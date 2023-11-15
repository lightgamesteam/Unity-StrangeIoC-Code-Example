using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIDetailedHomeworkMediator : BaseMediator
    {
        [Inject]
        public UIDetailedHomeworkView View { get; set; }

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