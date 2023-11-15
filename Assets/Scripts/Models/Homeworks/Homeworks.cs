using PFS.Assets.Scripts.Models.BooksLibraryModels;
using System;
using System.Globalization;
using UnityEngine;
using Json = Newtonsoft.Json;

namespace PFS.Assets.Scripts.Models.Requests.Homeworks
{
    /// <summary>
    /// Check homeworks request to server
    /// </summary>
    public class CheckHomeworksRequestModel : BasicRequestModel
    {
        [NonSerialized]
        public new Action<int> requestTrueAction;

        public CheckHomeworksRequestModel() : base()
        {

        }
    }

    /// <summary>
    /// Check homeworks response from server
    /// </summary>
    public class CheckHomeworksResponceModel
    {
        public int newWorks;

        public CheckHomeworksResponceModel(int newWorks)
        {
            this.newWorks = newWorks;
        }
    }

    /// <summary>
    /// Get homeworks by status request to server
    /// </summary>
    public class HomeworksByStatusRequstModel : BasicRequestModel
    {
        [Json.JsonProperty("status")]
        public string workStatus;
        [Json.JsonProperty("perpage")]
        public int countHomework;

        [NonSerialized]
        public new Action<Homework[]> requestTrueAction;

        public HomeworksByStatusRequstModel(Conditions.HomeworkStatus status,int setCountHomework, Action<Homework[]> requestTrueAction, Action requstFalseAction) : base()
        {
            workStatus = status.ToDescription();
            countHomework = setCountHomework;
            this.requestTrueAction = requestTrueAction;
            base.requestFalseAction = requstFalseAction;
        }
    }

    public class HomeworksByStatusResponceModel
    {
        [Json.JsonProperty("homeworks")]
        public Homework[] homeworks;

        public void SortHomeworks()
        {
            Array.Sort<Homework>(homeworks, new Comparison<Homework>((item1, item2) => item1.endHomeworkDateTime.CompareTo(item2.endHomeworkDateTime)));
        }
    }

    public class HomeworksStatsRequestModel : BasicRequestModel
    {
        [NonSerialized]
        public new Action<HomeworksStatsModel> requestTrueAction;

        public HomeworksStatsRequestModel(Action<HomeworksStatsModel> requestTrueAction, Action requstFalseAction) : base()
        {
            this.requestTrueAction = requestTrueAction;
            base.requestFalseAction = requstFalseAction;
        }
    }

    public class HomeworksStatsModel
    {
        [Json.JsonProperty("countDoneHomeworks")]
        public int homeworksDone = 0;

        [Json.JsonProperty("timeSpend")]
        public int totalSpentTime = 0;

        [Json.JsonProperty("countStars")]
        public ReceivedStarsModel[] receivedStarsInfos;

        [NonSerialized]
        public int[] receivedStarsCounts = new int[5];

        public class ReceivedStarsModel
        {
            public int count;
            public int sumTimeSpend;
            public int mark;
        }

        public void SetReceivedStars()
        {
            foreach (var starsInfo in receivedStarsInfos)
            {
                receivedStarsCounts[starsInfo.mark - 1] = starsInfo.count;
            }
        }
    }

    /// <summary>
    /// Short homework info
    /// </summary>
    public class Homework
    {
        [Json.JsonProperty("id")]
        public string workId;

        [Json.JsonProperty("status")]
        public string workStatus;

        [Json.JsonProperty("dateStartHomework")]
        public string beginHomeworkDate;

        [Json.JsonProperty("dateDeadline")]
        public string endHomeworkDate;

        [Json.JsonProperty("mark")]
        public int markForWork;

        [Json.JsonProperty("bookInfo")]
        public BookModel homeworkBook;

        [Json.JsonProperty("language")]
        public string language;

        [Json.JsonProperty("spentTime")]
        public int spentTime;

        [Json.JsonProperty("readOnly")]
        public bool readOnly;

        [Json.JsonProperty("isWithTesting")]
        public bool withQuiz;

        [NonSerialized]
        public Conditions.HomeworkStatus statusEnum;

        [NonSerialized]
        public Conditions.Languages languageEnum;

        [NonSerialized]
        public DateTime beginHomeworkDateTime;

        [NonSerialized]
        public DateTime endHomeworkDateTime;

        public void SetStatusEnum()
        {
            statusEnum = SetEnum<Conditions.HomeworkStatus>(workStatus);
        }

        public void SetLanguageEnum()
        {
            languageEnum = SetEnum<Conditions.Languages>(language);
        }

        private T SetEnum<T>(string value) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));

            foreach (var item in values)
            {
                if (item is Conditions.Languages)
                {
                    if (((Conditions.Languages)item).ToDescription() == value)
                    {
                        return (T)item;
                    }
                }
                else if (item is Conditions.HomeworkStatus)
                {
                    if (((Conditions.HomeworkStatus)item).ToDescription() == value)
                    {
                        return (T)item;
                    }
                }
            }

            return default(T);
        }

        public void ParseDateTimes()
        {
            ParseDateTime(beginHomeworkDate, out beginHomeworkDateTime);
            ParseDateTime(endHomeworkDate, out endHomeworkDateTime);
        }

        private void ParseDateTime(string date, out DateTime dateTime)
        {
            if (!DateTime.TryParse(date, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateTime))
            {
                Debug.LogError("HomeworkDateTime - parse error");
            }
        }
    }
}