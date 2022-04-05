using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;

namespace Zappy.ActionMap.ScreenMaps
{
    [Serializable, EditorBrowsable(EditorBrowsableState.Never), XmlInclude(typeof(TopLevelElement))]
    public class TaskActivityObject
    {
        private TaskActivityObject ancestor;
        private string cachedQueryString;
        private QueryCondition condition;
        private string controlType;
        private string frameworkName;
        private string friendlyName;
        private string id;
        internal static readonly StringComparer IdComparer = StringComparer.OrdinalIgnoreCase;
        private Dictionary<string, TaskActivityObject> idDictionary;
        private string name;
        [NonSerialized]
        private QueryElement queryId;
        private string[] searchConfigurationsList;
        private Collection<TaskActivityObject> uiObjects = new Collection<TaskActivityObject>();
        private Collection<string> windowTitles = new Collection<string>();

        public TaskActivityObject Add(TaskActivityObject uiObject, bool Reuse = false)
        {
            ZappyTaskUtilities.CheckForNull(uiObject, "uiObject");
            TaskActivityObject existingObject = GetExistingObject(uiObject);
            if (existingObject != null)
            {
                object[] objArray1 = { existingObject.Id };
                
                ScreenMapUtil.UpdateProperties(existingObject, uiObject);
                return existingObject;
            }
            if (string.IsNullOrEmpty(uiObject.Id))
            {
                uiObject.Id = ScreenMapUtil.GenerateId(uiObject, IdDictionary);
            }
            object[] args = { uiObject.Id };
            
            Descendants.Add(uiObject);
            return uiObject;
        }

        internal void AddWindowTitle(IList<string> sourceWindowTitles, int titleIndex)
        {
            if (sourceWindowTitles != null && sourceWindowTitles.Count > titleIndex && !TitleExists(WindowTitles, sourceWindowTitles[titleIndex]))
            {
                WindowTitles.Add(sourceWindowTitles[titleIndex]);
            }
        }

        internal void AddWindowTitles(IList<string> sourceWindowTitles)
        {
            if (sourceWindowTitles != null)
            {
                for (int i = 0; i < sourceWindowTitles.Count; i++)
                {
                    AddWindowTitle(sourceWindowTitles, i);
                }
            }
        }

        public void BindParameters(ValueMap valueMap)
        {
            if (Condition != null)
            {
                Condition.BindParameters(valueMap);
            }
            if (Descendants != null)
            {
                foreach (TaskActivityObject obj2 in Descendants)
                {
                    obj2.BindParameters(valueMap);
                }
            }
        }

        public virtual object Clone() =>
            FillDetails(new TaskActivityObject());

        private static bool CompareTitles(string sourceTitle, string titleToCompare) =>
            string.Equals(sourceTitle, titleToCompare, StringComparison.OrdinalIgnoreCase);

        public bool Contains(string objectId) =>
            IdDictionary.ContainsKey(objectId);

        internal virtual TaskActivityObject FillDetails(TaskActivityObject targetObject)
        {
            targetObject.CacheQueryString = CacheQueryString;
            targetObject.Condition = Condition;
            targetObject.ControlType = ControlType;
            targetObject.FriendlyName = FriendlyName;
            targetObject.Id = Id;
            targetObject.Name = Name;
            targetObject.QueryId = QueryId;
            if (SearchConfigurations != null)
            {
                targetObject.SearchConfigurations = new List<string>(SearchConfigurations).ToArray();
            }
            targetObject.SessionId = SessionId;
            targetObject.SpecialControlType = SpecialControlType;
            targetObject.Framework = Framework;
            targetObject.WindowTitles.Clear();
            foreach (string str in WindowTitles)
            {
                targetObject.WindowTitles.Add(str);
            }
            return targetObject;
        }

        private TaskActivityObject GetExistingObject(TaskActivityObject uiObject)
        {
            foreach (TaskActivityObject obj2 in uiObjects)
            {
                if (ScreenMapUtil.CompareUIObjects(obj2, uiObject))
                {
                    return obj2;
                }
            }
            return null;
        }

        public virtual string GetQueryString() =>
            GetQueryString(false);

