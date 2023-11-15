﻿using UnityEngine;
using strange.extensions.mediation.impl;


namespace PFS.Assets.Scripts.Views.Purchase
{
	public class UISubscriptionMediator : BaseMediator
	{

		[Inject]
		public UISubscriptionView View { get; set; }

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