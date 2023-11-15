using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetTeacherByPasswordRequestModel : BasicRequestModel
    {
        public string email;
        public string password;

        [NonSerialized] public Action<string> failAction;

        public GetTeacherByPasswordRequestModel(string email, string password, Action requestTrueAction, Action<string> failAction) : base(requestTrueAction, null)
        {
            this.email = email;
            this.password = password;
            this.failAction = failAction;
        }
    }
}