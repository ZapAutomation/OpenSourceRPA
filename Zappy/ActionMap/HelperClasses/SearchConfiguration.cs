using System;
using System.Collections.Generic;

namespace Zappy.ActionMap.HelperClasses
{
    public static class SearchConfiguration
    {
        public static readonly string AlwaysSearch = "AlwaysSearch";
        public static readonly string DisambiguateChild = "Distinct";
        public static readonly string ExpandWhileSearching = "Expand";
        public static readonly string NextSibling = "NextTo";
        public static readonly string VisibleOnly = "VisibleOnly";

        public static bool AreEqual(string[] configList, string[] configListToMatch)
        {
            if (configList == null || configListToMatch == null)
            {
                return configList == null && configListToMatch == null;
            }
            if (configList.Length != configListToMatch.Length)
            {
                return false;
            }
            foreach (string str in configList)
            {
                if (!ConfigurationExists(configListToMatch, str))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ConfigurationExists(IEnumerable<string> configList, string configNameToSearch)
        {
            if (configList != null && !string.IsNullOrEmpty(configNameToSearch))
            {
                foreach (string str in configList)
                {
                    if (NameEquals(configNameToSearch, str))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool NameEquals(string configName1, string configName2) =>
            string.Equals(configName1, configName2, StringComparison.OrdinalIgnoreCase);
    }
}