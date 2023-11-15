using UnityEngine;
using System.Collections;
using PFS.Assets.Scripts.Models.Quizzes;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartBaseView : BaseView
    {
        [Header("MainPart")]
        public UIQuizQuestionPartContentView partContent;

        //----------------------------------------
        [HideInInspector]
        public QuizQuestionPart quizQuestionPart;

        public virtual void LoadView()
        {

        }

        public virtual void RemoveView()
        {

        }

        public virtual void InitPart()
        {
            if (quizQuestionPart == null)
            {
                Debug.LogError("UIQuizQuestionPart - InitPart() - QuizQuestionPart - NULL");
                return;
            }

            if (partContent == null)
            {
                Debug.LogError("UIQuizQuestionPart - InitPart() - UIQuizQuestionPartContentView - NULL");
                return;
            }

            partContent.InitPart(quizQuestionPart);
        }

        protected void SelectPart()
        {
            if (quizQuestionPart == null)
            {
                Debug.LogError("UIQuizQuestionPart - SelectPart() - QuizQuestionPart - NULL");
                return;
            }

            ShowResult(quizQuestionPart.isCorrect);
            Dispatcher.Dispatch(EventGlobal.E_QuizPartChoice, quizQuestionPart.questionPartId);
        }

        private void ShowResult(bool result)
        {
            if (result)
            {
                CorrectAnswer();
            }
            else
            {
                WrongAnswer();
            }
        }

        protected virtual void CorrectAnswer()
        {
            Debug.Log("CorrectAnswer");
        }

        protected virtual void WrongAnswer()
        {
            Debug.Log("WrongAnswer");
        }

        public virtual IEnumerator ShowCorrectResult()
        {
            yield return null;
        }
    }
}