using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionType2View : UIQuizQuestionBaseView
    {
        [Header("Type2 params")]
        public float waitTimePerQuestion;
        public float waitTimeAfterQuestion;

        [Header("Conveyor")]
        public UIQuizConveyorRoadView conveyorRoad;

        [Header("Conveyor params")]
        [Range(0f, 20f)]
        public float speedConveyor = 1f;

        [Header("Gate Images")]
        [SerializeField] private Image frontGatePart;
        [SerializeField] private Image backGatePart;

        [Header("Gate Colors")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color winColor;
        [SerializeField] private Color loseColor;

        void OnValidate()
        {
            waitTimePerQuestion = Mathf.Max(waitTimePerQuestion, 0);
            waitTimeAfterQuestion = Mathf.Max(waitTimeAfterQuestion, 0);
        }

        public override void LoadView()
        {
            base.LoadView();
            StartConveyor();

            SetGateColor(defaultColor);

            Dispatcher.AddListener(EventGlobal.E_QuizType2VisualResult, ProcessGateVisualResult);
        }

        public override void RemoveView()
        {
            base.RemoveView();

            Dispatcher.RemoveListener(EventGlobal.E_QuizType2VisualResult, ProcessGateVisualResult);
        }

        protected override IEnumerator QuestionResultVisualDelays()
        {
            yield return new WaitForSeconds(waitTimePerQuestion);

            SetGateColor(defaultColor);
        }

        protected override IEnumerator CorrectAnswerDone()
        {
            yield return new WaitForSeconds(waitTimeAfterQuestion);
        }

        protected override IEnumerator ShowCorrectAnswer()
        {
            yield return StartCoroutine(ShowCorrectProcess());
        }

        private void StartConveyor()
        {
            conveyorRoad.speedConveyor = speedConveyor;
            conveyorRoad.InitAnim();
        }

        private void ProcessGateVisualResult(IEvent e)
        {
            if (e.data == null)
            {
                Debug.LogError("UIQuizQuestionType2View | SetLampColor => data - null");
                return;
            }

            bool res = (bool)e.data;

            SetGateColor(res ? winColor : loseColor);
        }

        private void SetGateColor(Color color)
        {
            frontGatePart.color = color;
            backGatePart.color = color;
        }
    }
}