using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class FeedbackComment
    {
        public string body;

        public FeedbackComment(string body)
        {
            this.body = body;
        }
    }

    public class SendFeedbackModel : BasicRequestModel
    {
        public string subject;
        public FeedbackComment comment;
        public int rating;

        [NonSerialized] public Action<string> failAction;

        public SendFeedbackModel(string subject, string body, int rating, Action requestTrueAction, Action<string> failAction) : base(requestTrueAction, null)
        {
            this.subject = subject;
            this.comment = new FeedbackComment(body);
            this.failAction = failAction;
            this.rating = rating;
        }
    }
}