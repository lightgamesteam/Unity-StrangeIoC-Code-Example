using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;
using PFS.Assets.Scripts.Views.SplashScreen;
using UnityEngine.Events;

namespace PFS.Assets.Scripts.Commands.Network
{
    public class StartSchoolModeForChildUniversalCommand : BaseNetworkCommand
    {
        private Conditions.GameModes mode = Conditions.GameModes.None;
        private bool isCheckIp = true;

        public override void Execute()
        {
            Retain();

            if (EventData.data != null)
            {
                mode = (Conditions.GameModes)EventData.data;
            }

            if (mode == Conditions.GameModes.SchoolModeForChildFeide)
            {
                isCheckIp = false;
            }

            if (PlayerPrefsModel.CurrentChildToken != "")
            {
                GetChildDataRequestModel requestModel = new GetChildDataRequestModel(isCheckIp,
                () =>
                {
                    UnityAction<object> action = (object data) =>
                    {
                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UITopPanelScreen, isAddToScreensList = false, showSwitchAnim = false });
                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIMainMenu, showSwitchAnim = false });
                    };
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UISplashScreen, data = new UISplashScreenView.SplashParams { isLogIn = false, action = action }, isAddToScreensList = false, showSwitchAnim = false });
                },
                () =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UILoginScreen, showSwitchAnim = false });
                });

                Dispatcher.Dispatch(EventGlobal.E_GetChildData, requestModel);
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UILoginScreen, showSwitchAnim = false });
            }

            Release();
        }
    }
}