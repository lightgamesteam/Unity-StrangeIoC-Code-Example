using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.UI;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class CheckSchoolAccessCommand : BaseCommand
    {
        public override void Execute()
        {
            Retain();
            Debug.Log("Sorry, your school does not have access to our application.Please contact your school administrator");

            PopupModel popupModel = new PopupModel(
                title: LocalizationKeys.NoticeKey,
                description: LocalizationKeys.SchoolAccessDenyKey,
                buttonText: LocalizationKeys.OkKey,
                isActiveCloseButton: false,
                callback: null);
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIUniversalPopup, data = popupModel, isAddToScreensList = false, showSwitchAnim = false });
            Release();
        }
    }
}