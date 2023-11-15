using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.DownloadedBooks
{
    public class UIDownloadedBooksMediator : BaseMediator
    {
        [Inject]
        public UIDownloadedBooksView View { get; set; }

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
        }
    }
}