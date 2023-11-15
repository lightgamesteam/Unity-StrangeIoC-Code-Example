using PFS.Assets.Scripts.Models.ScreenManagerModels;

namespace PFS.Assets.Scripts.Commands.ScreenManagerCommands
{
    public class ScreenManagerDeleteAllScreen : BaseCommand
    {
        [Inject]
        public ScreenManager ScreenManager { get; set; }

        private ScreenManagerBackModel back = new ScreenManagerBackModel();

        public override void Execute()
        {
            ScreenManager.DeleteAllScreens();
        }
    }
}