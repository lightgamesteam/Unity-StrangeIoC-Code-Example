using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.IO;

namespace PFS.Assets.Scripts.Views.MyProfile
{
	public class UIMyProfileMediator : BaseMediator
	{

		[Inject]
		public UIMyProfileView View { get; set; }

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