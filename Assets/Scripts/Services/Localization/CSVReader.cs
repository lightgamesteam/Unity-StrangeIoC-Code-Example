using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PFS.Assets.Scripts.Services.Localization
{
    public class CSVReader : MonoBehaviour
    {
        public TextAsset csvFile;
        public void Start()
        {
            string[,] grid = null;
            try
            {
                grid = SplitCsvGrid(csvFile.text);
                Debug.Log("size = " + (1 + grid.GetUpperBound(0)) + "," + (1 + grid.GetUpperBound(1)));
            }
            catch
            {
                Debug.LogError("Crash");
                Debug.Break();
            }

            if (grid != null)
            {
                try
                {
                    OutputDictionary(grid);
                }
                catch
                {
                    Debug.LogError("Crash");
                    Debug.Break();
                }
            }
            else
            {
                Debug.LogError("Parser localization ERROR");
            }
        }

        internal static Dictionary<string, Dictionary<string, string>> IncreaseLanguagePoolDictionary(Dictionary<string, Dictionary<string, string>> localization, string[,] grid)
        {
            for (int y = 2; y < grid.GetUpperBound(1); y++) //с второго индекса начинаются ключи
            {
                Dictionary<string, string> languages = new Dictionary<string, string>();
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {
                    try
                    {
                        if (grid[x, 0] != "" && x > 0)  //if not empty
                        {
                            string _key = grid[x, 1];
                            string _value = grid[x, y];
                            //  Debug.Log("languages Add:" + _key +"="+ _value);
                            languages.Add(_key, _value); //ЯЗЫК - СЛОВО
                        }
                    }
                    catch
                    {
                        Debug.LogError("Crash");
                        Debug.Break();
                    }
                }
                try
                {
                    grid[0, y] = grid[0, y].Replace("\n", "");
                    grid[0, y] = grid[0, y].Replace(" ", "");
                    grid[0, y] = grid[0, y].Replace("\t", "");
                }
                catch
                {
                    Debug.LogError("Crash");
                    Debug.Break();
                }
                //FIXIT на время редактирвоания фалйа локализации сделал обработку исключения дубликата ключей.
                string mainKey = string.Empty;
                try
                {
                    mainKey = grid[0, y];
                }
                catch
                {
                    Debug.LogError("Crash");
                    Debug.Break();
                }

                if (mainKey != null)
                {
                    mainKey = mainKey.ToLower();
                    if (mainKey != "" && localization.ContainsKey(mainKey) && languages != null)
                    {
                        foreach (var language in languages)
                        {
                            if (!localization[mainKey].ContainsKey(language.Key))
                            {
                                localization[mainKey].Add(language.Key, language.Value);
                            }
                        }
                    }
                }
            }
            Debug.Log("languages parsed");
            return localization;
        }

        // outputs the content of a 2D array, useful for checking the importer
        static public Dictionary<string, Dictionary<string, string>> OutputDictionary(string[,] grid)
        {
            Dictionary<string, Dictionary<string, string>> _localization = new Dictionary<string, Dictionary<string, string>>();
            string textOutput = "";
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                if (y > 1)  //с второго индекса начинаются ключи
                {
                    Dictionary<string, string> languages = new Dictionary<string, string>();
                    for (int x = 0; x < grid.GetUpperBound(0); x++)
                    {
                        try
                        {
                            textOutput += grid[x, y];
                            textOutput += "|";

                            if (grid[x, 0] != "" && x > 0)  //if not empty
                            {
                                string _key = grid[x, 1];
                                string _value = grid[x, y];
                                //  Debug.Log("languages Add:" + _key +"="+ _value);
                                languages.Add(_key, _value); //ЯЗЫК - СЛОВО
                            }
                        }
                        catch
                        {
                            Debug.LogError("Crash");
                            Debug.Break();
                        }
                    }
                    try
                    {
                        grid[0, y] = grid[0, y].Replace("\n", "");
                        grid[0, y] = grid[0, y].Replace(" ", "");
                        grid[0, y] = grid[0, y].Replace("\t", "");
                    }
                    catch
                    {
                        Debug.LogError("Crash");
                        Debug.Break();
                    }
                    //FIXIT на время редактирвоания фалйа локализации сделал обработку исключения дубликата ключей.
                    if (!_localization.ContainsKey(grid[0, y]))
                    {
                        string _key = "";
                        try
                        {
                            _key = grid[0, y];
                        }
                        catch
                        {
                            Debug.LogError("Crash");
                            Debug.Break();
                        }
                        if (_key != null && _key != "" && languages != null)
                        {
                            //Debug.Log("languages dic Add:" + _key);
                            _key = _key.ToLower();
                            _localization.Add(_key, languages);
                        }
                    }
                }
                textOutput += "\n";
            }

            //Debug.Log(textOutput);
            Debug.Log("languages parsed");
            return _localization;

        }

        // splits a CSV file into a 2D string array
        static public string[,] SplitCsvGrid(string csvText)
        {
            string[] lines = csvText.Split("\r"[0]);
            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]);
                width = Mathf.Max(width, row.Length);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width + 1, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Length; x++)
                {
                    outputGrid[x, y] = row[x];

                    // This line was to replace "" with " in my output.
                    // Include or edit it as you wish.
                    outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");

                    //ent
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ent/", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ ent /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ent /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ ent/", "\n");
                    //

                    //Ent
                    outputGrid[x, y] = outputGrid[x, y].Replace("/Ent/", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ Ent /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/Ent /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ Ent/", "\n");
                    //

                    //ENT
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ENT/", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ ENT /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ENT /", "\n");
                    outputGrid[x, y] = outputGrid[x, y].Replace("/ ENT/", "\n");
                    //


                    outputGrid[x, y] = outputGrid[x, y].Replace("\t", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace(";", " ");
                    //outputGrid[x, y] = outputGrid[x, y].Replace("\n", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("\0", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("=", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("\"", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("...", " ");

                    outputGrid[x, y] = outputGrid[x, y].Replace("  ", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("  ", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("  ", " ");
                    outputGrid[x, y] = outputGrid[x, y].Replace("  ", " ");

                    outputGrid[x, y] = outputGrid[x, y].Replace("{ ", "{");
                    outputGrid[x, y] = outputGrid[x, y].Replace(" }", "}");
                    outputGrid[x, y] = outputGrid[x, y].Replace("{ ", "{");
                    outputGrid[x, y] = outputGrid[x, y].Replace(" }", "}");
                    outputGrid[x, y] = outputGrid[x, y].Replace("{ ", "{");
                    outputGrid[x, y] = outputGrid[x, y].Replace(" }", "}");
                }
            }

            return outputGrid;
        }

        static public string[,] SplitCsvGrid(string csvText, string language)
        {
            bool parceOnleOneLanguage = false;
            int languageIndex = -1;
            if (string.IsNullOrEmpty(language))
            {
                return null;
            }
            string languageShort = language;
            string[] lines = csvText.Split("\r"[0]);
            string[] rowLang = SplitCsvLine(lines[1]);
            for (int i = 0; i < rowLang.Length; i++)
            {
                if (languageShort == rowLang[i])
                {
                    languageIndex = i;
                    parceOnleOneLanguage = true;
                    break;
                }
            }
            if (!parceOnleOneLanguage)
            {
                return SplitCsvGrid(csvText);

            }
            // creates new 2D string grid to output to
            string[,] outputGrid = new string[3, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]);

                outputGrid[0, y] = ParseReplace(row[0]);
                if (row.Length <= languageIndex)
                {
                    Debug.Log("Missed text by index " + languageIndex.ToString() + "and line " + y.ToString());
                }

                outputGrid[1, y] = ParseReplace(row[languageIndex]);
            }
            return outputGrid;
        }

        // splits a CSV row
        static public string[] SplitCsvLine(string lineparse)
        {
            string line = "";
            line = lineparse.Replace("\n", "");
            line = line.Replace("\0", "");
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                    @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                    System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }

        private static string GetLanguageShort(string language)
        {
            if (string.IsNullOrEmpty(language) || language.Length < 2)
            {
                return null;
            }
            string shortLanguage = char.ToString(language[0]) + char.ToString(language[1]);
            shortLanguage = shortLanguage.ToLower();
            return shortLanguage;
        }

        private static string ParseReplace(string str)
        {
            string result = str;
            result = result.Replace("\"\"", "\"");

            //ent
            result = result.Replace("/ent/", "\n");
            result = result.Replace("/ ent /", "\n");
            result = result.Replace("/ent /", "\n");
            result = result.Replace("/ ent/", "\n");
            //

            //Ent
            result = result.Replace("/Ent/", "\n");
            result = result.Replace("/ Ent /", "\n");
            result = result.Replace("/Ent /", "\n");
            result = result.Replace("/ Ent/", "\n");
            //

            //ENT
            result = result.Replace("/ENT/", "\n");
            result = result.Replace("/ ENT /", "\n");
            result = result.Replace("/ENT /", "\n");
            result = result.Replace("/ ENT/", "\n");
            //


            result = result.Replace("\t", " ");
            result = result.Replace(";", " ");
            //outputGrid[x, y] = outputGrid[x, y].Replace("\n", " ");
            result = result.Replace("\0", " ");
            result = result.Replace("=", " ");
            result = result.Replace("\"", " ");
            result = result.Replace("...", " ");

            result = result.Replace("  ", " ");
            result = result.Replace("  ", " ");
            result = result.Replace("  ", " ");
            result = result.Replace("  ", " ");

            result = result.Replace("{ ", "{");
            result = result.Replace(" }", "}");
            result = result.Replace("{ ", "{");
            result = result.Replace(" }", "}");
            result = result.Replace("{ ", "{");
            result = result.Replace(" }", "}");
            return result;
        }

        /// <summary>
        /// Получить список всех доступных языков
        /// </summary>
        /// <returns></returns>
        public static string[] GetListLanguages(string csvText)
        {
            string[] lines = csvText.Split("\r"[0]);
            string[] rowLang = SplitCsvLine(lines[1]);
            return rowLang;
        }
    }
}