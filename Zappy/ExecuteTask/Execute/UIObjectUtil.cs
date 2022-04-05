using System.Globalization;
using System.Text;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Execute
{
    internal static class UIObjectUtil
    {
        private static string GetAncestorInformation(string uiObjectName, ScreenIdentifier map, out string ancestorTypeFriendlyName)
        {
            ancestorTypeFriendlyName = string.Empty;
            if (string.IsNullOrEmpty(uiObjectName) || map == null)
            {
                return string.Empty;
            }
            TaskActivityObject uIObjectFromUIObjectId = map.GetUIObjectFromUIObjectId(uiObjectName);
            if (uIObjectFromUIObjectId == null || string.IsNullOrEmpty(uIObjectFromUIObjectId.ControlType))
            {
                return string.Empty;
            }
            string str2 = ScreenIdentifier.ParentElementId(uiObjectName);
            TaskActivityObject ancestor = null;
            if (!string.IsNullOrEmpty(str2))
            {
                ancestor = map.GetUIObjectFromUIObjectId(str2);
            }
            return GetFriendlyNameBasedOnPositionInAncestor(uIObjectFromUIObjectId, ancestor, out ancestorTypeFriendlyName);
        }

        internal static string GetControlType(TaskActivityObject uiObject) =>
            uiObject?.ControlType;

        private static string GetFriendlyName(TaskActivityObject uiObject)
        {
            if (uiObject == null)
            {
                return string.Empty;
            }
            return GetFriendlyNameFromObject(uiObject);
        }

        internal static string GetFriendlyNameAndTypeName(string uiObjectName, ScreenIdentifier map, out string friendlyTypeName)
        {
            friendlyTypeName = string.Empty;
            if (string.IsNullOrEmpty(uiObjectName) || map == null)
            {
                return string.Empty;
            }
            TaskActivityObject uIObjectFromUIObjectId = map.GetUIObjectFromUIObjectId(uiObjectName);
            friendlyTypeName = GetFriendlyTypeName(uIObjectFromUIObjectId);
            string hierarchicalFriendlyName = string.Empty;
            if (IsHierarchicalPathItem(uIObjectFromUIObjectId))
            {
                hierarchicalFriendlyName = GetHierarchicalFriendlyName(map, uiObjectName);
            }
            else
            {
                hierarchicalFriendlyName = GetFriendlyName(uIObjectFromUIObjectId);
            }
            if (string.IsNullOrEmpty(hierarchicalFriendlyName))
            {
                string ancestorTypeFriendlyName = string.Empty;
                string str3 = GetAncestorInformation(uiObjectName, map, out ancestorTypeFriendlyName);
                if (!string.IsNullOrEmpty(str3))
                {
                    hierarchicalFriendlyName = str3;
                    friendlyTypeName = ancestorTypeFriendlyName;
                }
            }
            return hierarchicalFriendlyName;
        }

        private static string GetFriendlyNameBasedOnPositionInAncestor(TaskActivityObject uiObject, TaskActivityObject ancestor, out string ancestorTypeFriendlyName)
        {
            string str = string.Empty;
            ancestorTypeFriendlyName = string.Empty;
            if (ancestor != null && !string.IsNullOrEmpty(ancestor.ControlType))
            {
                string friendlyNameFromObject = GetFriendlyNameFromObject(ancestor);
                string friendlyTypeName = GetFriendlyTypeName(uiObject);
                string friendlyName = null;                if (string.IsNullOrEmpty(friendlyNameFromObject) || string.IsNullOrEmpty(friendlyTypeName) || string.IsNullOrEmpty(friendlyName))
                {
                    return string.Empty;
                }
                object propertyValue = uiObject.Condition.GetPropertyValue("Instance");
                int result = -1;
                if (propertyValue != null && !int.TryParse(propertyValue.ToString(), out result))
                {
                    result = -1;
                }
                ancestorTypeFriendlyName = friendlyName;
                if (SearchConfiguration.ConfigurationExists(uiObject.SearchConfigurations, SearchConfiguration.NextSibling))
                {
                    if (result > 0)
                    {
                        object[] objArray1 = { friendlyTypeName, result, friendlyNameFromObject };
                        return string.Format(CultureInfo.CurrentCulture, Resources.NextToInstanceInfo, objArray1);
                    }
                    object[] args = { friendlyTypeName, friendlyNameFromObject };
                    return string.Format(CultureInfo.CurrentCulture, Resources.NextToInfo, args);
                }
                if (result > 0)
                {
                    object[] objArray3 = { friendlyTypeName, result, friendlyNameFromObject };
                    str = string.Format(CultureInfo.CurrentCulture, Resources.AncestorInstanceInfo, objArray3);
                }
            }
            return str;
        }

        internal static string GetFriendlyNameFromObject(TaskActivityObject uiObject)
        {
            string friendlyName = uiObject.FriendlyName;
            if (!string.IsNullOrEmpty(friendlyName))
            {
                friendlyName = friendlyName.Trim();
                if (!string.IsNullOrEmpty(friendlyName))
                {
                    object[] args = { friendlyName };
                    friendlyName = string.Format(CultureInfo.CurrentCulture, Resources.FormattedFriendlyName, args);
                }
            }
            return friendlyName;
        }

        private static string GetFriendlyTypeName(TaskActivityObject uiObject)
        {
            if (uiObject == null)
            {
                return string.Empty;
            }
            return ControlType.GetControlType(GetControlType(uiObject)).FriendlyName;
        }

        private static string GetHierarchicalFriendlyName(ScreenIdentifier map, string uiObjectName)
        {
            string str = uiObjectName;
            TaskActivityObject uiObject = null;
            StringBuilder builder = new StringBuilder();
            string friendlyName = null;
            bool flag = true;
            while (!string.IsNullOrEmpty(str))
            {
                uiObject = map.GetUIObjectFromUIObjectId(str);
                if (uiObject == null || !IsHierarchicalPathItem(uiObject))
                {
                    break;
                }
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Insert(0, Resources.HierarchicalNameDelimiter);
                }
                friendlyName = GetFriendlyName(uiObject);
                str = ScreenIdentifier.ParentElementId(str);
                if (string.IsNullOrEmpty(friendlyName))
                {
                    string str3;
                    friendlyName = GetFriendlyNameBasedOnPositionInAncestor(uiObject, map.GetUIObjectFromUIObjectId(str), out str3);
                }
                builder.Insert(0, friendlyName);
            }
            return builder.ToString();
        }

        internal static TaskActivityObject GetUIObject(ZappyTaskAction action, ScreenIdentifier map)
        {
            TaskActivityObject uIObjectFromUIObjectId = null;
            if (map != null && action != null && !string.IsNullOrEmpty(action.TaskActivityIdentifier))
            {
                uIObjectFromUIObjectId = map.GetUIObjectFromUIObjectId(action.TaskActivityIdentifier);
            }
            return uIObjectFromUIObjectId;
        }

        internal static TaskActivityObject GetUIObject(string uiObjectName, ScreenIdentifier map)
        {
            TaskActivityObject uIObjectFromUIObjectId = null;
            if (map != null && !string.IsNullOrEmpty(uiObjectName))
            {
                uIObjectFromUIObjectId = map.GetUIObjectFromUIObjectId(uiObjectName);
            }
            return uIObjectFromUIObjectId;
        }

        private static bool IsHierarchicalPathItem(TaskActivityObject uiObject)
        {
            if (uiObject != null)
            {
                string controlType = GetControlType(uiObject);
                if ((ControlType.TreeItem.NameEquals(controlType) || ControlType.MenuItem.NameEquals(controlType)) || ControlType.CheckBoxTreeItem.NameEquals(controlType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}