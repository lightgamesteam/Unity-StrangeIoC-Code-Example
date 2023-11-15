using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Buttons
{
	public class UIDeleteButtonMediator : BaseMediator
	{
		[Inject] public UIDeleteButtonView View { get; set; }

		public override void PreRegister()
		{ }

		public override void OnRegister() => View.LoadView();

		public override void OnRemove() => View.RemoveView();

		public override void OnAppBackButton()
		{ }
	}
}