﻿using strange.extensions.mediation.impl;

namespace PFS.Assets.Scripts.Views.Quizzes
{
	public class UIQuizQuestionType1Mediator : BaseMediator
	{
		[Inject]
		public UIQuizQuestionType1View View { get; set; }

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