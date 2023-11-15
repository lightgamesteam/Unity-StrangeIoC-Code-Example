
using PFS.Assets.Scripts.Views;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class UIBlockerHideCommand : BaseCommand
    {
        public override void Execute()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UIBlockerScreen);
        }
    }
}