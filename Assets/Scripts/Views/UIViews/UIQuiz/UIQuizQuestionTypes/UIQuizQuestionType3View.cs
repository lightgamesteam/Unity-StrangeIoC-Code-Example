using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionType3View : UIQuizQuestionBaseView
    {
        [Header("Type3 params")]
        public float waitTimePerQuestion;
        public float waitTimeAfterQuestion;

        [Header("SliceObject")]
        public GameObject sliceTrail;

        [Header("Slice params")]
        public float distanceToMove = 0.1f;

        private GameObject sliceTrailInstance;
        private Vector2 lastSliceTrailPos;

        void OnValidate()
        {
            waitTimePerQuestion = Mathf.Max(waitTimePerQuestion, 0);
            waitTimeAfterQuestion = Mathf.Max(waitTimeAfterQuestion, 0);
            distanceToMove = Mathf.Max(distanceToMove, 0);
        }

        public override void LoadView()
        {
            base.LoadView();
            StartCoroutine(InitPartsAnim());
        }

        public override void RemoveView()
        {
            base.RemoveView();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Input.GetMouseButtonDown(0))
                {
                    InstantiateSliceTrail(mousePos);
                }
                if (sliceTrailInstance)
                {
                    sliceTrailInstance.transform.position = mousePos;

                    if (Mathf.Sqrt(Mathf.Pow(mousePos.x - lastSliceTrailPos.x, 2) + Mathf.Pow(mousePos.y - lastSliceTrailPos.y, 2)) > distanceToMove)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(mousePos, mousePos, 0);
                        if (hit)
                        {
                            if (hit.transform.tag == "QuizBox")
                            {
                                var part = hit.transform.GetComponent<UIQuizQuestionPartType3View>();
                                if (part)
                                {
                                    part.SliceProcess();
                                }
                            }
                        }
                    }

                    lastSliceTrailPos = mousePos;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                DestroySliceTrail();
            }
        }

        protected override IEnumerator QuestionResultVisualDelays()
        {
            yield return new WaitForSeconds(waitTimePerQuestion);
        }

        protected override IEnumerator CorrectAnswerDone()
        {
            DisablePartsJump();
            yield return new WaitForSeconds(waitTimeAfterQuestion);
        }

        protected override IEnumerator ShowCorrectAnswer()
        {
            DisablePartsJump();
            yield return StartCoroutine(ShowCorrectProcess());
        }

        private void InstantiateSliceTrail(Vector2 position)
        {
            if (sliceTrail)
            {
                sliceTrailInstance = Instantiate(sliceTrail, position, Quaternion.identity, null);
                lastSliceTrailPos = position;
            }
        }

        private void DestroySliceTrail()
        {
            if (sliceTrailInstance)
            {
                Destroy(sliceTrailInstance);
            }
        }

        private IEnumerator InitPartsAnim()
        {
            if (!string.IsNullOrEmpty(quizQuestion.narationURL))
            {
                yield return new WaitUntil(() => quizQuestion.narationClip);
                yield return new WaitForSeconds(quizQuestion.narationClip.length);
            }

            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType3View partType3 = (UIQuizQuestionPartType3View)part;
                if (partType3 != null)
                {
                    partType3.StartJumpProcess();
                }
            }
        }

        private void DisablePartsJump()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType3View partType3 = (UIQuizQuestionPartType3View)part;
                if (partType3)
                {
                    partType3.jump = false;
                }
            }
        }
    }
}