using System;
namespace PFS.Assets.Scripts.Models.Statistics
{
    public class QuizStatisticsModel
    {
        public StatisticsExample[] Week { get; set; }
        public int TimeSlice { get; set; } = -1;

        public QuizStatisticsModel()
        {

        }

        public QuizStatisticsModel(StatisticsExample[] getWeek, Action requestTrueAction, Action requstFalseAction)
        {
            this.Week = getWeek;
        }
    }

    public class StatisticsExample
    {
        public int Day { get; private set; }
        public int Value { get; private set; }
    }
}