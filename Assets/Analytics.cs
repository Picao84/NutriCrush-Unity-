using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;

namespace Assets
{
    public static class Analytics
    {
        public static async Task InitializeAnalytics(bool isDebug)
        {
            try
            {

                InitializationOptions options;

                if (isDebug)
                {

                    options = new InitializationOptions().SetOption("com.unity.services.core.environment-name", "debug");
                }
                else
                {
                    options = new InitializationOptions().SetOption("com.unity.services.core.environment-name", "release");
                }

                await UnityServices.InitializeAsync(options);

                AnalyticsService.Instance.StartDataCollection();

                //TEST
                UnityEngine.Debug.LogException(new Exception("Test Exception"));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public static void LogEvent(Unity.Services.Analytics.Event eventToLog)
        {
            try
            {

                AnalyticsService.Instance.RecordEvent(eventToLog);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }
    }
}
