using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static PFS.Assets.Scripts.Views.Library.UIMovePanelView;

namespace PFS.Assets.Scripts.Views.Library
{
    public class UISwipePanelView : BaseView, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private enum DragStatus { Begin, End }
        private enum DragMode { Drag, Click }

        [SerializeField] private DragMode mode = DragMode.Drag;
        [Space(5)]
        [SerializeField, Range(1, 20)] private float distance = 10;
        [SerializeField, Range(0.1f, 1f)] private float time = 0.3f;

        private Vector2 startPoint;
        private float curretTimeDrag = 0;

        private DragStatus status = DragStatus.End;
        private ScreenState moveTo = ScreenState.Down;

        private IEnumerator timer;

        public void LoadView()
        { }

        public void RemoveView()
        { }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mode != DragMode.Drag)
            {
                return;
            }

            if (status == DragStatus.End)
            {
                status = DragStatus.Begin;

                startPoint = eventData.position;

                if (timer != null)
                {
                    StopCoroutine(timer);
                }

                curretTimeDrag = 0;
                timer = SetTimer();
                StartCoroutine(timer);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mode != DragMode.Drag)
            {
                return;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mode != DragMode.Drag)
            {
                return;
            }

            if (status == DragStatus.Begin)
            {
                status = DragStatus.End;
                StopCoroutine(timer);
                if (IsMovePanel(startPoint, eventData.position, distance, curretTimeDrag, time))
                {
                    Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanel, moveTo);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (mode != DragMode.Click)
            {
                return;
            }

            Dispatcher.Dispatch(EventGlobal.E_MoveLibraryPanelInversion);
        }

        private IEnumerator SetTimer()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                curretTimeDrag += Time.deltaTime;
            }
        }

        private bool IsMovePanel(Vector2 start, Vector2 end, float deltaMove, float time, float deltaTime)
        {
            float resMove = start.y - end.y;
            moveTo = resMove < 0 ? ScreenState.Up : ScreenState.Down;
            return Mathf.Abs(resMove) > deltaMove && time < deltaTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = new Vector3(10, 10, 1);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}