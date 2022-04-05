using System;
using System.Collections.Generic;

namespace Zappy.Decode.Helper
{
    public static class TelemetryHelper
    {
        public static void DisposeNonVsTelemetrySession()
        {
            try
            {
                            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        private static void LogError(Exception ex)
        {
            object[] args = { ex };
            
        }

        public static void PostNonVsAssetEvent(string eventName, string assetId, IDictionary<string, object> properties)
        {
            try
            {
                            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        public static void PostNonVsUserTaskEvent(string eventName, string result, IDictionary<string, object> properties)
        {
            try
            {
                            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        public static void PostVsAssetEvent(string eventName, string assetId, IDictionary<string, object> properties)
        {
            try
            {
                            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        public static void PostVsUserTaskEvent(string eventName, string result, IDictionary<string, object> properties)
        {
            try
            {
                            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }
    }
}