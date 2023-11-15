using UnityEngine;
using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.MainMenu
{
	public class UIMainMenuMediator : BaseMediator
	{

		[Inject]
		public UIMainMenuView View { get; set; }

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
			Debug.Log("dfsdf");

		}
	}
}