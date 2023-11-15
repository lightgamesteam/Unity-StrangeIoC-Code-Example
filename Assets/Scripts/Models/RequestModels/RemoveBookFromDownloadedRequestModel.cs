using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class RemoveBookFromDownloadedRequestModel : BasicRequestModel
    {
        public string bookId = string.Empty;
        public string languageName = string.Empty;

        public RemoveBookFromDownloadedRequestModel() : base()
        {

        }

        public RemoveBookFromDownloadedRequestModel(string bookId, string languageName, Action successAction, Action failAction) : base(successAction, failAction)
        {
            this.bookId = bookId;
            this.languageName = languageName;
        }
    }
}