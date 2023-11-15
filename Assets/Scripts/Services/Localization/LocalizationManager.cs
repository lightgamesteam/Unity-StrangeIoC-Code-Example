using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PFS.Assets.Scripts.Services.Localization
{
    public class LocalizationManager
    {
        public static LocalizationManager Instance { get; private set; }
        public Dictionary<string, Dictionary<string, string>> localization = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// string with all keys and languages and texts
        /// </summary>
        private string text_asset = null;

        /// <summary>
        /// Доступные языки 
        /// </summary>
        //private List<string> availableLanguages = new List<string>();

        public LocalizationManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            //LocalizationReinit(null);
        }

        /// <summary>
        /// Обновить словарь локализации
        /// </summary>
        /// <param name="localizationtemp">
        /// Сдесь будет хранится данные с CSV файла
        /// </param>
        /// <param name="language"></param>
        public void LocalizationReinit(string language = null)
        {
            Debug.Log("localization start Parse");
            text_asset = null;
            localization = new Dictionary<string, Dictionary<string, string>>();

            if (language == null)
            {
                language = GetLanguage();
            }

            Debug.Log("Language =" + language);


            text_asset = GetLocalAsset();
            Debug.Log(text_asset);

            try
            {
                //availableLanguages = new List<string>(CSVReader.GetListLanguages(text_asset));
                //availableLanguages.RemoveAt(0); //delete first element becouse it is empty
                localization = CSVReader.OutputDictionary(CSVReader.SplitCsvGrid(text_asset, language));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log("localization End Parse");


            //--------------------------------

            //if (availableLanguages == null)
            //{
            //    availableLanguages = new List<string>();
            //}

            //string[] arrLanguage = null;

            //try
            //{

            //    if (localizationtemp == null)
            //    {
            //        text_asset = GetLocalAsset();
            //        if (availableLanguages.Count == 0)
            //        {
            //            arrLanguage = CSVReader.GetListLanguages(text_asset);
            //        }
            //        localizationtemp = CSVReader.OutputDictionary(CSVReader.SplitCsvGrid(text_asset, language));
            //    }
            //    localization = localizationtemp;


            //}
            //catch
            //{

            //}

            //if (availableLanguages.Count == 0)
            //{
            //    string testKey = "testkey";
            //    if (localization.ContainsKey(testKey) && arrLanguage != null)
            //    {
            //        for (int i = 1; i < arrLanguage.Length; i++)
            //        {
            //            availableLanguages.Add(arrLanguage[i]);
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogError("Not key:" + testKey);
            //    }
            //}


        }

        public void AddLanguageToLocalizationPool(string language)
        {
            Debug.Log("localization start Parse");
            if (localization == null)
            {
                localization = new Dictionary<string, Dictionary<string, string>>();
            }

            Debug.Log("new Language =" + language);

            if (text_asset == null)
            {
                text_asset = GetLocalAsset();
            }

            Debug.Log(text_asset);

            try
            {
                //availableLanguages = new List<string>(CSVReader.GetListLanguages(text_asset));
                //availableLanguages.RemoveAt(0); //delete first element becouse it is empty
                localization = CSVReader.IncreaseLanguagePoolDictionary(localization, CSVReader.SplitCsvGrid(text_asset, language));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log("localization End Parse");
        }

        /// <summary>
        /// saved or system(if no saved language) language
        /// </summary>
        /// <returns> language </returns>
        public string GetLanguage()
        {
            if (PlayerPrefs.GetString("Language", "") == "")
            {
                SystemLanguage language = SystemLanguage.English;
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        language = SystemLanguage.English;
                        break;
                    case SystemLanguage.Norwegian:
                        language = SystemLanguage.Norwegian;
                        break;
                    case SystemLanguage.Danish:
                        language = SystemLanguage.Danish;
                        break;
                    default:
                        Debug.LogErrorFormat("LocalizationManager => GetLanguage: Application.systemLanguage - {0} don't expected. Set language to English", language);
                        language = SystemLanguage.English;
                        break;
                }
                SetLanguage(language.ToString());
                return language.ToString();
            }
            else
            {
                return PlayerPrefs.GetString("Language");
            }
        }

        /// <summary>
        /// Save language
        /// </summary>
        /// <param name="name"></param>
        public void SetLanguage(string name)
        {
            Debug.Log("I set language = " + name);
            PlayerPrefs.SetString("Language", name);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get text by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLocalizationText(string key, string language = null)
        {
            string _key = key.ToLower().Replace(" ", "");
            if (language == null)
            {
                language = GetLanguage();
            }
            else
            {
                if (!localization[_key].ContainsKey(language))
                {
                    AddLanguageToLocalizationPool(language);
                }
            }

            if (localization.ContainsKey(_key) && localization[_key].ContainsKey(language))
            {
                // {newLine} like new line
                return localization[_key][language].Replace("~", "\n");
            }
            return "Without adding text in localization Key: " + _key;
        }

        /// <summary>
        /// Get string with all keys and texts from Google doc
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IEnumerator GetGlobalAsset()
        {
            string url = "https://docs.google.com/spreadsheets/d/106Y3i_F7U1OHAR_hGVbhHXiSooJ1O_zr6Oj49c2JRzg/edit#gid=0";

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    text_asset = null;
                    Debug.Log("Download Localization ERROR " + www.error);
                }
                else
                {
                    Debug.Log("GetGlobalAsset_Noerror");
                    text_asset = www.downloadHandler.text;
                    Dictionary<string, Dictionary<string, string>> localization = CSVReader.OutputDictionary(CSVReader.SplitCsvGrid(text_asset));
                }

                string path = Application.dataPath + "/Resources/Localization/Localization.csv";
                File.WriteAllBytes(path, www.downloadHandler.data);
            }
        }

        /// <summary>
        /// Get string with all keys and texts from Resources/XML/Localization
        /// </summary>
        /// <returns></returns>
        private string GetLocalAsset()
        {
            TextAsset asset = null;
            string text = "";
            asset = Resources.Load("Localization/Localization") as TextAsset;
            if (asset != null)
            {
                text = asset.text;
            }
            else
            {
                Debug.Log("Cant read localization file");
            }
            return text;
        }

        internal string GetLocalizationText(object favoriteWordsKey)
        {
            throw new NotImplementedException();
        }
    }
}