using PFS.Assets.Scripts.Commands.Network.Authorization;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Authorization;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Views;
using PFS.Assets.Scripts.Views.SplashScreen;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PFS.Assets.Scripts.Commands.StartGame
{
    public class StartGameCommand : BaseCommand
    {
        [Inject] public DeepLinkModel DeepLinkModel { get; private set; }
        [Inject] public SoundManagerModel SoundManager { get; private set; }
        [Inject] public BooksLibrary BooksLibrary { get; private set; }
        [Inject] public IExecutor CoroutineExecutor { get; private set; }

        public override void Execute()
        {
            Retain();
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = true;
#endif

            // Disable screen dimming
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            Input.multiTouchEnabled = false;

            // Dispatcher.Dispatch(EventGlobal.E_InitManager, "PurchaseManager");
            Dispatcher.Dispatch(EventGlobal.E_InitManager, "SoundManager");
            Dispatcher.Dispatch(EventGlobal.E_InitManager, "PoolObjects");
            Dispatcher.Dispatch(EventGlobal.E_InitManager, "PurchaseManager");
            CoroutineExecutor.Execute(waitSound());


            if (!string.IsNullOrEmpty(DeepLinkModel.DeepLinkUrl))
            {
                Debug.Log("Deep Link == " + DeepLinkModel.DeepLinkUrl);
                Dispatcher.Dispatch(EventGlobal.E_GetChildDataByDeepLink, new GetChildByDeeplinkRequestModel(DeepLinkModel.token,
                () =>
                {
                    UnityAction<object> action = (object data) =>
                    {
                        SwitchModeModel.Mode = Conditions.GameModes.SchoolModeForChildDeepLink;
                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UITopPanelScreen, isAddToScreensList = false, showSwitchAnim = false });
                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIMainMenu, showSwitchAnim = false });
                    };
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UISplashScreen, data = new UISplashScreenView.SplashParams { isLogIn = false, action = action }, isAddToScreensList = false, showSwitchAnim = false });
                },
                () =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UILoginScreen, showSwitchAnim = false });
                }));
            }
            else
            {
                if (SwitchModeModel.Mode == Conditions.GameModes.None)
                {
                    Debug.Log("None Mode");
                    UnityAction<object> action = (object data) =>
                    {
                        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UILoginScreen, showSwitchAnim = false, data = data });
                    };
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UISplashScreen, data = new UISplashScreenView.SplashParams { isLogIn = true, action = action }, isAddToScreensList = false, showSwitchAnim = false });
                }
                else
                {
                    Debug.Log("Mode == " + SwitchModeModel.Mode);
                    Dispatcher.Dispatch(EventGlobal.E_StartSchoolModeForChildUniversal, SwitchModeModel.Mode);
                }
            }
            Release();
        }

        /// <summary>
        /// Wait all main processes (if necessary). Setting user sound configuration. Play Main Theme
        /// </summary>
        /// <returns></returns>
        private IEnumerator waitSound()
        {
            yield return null;
            SoundManager.isMusic = PlayerPrefsModel.IsMusic;
            SoundManager.isSound = PlayerPrefsModel.isSound;
            SoundManager.soundVolume = PlayerPrefsModel.SoundVolume;
            SoundManager.musicVolume = PlayerPrefsModel.MusicVolume;
            // Dispatcher.Dispatch(EventGlobal.E_SoundMainTheme);
        }
    }
}