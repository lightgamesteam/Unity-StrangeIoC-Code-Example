using Conditions;
using PFS.Assets.Scripts.Services.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Models
{
    public static class LanguagesModel
    {
        public static Languages DefaultLanguage;
        public static List<Languages> AdditionalLanguage = new List<Languages>();


        private static Languages selectedLanguage = Languages.None;
        public static Languages SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }
            set
            {
                if (value == DefaultLanguage || AdditionalLanguage.Contains(value))
                {
                    selectedLanguage = value;
                    LocalizationManager.Instance.SetLanguage(value.ToString());
                    MainContextView.DispatchStrangeEvent(EventGlobal.E_Reinitlocalization);
                }
                else
                {
                    Debug.LogErrorFormat("LanguagesModel => SelectedLanguage: Not expected language - {0}", value);
                }
            }
        }

        public static List<Languages> allowedTranslation = new List<Languages>();
    }
}