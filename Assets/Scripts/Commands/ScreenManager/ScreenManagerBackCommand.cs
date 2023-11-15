using PFS.Assets.Scripts.Models.ScreenManagerModels;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.ScreenManagerCommands
{
    public class ScreenManagerBackCommand : BaseCommand
    {
        [Inject]
        public ScreenManager ScreenManager { get; set; }

        private ScreenManagerBackModel back = new ScreenManagerBackModel();

        public override void Execute()
        {
            Debug.Log("ScreenManagerBackCommand");

            if (EventData.data != null)
            {
                back = EventData.data as ScreenManagerBackModel;
            }

            ScreenManager.BackScreen(back);
        }
    }
}