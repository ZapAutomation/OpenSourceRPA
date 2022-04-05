using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Properties;

namespace Zappy.Decode.Aggregator
{
    internal static class ActionFilterManager
    {
        private static ZappyTaskActionFilter[] filters;
        private static Dictionary<string, List<ZappyTaskActionFilter>> groupFilterMap;
        private static int mergeTimeout;
        private static int remoteTestingMergeTimeout;
        private static Dictionary<string, string> reservedActionFilterGroupNames;

        private static void ActionFilterSchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                object[] args = { e.Message };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidFilterFile, args));
            }
        }

        private static bool ActivitiesOnEditControl(ZappyTaskActionStack actions)
        {
            if (actions != null && actions.Count >= 2)
            {
                ZappyTaskAction action = actions.Peek();
                ZappyTaskAction action2 = actions.Peek(1);
                ControlType edit = ControlType.Edit;
                ControlType document = ControlType.Document;
                if (action != null && action2 != null && action.ActivityElement != null && action2.ActivityElement != null)
                {
                    return string.Equals(action.ActivityElement.ControlTypeName, action2.ActivityElement.ControlTypeName, StringComparison.Ordinal) && edit.NameEquals(action.ActivityElement.ControlTypeName) || document.NameEquals(action.ActivityElement.ControlTypeName) && action.ActivityElement.Equals(action2.ActivityElement);
                }
            }
            return false;
        }

        private static void AddToFilterDictionary(ZappyTaskActionFilter filter, Dictionary<ZappyTaskActionFilterCategory, List<ZappyTaskActionFilter>> categoryFilterDictionary)
        {
            if (!categoryFilterDictionary.ContainsKey(filter.Category))
            {
                categoryFilterDictionary.Add(filter.Category, new List<ZappyTaskActionFilter>());
            }
            categoryFilterDictionary[filter.Category].Add(filter);
        }

        internal static void CleanupFilterList()
        {
            
            filters = null;
            groupFilterMap = null;
        }

        public static void ConfigureActionFilters(RecorderOptions options)
        {
            if (options != null)
            {
                foreach (KeyValuePair<string, bool> pair in options.AggregatorGroupSetting)
                {
                    if (groupFilterMap.ContainsKey(pair.Key))
                    {
                        object[] args = { pair.Key, pair.Value };
                        
                        List<ZappyTaskActionFilter> list = groupFilterMap[pair.Key];
                        foreach (ActionFilter filter in list)
                        {
                            filter.EnabledInternal = pair.Value;
                        }
                    }
                    else
                    {
                        object[] objArray2 = { pair.Key };
                        
                    }
                }
                mergeTimeout = options.AggregatorTimeout;
                remoteTestingMergeTimeout = options.RemoteTestingAggregatorTimeout;
            }
        }


        public static void FilterActivities(ZappyTaskActionStack actions)
        {
            
                        

            foreach (ZappyTaskActionFilter filter in filters)
            {
                try
                {
                    if (actions.Count != 0 && (!ShouldProcessRule(actions, filter) || !filter.ProcessRule(actions)))
                    {
                        continue;
                    }
                    
                }
                catch (SystemException exception)
                {
                    CrapyLogger.log.Error(exception);
                                                                            }
                break;
            }
            
        }

        private static bool HasTimeoutOccured(ZappyTaskActionStack actions, ref int timeBetweenActivities)
        {
            timeBetweenActivities = AggregatorUtilities.TimeDifference(actions.Peek(), actions.Peek(1));
            if (!ZappyTaskUtilities.IsRemoteTestingEnabled || ActivitiesOnEditControl(actions))
            {
                return timeBetweenActivities > MergeTimeout;
            }
            return timeBetweenActivities > RemoteTestingMergeTimeout;
        }

        public static void InitializeActionFilters(bool isRemoteTestingEnabled)
        {
            using (Stream stream = new MemoryStream(Resources.AdvanceFiltersraf))
                        {
                List<ZappyTaskActionFilter> list = new List<ZappyTaskActionFilter>();

                Dictionary<ZappyTaskActionFilterCategory, List<ZappyTaskActionFilter>> categoryFilterDictionary =
                    new Dictionary<ZappyTaskActionFilterCategory, List<ZappyTaskActionFilter>>();
                ReadFromStream(stream, categoryFilterDictionary, isRemoteTestingEnabled);
                ReadCustomFilters(categoryFilterDictionary);

                foreach (ZappyTaskActionFilterCategory category in Enum.GetValues(typeof(ZappyTaskActionFilterCategory)))
                {
                    if (categoryFilterDictionary.ContainsKey(category))
                    {
                        list.AddRange(categoryFilterDictionary[category]);
                    }
                }

                filters = list.ToArray();
                groupFilterMap = new Dictionary<string, List<ZappyTaskActionFilter>>(StringComparer.OrdinalIgnoreCase);
                foreach (ZappyTaskActionFilter filter in filters)
                {
                    if (groupFilterMap.ContainsKey(filter.Group))
                    {
                        groupFilterMap[filter.Group].Add(filter);
                    }
                    else if (!string.IsNullOrEmpty(filter.Group))
                    {
                        List<ZappyTaskActionFilter> list2 = new List<ZappyTaskActionFilter> {
                            filter
                        };
                        groupFilterMap.Add(filter.Group, list2);
                    }
                }
                object[] args = { filters.Length };
                
            }
        }

        private static bool IsAggregationNeeded(ZappyTaskActionStack actions, ZappyTaskActionFilter filter)
        {
            bool needFiltering = true;
            if (!string.Equals(filter.Group, "SystemAggregators", StringComparison.OrdinalIgnoreCase))
            {
                if (filter.FilterType != ZappyTaskActionFilterType.Unary)
                {
                    needFiltering = (actions.Peek() as ZappyTaskAction).NeedFiltering && (actions.Peek(1) as ZappyTaskAction).NeedFiltering;
                }
                else
                {
                    needFiltering = (actions.Peek() as ZappyTaskAction).NeedFiltering;
                }
            }
            if (!needFiltering)
            {
                object[] args = { filter.Name };
                
            }
            return needFiltering;
        }

        private static bool IsCustomCategory(ZappyTaskActionFilterCategory uITaskActionFilterCategory)
        {
            if (uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PostCritical && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PreGeneral && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PostGeneral && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PreSimpleToCompoundActionConversion && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PostSimpleToCompoundActionConversion && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.PreRedundantActionDeletion)
            {
                return uITaskActionFilterCategory == ZappyTaskActionFilterCategory.PostRedundantActionDeletion;
            }
            return true;
        }

        private static bool IsReservedCategory(ZappyTaskActionFilterCategory uITaskActionFilterCategory)
        {
            if (uITaskActionFilterCategory != ZappyTaskActionFilterCategory.Critical && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.General && uITaskActionFilterCategory != ZappyTaskActionFilterCategory.SimpleToCompoundActionConversion)
            {
                return uITaskActionFilterCategory == ZappyTaskActionFilterCategory.RedundantActionDeletion;
            }
            return true;
        }

        private static bool IsReservedGroupName(string groupName)
        {
            if (reservedActionFilterGroupNames == null)
            {
                reservedActionFilterGroupNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                reservedActionFilterGroupNames.Add("SetValueAggregators", "SetValueAggregators");
                reservedActionFilterGroupNames.Add("AbsorbWindowResizeAndSetFocusAggregators", "AbsorbWindowResizeAndSetFocusAggregators");
                reservedActionFilterGroupNames.Add("IEDialogAggregators", "IEDialogAggregators");
                reservedActionFilterGroupNames.Add("SystemAggregators", "SystemAggregators");
                reservedActionFilterGroupNames.Add("MiscellaneousAggregators", "MiscellaneousAggregators");
                reservedActionFilterGroupNames.Add("DeleteImplicitHoverAggregator", "DeleteImplicitHoverAggregator");
                reservedActionFilterGroupNames.Add("RecordImplicitHoverAggregator", "RecordImplicitHoverAggregator");
                reservedActionFilterGroupNames.Add("LaunchApplicationAggregators", "LaunchApplicationAggregators");
            }
            return !string.IsNullOrEmpty(groupName) && reservedActionFilterGroupNames.ContainsKey(groupName);
        }

        private static bool IsTimedOut(ZappyTaskActionStack actions, ZappyTaskActionFilter filter)
        {
            bool flag = false;
            int timeBetweenActivities = -1;
            if (filter.FilterType != ZappyTaskActionFilterType.Unary && filter.ApplyTimeout)
            {
                flag = HasTimeoutOccured(actions, ref timeBetweenActivities);
            }
            if (flag)
            {
                object[] args = { filter.Name, timeBetweenActivities, MergeTimeout };
                
            }
            return flag;
        }

        private static ZappyTaskActionFilter ReadCodeFilter(XmlNode child)
        {
            Type type = Type.GetType(child.Attributes["ClassName"].Value);
            if (type == null || !type.IsSubclassOf(typeof(ActionFilter)))
            {
                return null;
                            }
            ActionFilter filter = (ActionFilter)type.GetConstructor(Type.EmptyTypes).Invoke(null);
            filter.CategoryInternal = (ZappyTaskActionFilterCategory)Enum.Parse(typeof(ZappyTaskActionFilterCategory), child.Attributes["Category"].Value);
            return filter;
        }

        private static void ReadCustomFilters(Dictionary<ZappyTaskActionFilterCategory, List<ZappyTaskActionFilter>> categoryFilterDictionary)
        {
            
            IList<ZappyTaskActionFilter> extensions = ZappyTaskService.Instance.GetExtensions<ZappyTaskActionFilter>();
            if (extensions != null)
            {
                int count = extensions.Count;
                foreach (ZappyTaskActionFilter filter in extensions)
                {
                    if (filter != null)
                    {
                        if (filter.FilterType != ZappyTaskActionFilterType.Unary && filter.FilterType != ZappyTaskActionFilterType.Binary)
                        {
                            object[] objArray1 = { filter.Name, filter.FilterType };
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.CustomActionFilterError1, objArray1));
                        }
                        if (IsReservedGroupName(filter.Group))
                        {
                            object[] objArray2 = { filter.Name, filter.Group, filter.Category };
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.CustomActionFilterError2, objArray2));
                        }
                        if (!IsCustomCategory(filter.Category))
                        {
                            object[] objArray4 = { filter.Name, filter.Group, filter.Category };
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.CustomActionFilterError3, objArray4));
                        }
                        object[] args = { filter.Name, filter.Group, filter.Category };
                        
                        AddToFilterDictionary(filter, categoryFilterDictionary);
                    }
                }
            }
        }

        private static void ReadFromStream(Stream streamFilters, Dictionary<ZappyTaskActionFilterCategory, List<ZappyTaskActionFilter>> categoryFilterDictionary, bool isRemoteTestingEnabled)
        {
            using (Stream stream = new MemoryStream(Resources.AdvanceFiltersxsd))
                        {
                XmlTextReader reader = new XmlTextReader(stream)
                {
                    DtdProcessing = DtdProcessing.Prohibit
                };
                XmlSchema schema = XmlSchema.Read(reader, null);
                XmlDocument document = new XmlDocument();
                document.Schemas.Add(schema);
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit
                };
                using (XmlReader reader2 = XmlReader.Create(streamFilters, settings))
                {
                    document.Load(reader2);
                }
                document.Validate(ActionFilterSchemaValidationHandler);
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    ZappyTaskActionFilter filter = null;
                    if (string.Equals(node.Name, "CodeFilter", StringComparison.Ordinal))
                    {
                        filter = ReadCodeFilter(node);
                    }
                    else if (string.Equals(node.Name, "XmlFilter", StringComparison.Ordinal))
                    {
                        filter = ReadXmlFilter(node);
                    }
                    if (filter != null)
                    {
                        bool flag = IsReservedCategory(filter.Category);
                        AddToFilterDictionary(filter, categoryFilterDictionary);
                    }
                }
            }
        }

        private static ZappyTaskActionFilter ReadXmlFilter(XmlNode child)
        {
            bool flag;
            bool flag2;
            string filterName = child.Attributes["Name"].Value;
            string filteringQuery = child.Attributes["FilteringQuery"].Value;
            string outputQuery = child.Attributes["OutputQuery"].Value;
            string str4 = child.Attributes["ApplyTimeout"].Value;
            string groupName = child.Attributes["Group"].Value;
            string str6 = child.Attributes["Category"].Value;
            ZappyTaskActionFilterType filterType = (ZappyTaskActionFilterType)Enum.Parse(typeof(ZappyTaskActionFilterType), child.Attributes["FilterType"].Value);
            if (!bool.TryParse(str4, out flag))
            {
                CrapyLogger.log.ErrorFormat("XSD validation not working.");
                object[] args = { filterName };
                CrapyLogger.log.ErrorFormat("Invalid value for applyTimeout for filter {0}", args);
            }
            AdvanceFilter filter = new AdvanceFilter(filterName, filteringQuery, outputQuery, filterType, flag, groupName);
            XmlAttribute attribute = child.Attributes["Enabled"];
            if (attribute != null && bool.TryParse(attribute.Value, out flag2))
            {
                filter.EnabledInternal = flag2;
            }
            filter.CategoryInternal = (ZappyTaskActionFilterCategory)Enum.Parse(typeof(ZappyTaskActionFilterCategory), str6);
            return filter;
        }

        private static bool ShouldProcessRule(ZappyTaskActionStack actions, ZappyTaskActionFilter filter) =>
            filter.Enabled && (filter.FilterType == ZappyTaskActionFilterType.Binary && actions.Count >= 2 || filter.FilterType == ZappyTaskActionFilterType.Unary && actions.Count >= 1) && !IsTimedOut(actions, filter) && IsAggregationNeeded(actions, filter);

        public static int MergeTimeout
        {
            get =>
                mergeTimeout;
            internal set
            {
                mergeTimeout = value;
            }
        }

        public static int RemoteTestingMergeTimeout =>
            remoteTestingMergeTimeout;
    }
}

