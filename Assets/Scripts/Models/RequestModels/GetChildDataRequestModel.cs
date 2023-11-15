using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetChildDataRequestModel : BasicRequestModel
    {
        public bool isCheckIp;

        public GetChildDataRequestModel(bool isCheckIp, Action requestTrueAction, Action requestFalseAction) : base(requestTrueAction, requestFalseAction)
        {
            this.isCheckIp = isCheckIp;
        }
    }
}