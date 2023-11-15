using System;
using System.Collections.Generic;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetBooksByIdRequestModel : BasicRequestModel
    {
        public List<string> bookIds = new List<string>();

        public GetBooksByIdRequestModel() : base()
        {

        }

        public GetBooksByIdRequestModel(List<string> bookIds, Action successAction, Action failAction) : base(successAction, failAction)
        {
            this.bookIds = bookIds;
        }
    }
}