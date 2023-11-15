using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizConveyorRoadView : BaseView
    {
        [Header("UI")]
        public Transform road;
        public Transform buffer;

        [Header("Params")]
        [Range(0f, 20f)]
        public float speedConveyor = 1f;
        [Range(1f, 10000f)]
        public float sizeConveyorPart;
        public bool reverse;
        [Range(1f, 100f)]
        public float accelTime;

        private Vector3 basePos;
        private float speedKoef = 100f;

        public void LoadView()
        {
            basePos = road.localPosition;
        }

        public void RemoveView()
        {

        }

        public void InitAnim()
        {
            StopAllCoroutines();
            StartCoroutine(SetAnim());
        }

        private IEnumerator SetAnim()
        {
            while (true)
            {
                bool currentSate = reverse;
                Vector3 newPos = new Vector3(basePos.x + (currentSate ? -sizeConveyorPart : sizeConveyorPart), basePos.y, basePos.z);

                if (currentSate)
                {
                    while (road.localPosition.x > newPos.x)
                    {
                        yield return StartCoroutine(MoveAnim(currentSate, newPos));
                    }
                }
                else
                {
                    while (road.localPosition.x < newPos.x)
                    {
                        yield return StartCoroutine(MoveAnim(currentSate, newPos));
                    }
                }

                //move items
                while (road.childCount > 0)
                {
                    road.GetChild(0).SetParent(buffer);
                }

                road.localPosition = basePos;
                yield return null;

                while (buffer.childCount > 0)
                {
                    buffer.GetChild(0).SetParent(road);
                }
            }
        }

        private IEnumerator MoveAnim(bool currentSate, Vector3 newPos)
        {
            road.localPosition = Vector3.MoveTowards(road.localPosition, newPos, speedConveyor * speedKoef * Time.deltaTime);
            yield return new WaitForEndOfFrame();

            if (currentSate != reverse)
            {
                InitAnim();
            }
        }
    }
}