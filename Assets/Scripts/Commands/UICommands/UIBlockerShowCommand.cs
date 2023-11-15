using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class UIBlockerShowCommand : BaseCommand
    {
        public override void Execute()
        {
            Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel { screenName = UIScreens.UIBlockerScreen, isAddToScreensList = false, showSwitchAnim = false });
        }
    }
}