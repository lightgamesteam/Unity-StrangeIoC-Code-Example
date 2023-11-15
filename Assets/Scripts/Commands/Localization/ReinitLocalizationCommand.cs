using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Localization
{
    public class ReinitLocalizationCommand : BaseCommand
    {
        public override void Execute()
        {
            SystemLanguage language = SystemLanguage.English;
            Retain();
            Debug.Log("ReinitLocalization");
            //language = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), "dfdf");
            if (EventData.data != null)
            {
                try
                {
                    language = (SystemLanguage)EventData.data;
                }
                catch (ArgumentException)
                {
                    Debug.LogError("Wrong language");
                }

                LocalizationManager.SetLanguage(language.ToString());
            }
            else
            {
                Debug.Log("Doesnt set language");
            }

            LocalizationManager.LocalizationReinit();
            Release();
        }

    }
}