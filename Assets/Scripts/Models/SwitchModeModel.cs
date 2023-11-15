namespace PFS.Assets.Scripts.Models
{
    public class SwitchModeModel
    {
        public static Conditions.GameModes Mode { get { return PlayerPrefsModel.Mode; } set { PlayerPrefsModel.Mode = value; } }
    }
}