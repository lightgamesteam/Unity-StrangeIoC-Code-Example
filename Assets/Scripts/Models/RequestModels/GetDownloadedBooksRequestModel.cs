using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetDownloadedBooksRequestModel : BasicRequestModel
    {
        public int page = 0;
        public int perpage = 0;

        public GetDownloadedBooksRequestModel() : base()
        {

        }

        public GetDownloadedBooksRequestModel(int page, int perpage, Action successAction, Action failAction) : base(successAction, failAction)
        {
            this.page = page;
            this.perpage = perpage;
        }
    }
}