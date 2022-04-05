using System.Text.RegularExpressions;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.Decode.Mssa
{
    internal static class ExtensionUtilities
    {
        public static bool IsQueryConditionSubSetOfParent(string elementTechnologyName, IQueryCondition[] element, ITaskActivityElement parent)
        {
            if (element == null || parent == null || !UITechnologyManager.AreEqual(elementTechnologyName, parent.Framework))
            {
                return false;
            }
            foreach (QueryCondition condition in element)
            {
                PropertyCondition condition2 = condition as PropertyCondition;
                if (condition2 != null && !condition2.Match(parent))
                {
                    return false;
                }
            }
            return true;
        }

        public static string SanitizeFriendlyName(string friendlyName, string ellipsisString)
        {
            if (!string.IsNullOrEmpty(friendlyName))
            {
                friendlyName = Regex.Replace(friendlyName, @"\s+", " ");
                if (friendlyName.Length > 50)
                {
                    friendlyName = friendlyName.Substring(0, 50) + ellipsisString;
                }
            }
            return friendlyName;
        }


        internal static string SanitizeFriendlyName(string friendlyName) =>
            ZappyTaskUtilities.SanitizeFriendlyName(friendlyName, Resources.Ellipsis);

    }
}