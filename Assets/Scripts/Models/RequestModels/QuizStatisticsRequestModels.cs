namespace PFS.Assets.Scripts.Models.Requests
{
    public class QuizStartedRequestModel : BasicRequestModel
    {
        public string quizId;

        public QuizStartedRequestModel() : base()
        {

        }

        public QuizStartedRequestModel(string quizId) : base()
        {
            this.quizId = quizId;
        }
    }

    public class QuizQuittedRequestModel : BasicRequestModel
    {
        public string quizId;

        public QuizQuittedRequestModel() : base()
        {

        }

        public QuizQuittedRequestModel(string quizId) : base()
        {
            this.quizId = quizId;
        }
    }
}