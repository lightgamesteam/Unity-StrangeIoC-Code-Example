using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.BooksGrid
{
	public class UIBooksGridLoadingMediator : BaseMediator
	{
		[Inject] public UIBooksGridLoadingView View { get; set; }

		public override void PreRegister()
		{ }

		public override void OnRegister() => View.LoadView();

		public override void OnRemove() => View.RemoveView();

		public override void OnAppBackButton()
		{ }
	}
}