namespace PFS.Assets.Scripts.Models.ScreenManagerModels
{
    public class ShowScreenModel
    {
        public string screenName = "";
        public string spetialPath = "";
        public object data = null;
        public bool isAddToScreensList = true; // add or not to screens list
        public bool showSwitchAnim = true;
    }

    public class ScreenManagerBackModel
    {
        public ShowScreenModel currentScreen = null;
        public ShowScreenModel moveToScreen = null;
    }
}