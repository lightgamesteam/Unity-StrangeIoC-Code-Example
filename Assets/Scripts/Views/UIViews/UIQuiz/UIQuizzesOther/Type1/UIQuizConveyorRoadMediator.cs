﻿using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Quizzes
{
	public class UIQuizConveyorRoadMediator : BaseMediator
	{
		[Inject]
		public UIQuizConveyorRoadView View { get; set; }

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
		}
	}
}