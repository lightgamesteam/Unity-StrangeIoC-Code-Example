using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.IO;

namespace PFS.Assets.Scripts.Views.QuizStats
{
	public class UIQuizStatsMediator : BaseMediator
	{

		[Inject]
		public UIQuizStatsView View { get; set; }

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