using System;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class DownloadedOpenedBookRequestModel : BasicRequestModel
    {
        public string bookId;
        public string languageName;
        public string homeworkId;

        [NonSerialized]
        public bool waitResponse = true;

        public DownloadedOpenedBookRequestModel() : base()
        {

        }

        public DownloadedOpenedBookRequestModel(string bookId, string languageName, string homeworkId, Action requestTrueAction, Action requstFalseAction, bool waitResponse) : base(requestTrueAction, requstFalseAction)
        {
            this.bookId = bookId;
            this.languageName = languageName;
            this.homeworkId = homeworkId;
            this.waitResponse = waitResponse;
        }
    }

    public class FlipPageBookRequestModel : BasicRequestModel
    {
        public string bookId;
        public int pageNo;
        public int pagewords;
        public int totalPages;
        public int timeOnPage;
        public string homeworkId;

        [NonSerialized]
        public bool waitResponse = true;

        public FlipPageBookRequestModel() : base()
        {

        }

        public FlipPageBookRequestModel(string bookId, int pageNo, int pagewords, int totalPages, int timeOnPage, string homeworkId, Action requestTrueAction, Action requstFalseAction, bool waitResponse) : base(requestTrueAction, requstFalseAction)
        {
            this.bookId = bookId;
            this.pageNo = pageNo;
            this.pagewords = pagewords;
            this.totalPages = totalPages;
            this.waitResponse = waitResponse;
            this.timeOnPage = timeOnPage;
            this.homeworkId = homeworkId;
        }
    }

    public class WordClickBookRequestModel : BasicRequestModel
    {
        public string bookId;
        public int pageNo;
        public string word;
        public string homeworkId;

        [NonSerialized]
        public bool waitResponse = true;

        public WordClickBookRequestModel() : base()
        {

        }

        public WordClickBookRequestModel(string bookId, int pageNo, string word, string homeworkId, Action requestTrueAction, Action requstFalseAction, bool waitResponse) : base(requestTrueAction, requstFalseAction)
        {
            this.bookId = bookId;
            this.pageNo = pageNo;
            this.word = word;
            this.waitResponse = waitResponse;
            this.homeworkId = homeworkId;
        }
    }

    public class FinishedBookRequestModel : BasicRequestModel
    {
        public string bookId;
        public string homeworkId;
        public string language;

        [NonSerialized]
        public bool waitResponse = true;

        public FinishedBookRequestModel() : base()
        {

        }

        public FinishedBookRequestModel(string bookId, string homeworkId, string language, Action requestTrueAction, Action requstFalseAction, bool waitResponse = true) : base(requestTrueAction, requstFalseAction)
        {
            this.bookId = bookId;
            this.homeworkId = homeworkId;
            this.language = language;
            this.waitResponse = waitResponse;
        }
    }
}