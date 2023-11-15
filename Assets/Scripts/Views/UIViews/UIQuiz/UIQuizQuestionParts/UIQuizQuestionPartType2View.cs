using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartType2View : UIQuizQuestionPartBaseView, IPointerDownHandler, IPointerUpHandler
    {
        private const float MoveKoef = 260f;

        [Header("Oblects")]
        public GameObject conveyorRoad;
        public GameObject checkBox;

        [Header("Params"), Tooltip("Distance to check done")]
        public float maxDistance;
        [Tooltip("Back to start point manipulator speed")]
        public float backSpeedAnim;
        [Tooltip("Close manipulator speed")]
        public float closeSpeedAnim;

        private RectTransform rectTR;

        private float minDistance;
        private bool isCanMove = true;

        private IEnumerator dragAndDrop;
        private IEnumerator moveBack;

        private delegate TResult Method<out TResult>();

        void OnValidate()
        {
            maxDistance = Mathf.Max(maxDistance, 0);
            backSpeedAnim = Mathf.Max(backSpeedAnim, 0);
        }

        public override void LoadView()
        {
            base.LoadView();

            rectTR = GetComponent<RectTransform>();

            SetMinDistance(out minDistance);
        }

        public override void RemoveView()
        {
            base.RemoveView();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isCanMove)
            {
                StartCoroutineIEnum(ref dragAndDrop, DragProcess);
                StopCoroutineIEnum(ref moveBack);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isCanMove)
            {
                StartCoroutineIEnum(ref moveBack, MoveBack);
                StopCoroutineIEnum(ref dragAndDrop);
            }
        }

        protected override void CorrectAnswer()
        {
            base.CorrectAnswer();
            Dispatcher.Dispatch(EventGlobal.E_QuizType2VisualResult, true);

            isCanMove = false;
        }

        protected override void WrongAnswer()
        {
            base.WrongAnswer();
            Dispatcher.Dispatch(EventGlobal.E_QuizType2VisualResult, false);

            isCanMove = false;
        }

        private void SetMinDistance(out float minDistance)
        {
            minDistance = 0f;
            if (rectTR)
            {
                minDistance = rectTR.rect.height;
            }
        }

        private void StartCoroutineIEnum(ref IEnumerator coroutine, Method<IEnumerator> coroutineMethod)
        {
            StopCoroutineIEnum(ref coroutine);
            coroutine = coroutineMethod();
            StartCoroutine(coroutine);
        }

        private void StopCoroutineIEnum(ref IEnumerator coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator DragProcess()
        {
            float mousePositionY, lastMousePositionY, mousePositionYDelta;
            mousePositionY = lastMousePositionY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

            while (true)
            {
                yield return new WaitForEndOfFrame();

                mousePositionY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                mousePositionYDelta = Mathf.Abs(mousePositionY - lastMousePositionY);
                mousePositionYDelta = mousePositionY > lastMousePositionY ? mousePositionYDelta * -1 : mousePositionYDelta;
                SetPartSize(mousePositionYDelta);
                lastMousePositionY = mousePositionY;

                yield return StartCoroutine(CheckDragDone());
            }
        }

        private void SetPartSize(float distance)
        {
            float newSizeY = rectTR.sizeDelta.y + (distance * MoveKoef);
            newSizeY = Mathf.Clamp(newSizeY, minDistance, maxDistance);
            rectTR.sizeDelta = new Vector2(rectTR.sizeDelta.x, newSizeY);
        }

        private IEnumerator CheckDragDone()
        {
            if (rectTR.sizeDelta.y == maxDistance)
            {
                partContent.transform.parent.transform.SetParent(conveyorRoad.transform);

                yield return new WaitForEndOfFrame();

                var eventData = new PointerEventData(EventSystem.current);
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);

                OnPointerUp(eventData);

                Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);

                yield return StartCoroutine(WaitResult());
            }
        }

        private IEnumerator MoveBack()
        {
            while (Mathf.Abs(rectTR.sizeDelta.y - minDistance) > 0.1f)
            {
                rectTR.sizeDelta = new Vector2(rectTR.sizeDelta.x, Mathf.Lerp(rectTR.sizeDelta.y, minDistance, backSpeedAnim * Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }

            rectTR.sizeDelta = new Vector2(rectTR.sizeDelta.x, minDistance);
        }

        private IEnumerator WaitResult()
        {
            yield return new WaitUntil(() => partContent.transform.position.x >= checkBox.transform.position.x);
            SelectPart();
            partContent.transform.parent.gameObject.SetActive(false);
        }

        public override IEnumerator ShowCorrectResult()
        {
            while (rectTR.sizeDelta.y < maxDistance)
            {
                rectTR.sizeDelta = new Vector2(rectTR.sizeDelta.x, rectTR.sizeDelta.y + Time.deltaTime * 1000f);
                yield return new WaitForEndOfFrame();
            }

            rectTR.sizeDelta = new Vector2(rectTR.sizeDelta.x, maxDistance);

            yield return StartCoroutine(CheckDragDone());

            yield return new WaitForSeconds(2f);
        }
    }
}