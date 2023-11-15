using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IngameDebugConsole;
namespace PFS.Assets.Scripts.Views.DebugScreen
{
    public class DebugView : BaseView
    {
        //---RESET USER----------------
        public Button resetUser;
        //------------------------------

        //---CLEAR PLAYER PREFS---------
        public Button deltePlayerPrefs;
        //------------------------------

        //---CLEAR UNUSED ASSETS--------
        public Button clearUnusedAssets;
        //------------------------------

        [Header("Switchers")]
        //---SHOW/HIDE CONSOLE----------
        public Button showHideConsole;
        public Slider showHideConsoleSlider;
        public DebugLogManager debugManager;
        private bool isConsoleShowed = false;
        //-----------------------------

        //---SHOW/HIDE FPS-------------
        public Button showHideFps;
        public Slider showHideFpsSlider;
        public GameObject fps;
        private bool isFpsShowed = true;
        //-----------------------------

        //---SHOW/HIDE LOGIN BUTTONS-------------
        public Button showHideLoginButtons;
        public Slider showHideLoginButtonsSlider;
        private bool isLoginButtonsShowed = false;
        //---------------------------------------

        //---SERVER TYPE---------------
        public TMP_Dropdown serverTypeDropdown;
        private string[] serverTypes = new string[] { "Prod",
                                                  "Test",
                                                  "Local"
    };
        //------------------------------

        //---BUILD VERSION-------------
        public TextMeshProUGUI buildVersion;
        public string version;
        //-----------------------------

        //---QUIZ QUESTIONS-------------
        public TMP_Dropdown changeQuizQuestionsDropdown;
        public static Conditions.QuizQuestionsType QuizQuestionsType { get; private set; } = Conditions.QuizQuestionsType.Visible;
        //-----------------------------


        public void LoadView()
        {

            //---RESET USER----------------
            InitResetUser();
            //-----------------------------


            //---SHOW/HIDE FPS-------------
            InitShowHideFps();
            //-----------------------------

            //---CLEAR PLAYER PREFS---------
            InitDeletePleyerPrefs();
            //------------------------------

            //---CLEAR UNUSED ASSETS--------
            InitClearUnusedAssets();
            //------------------------------

            //---SHOW/HIDE CONSOLE----------
            InitShowHideConsole();
            //-----------------------------

            //---SHOW/HIDE LOGIN BUTTONS----
            InitShowHideLoginButtons();
            //------------------------------

            //---SERVER TYPE---------------
            InitServerType();
            //------------------------------

            //---BUILD VERSION-------------
            SetBuildVersion();
            //-----------------------------

            //---QUIZ QUESTIONS------------
            SetQuizQuestionsDropDown();
            //-----------------------------
        }

        public void RemoveView()
        {

        }

        #region PanelButtonMethods

        #region ---RESET USER---------
        private void InitResetUser()
        {
            resetUser.onClick.AddListener(ResetUser);
        }
        private void ResetUser()
        {
            Debug.Log("reset user start ");
            Dispatcher.Dispatch(EventGlobal.E_ResetUser, new BasicRequestModel(
               () =>
               {
                   PlayerPrefs.DeleteAll();
                   PlayerPrefs.Save();
                   Debug.Log("User Reseted");
               },
               () =>
               {
                   Debug.Log("User Reseted - FALSe");
               }));
        }
        #endregion

        #region ---CLEAR PLAYER PREFS---------
        private void InitDeletePleyerPrefs()
        {
            deltePlayerPrefs.onClick.AddListener(DeletePlayerPrefs);
        }
        private void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefsDeleted");

        }
        #endregion

        #region ---SHOW/HIDE CONSOLE----------
        private void InitShowHideConsole()
        {
            showHideConsole.onClick.AddListener(ShowHideConsole);
            ShowHideDebugLogs(PlayerPrefsModel.DebugLogState);
            ShowHideConsole(PlayerPrefsModel.DebugLogState);
        }

        private void ShowHideConsole()
        {
            isConsoleShowed = !isConsoleShowed;

            showHideConsoleSlider.value = isConsoleShowed ? 1 : 0;
            if (isConsoleShowed)
            {
                debugManager.ShowLogWindow();
            }
            else
            {
                debugManager.HideLogWindow();
            }
            ShowHideDebugLogs(isConsoleShowed);
        }

        private void ShowHideConsole(bool show)
        {
            isConsoleShowed = show;

            showHideConsoleSlider.value = isConsoleShowed ? 1 : 0;
            if (isConsoleShowed)
            {
                debugManager.ShowLogWindow();
            }
            else
            {
                debugManager.HideLogWindow();
            }
            ShowHideDebugLogs(isConsoleShowed);
        }

        private void ShowHideDebugLogs(bool show)
        {
            Debug.unityLogger.logEnabled = true;
            PlayerPrefsModel.DebugLogState = show;
        }
        #endregion

        #region ---SHOW/HIDE FPS-------------
        private void InitShowHideFps()
        {
            showHideFps.onClick.AddListener(ShowHideFps);
            showHideFps.onClick.Invoke();
        }
        private void ShowHideFps()
        {
            isFpsShowed = !isFpsShowed;
            showHideFpsSlider.value = isFpsShowed ? 1 : 0;

            if (fps != null)
            {
                fps.SetActive(isFpsShowed);
            }
            else
            {
                Debug.LogError("Debug Screen can't find fps");
            }
        }
        #endregion


        #region ---SHOW/HIDE LOGIN BUTTONS-------------
        private void InitShowHideLoginButtons()
        {
            showHideLoginButtons.onClick.AddListener(ShowHideLoginButtons);
        }
        private void ShowHideLoginButtons()
        {
            isLoginButtonsShowed = !isLoginButtonsShowed;
            showHideLoginButtonsSlider.value = isLoginButtonsShowed ? 1 : 0;

            Dispatcher.Dispatch(EventGlobal.E_ShowHideLoginButtons, isLoginButtonsShowed);
        }
        #endregion

        #endregion

        #region ---CLEAR UNUSED ASSETS--------
        private void InitClearUnusedAssets()
        {
            clearUnusedAssets.onClick.AddListener(ClearUnusedAssets);
        }
        private void ClearUnusedAssets()
        {
            Dispatcher.Dispatch(EventGlobal.E_ClearUnusedAssetBundles);
        }
        #endregion

        #region ---SERVER TYPE--------
        private void InitServerType()
        {
            serverTypeDropdown.onValueChanged.AddListener((value) =>
            {
                SetServerType(serverTypes[value]);
            });
            SetServerTypeDropDown(PlayerPrefsModel.ServerTypeName);
        }

        private void SetServerType(string serverType)
        {
            PlayerPrefsModel.ServerTypeName = serverType;
        }

        private void SetServerTypeDropDown(string serverType)
        {
            for (int i = 0; i < serverTypes.Length; i++)
            {
                if (serverTypes[i] == serverType)
                {
                    serverTypeDropdown.value = i;
                }
            }
        }
        #endregion

        #region ---BUID VERSION--------
        private void SetBuildVersion()
        {
            buildVersion.text = version;
        }
        #endregion

        #region ---QUIZ QUESTIONS--------
        private void SetQuizQuestionsDropDown()
        {
            changeQuizQuestionsDropdown.value = (int)QuizQuestionsType;
            changeQuizQuestionsDropdown.onValueChanged.AddListener((value) =>
            {
                QuizQuestionsType = (Conditions.QuizQuestionsType)value;
            });
        }
        #endregion
    }
}