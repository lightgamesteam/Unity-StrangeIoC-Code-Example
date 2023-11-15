using PFS.Assets.Scripts.Models.ScreenManagerModels;

namespace PFS.Assets.Scripts.Commands.ScreenManagerCommands
{
    public class ScreenManagerDeleteCurrentScreen : BaseCommand
    {
        [Inject]
        public ScreenManager ScreenManager { get; set; }

        private ScreenManagerBackModel back = new ScreenManagerBackModel();

        public override void Execute()
        {
            ScreenManager.DeleteCurrentScreen();
        }
    }
}