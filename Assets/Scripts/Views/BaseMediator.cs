using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using System.Collections;

namespace strange.extensions.mediation.impl
{
    public class BaseMediator : Mediator
    {
        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public IEventDispatcher Dispatcher { get; set; }

        //[Inject]
        //public IExecutor courutine { get; set; }


        protected bool wasHidden = false;
        private bool wasUnregistered = false;

        public void Disable()
        {
            wasHidden = true;
            OnSleep();
            Debug.Log("Disable");

        }

        private void OnEnable()
        {

            if (wasUnregistered)
            {
               // Debug.Log("11111");
                wasUnregistered = false;
                OnRegister();
            }

            GetComponent<MonoBehaviour>().StartCoroutine(WaitDispatcher());
          
            //if (dispatcher != null)
            //{
            //    Debug.Log("12345");
            //    dispatcher.AddListener(EventGlobal.E_AppBackButton, onAppBackButton);
            //}
        }

        private IEnumerator WaitDispatcher()
        {
            while (Dispatcher == null)
            {
                yield return null;
            }
            if (Dispatcher != null)
            {
                Dispatcher.AddListener(EventGlobal.E_AppBackButton, OnAppBackButton);
            }
        }

        private void OnDisable()
        {
            //Debug.Log("OnDisable");
            if (!wasUnregistered)
            {
                wasUnregistered = true;
                OnRemove();
            }
            if (Dispatcher != null)
            {
                Dispatcher.RemoveListener(EventGlobal.E_AppBackButton, OnAppBackButton);
            }
        }

        public virtual void OnAppBackButton()
        {
            Debug.Log("onAppBackButton");

        }


        public override void OnRegister()
        {
            Debug.Log("OnRegister");
            base.OnRegister();
            wasUnregistered = false;
            Debug.Log("some");

        }

        public override void OnRemove()
        {
            Debug.Log("OnRemove");
            base.OnRemove();
            wasUnregistered = true;
        }

        protected virtual void OnEnabled()
        {
            Debug.Log("virtual on enable");
            OnRegister();
        }
        protected virtual void OnSleep()
        {
            Debug.Log("onsleep");
        }
        public void Enable()
        {
            Debug.Log("Enable");
            if (wasHidden)
            {
                OnEnabled();
                wasHidden = false;
            }
        }

    }
}