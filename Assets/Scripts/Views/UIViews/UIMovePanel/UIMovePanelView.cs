using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UIMovePanelView : BaseView
    {
        [Inject] public Analytics Analytics { get; private set; }

        public enum ScreenState { Down, Up }

        [Header("UI")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private RectTransform upPoint;

        [Header("Additional UI")]
        [SerializeField] private Button upPanelButtonGM;
        [SerializeField] private CanvasGroup upPanelButtonCanvasGroup;
        [SerializeField] private GameObject swipeArea;

        [Header("Panel move params")]
        [SerializeField, Range(0.1f, 10f)] private float panelSpeed;
        [SerializeField, Range(-100f, 100f)] private float finishPointDelta;

        [Header("Ataptive params")]
        [SerializeField, Range(-100f, 100f), Tooltip("4:3")] private float topSizeTablet;
        [SerializeField, Range(-100f, 100f), Tooltip("16:9")] private float topSizePhone;

        private float screenAspectRatio1 = 16f / 9f;    //max screen
        private float screenAspectRatio2 = 4f / 3f;     //min screen

        private float adaptiveUpPointYPos;

        public ScreenState LastState { get; private set; } = ScreenState.Down;
        private float downPoint;
        private IEnumerator panelMove;
        private IEnumerator upPanelButtonAlphaIEnum;

        public void LoadView()
        {
            SetPanelDownPoint();
            InitAdditionalElements(LastState);
            SetAdditionalUpPointPosition();

            upPanelButtonGM.onClick.AddListener(() =>
            {
                HidePanel();
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            });

            Dispatcher.AddListener(EventGlobal.E_MoveLibraryPanel, InitMovePanel);
            Dispatcher.AddListener(EventGlobal.E_MoveLibraryPanelInversion, InitMovePanelInversion);
            Dispatcher.AddListener(EventGlobal.E_ArrowKeyDown, MovePanelByArrows);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_MoveLibraryPanel, InitMovePanel);
            Dispatcher.RemoveListener(EventGlobal.E_MoveLibraryPanelInversion, InitMovePanelInversion);
            Dispatcher.RemoveListener(EventGlobal.E_ArrowKeyDown, MovePanelByArrows);
        }

        private void SetPanelDownPoint()
        {
            downPoint = panel.sizeDelta.y - finishPointDelta;
        }

        private void InitMovePanel(IEvent e)
        {
            ScreenState res = (ScreenState)e.data;

            if (res != LastState)
            {
                SetMovePanel(LastState == ScreenState.Down ? ScreenState.Up : ScreenState.Down);
            }
        }

        private void InitMovePanelInversion()
        {
            Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanel, LastState == ScreenState.Down ? ScreenState.Up : ScreenState.Down);
        }

        private void SetMovePanel(ScreenState state)
        {
            if (panelMove != null)
            {
                StopCoroutine(panelMove);
            }

            Analytics.LogEvent(EventName.NavigationBookInventory,
            new System.Collections.Generic.Dictionary<Property, object>()
            {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });

            panelMove = MovePanel(state);
            StartCoroutine(panelMove);

            EndScrollingParams(state);
        }

        private IEnumerator MovePanel(ScreenState state)
        {
            float pointY = state == ScreenState.Up ? upPoint.anchoredPosition.y + finishPointDelta : downPoint;

            while (Mathf.Abs(panel.sizeDelta.y - pointY) > finishPointDelta)
            {
                panel.sizeDelta = new Vector2(panel.sizeDelta.x, Mathf.Lerp(panel.sizeDelta.y, pointY, Time.deltaTime * panelSpeed));
                yield return new WaitForEndOfFrame();
            }

            panel.sizeDelta = new Vector2(panel.sizeDelta.x, state == ScreenState.Up ? upPoint.anchoredPosition.y : downPoint + finishPointDelta);
        }

        private void EndScrollingParams(ScreenState state)
        {
            LastState = state;
            scroll.enabled = state != ScreenState.Down;
            scroll.verticalNormalizedPosition = 1f;

            InitAdditionalElements(state);
        }

        private void MovePanelByArrows(IEvent e)
        {
            KeyCode keyCode = (KeyCode)e.data;

            if (keyCode == KeyCode.UpArrow)
            {
                Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanel, ScreenState.Up);
            }
            else if (keyCode == KeyCode.DownArrow)
            {
                Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanel, ScreenState.Down);
            }
        }

        private void HidePanel()
        {
            Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanel, ScreenState.Down);
        }

        private void InitAdditionalElements(ScreenState state)
        {
            upPanelButtonCanvasGroup.interactable = state == ScreenState.Up;
            swipeArea.SetActive(state == ScreenState.Down);

            if (upPanelButtonAlphaIEnum != null)
            {
                StopCoroutine(upPanelButtonAlphaIEnum);
            }

            upPanelButtonAlphaIEnum = DownPanelButtonAlpha(state);
            StartCoroutine(upPanelButtonAlphaIEnum);
        }

        private IEnumerator DownPanelButtonAlpha(ScreenState state)
        {
            float alpha = state == ScreenState.Up ? 1 : 0;

            while (Mathf.Abs(upPanelButtonCanvasGroup.alpha - alpha) > 0.01f)
            {
                upPanelButtonCanvasGroup.alpha = Mathf.Lerp(upPanelButtonCanvasGroup.alpha, alpha, Time.deltaTime * panelSpeed * 1.2f);
                yield return new WaitForEndOfFrame();
            }

            upPanelButtonCanvasGroup.alpha = alpha;
        }

        public Transform GetContentTransform() => scroll.content;

        public ScrollRect GetScroll() => scroll;

        #region Up point
        private void SetAdditionalUpPointPosition()
        {
            float topTablet, topPhone;
            float realScreeenResolutionDivision = (float)Screen.width / Screen.height;
            GetCoef(topSizeTablet, topSizePhone, out topTablet, out topPhone);

            adaptiveUpPointYPos = (float)Math.Truncate((decimal)(topTablet * realScreeenResolutionDivision + topPhone));

            upPoint.anchoredPosition = new Vector2(upPoint.anchoredPosition.x, upPoint.anchoredPosition.y + adaptiveUpPointYPos);
        }

        private void GetCoef(float x, float y, out float a, out float b)
        {
            a = (y - x) / (screenAspectRatio1 - screenAspectRatio2);
            b = y - screenAspectRatio1 * a;
        }
        #endregion
    }
}