        internal string GetQueryString(bool generatingTopLevelElement)
        {
            if (CacheQueryString && !string.IsNullOrEmpty(cachedQueryString))
            {
                return cachedQueryString;
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            if (!generatingTopLevelElement && !string.IsNullOrEmpty(Framework))
            {
                builder2.AppendFormat("[{0}]", Framework);
            }
            if (SearchConfigurations != null)
            {
                foreach (string str2 in SearchConfigurations)
                {
                    if (!SearchConfiguration.NameEquals(str2, SearchConfiguration.ExpandWhileSearching))
                    {
                        object[] args = { str2 };
                        builder2.AppendFormat(CultureInfo.InvariantCulture, "[{0}]", args);
                    }
                }
            }
            builder2.Replace("][", ", ");
            builder.Append(";");
            builder.Append(builder2);
            builder.Append(condition);
            string str = builder.ToString();
            if (CacheQueryString)
            {
                cachedQueryString = str;
            }
            return str;
        }

        public string GetQueryString(string partialId)
        {
            string queryString = string.Empty;
            if (!string.IsNullOrEmpty(partialId))
            {
                string remainingScreenMapId = string.Empty;
                string nextId = ScreenMapUtil.GetNextId(partialId, out remainingScreenMapId);
                queryString = GetUIObjectFromId(nextId).GetQueryString(remainingScreenMapId);
                if (this is TopLevelElement)
                {
                    return queryString;
                }
            }
            return GetQueryString() + queryString;
        }

        public TaskActivityObject GetUIObjectFromId(string uiObjectId)
        {
            foreach (TaskActivityObject obj2 in Descendants)
            {
                if (IdComparer.Equals(uiObjectId, obj2.Id))
                {
                    return obj2;
                }
            }
            object[] args = { uiObjectId };
            return null;
                    }

        public TaskActivityObject GetUIObjectFromPartialUIObjectId(string partialUIObjectId)
        {
            if (string.IsNullOrEmpty(partialUIObjectId))
            {
                return this;
            }
            string remainingScreenMapId = string.Empty;
            string nextId = ScreenMapUtil.GetNextId(partialUIObjectId, out remainingScreenMapId);
            return GetUIObjectFromId(nextId).GetUIObjectFromPartialUIObjectId(remainingScreenMapId);
        }

        public void ParameterizeProperty(string propertyName, string parameterName)
        {
            if (Condition != null)
            {
                Condition.ParameterizeProperty(propertyName, parameterName);
            }
        }

        public bool Remove(TaskActivityObject uiObject)
        {
            if (uiObject == null)
            {
                return false;
            }
            return Remove(uiObject.Id);
        }

        public bool Remove(string objectId)
        {
            if (IdDictionary.ContainsKey(objectId))
            {
                TaskActivityObject item = IdDictionary[objectId];
                IdDictionary.Remove(objectId);
                uiObjects.Remove(item);
                return true;
            }
            return false;
        }

        private static bool TitleExists(IList<string> windowTitles, string titleToMatch)
        {
            foreach (string str in windowTitles)
            {
                if (CompareTitles(str, titleToMatch))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString() =>
            GetQueryString();

        internal TaskActivityObject Ancestor
        {
            get =>
                ancestor;
            set
            {
                ancestor = value;
            }
        }

        internal bool CacheQueryString { get; set; }

        [XmlElement(ElementName = "Condition")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public QueryCondition Condition
        {
            get =>
                condition;
            set
            {
                condition = value;
            }
        }

        [XmlAttribute]
        public string ControlType
        {
            get =>
                controlType;
            set
            {
                controlType = value;
            }
        }

        [XmlArrayItem(IsNullable = false)]
        public Collection<TaskActivityObject> Descendants =>
            uiObjects;

        [XmlAttribute]
        public string FriendlyName
        {
            get =>
                friendlyName;
            set
            {
                friendlyName = value;
            }
        }

        [XmlAttribute]
        public string Id
        {
            get =>
                id;
            set
            {
                id = value;
            }
        }

        private Dictionary<string, TaskActivityObject> IdDictionary
        {
            get
            {
                if (idDictionary == null)
                {
                    idDictionary = new Dictionary<string, TaskActivityObject>(StringComparer.OrdinalIgnoreCase);
                    foreach (TaskActivityObject obj2 in Descendants)
                    {
                        if (!idDictionary.ContainsKey(obj2.Id))
                        {
                            idDictionary.Add(obj2.Id, obj2);
                        }
                    }
                }
                return idDictionary;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public TaskActivityObject this[string uiObjectId]
        {
            get
            {
                TaskActivityObject obj2;
                if (!IdDictionary.TryGetValue(uiObjectId, out obj2))
                {
                    object[] args = { uiObjectId };
                    CrapyLogger.log.ErrorFormat("ID {0} not found", args);
                                    }
                return obj2;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public string Name
        {
            get =>
                name;
            set
            {
                name = value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public QueryElement QueryId
        {
            get =>
                queryId;
            set
            {
                queryId = value;
            }
        }

        [XmlArrayItem(ElementName = "SearchConfiguration", IsNullable = false)]
        public string[] SearchConfigurations
        {
            get =>
                searchConfigurationsList;
            set
            {
                searchConfigurationsList = value;
            }
        }

        [XmlAttribute]
        public string SessionId { get; set; }

        [XmlAttribute]
        public SpecialControlType SpecialControlType { get; set; }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public int SupportLevel { get; set; }

        [XmlElement(ElementName = "SupportLevel")]
        public string SupportLevelWrapper
        {
            get
            {
                if (SupportLevel != -1)
                {
                    return SupportLevel.ToString(CultureInfo.InvariantCulture);
                }
                return null;
            }
            set
            {
                SupportLevel = -1;
                int result = 0;
                if (int.TryParse(value, out result))
                {
                    SupportLevel = result;
                }
            }
        }

        public string Framework
        {
            get =>
                frameworkName;
            set
            {
                frameworkName = value;
            }
        }

        [XmlArrayItem(ElementName = "WindowTitle", IsNullable = false)]
        public Collection<string> WindowTitles =>
            windowTitles;
    }
}
