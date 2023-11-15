using PFS.Assets.Scripts.Views.MyProfile;
using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIHomeworkStatisticsMediator : BaseMediator
    {
        [Inject]
        public UIHomeworkStatisticsView View { get; set; }

        public override void PreRegister()
        { }

        public override void OnRegister() => View.LoadView();

        public override void OnRemove() => View.RemoveView();

        public override void OnAppBackButton()
        { }
    }
}