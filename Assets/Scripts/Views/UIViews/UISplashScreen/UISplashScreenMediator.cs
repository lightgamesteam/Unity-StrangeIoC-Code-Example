using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.SplashScreen
{
	public class UISplashScreenMediator : BaseMediator
	{
		[Inject] public UISplashScreenView View { get; set; }

		public override void PreRegister()
		{ }

		public override void OnRegister() => View.LoadView();

		public override void OnRemove() => View.RemoveView();

		public override void OnAppBackButton()
		{ }
	}
}