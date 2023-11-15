using System.Collections.Generic;

namespace Assets.Scripts.Services.Analytics
{
    public class Analytics
    {
        public static Analytics Instance { get; set; }

        [Inject(AnalyticsServices.Amplitude)] public IAnalyticsService Amplitude { get; private set; }

        public Analytics()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void LogEvent(EventName eventName,  Dictionary<Property, object> options)
        {
            Amplitude.LogEvent(eventName, options);
        }
    }

    enum AnalyticsServices
    {
        Amplitude = 0
    }
}


