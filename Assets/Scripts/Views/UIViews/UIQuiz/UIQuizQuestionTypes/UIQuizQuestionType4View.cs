using PFS.Assets.Scripts.Models.Quizzes;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionType4View : UIQuizQuestionBaseView
    {
        [Header("Type4 params")]
        public float waitTimePerQuestion;
        public float waitTimeAfterQuestion;

        [Header("Win effect")]
        public Transform winEffectArea;
        public GameObject winEffectGM;

        [Header("Parts containers")]
        public Transform mainContainer;

        [Header("Mix params")]
        public float waitTimeShowParts;

        private List<string> results = new List<string>();

        void OnValidate()
        {
            waitTimePerQuestion = Mathf.Max(waitTimePerQuestion, 0);
            waitTimeAfterQuestion = Mathf.Max(waitTimeAfterQuestion, 0);
            waitTimeShowParts = Mathf.Max(waitTimeShowParts, 0);
        }

        public override void LoadView()
        {
            SetMainParts();
            base.LoadView();

            RotateAllParts(true, fast: true);
            StartCoroutine(InitPartsAnim());

            Dispatcher.AddListener(EventGlobal.E_QuizType4VisualResult, AnalizedSelectedParts);
            Dispatcher.AddListener(EventGlobal.E_QuizType4ClearResults, ClearResults);
            Dispatcher.AddListener(EventGlobal.E_QuizNextTry, StartMix);
        }

        public override void RemoveView()
        {
            base.RemoveView();

            Dispatcher.RemoveListener(EventGlobal.E_QuizType4VisualResult, AnalizedSelectedParts);
            Dispatcher.RemoveListener(EventGlobal.E_QuizType4ClearResults, ClearResults);
            Dispatcher.RemoveListener(EventGlobal.E_QuizNextTry, StartMix);
        }

        //protected override IEnumerator QuestionResultVisualDelays()
        //{
        //    yield return new WaitForSeconds(waitTimePerQuestion);
        //}

        protected override IEnumerator CorrectAnswerDone()
        {
            if (quizQuestion.IsQuestionWithEffects())
            {
                InitWinEffect();
            }

            yield return new WaitForSeconds(waitTimeAfterQuestion);
        }

        protected override IEnumerator ShowCorrectAnswer()
        {
            yield return StartCoroutine(ShowCorrectProcess());
        }

        private void SetMainParts()
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType4View partType4 = (UIQuizQuestionPartType4View)part;

                if (partType4 != null)
                {
                    partType4.SetMainPart();
                }
            }
        }

        private IEnumerator InitPartsAnim()
        {
            if (!string.IsNullOrEmpty(quizQuestion.narationURL))
            {
                yield return new WaitUntil(() => quizQuestion.narationClip);
                yield return new WaitForSeconds(quizQuestion.narationClip.length);
            }

            StartMix();
        }

        private void StartMix()
        {
            StartCoroutine("MixProcess");
        }

        private IEnumerator MixProcess()
        {
            yield return new WaitForEndOfFrame();

            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);
            MixParts();
            RotateAllParts(false);

            yield return new WaitForSeconds(waitTimeShowParts);

            RotateAllParts(true);
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
        }

        private void MixParts()
        {
            GameObject buffer = new GameObject("buffer");
            buffer.transform.SetParent(transform);

            while (mainContainer.childCount > 0)
            {
                mainContainer.GetChild(0).SetParent(buffer.transform);
            }

            while (buffer.transform.childCount > 0)
            {
                int partNum = Random.Range(0, buffer.transform.childCount);
                buffer.transform.GetChild(partNum).SetParent(mainContainer);
            }

            Destroy(buffer);
        }

        private void RotateAllParts(bool toBackSide, bool fast = false)
        {
            foreach (var part in questionParts)
            {
                UIQuizQuestionPartType4View partType4 = (UIQuizQuestionPartType4View)part;

                if (partType4 != null)
                {
                    if (partType4.gameObject.activeInHierarchy)
                    {
                        if (fast)
                        {
                            partType4.RotateFast(toBackSide);
                        }
                        else
                        {
                            partType4.RotatePart(toBackSide);
                        }
                    }
                    if (partType4.connectPart != null)
                    {
                        if (partType4.connectPart.gameObject.activeInHierarchy)
                        {
                            if (fast)
                            {
                                partType4.connectPart.RotateFast(toBackSide);
                            }
                            else
                            {
                                partType4.connectPart.RotatePart(toBackSide);
                            }
                        }
                    }
                }
            }
        }

        private void AnalizedSelectedParts(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizQuestionType4View | AnalizedSelectedParts => data - null");
                return;
            }

            QuizResultType4 result = e.data as QuizResultType4;

            if (result == null)
            {
                Debug.LogError("UIQuizQuestionType4View | AnalizedSelectedParts => result - null");
                return;
            }

            results.Add(result.partId);

            if (results.Count >= 2)
            {
                StartCoroutine(AnalizeProcess(result));
            }
        }

        private IEnumerator AnalizeProcess(QuizResultType4 result)
        {
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, true);
            yield return new WaitForSeconds(waitTimePerQuestion);
            Dispatcher.Dispatch(EventGlobal.E_QuizBlocker, false);
            yield return new WaitForEndOfFrame();

            if (results.Count >= 2)
            {
                if (results[0] != results[1])
                {
                    RotateAllParts(true);
                }

                if (result.action != null)
                {
                    result.action();
                }

                results.Clear();
            }
        }

        private void ClearResults()
        {
            results.Clear();
        }

        private void InitWinEffect()
        {
            if (winEffectGM && winEffectArea)
            {
                Instantiate(winEffectGM, winEffectArea);
            }
        }
    }
}