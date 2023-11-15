using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Components
{
    public class FeaturedBooksScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Scroll collision")]
        public ScrollRect scroll;

        [Header("Options")]
        public float swipeThreshold;

        [HideInInspector] public bool isEnable = true;

        public delegate void ScrollDragProcess(float swipeValue);
        public event ScrollDragProcess processDrag;

        public delegate void ScrollSwipeProcess(bool isNextDirection);
        public event ScrollSwipeProcess processSwipe;

        public delegate void ScrollEndDragProcess(float endSwipeValue);
        public event ScrollEndDragProcess processDragEnd;

        private float currentDragValue;

        public void OnBeginDrag(PointerEventData eventData)
        {
            currentDragValue = 0.0f;

            scroll?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isEnable)
            {
                currentDragValue += eventData.delta.x;

                if (Mathf.Abs(currentDragValue) > swipeThreshold)
                {
                    processSwipe?.Invoke(currentDragValue < 0.0f);

                    currentDragValue = 0.0f;
                }
                else
                {
                    processDrag?.Invoke(Mathf.Clamp01(Mathf.Abs(currentDragValue) / swipeThreshold));
                }
            }

            scroll?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isEnable)
            {
                processDragEnd?.Invoke(Mathf.Clamp01(Mathf.Abs(currentDragValue) / swipeThreshold));

                currentDragValue = 0.0f;
            }

            scroll?.OnEndDrag(eventData);
        }
    }
}