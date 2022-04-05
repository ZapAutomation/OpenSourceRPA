using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Zappy.Decode.Helper;
using Zappy.Properties;

namespace Zappy.ActionMap.TaskAction
{

    [Serializable]
#if COMENABLED
      [Guid("40C532AC-06B4-412D-8D48-F11804C7F9DC"), ComVisible(true)]
#endif
    public class ZappyTaskActionLogEntry
    {
        private List<ContextEntry> context;

        public ZappyTaskActionLogEntry()
        {
            ControlType = string.Empty;
            context = new List<ContextEntry>();
            Format = string.Empty;
            Value = new Entry(typeof(string), string.Empty);
        }

        internal static string GetLocalizedControlType(string controlTypeActual)
        {
            return controlTypeActual;
        }

        internal static string GetLocalizedString(string value, bool hasNoFriendlyName)
        {
            string str = value;
            if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
            {
                str = value.Substring(1, value.Length - 2);
            }
            else if (hasNoFriendlyName)
            {
                str = Resources.ResourceManager.GetString(value + "ForNoFriendlyName");
            }
            if (str == null)
            {
                str = value;
            }
            return str;
        }

        internal string GetPartialActionLogString() =>
            Regex.Replace(GetLocalizedString(Format, string.IsNullOrWhiteSpace(Target)), "(<Action>)|(<Value>)|(<Target>)|(<ControlType>)", delegate (Match match)
            {
                if (string.Equals(match.Value, "<Action>") && Action != null)
                {
                    return GetLocalizedString(Action, false);
                }
                if (string.Equals(match.Value, "<Value>") && Value != null)
                {
                    return Value.ToString();
                }
                if (string.Equals(match.Value, "<Target>") && Target != null)
                {
                    return Target;
                }
                if (string.Equals(match.Value, "<ControlType>") && ControlType != null)
                {
                    return GetLocalizedControlType(ControlType);
                }
                return string.Empty;
            }, RegexOptions.IgnoreCase);

        public override string ToString()
        {
            string str = "";
            if (Context != null)
            {
                foreach (ContextEntry entry in Context)
                {
                    object[] args = { str, entry };
                                    }
            }
            return Regex.Replace(GetLocalizedString(Format, string.IsNullOrWhiteSpace(Target)), "(<Action>)|(<Value>)|(<Target>)|(<ControlType>)", delegate (Match match)
            {
                if (string.Equals(match.Value, "<Action>") && Action != null)
                {
                    return GetLocalizedString(Action, false);
                }
                if (string.Equals(match.Value, "<Value>") && Value != null)
                {
                    return Value.ToString();
                }
                if (string.Equals(match.Value, "<Target>") && Target != null)
                {
                    return Target;
                }
                if (string.Equals(match.Value, "<ControlType>") && ControlType != null)
                {
                    return GetLocalizedControlType(ControlType);
                }
                return string.Empty;
            }, RegexOptions.IgnoreCase) + str;
        }

        public string Action { get; set; }

        [XmlArray]
        public List<ContextEntry> Context =>
            context;

        public string ControlType { get; set; }

        public string Format { get; set; }

        public string Target { get; set; }

        public Entry Value { get; set; }
    }
}