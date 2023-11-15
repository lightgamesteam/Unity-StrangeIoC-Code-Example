using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UIFeaturedBooksItemMediator : BaseMediator
    {
        [Inject]
        public UIFeaturedBooksItemView View { get; set; }

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