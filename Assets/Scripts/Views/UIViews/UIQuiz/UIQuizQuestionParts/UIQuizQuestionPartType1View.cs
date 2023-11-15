using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartType1View : UIQuizQuestionPartBaseView
    {
        [Header("UI")]
        public Button partButton;
        public Image win;
        public Image lose;
        public QuizQuestionBackground backgroundImage;

        [Header("Animation Params")]
        [SerializeField, Range(0.0f, 1.0f)] private float imageMarkAnimDuration;

        private void OnValidate()
        {
            //SetLampsColor(Color.clear);
        }

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

        private void PartButtonClick()
        {
            SelectPart();
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        protected override void CorrectAnswer()
        {
            base.CorrectAnswer();

            partButton.interactable = false;
            backgroundImage.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);
            InitCheckResult(true);
        }

        protected override void WrongAnswer()
        {
            base.WrongAnswer();

            partButton.interactable = false;
            backgroundImage.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Lose);
            InitCheckResult(false);
        }

        public override IEnumerator ShowCorrectResult()
        {
            for (int i = 0; i < 3; i++)
            {
                backgroundImage.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);
                yield return new WaitForSeconds(0.5f);
                backgroundImage.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Default);
                yield return new WaitForSeconds(0.5f);
            }

            backgroundImage.SetBackImageState(QuizQuestionBackground.BackgroundImageState.Win);
            yield return new WaitForSeconds(1f);
        }

        private void InitCheckResult(bool result)
        {
            if (result)
            {
                StartImageFadeInAnimation(win);
                StartImageFadeInAnimation(win.transform.GetChild(0).GetComponent<Image>());
            }
            else
            {
                StartImageFadeInAnimation(lose);
                StartImageFadeInAnimation(lose.transform.GetChild(0).GetComponent<Image>());
            }
        }

        private void StartImageFadeInAnimation(Image image)
        {
            if (image)
            {
                image.gameObject.SetActive(true);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
                image.DOFade(1.0f, imageMarkAnimDuration);
            }
        }
    }
}