using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetChildClassesRequestModel : BasicRequestModel
    {
        public string search;

        [NonSerialized]
        public Action<ClassModel[]> successAction;

        public GetChildClassesRequestModel() : base()
        { }

        public GetChildClassesRequestModel(Action<ClassModel[]> successAction, Action requestFalseAction) : base(null, requestFalseAction)
        {
            this.successAction = successAction;
        }
    }

}