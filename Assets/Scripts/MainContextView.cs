using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;

public class MainContextView : strange.extensions.context.impl.ContextView
{
    private bool isRunContext = false;
    static public bool isPauseDisable = false;

    public static MainContextView instance = null;
    public static IEventDispatcher strangeDispatcher = null;

    void Start()
    {
        GameObject.DontDestroyOnLoad(this);
        instance = this;
        isRunContext = false;
    }

    public static void DispatchStrangeEvent(object eventType)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.Dispatch(eventType);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }

    public static void DispatchStrangeEvent(object eventType, object data)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.Dispatch(eventType, data);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }

    /// Remove a previously registered observer with exactly one argument from this Dispatcher
    public static void AddListenerStrangeEvent(object evt, EventCallback callback)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.AddListener(evt, callback);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }

    public static void AddListenerStrangeEvent(object evt, EmptyCallback callback)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.AddListener(evt, callback);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }

    /// Remove a previously registered observer with exactly no arguments from this Dispatcher
    public static void RemoveListenerStrangeEvent(object evt, EmptyCallback callback)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.RemoveListener(evt, callback);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }

    /// Remove a previously registered observer with exactly no arguments from this Dispatcher
    public static void RemoveListenerStrangeEvent(object evt, EventCallback callback)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            strangeDispatcher.RemoveListener(evt, callback);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
    }


    /// Returns true if the provided observer is already registered
    public static bool HasListenerStrangeEvent(object evt, EventCallback callback)
    {
        if (strangeDispatcher == null && instance != null && instance.context != null)
        {
            if ((instance.context as MainContextInput).dispatcher != null)
            {
                strangeDispatcher = (instance.context as MainContextInput).dispatcher;
            }
        }

        if (strangeDispatcher != null)
        {
            return strangeDispatcher.HasListener(evt, callback);
        }
        else
        {
            Debug.LogError("strangeDispatcher Not Redy");
        }
        return false;
    }

    void Update()
    {
        if (!isRunContext)
        {
            isRunContext = true;
            MonoBehaviour view = this;
            if (view != null)
            {
                context = new MainContextInput(view);
                context.Start();
            }
            else
            {
                Debug.LogError("MonoBehaviour == NULL & MainContextInput == NULL! ERROR context Not Started");
            }
        }
        else
        {
            if (context != null)
            {
                (context as MainContextInput).Update();
                if (strangeDispatcher == null)
                {
                    if ((context as MainContextInput).dispatcher != null)
                    {
                        strangeDispatcher = (context as MainContextInput).dispatcher;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (context != null)
        {
            (context as MainContextInput).FixedUpdate();
        }
    }

    void LateUpdate()
    {
        if (context != null)
        {
            (context as MainContextInput).LateUpdate();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!isRunContext || instance == null || strangeDispatcher == null)
        {
            return;
        }

        if (context != null)
        {
            (context as MainContextInput).OnApplicationPause(pauseStatus);
        }

        /* string[] texts = { "Зайди в игру", "Ну Зайди в игру", "Я кому говорю Зайди в игру", "Ксеноморфы Тусят с машей",
		  "Слышыш? Харе сидеть Зайди в игру", "Зайди в игру там все круто", "Слуги взывают к тибе милорд",
		 "Зайди в игру Мы хотим больше золота", "Зайди в игру вы продумали все", "Зайди в игру не буддь скотиной" ,
		 "Ксеноморфы сели Машу", "Зайди в игру вы продумали все", "Зайди в игру не буддь скотиной" };
		 for (int a = 0; a < texts.Length; ++a)
		 {
		     PushNotificationData data = new PushNotificationData(System.DateTime.Now.AddSeconds(1 + 5 * a), texts[a], texts[a]);
		     DispatchStrangeEvent(EventGlobal.E_PushNotificationLocalSend, data);
		 }
		 */
        /*
		if(pauseStatus && !isPauseDisable)
		{
			// Delete All GameObject end restart Context
			GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
			foreach(GameObject go in allObjects)
			Destroy(go);
			Application.LoadLevel(0);
		}*/
    }

    public void OnApplicationFocus(bool focus)
    {
        if (context != null)
        {
            (context as MainContextInput).OnApplicationFocus(focus);
        }
    }

    private void OnApplicationQuit()
    {
        DispatchStrangeEvent(EventGlobal.E_ApplicationQuittedCommand);
    }
}