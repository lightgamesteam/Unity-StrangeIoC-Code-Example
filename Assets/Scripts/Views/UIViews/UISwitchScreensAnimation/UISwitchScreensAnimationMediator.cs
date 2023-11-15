using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.SwitchScreen
{
	public class UISwitchScreensAnimationMediator : BaseMediator
	{
		[Inject] public UISwitchScreensAnimationView View { get; set; }

		public override void PreRegister()
		{ }

		public override void OnRegister() => View.LoadView();

		public override void OnRemove() => View.RemoveView();

		public override void OnAppBackButton()
		{ }
	}
}