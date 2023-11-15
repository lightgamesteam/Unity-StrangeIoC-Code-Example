using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetChildByPasswordRequestModel : BasicRequestModel
    {
        [Newtonsoft.Json.JsonProperty("loginId")]
        public string login;
        public string password;

        [NonSerialized] public Action<string> failAction;

        public GetChildByPasswordRequestModel(string login, string password, Action requestTrueAction, Action<string> failAction) : base(requestTrueAction, null)
        {
            this.login = login;
            this.password = password;
            this.failAction = failAction;
        }
    }
}