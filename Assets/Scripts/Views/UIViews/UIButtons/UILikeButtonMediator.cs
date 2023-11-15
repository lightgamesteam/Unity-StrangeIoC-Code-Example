using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Buttons
{
	public class UILikeButtonMediator : BaseMediator
	{
		[Inject] public UILikeButtonView View { get; set; }

		public override void PreRegister()
		{ }

		public override void OnRegister() => View.LoadView();

		public override void OnRemove() => View.RemoveView();

		public override void OnAppBackButton()
		{ }
	}
}