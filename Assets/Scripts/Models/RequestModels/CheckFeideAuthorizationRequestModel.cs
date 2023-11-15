using System;
using Newtonsoft.Json;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class CheckFeideAuthorazitionRequestModel : BasicRequestModel
    {
        [JsonProperty("code")]
        public string authorizationCode = "";

        public CheckFeideAuthorazitionRequestModel(string authorizationCode, Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            this.authorizationCode = authorizationCode;
        }
    }
}