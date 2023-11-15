using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PFS.Assets.Scripts.Commands;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

public class OpenDebugScreenCommand : BaseCommand
{
    [Inject]
    public IExecutor CoroutineExecutor { get; private set; }

    private GameObject debugPanelInDebugScreen;
    private GameObject consolePanel;

    public override void Execute()
    {
        Retain();

        Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.DebugScreen,  isAddToScreensList=false, showSwitchAnim = false });

        debugPanelInDebugScreen = GameObject.Find("DebugPanel");
        consolePanel = GameObject.Find("DebugLogWindow");
        MainContextInput.debugScreen = debugPanelInDebugScreen;
        MainContextInput.consolePanel = consolePanel;
        debugPanelInDebugScreen.SetActive(false);
        consolePanel.SetActive(false);

        Release();
    }

}
