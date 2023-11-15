using PFS.Assets.Scripts.Views;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Models.ScreenManagerModels
{
    public class ScreenManager
    {
        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public IEventDispatcher Dispatcher { get; set; }

        public List<object> screenList = new List<object>();

        public void AddScreen(object screen)
        {
            if (GetScreenModel(screen).screenName == UIScreens.UIMainMenu)
            {
                ClearScreen();
            }
            screenList.Add(screen);
        }

        private void ClearScreen()
        {
            screenList.Clear();
        }

        private void DeleteNumScreen(object screen)
        {
            screenList.Remove(screen);
        }

        private int DeleteLastScreen()
        {
            if (screenList.Count > 0)
            {
                screenList.RemoveAt(screenList.Count - 1);
            }

            return screenList.Count;
        }

        private ShowScreenModel GetScreenModel(object screen)
        {
            ShowScreenModel ssm = null;
            ssm = screen as ShowScreenModel;
            if (ssm == null)
            {
                ssm = new ShowScreenModel();
                ssm.screenName = screen.ToString();
                Debug.Log(screen.ToString());
            }

            // don't add to screen list when executed backScreen()
            ssm.isAddToScreensList = false;

            return ssm;
        }

        private void HideCurrentScreen()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, GetCurrentScreen().screenName);
        }

        private ShowScreenModel GetCurrentScreen()
        {
            if (screenList.Count > 0)
            {
                return GetScreenModel(screenList[screenList.Count - 1]);
            }

            return new ShowScreenModel();
        }

        public void BackScreen(ScreenManagerBackModel sreens)
        {
            HideCurrentScreen();

            if (IsLastScreenInList(sreens.currentScreen))
            {
                if (DeleteLastScreen() > 0)
                {
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, GetCurrentScreen());
                }
                else
                {
                    Debug.LogError("screenList.Count = 0");
                }
            }
            else
            {
                DeleteLastScreen();
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, sreens.currentScreen.screenName);
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, sreens.moveToScreen);
            }
        }

        public bool IsLastScreenInList(ShowScreenModel sreen)
        {
            if (sreen == null)
            {
                return true;
            }

            return GetCurrentScreen().screenName == sreen.screenName;
        }

        public bool IsBackToMainScreen()
        {
            if (screenList.Count > 1)
            {
                return GetScreenModel(screenList[screenList.Count - 2]).screenName == UIScreens.UIMainMenu;
            }

            return false;
        }

        public bool IsBackToSearchResultScreen()
        {
            if (screenList.Count > 1)
            {
                return GetScreenModel(screenList[screenList.Count - 2]).screenName == UIScreens.UIBooksSearchResult;
            }

            return false;
        }

        public void DeleteCurrentScreen()
        {
            HideCurrentScreen();
            DeleteLastScreen();
        }

        public void DeleteAllScreens()
        {
            List<string> screenNames = new List<string>();

            int screenCount = screenList.Count;
            for (int i = 0; i < screenCount; i++)
            {
                screenNames.Add(GetScreenModel(screenList[i]).screenName);
            }

            screenList.Clear();

            for (int i = 0; i < screenNames.Count; i++)
            {
                Debug.Log(screenNames[i]);
                Dispatcher.Dispatch(EventGlobal.E_HideScreen, screenNames[i]);
            }

        }
    }
}