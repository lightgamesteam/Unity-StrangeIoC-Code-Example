using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class ReceiptRequestModel : BasicRequestModel
    {
        public string receipt;
        public string productId;

        public ReceiptRequestModel()
        {

        }
        public ReceiptRequestModel(string receipt, string productId, Action requestTrueAction, Action requstFalseAction) : base(requestTrueAction, requstFalseAction)
        {
            this.receipt = receipt;
            this.productId = productId;
        }
    }
}