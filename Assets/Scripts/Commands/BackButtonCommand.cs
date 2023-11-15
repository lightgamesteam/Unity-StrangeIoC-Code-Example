using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class BackButtonCommand : BaseCommand
    {
        public override void Execute()
        {
            PopupModel popupModel = new PopupModel(
             title: LocalizationKeys.NoticeKey,
             description: LocalizationKeys.AppExitTextKey,
             buttonText: LocalizationKeys.ExitKey,
             isActiveCloseButton: true,
             callback: () => { Application.Quit(); });
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false, showSwitchAnim = false });
        }
    }
}