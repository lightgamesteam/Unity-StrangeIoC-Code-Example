using System.Collections.Generic;

namespace Assets.Scripts.Services.Analytics
{
    public interface IAnalyticsService
    {
        void LogEvent(EventName eventName,  Dictionary<Property, object> options);
    }
}
