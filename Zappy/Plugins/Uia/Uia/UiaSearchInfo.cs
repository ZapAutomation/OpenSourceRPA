using System;
using System.Collections.Generic;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Plugins.Uia.Uia
{
    internal class UiaSearchInfo : ILastInvocationInfo
    {
        private int hResult;
        private string message;
        private static List<string> supportedProperties = InitSupportedProperties();

        internal UiaSearchInfo(string infoMessage, uint hresultCode)
        {
            message = infoMessage;
            hResult = (int)hresultCode;
        }

        public string[] GetInfoProperties() =>
            supportedProperties.ToArray();

        public object GetInfoPropertyValue(string propertyName)
        {
            ZappyTaskUtilities.CheckForNull(propertyName, "propertyName");
            if (propertyName != "HResult")
            {
                throw new NotSupportedException();
            }
            return hResult;
        }

        private static List<string> InitSupportedProperties() =>
            new List<string> { "HResult" };

        public ILastInvocationInfo InnerInfo =>
            null;

        public string Message =>
            message;

        public string Source =>
            "UIA";
    }
}

