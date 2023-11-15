using UnityEngine;
using strange.extensions.context.api;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Views;

public class MainContextInput : MainContextRoot
{    
    public static GameObject debugScreen;
    public static GameObject consolePanel;

    private static bool appPause;
    public static bool AppPause { get { return appPause; } }
    private const int tapCountToOpenDebugPanel = 10;
    private int tapCount = 0;
    private float timer = 0f;
    private bool showDebugPanel = false;

    public MainContextInput(MonoBehaviour contextView) : base(contextView)
    {
    }

    public override IContext Start()
    {
        IContext c = base.Start();
        return c;
    }

    public void OnApplicationPause(bool pause)
    {
        appPause = pause;
        if (pause)
        {
            Debug.Log("pause on");
        }
        else
        {
            Debug.Log("pause off");
        }
    }

    public void OnApplicationFocus(bool focus)
    {

    }

    public void Update()
    {
        if (dispatcher != null)
        {
            UpdateInput();
            dispatcher.Dispatch(EventGlobal.E_AppUpdate, Time.deltaTime);
        }
        else
        {
            Debug.LogError("Update ERROR!!! dispatcher == null");
        }
    }

    public void FixedUpdate()
    {
        if (dispatcher != null)
        {
            dispatcher.Dispatch(EventGlobal.E_AppFixedUpdate, Time.fixedDeltaTime);
        }
        else
        {
            Debug.LogError("FixedUpdate ERROR!!! dispatcher == null");
        }
    }

    public void LateUpdate()
    {
        if (dispatcher != null)
        {
            dispatcher.Dispatch(EventGlobal.E_AppLateUpdate, Time.deltaTime);
        }
        else
        {
            Debug.LogError("LateUpdate ERROR!!! dispatcher == null");
        }
    }


    public void UpdateInput()
    {
        ProcessInputEvents();

#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Time.timeScale = 7;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Time.timeScale = 1;
            }
        }
#endif

#if DEVELOP
        if (tapCount > 0)
        {
            timer += Time.unscaledDeltaTime;
        }

        if (timer > 0.3f)
        {
            tapCount = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            tapCount++;
            timer = 0;
            if (tapCount == tapCountToOpenDebugPanel)
            {
                showDebugPanel = !showDebugPanel;
#if DEVELOP
                if (showDebugPanel)
                {
                    debugScreen.SetActive(true);
                    consolePanel.SetActive(true);
                }
                else
                {
                    debugScreen.SetActive(false);
                    consolePanel.SetActive(false);
                }
#endif
            }
        }
#endif

    }

    private void ProcessInputEvents()
    {
        if (Input.GetKey(KeyCode.Escape) || ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.W)))
        {
            Debug.Log("Escape Key EVENT");
            dispatcher.Dispatch(EventGlobal.E_AppBackButton);
        }

        ProcessArrowsEvents();
        ProcessKeysEvents();
    }

    private void ProcessKeysEvents()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            dispatcher.Dispatch(EventGlobal.E_TabKeyDown);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            dispatcher.Dispatch(EventGlobal.E_EnterKeyDown);
        }
    }

    private void ProcessArrowsEvents()
    {
        KeyCode keyCode = KeyCode.None;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            keyCode = KeyCode.LeftArrow;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            keyCode = KeyCode.RightArrow;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            keyCode = KeyCode.UpArrow;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            keyCode = KeyCode.DownArrow;
        }

        if (keyCode != KeyCode.None)
        {
            MainContextView.DispatchStrangeEvent(EventGlobal.E_ArrowKeyDown, keyCode);
        }
    }
}