using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class BasicRequestModel
    {
        [NonSerialized] public Action requestTrueAction;
        [NonSerialized] public Action requestFalseAction;

        public BasicRequestModel()
        {

        }

        public BasicRequestModel(Action requestTrueAction, Action requestFalseAction)
        {
            this.requestTrueAction = requestTrueAction;
            this.requestFalseAction = requestFalseAction;
        }
    }
}