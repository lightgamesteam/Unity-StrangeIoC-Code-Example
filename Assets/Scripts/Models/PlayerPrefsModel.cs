using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Models
{
    public static class PlayerPrefsModel
    {

        public static IPlayerPrefsStrategy strategy { get; set; }

        #region const names
        private const string CurrentChildTokenPref = "CurrentChildToken";
        private const string CurrentChildIdPref = "CurrentChildId";
        private const string ModePref = "Mode";
        private const string AssetBundleFolder = "AssetBundleFolder";
        private const string ServerType = "ServerType";
        private const string DebugLog = "DebugLog";
        private const string CountryCodePref = "CountryCode";
        private const string FirstRunPref = "FirstRun";
        private const string FirstTimestampPref = "FirstLoginTimestamp";

        #endregion

        #region Audio
        public static bool IsMusic
        {
            get { return strategy.GetInt("Music", 1) == 1; }
            set { strategy.SetInt("Music", value ? 1 : 0); }
        }

        public static bool isSound
        {
            get { return strategy.GetInt("Sound", 1) == 1; }
            set { strategy.SetInt("Sound", value ? 1 : 0); }
        }

        public static float MusicVolume
        {
            get { return strategy.GetFloat("MusicVolume", 0.5f); }
            set { strategy.SetFloat("MusicVolume", value); }
        }

        public static float SoundVolume
        {
            get { return strategy.GetInt("SoundVolume", 1); }
            set { strategy.SetFloat("SoundVolume", value); }
        }
        #endregion

        public static string FirstTimestamp
        {
            get { return PlayerPrefs.GetString(FirstTimestampPref, String.Empty); }
            set { PlayerPrefs.SetString(FirstTimestampPref, value); }
        }

        public static string CurrentChildToken
        {
            get { return strategy.GetString(CurrentChildTokenPref, ""); }
            set { strategy.SetString(CurrentChildTokenPref, value); }
        }

        public static string CurrentChildId
        {
            get { return strategy.GetString(CurrentChildIdPref, ""); }
            set { strategy.SetString(CurrentChildIdPref, value); }
        }

        public static Conditions.GameModes Mode
        {
            get
            {
                return strategy.GetString(ModePref, "") == "" ?
                  Conditions.GameModes.None :
                  (Conditions.GameModes)Enum.Parse(typeof(Conditions.GameModes), strategy.GetString(ModePref, ""));
            }
            set { strategy.SetString(ModePref, value.ToString()); }
        }

        public static string AssetBundleFolderName
        {
            get { return strategy.GetString(AssetBundleFolder, "prod"); }
            set { strategy.SetString(AssetBundleFolder, value); }
        }

        public static string ServerTypeName
        {
            get { return strategy.GetString(ServerType, "Prod"); }
            set { strategy.SetString(ServerType, value); }
        }

        public static string CountryCode
        {
            get { return strategy.GetString(CountryCodePref, "UK"); }
            set { strategy.SetString(CountryCodePref, value); }
        }

        public static bool DebugLogState
        {
            get { return strategy.GetInt(DebugLog, 0) == 1; }
            set { strategy.SetInt(DebugLog, value ? 1 : 0); }
        }

        public static bool FirstRun
        {
            get
            {
                if (PlayerPrefs.GetInt(FirstRunPref, 1) == 1)
                {
                    PlayerPrefs.SetInt(FirstRunPref, 0);
                    return true;
                }
                return false;
            }
        }

        public static void ClearLogoutPlayerPrefs()
        {
            PlayerPrefs.DeleteKey(CurrentChildTokenPref);
            PlayerPrefs.DeleteKey(CurrentChildIdPref);
            PlayerPrefs.DeleteKey(ModePref);
        }


        //Reads all fields with old strategy, and writes them with new strategy, then defaults to new strategy
        public static void UpdateToStrategy(IPlayerPrefsStrategy newStrategy)
        {
            FirstTimestamp = DateTime.Now.ToString();
            string cryptorKey = Cryptor.CreateMD5(FirstTimestamp);

            newStrategy.cryptorKey = cryptorKey;

            newStrategy.SetInt("Music", IsMusic ? 1 : 0);
            newStrategy.SetInt("Sound", isSound ? 1 : 0);
            newStrategy.SetFloat("MusicVolume", MusicVolume);
            newStrategy.SetFloat("SoundVolume", SoundVolume);
            newStrategy.SetString(CurrentChildTokenPref, CurrentChildToken);
            newStrategy.SetString(CurrentChildIdPref, CurrentChildId);
            newStrategy.SetString(ModePref, "");
            newStrategy.SetString(AssetBundleFolder, AssetBundleFolderName);
            newStrategy.SetString(ServerType, ServerTypeName);
            newStrategy.SetString(CountryCodePref, CountryCode);
            newStrategy.SetInt(DebugLog, DebugLogState ? 1 : 0);

            PlayerPrefsModel.strategy = newStrategy;
        }

        public static void UpdateCryptorKey()
        {
            strategy.cryptorKey = Cryptor.CreateMD5(FirstTimestamp);

        }
    }
}