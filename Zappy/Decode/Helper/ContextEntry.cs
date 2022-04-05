using System;
using System.Text.RegularExpressions;
using Zappy.ActionMap.TaskAction;

namespace Zappy.Decode.Helper
{
    [Serializable]
    public class ContextEntry
    {
        public ContextEntry()
        {
            ControlType = string.Empty;
            Format = string.Empty;
        }

        public ContextEntry(string format, string value)
        {
            ControlType = string.Empty;
            Format = format;
            Target = value;
        }

        public override string ToString() =>
            Regex.Replace(ZappyTaskActionLogEntry.GetLocalizedString(Format, string.IsNullOrWhiteSpace(Target)), "((<Target>)|(<ControlType>))", delegate (Match match)
            {
                if (string.Equals(match.Value, "<Target>", StringComparison.OrdinalIgnoreCase) && Target != null)
                {
                    return Target;
                }
                if (string.Equals(match.Value, "<ControlType>", StringComparison.OrdinalIgnoreCase) && ControlType != null)
                {
                    return ZappyTaskActionLogEntry.GetLocalizedControlType(ControlType);
                }
                return string.Empty;
            }, RegexOptions.IgnoreCase);

        public string ControlType { get; set; }

        public string Format { get; set; }

        public string Target { get; set; }
    }
}