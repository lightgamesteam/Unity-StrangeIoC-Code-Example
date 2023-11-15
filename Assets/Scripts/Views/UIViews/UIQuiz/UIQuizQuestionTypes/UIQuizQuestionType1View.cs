using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionType1View : UIQuizQuestionBaseView
    {
        [Header("Type1 params")]
        public float waitTimeAfterQuestion;

        [Header("Animation")]
        [SerializeField] private Animation conveyorAnimation;

        private float clipLenght;
        private float currentSpeed = 1.0f;
        private const string conveyorAnimName = "Conveyor";

        private IEnumerator conveyorEnum;

        void OnValidate()
        {
            waitTimeAfterQuestion = Mathf.Max(waitTimeAfterQuestion, 0);
        }

        public override void LoadView()
        {
            base.LoadView();
            InitConveyor();
        }

        public override void RemoveView()
        {
            base.RemoveView();
        }

        protected override IEnumerator CorrectAnswerDone()
        {
            StopConveyor();
            yield return new WaitForSeconds(waitTimeAfterQuestion);
        }

        protected override IEnumerator ShowCorrectAnswer()
        {
            StopConveyor();
            yield return StartCoroutine(ShowCorrectProcess());
        }

        private void InitConveyor()
        {
            clipLenght = conveyorAnimation[conveyorAnimName].length;

            StopConveyor();
            conveyorAnimation.Play();

            conveyorEnum = ChangeConveyorDirection();
            StartCoroutine(conveyorEnum);
        }

        private void StopConveyor()
        {
            conveyorAnimation.Stop();

            if (conveyorEnum != null)
            {
                StopCoroutine(conveyorEnum);
            }
        }

        private IEnumerator ChangeConveyorDirection()
        {
            yield return new WaitForSeconds(clipLenght);

            currentSpeed *= -1.0f;

            conveyorAnimation[conveyorAnimName].speed = currentSpeed;
            conveyorAnimation.Play();

            conveyorEnum = ChangeConveyorDirection();
            StartCoroutine(conveyorEnum);
        }
    }
}