
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services.Analytics
{
    public class AmplitudeAnalytics : IAnalyticsService
    {
        private Amplitude amplitude;

        public AmplitudeAnalytics()
        {
            amplitude = GetAmplitude();
        }

        public void LogEvent(EventName eventName, Dictionary<Property, object> options)
        {
            WriteToAmplitude(eventName.ToDescription(), options);
        }

        private Amplitude GetAmplitude()
        {
            Amplitude amplitude = null;
            try
            {
                // Get the a instance for a project. 
                amplitude = Amplitude.getInstance("313232");

                // Whether to enable automatic session events.
                amplitude.trackSessionEvents(true);

                // Whether to use advertising Id for device Id.
                amplitude.useAdvertisingIdForDeviceId();

                // Initialization.
                amplitude.init("a900f7c9023042c7689989a8f8d6d73e");//<AMPLITUDE_API_KEY>
            }
            catch (Exception ex)
            {
                Debug.LogError("Amplitude -> GetInstance -> Got Exception: " + ex.Message);
                Debug.LogError("Amplitude -> GetInstance -> Got ExceptionFull: " + ex);
            }

            return amplitude;
        }

        private void WriteToAmplitude(string eventName, Dictionary<Property, object> options)
        {
            ///JSONObject eventProperties = newJ SONObject().put("key", "value");
            //JSONObject groups = new JSONObject().put("orgId", 10);

            Dictionary<string, object> dictForAmplitude = new Dictionary<string, object>();
            foreach (var record in options)
            {
                dictForAmplitude.Add(record.Key.ToDescription(), record.Value);
            }
            // Set user properties.
            try
            {
                amplitude.logEvent(eventName, dictForAmplitude);

                // Flush all the events immediately.
                amplitude.uploadEvents();
            }
            catch (Exception ex)
            {
                Debug.LogError("Amplitude -> WriteToAmplitude -> Got Exception: " + ex.Message);
                Debug.LogError("Amplitude -> WriteToAmplitude -> Got ExceptionFull: " + ex);
            }
        }

    }
}
