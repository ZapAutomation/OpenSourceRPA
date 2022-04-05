using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zappy.SharedInterface.Helper
{
    public static class HelperFunctions
    {

        public static Dictionary<Type, string> TypeToHumanizedString;
        public static string Humanize(this string str)
        {
            StringBuilder sb = new StringBuilder(str);
            sb = sb.Replace("Action", "").Replace("Node", "").Replace("Activity", "").Replace("_", " ");
            str = sb.ToString();
            str = Regex.Replace(str.Split('.').Last(), "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            sb.Length = 0;
            sb.Append(str);
            if (sb.Length > 0 && sb[0] >= 'a' && sb[0] <= 'z')
                sb[0] -= ' ';

            str = sb.ToString();
            return str;
        }

        public static string HumanizeNameForIZappyAction(IZappyAction action)
        {
            string humanisedType = string.Empty;
            TypeToHumanizedString.TryGetValue(action.GetType(), out humanisedType);
            return humanisedType;
        }
    }
}
