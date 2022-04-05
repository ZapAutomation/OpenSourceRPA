using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.ScreenMaps
{
    internal static class ScreenMapUtil
    {
        private const int MaxIdLength = 20;
        private const string NameString = "Name";
        internal const string ScreenMapIdSeperator = ".";
        
        public static bool CompareString(string firstString, string secondString, StringComparison comparisonType) =>
            string.IsNullOrEmpty(firstString) && string.IsNullOrEmpty(secondString) || string.Equals(firstString, secondString, comparisonType);

        public static bool CompareUIObjects(TaskActivityObject firstObject, TaskActivityObject secondObject) =>
            TaskActivityObject.IdComparer.Equals(firstObject.Id, secondObject.Id);

        private static void FillPropertyFromUIElement(TaskActivityObject obj, ITaskActivityElement element)
        {
            obj.ControlType = element.ControlTypeName;
            obj.Framework = element.Framework;
            obj.Name = element.Name;
            obj.QueryId = element.QueryId as QueryElement;
            obj.Condition = element.QueryId.Condition as QueryCondition;
            obj.FriendlyName = element.FriendlyName;
            obj.SearchConfigurations = element.QueryId.SearchConfigurations;
            TaskActivityElement element2 = element as TaskActivityElement;
            if (element2 != null)
            {
                if (element2.WindowTitles != null)
                {
                    obj.WindowTitles.Clear();
                    TaskActivityElement topLevelElement = element2.TopLevelElement;
                    bool flag = IsBrowserWindow(topLevelElement);
                    foreach (string str in element2.WindowTitles)
                    {
                        if (!flag || !UpdateBrowserWindowTitles(topLevelElement, obj, str))
                        {
                            obj.WindowTitles.Add(str);
                        }
                    }
                }
                obj.SupportLevel = element2.SupportLevel;
            }
        }

        private static void FillPropertyOfTopLevelElementFromUIElement(TaskActivityObject obj, ITaskActivityElement element)
        {
            FillPropertyFromUIElement(obj, element);
            obj.SessionId = element.WindowHandle.ToString();
            if (element.QueryId.Ancestor != null)
            {
                TaskActivityObject obj2 = new TaskActivityObject();
                FillPropertyOfTopLevelElementFromUIElement(obj2, element.QueryId.Ancestor);
                obj.Ancestor = obj2;
            }
        }

        public static TopLevelElement FillTopLevelElementFromUIElement(ITaskActivityElement element, TopLevelElement obj, bool stripBrowserWindowTitleSuffix)
        {
            FillPropertyOfTopLevelElementFromUIElement(obj, element);
            obj.SpecialControlType = GetSpecialControlType(element);
            HandleBrowserTitles(element, obj, stripBrowserWindowTitleSuffix);
            return obj;
        }

        public static TaskActivityObject FromUIElement(ITaskActivityElement element)
        {
            TaskActivityObject obj2 = new TaskActivityObject();
            return FromUIElement(element, obj2);
        }

        public static TaskActivityObject FromUIElement(ITaskActivityElement element, TaskActivityObject obj)
        {
            FillPropertyFromUIElement(obj, element);
            return obj;
        }

        public static string GenerateId(TaskActivityObject obj, Dictionary<string, TaskActivityObject> idHash) =>
            GenerateId(obj, idHash, null);

        public static string GenerateId(TaskActivityObject obj, Dictionary<string, TaskActivityObject> idHash,
            Dictionary<string, string> idExclude, bool Reuse = false)
        {
            string str = "Item";
            if (obj.FriendlyName != null)
            {
                str = MakePascal(obj.FriendlyName);
            }
            if (str.Length > 20)
            {
                str = str.Substring(0, 20);
            }
            str = str + obj.ControlType;

            string key = str;
            if (!Reuse)
            {
                int num = 0;
                while (idHash.ContainsKey(key) || idExclude != null && idExclude.ContainsKey(key))
                {
                    num++;
                    key = str + num;
                }
            }
                        
            idHash[key] = obj;
            return key;
        }

        internal static string GetCompleteQueryId(TaskActivityElement pluginNode)
        {
            StringBuilder builder = new StringBuilder();
            Stack<string> stack = new Stack<string>();
            while (pluginNode != null)
            {
                ZappyTaskService.Instance.UpdateQueryIdForTopLevelElement(pluginNode);

                if (FrameworkUtilities.IsTopLevelElement(pluginNode))
                {
                    TopLevelElement element2 = new TopLevelElement();
                    stack.Push(FillTopLevelElementFromUIElement(pluginNode, element2, false).ToString());
                    break;
                }
                stack.Push(FromUIElement(pluginNode).ToString());
                for (ITaskActivityElement element = pluginNode.QueryId.Ancestor; element != null && !FrameworkUtilities.IsTopLevelElement(element); element = element.QueryId.Ancestor)
                {
                    stack.Push(FromUIElement(element).ToString());
                }
                pluginNode = FrameworkUtilities.TopLevelElement(pluginNode);
            }
            while (stack.Count > 0)
            {
                builder.Append(stack.Pop());
            }
            return builder.ToString();
        }

        internal static PropertyCondition GetCondition(IQueryCondition queryConditions, string propertyName)
        {
            if (queryConditions != null && queryConditions.Conditions != null)
            {
                foreach (IQueryCondition condition in queryConditions.Conditions)
                {
                    PropertyCondition condition2 = condition as PropertyCondition;
                    if (condition2 != null)
                    {
                        if (string.Equals(condition2.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            return condition2;
                        }
                    }
                    else
                    {
                        return GetCondition(condition, propertyName);
                    }
                }
            }
            return new PropertyCondition(propertyName, null);
        }

        internal static string GetLastId(string uiObjectId, out string previousScreenMapId)
        {
            ZappyTaskUtilities.CheckForNull(uiObjectId, "TaskActivityIdentifier");
            previousScreenMapId = string.Empty;
            int length = uiObjectId.LastIndexOf(ScreenIdentifier.ScreenMapIdSeperator, StringComparison.OrdinalIgnoreCase);
            if (length < 0)
            {
                return uiObjectId;
            }
            previousScreenMapId = uiObjectId.Substring(0, length);
            if (uiObjectId.Length > length + 1)
            {
                return uiObjectId.Substring(length + 1);
            }
            return string.Empty;
        }

        internal static string GetNextId(string uiObjectId, out string remainingScreenMapId)
        {
            remainingScreenMapId = string.Empty;
            int index = uiObjectId.IndexOf(ScreenIdentifier.ScreenMapIdSeperator, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return uiObjectId;
            }
            if (uiObjectId.Length > index + 1)
            {
                remainingScreenMapId = uiObjectId.Substring(index + 1);
            }
            return uiObjectId.Substring(0, index);
        }

        public static SpecialControlType GetSpecialControlType(ITaskActivityElement element)
        {
                                                                                                                                                            return SpecialControlType.None;
        }

        public static string GetUniqueName(TaskActivityObject uiObject, string screenMapId)
        {
            ZappyTaskUtilities.CheckForNull(screenMapId, "screenMapId");
            ZappyTaskUtilities.CheckForNull(uiObject, "uiObject");
            StringBuilder builder = new StringBuilder();
            builder.Append(".");
            builder.Append(uiObject.Id);
            while (uiObject.Ancestor != null)
            {
                uiObject = uiObject.Ancestor;
                builder.Insert(0, uiObject.Id);
                builder.Insert(0, ".");
            }
            builder.Insert(0, screenMapId);
            return builder.ToString();
        }

        public static void HandleBrowserTitles(ITaskActivityElement element, TopLevelElement obj, bool stripBrowserWindowTitleSuffix)
        {
            if ((obj.SpecialControlType == SpecialControlType.BrowserWindow) & stripBrowserWindowTitleSuffix)
            {
                PropertyCondition condition = GetCondition(obj.Condition, "Name");
                TaskActivityElement element2 = element as TaskActivityElement;
                if (element2 != null)
                {
                    string pageTitle = condition.Value?.ToString();
                                                                                                                    }
            }
        }

        public static bool IsBrowserWindow(TaskActivityElement element) =>
            GetSpecialControlType(element) == SpecialControlType.BrowserWindow;

        public static string MakePascal(string name) =>
            CodeIdentifier.MakePascal(CodeIdentifier.MakeValid(name));

        public static bool UpdateBrowserWindowTitles(TaskActivityElement topLevelElement, TaskActivityObject obj, string windowTitle)
        {
                                                                                    return false;
        }

        public static void UpdateProperties(TaskActivityObject targetObject, TaskActivityObject sourceObject)
        {
            if (sourceObject.ControlType != null)
            {
                targetObject.ControlType = sourceObject.ControlType;
            }
            if (sourceObject.Framework != null)
            {
                targetObject.Framework = sourceObject.Framework;
            }
            if (sourceObject.Name != null)
            {
                targetObject.Name = sourceObject.Name;
            }
            if (sourceObject.QueryId != null)
            {
                targetObject.QueryId = sourceObject.QueryId;
            }
            if (!string.IsNullOrEmpty(sourceObject.FriendlyName))
            {
                targetObject.FriendlyName = sourceObject.FriendlyName;
            }
        }
    }
}
