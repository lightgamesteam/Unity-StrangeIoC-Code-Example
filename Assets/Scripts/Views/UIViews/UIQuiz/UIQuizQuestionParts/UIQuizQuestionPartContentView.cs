using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Quizzes;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartContentView : BaseView
    {
        [Inject]
        public PoolModel Pool { get; set; }

        [Header("UI")]
        public TextMeshProUGUI partTextInfo;
        public Image partImage;

        public enum QuizQuestionPartState { Text, Image, Full }

        public void LoadView()
        {
            partImage.color = Color.clear;
        }

        public void RemoveView()
        {

        }

        public void InitPart(QuizQuestionPart quizQuestionPart)
        {
            if (quizQuestionPart == null)
            {
                Debug.LogError("UIQuizQuestionPartView - InitPart() - QuizQuestionPart - NULL");
                return;
            }

            bool isText = !string.IsNullOrEmpty(quizQuestionPart.partInfo),
                 isImage = !string.IsNullOrEmpty(quizQuestionPart.imageURL);

            if (isText)
            {
                SetPartText(quizQuestionPart.partInfo);
            }

            if (isImage)
            {
                if (quizQuestionPart.partSprite != null)
                {
                    partImage.sprite = quizQuestionPart.partSprite;
                }
                else
                {
                    LoadPartImage(partImage, quizQuestionPart);
                }
            }

            SetPartState(GetPartState(isText, isImage));
        }

        private void SetPartText(string text)
        {
            if (partTextInfo)
            {
                partTextInfo.text = text;
            }
        }

        private void LoadPartImage(Image image, QuizQuestionPart quizQuestionPart)
        {
            QuizHelper.GetQuestionPartImage(quizQuestionPart.imageURL, (Sprite sprite) =>
            {
                SetPartImage(image, quizQuestionPart, sprite);
            },
            () =>
            {
                SetPartImage(image, quizQuestionPart, Pool.QuizPartDefault);
            });
        }

        private void SetPartImage(Image image, QuizQuestionPart quizQuestionPart, Sprite sprite)
        {
            if (quizQuestionPart != null && image != null)
            {
                quizQuestionPart.partSprite = sprite;
                image.color = Color.white;
                image.sprite = sprite;
            }
        }

        private QuizQuestionPartState GetPartState(bool isText, bool isImage)
        {
            QuizQuestionPartState result = QuizQuestionPartState.Full;

            if (isText && isImage)
            {
                result = QuizQuestionPartState.Full;
            }
            else if (isText)
            {
                result = QuizQuestionPartState.Text;
            }
            else if (isImage)
            {
                result = QuizQuestionPartState.Image;
            }

            return result;
        }

        private void SetPartState(QuizQuestionPartState partState)
        {
            if (partTextInfo)
            {
                partTextInfo.gameObject.SetActive(partState != QuizQuestionPartState.Image);
            }

            partImage.gameObject.SetActive(partState != QuizQuestionPartState.Text);
        }
    }
}