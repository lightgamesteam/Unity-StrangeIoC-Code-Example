using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetChildByClassCodeRequestModel : BasicRequestModel
    {
        public string classCode;
        public string firstName;

        [NonSerialized]
        public Action<string> failAction;

        public GetChildByClassCodeRequestModel(string classCode, string firstName, Action requestTrueAction, Action<string> failAction) : base(requestTrueAction, null)
        {
            this.classCode = classCode;
            this.firstName = firstName;
            this.failAction = failAction;
        }
    }
}