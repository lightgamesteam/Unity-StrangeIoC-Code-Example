using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class QuizRequestModel : BasicRequestModel
    {
        public string bookId;
        public string questionsType;
        public string language;
        public string homeworkId;

        public QuizRequestModel() : base()
        {
            bookId = string.Empty;
        }

        public QuizRequestModel(Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            bookId = string.Empty;
            questionsType = Conditions.QuizQuestionsType.Visible.ToDescription();
            homeworkId = string.Empty;
        }

        public QuizRequestModel(string bookId, Conditions.QuizQuestionsType questionsType, string homeworkId, string language, Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            this.bookId = bookId;
            this.questionsType = questionsType.ToDescription();
            this.homeworkId = homeworkId;
            this.language = language;
        }
    }
}