using PFS.Assets.Scripts.Models.Quizzes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartType4View : UIQuizQuestionPartBaseView
    {
        [Header("UI")]
        public Button partButton;
        public Transform body;
        public GameObject frontSide;
        public GameObject backSide;

        [Header("Connect part")]
        public UIQuizQuestionPartType4View connectPart;

        [Header("Params")]
        [Range(0, 20)]
        public float rotateSpeed;

        public bool MainPart { get; private set; }

        public bool Selected { get; private set; }

        private IEnumerator rotateIEnum;

        public override void LoadView()
        {
            base.LoadView();
            partButton.onClick.AddListener(PartButtonClick);
        }

        public override void RemoveView()
        {
            base.RemoveView();
            partButton.onClick.RemoveListener(PartButtonClick);
        }

        public override void InitPart()
        {
            if (MainPart)
            {
                base.InitPart();
                if (connectPart != null)
                {
                    connectPart.quizQuestionPart = quizQuestionPart;
                    connectPart.InitPart();
                }
            }
            else
            {
                Action action = () => base.InitPart();
                StartCoroutine(WaitPartContentImage(action));
            }
        }

        private IEnumerator WaitPartContentImage(Action action)
        {
            if (!string.IsNullOrEmpty(quizQuestionPart.imageURL))
            {
                yield return new WaitUntil(() => quizQuestionPart.partSprite != null);
            }

            partContent.partImage.color = Color.white;

            if (action != null)
            {
                action();
            }
        }

        protected override void CorrectAnswer()
        {
            base.CorrectAnswer();

            partButton.interactable = false;
        }

        protected override void WrongAnswer()
        {
            base.WrongAnswer();

            partButton.interactable = false;

            gameObject.SetActive(false);
            if (connectPart != null)
            {
                connectPart.gameObject.SetActive(false);
            }
        }

        public override IEnumerator ShowCorrectResult()
        {
            RotatePart();
            if (connectPart != null)
            {
                connectPart.RotatePart();
            }

            yield return new WaitForSeconds(3f);
        }

        public void SetMainPart()
        {
            MainPart = true;

            if (connectPart != null)
            {
                connectPart.SetOtherPart();
            }
        }

        public void SetOtherPart()
        {
            MainPart = false;
        }

        private void PartButtonClick()
        {
            RotatePart();

            if (Selected)
            {
                Action actionMethod = null;
                if (IsConnectPartSelected())
                {
                    actionMethod = () => SelectPart();
                }

                QuizResultType4 result = new QuizResultType4 { partId = quizQuestionPart.questionPartId, action = actionMethod };

                Dispatcher.Dispatch(EventGlobal.E_QuizType4VisualResult, result);
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_QuizType4ClearResults);
            }

            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        public void RotatePart(bool? toBackSide = null)
        {
            bool isBackSide = body.localRotation.eulerAngles.y == 180f;

            if (toBackSide != null)
            {
                if (toBackSide != isBackSide)
                {
                    isBackSide = (bool)toBackSide;
                }
                else
                {
                    return;
                }
            }
            else
            {
                isBackSide = !isBackSide;
            }

            Selected = !isBackSide;

            if (rotateIEnum != null)
            {
                StopCoroutine(rotateIEnum);
            }
            rotateIEnum = RotateProcess(isBackSide ? 180f : 0f);
            StartCoroutine(rotateIEnum);
        }

        private IEnumerator RotateProcess(float newRotationY)
        {
            partButton.interactable = false;

            float koef = newRotationY > Mathf.Abs(body.localRotation.eulerAngles.y) ? 1f : -1f;
            float lastRotationY = body.localRotation.eulerAngles.y;
            float applyiedRotation = 0.0f;

            while (Mathf.Abs(body.localRotation.eulerAngles.y - newRotationY) > Mathf.Abs(body.localRotation.eulerAngles.y - lastRotationY) * 1.2f) // 1.2f - increase by 20%
            {
                applyiedRotation = koef * rotateSpeed * 100f * Time.deltaTime;
                lastRotationY = body.localRotation.eulerAngles.y;

                body.Rotate(new Vector3(body.localRotation.eulerAngles.x, applyiedRotation, body.localRotation.eulerAngles.z), Space.Self);

                ProcessSidesOrderChanging(lastRotationY, lastRotationY + applyiedRotation);

                yield return new WaitForEndOfFrame();
            }

            body.localRotation = Quaternion.Euler(body.localRotation.eulerAngles.x, newRotationY, body.localRotation.eulerAngles.z);

            partButton.interactable = true;
        }

        public void RotateFast(bool toBackSide)
        {
            Debug.Log("ROTATE FAST");

            Selected = false;

            if (toBackSide)
            {
                backSide.transform.SetAsLastSibling();
            }
            else
            {
                frontSide.transform.SetAsLastSibling();
            }

            body.localRotation = Quaternion.Euler(body.localRotation.eulerAngles.x, toBackSide ? 180f : 0, body.localRotation.eulerAngles.z);
        }

        private bool IsConnectPartSelected()
        {
            if (connectPart != null)
            {
                return connectPart.Selected;
            }

            return false;
        }

        private void ProcessSidesOrderChanging(float lastRotationY, float newRotationY)
        {
            lastRotationY = Mathf.Clamp(lastRotationY, 0.0f, 180.0f);
            newRotationY = Mathf.Clamp(newRotationY, 0.0f, 180.0f);

            if (lastRotationY >= 90.0f && newRotationY < 90.0f)
            {
                frontSide.transform.SetAsLastSibling();
            }
            else if (lastRotationY <= 90.0f && newRotationY > 90.0f)
            {
                Debug.Log("Last rotation: " + lastRotationY + " Current rotation: " + body.localRotation.eulerAngles.y);

                backSide.transform.SetAsLastSibling();
            }
        }
    }
}