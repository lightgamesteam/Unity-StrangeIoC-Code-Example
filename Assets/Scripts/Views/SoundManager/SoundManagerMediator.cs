using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Sounds
{
    public class SoundManagerMediator : BaseMediator
    {
        [Inject]
        public SoundManagerView View { get; set; }

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