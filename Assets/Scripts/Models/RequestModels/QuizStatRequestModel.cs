using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class QuizStatRequestModel : BasicRequestModel
    {
        public int weeksAgo;
        public QuizStatRequestModel(int number, Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            this.weeksAgo = number;
        }

    }
}