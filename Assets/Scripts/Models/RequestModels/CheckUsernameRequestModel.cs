using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class CheckUsernameRequestModel : BasicRequestModel
    {
        [Newtonsoft.Json.JsonProperty("loginId")]
        public string username;

        [NonSerialized] public bool isTeacher;
        [NonSerialized] public Action<bool> successAction;

        public CheckUsernameRequestModel() : base()
        {

        }

        public CheckUsernameRequestModel(string username, bool isTeacher, Action<bool> successAction, Action requstFalseAction) : base(null, requstFalseAction)
        {
            this.username = username;
            this.successAction = successAction;
            this.isTeacher = isTeacher;
            Debug.Log("isTeacher" + isTeacher);
        }
    }
}