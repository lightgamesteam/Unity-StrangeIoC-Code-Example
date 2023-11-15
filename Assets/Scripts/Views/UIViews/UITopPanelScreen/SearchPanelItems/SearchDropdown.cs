using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections;

namespace PFS.Assets.Scripts.Views.TopPanel
{
    public class SearchDropdown : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Image arrow;
        [SerializeField] private RectTransform panel;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button blocker;

        [Header("Animation params")]
        [SerializeField, Range(0f, 3f)] private float arrowAnimDuration;
        [Space(10)]
        [SerializeField, Range(0f, 5f)] private float panelAlphaAnimDuration;
        [SerializeField, Range(0f, 3f)] private float panelAlphaDelay;
        [SerializeField] private Vector2 panelSize;
        [SerializeField, Range(0f, 5f)] private float panelSizeAnimDuration;

        private bool isSelected;
        private CanvasGroup[] canvasGroups;
        private IEnumerator panelAnimIEnum, panelSizeAnimIEnum;
        private Canvas canvas;
        private GraphicRaycaster graphicRaycaster;

        public RectTransform GetPanel()
        {
            return panel;
        }

        public void SetPanelSize(Vector2 newDelta)
        {
            panelSize = newDelta;
        }

        private void OnValidate()
        {
            panelSize = panel.sizeDelta;
        }

        private void OnDisable()
        {
            CloseDropdownFast();
        }

        void Start()
        {
            button.onClick.AddListener(SelectDropdown);
            blocker.onClick.AddListener(CloseDropdown);
        }

        private void SelectDropdown()
        {
            if (isSelected)
            {
                CloseDropdown();
            }
            else
            {
                OpenDropdown();
            }
        }
        private void OpenDropdown()
        {
            blocker.enabled = true;
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
            canvas.sortingLayerName = "UI";
            graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

            GetCanvasGroups();

            UnityAction logicBeforeAnim = () =>
            {
                panel.gameObject.SetActive(true);
                arrow.transform.DOScaleX(1f, arrowAnimDuration);
                isSelected = true;
            };

            PanelAnim(openAnim: true, logicBeforeAnim, null);
        }

        private void CloseDropdown()
        {
            Destroy(graphicRaycaster);
            Destroy(canvas);
            blocker.enabled = false;

            GetCanvasGroups();

            UnityAction logicBeforeAnim = () =>
            {
                arrow.transform.DOScaleX(-1f, arrowAnimDuration);
                isSelected = false;
            };

            UnityAction logicAfterAnim = () =>
            {
                panel.gameObject.SetActive(false);
            };

            PanelAnim(openAnim: false, logicBeforeAnim, logicAfterAnim);
        }

        public void CloseDropdownFast()
        {
            arrow.transform.localScale = new Vector3(-1f, 1f, 1f);
            isSelected = false;
            panel.gameObject.SetActive(false);
        }

        private void GetCanvasGroups()
        {
            if (canvasGroups == null || canvasGroups.Length == 0)
            {
                canvasGroups = content.GetComponentsInChildren<CanvasGroup>();
            }
        }

        #region Panel animation
        private void PanelAnim(bool openAnim, UnityAction logicBeforeAnim, UnityAction logicAfterAnim)
        {
            logicBeforeAnim?.Invoke();

            if (panelAnimIEnum != null)
            {
                StopCoroutine(panelAnimIEnum);
            }

            if (panelSizeAnimIEnum != null)
            {
                StopCoroutine(panelSizeAnimIEnum);
            }

            panelAnimIEnum = openAnim ? OpenPanelAnimCoroutine(logicAfterAnim) : ClosePanelAnimCoroutine(logicAfterAnim);
            StartCoroutine(panelAnimIEnum);
        }

        private IEnumerator OpenPanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            foreach (var item in canvasGroups)
            {
                item.alpha = 0;
            }

            panelSizeAnimIEnum = SizePanelAnim(toSmall: false);
            StartCoroutine(panelSizeAnimIEnum);

            for (int i = 0; i < canvasGroups.Length; i++)
            {
                canvasGroups[i].DOFade(1, panelAlphaAnimDuration);

                yield return new WaitForSeconds(panelAlphaDelay);
            }

            logicAfterAnim?.Invoke();
        }

        private IEnumerator ClosePanelAnimCoroutine(UnityAction logicAfterAnim)
        {
            foreach (var item in canvasGroups)
            {
                item.alpha = 1;
            }

            for (int i = 0; i < canvasGroups.Length; i++)
            {
                canvasGroups[i].DOFade(0, panelAlphaAnimDuration);

                yield return new WaitForSeconds(panelAlphaDelay);
            }

            if (panelAlphaDelay < panelAlphaAnimDuration)
            {
                yield return new WaitForSeconds(panelAlphaAnimDuration - panelAlphaDelay);
            }

            panelSizeAnimIEnum = SizePanelAnim(toSmall: true);
            yield return StartCoroutine(panelSizeAnimIEnum);

            logicAfterAnim?.Invoke();
        }

        private IEnumerator SizePanelAnim(bool toSmall)
        {
            Vector2 panelZeroY = new Vector2(panelSize.x, 0f);

            panel.sizeDelta = toSmall ? panelSize : panelZeroY;
            panel.DOSizeDelta(toSmall ? panelZeroY : panelSize, panelSizeAnimDuration);

            yield return new WaitForSeconds(panelSizeAnimDuration);
        }
        #endregion
    }
}