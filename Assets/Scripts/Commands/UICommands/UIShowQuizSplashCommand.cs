using System.Collections;
using UnityEngine;
using Conditions;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class UIShowQuizSplashCommand : BaseCommand
    {
        [Inject] public IExecutor Coroutine { get; private set; }

        public override void Execute()
        {
            Languages language = Languages.English;

            if (EventData.data is Languages)
            {
                language = (Languages)EventData.data;
            }

            Coroutine.Execute(InitSplash(language));
        }

        private IEnumerator InitSplash(Languages language)
        {
            // need wait closed native plugin
            yield return new WaitUntil(() => !MainContextInput.AppPause);

            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIQuizSplashScreen, data = language, isAddToScreensList = false });
            //need close unity book
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookAnimated);
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBookSong);
        }
    }
}