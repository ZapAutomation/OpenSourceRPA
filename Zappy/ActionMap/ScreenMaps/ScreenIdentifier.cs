using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.ScreenMaps
{
    [Serializable, EditorBrowsable(EditorBrowsableState.Never),]
    public class ScreenIdentifier
    {
        private const string checkbox = "check box";
        private const string ControlTypeString = "ControlType";
        private Dictionary<ITaskActivityElement, TaskActivityObject> elementDictionary;
        private Dictionary<string, string> excludeTopLevelId = new Dictionary<string, string>();
        private string fileName;
        private string id;
        private Dictionary<string, TaskActivityObject> idDictionary;
        private const int MaxIdLength = 20;
        private const string menuItem = "menu item";
        private const string outlineitem = "outline item";
        private const string role = "Role";
        private Collection<TopLevelElement> topLevelElements = new Collection<TopLevelElement>();
        internal static string ScreenMapIdSeperator = ".";
        private UniqueNameHelper uniqueNameHelper = new UniqueNameHelper();
        public bool ReuseID { get; set; }

        internal void AddExcludeTopLevelIds(IEnumerable<TopLevelElement> topElements)
        {
            foreach (TaskActivityObject obj2 in topElements)
            {
                if (!excludeTopLevelId.ContainsKey(obj2.Id))
                {
                    excludeTopLevelId.Add(obj2.Id, obj2.Id);
                }
            }
        }

                private void AddToElementDictionary(ITaskActivityElement element, TaskActivityObject uiObject)
        {
            try
            {
                ElementDictionary.Add(element, uiObject);
            }
            catch (ArgumentException exception)
            {
                object[] args = { element, exception };
                CrapyLogger.log.ErrorFormat("Error adding element to ElementDictionary - {0} : {1}", args);
            }
        }

                private TopLevelElement AddTopLevelElement(TopLevelElement window, Dictionary<string, string> excludeId)
        {
            object[] args = { window };
            
            if (ReferenceEquals(window, null))
            {
                CrapyLogger.log.Error("Null window");
                return window;

            }
            string friendlyName = window.FriendlyName;
            TopLevelElement existingTopLevelElement = GetExistingTopLevelElement(window);
            if (existingTopLevelElement != null)
            {
                ScreenMapUtil.UpdateProperties(existingTopLevelElement, window);
                return existingTopLevelElement;
            }
            window.FriendlyName = friendlyName;
            if (string.IsNullOrEmpty(window.Id))
            {
                window.Id = ScreenMapUtil.GenerateId(window, IdDictionary, excludeId, ReuseID);
            }
            TopLevelWindows.Add(window);
            return window;
        }

        public void externalAddTopLevelElement(TopLevelElement window)
        {
            lock (this)
            {
                AddTopLevelElement(window, null);
            }
        }
        public TaskActivityObject AddUIObject(ITaskActivityElement element)
        {
            if (element == null)
            {
                return null;
            }
            TaskActivityObject obj2 = null;
            if (!ElementDictionary.TryGetValue(element, out obj2))
            {
                if (FrameworkUtilities.IsTopLevelElement(element))
                {
                    TopLevelElement window = ScreenMapUtil.FillTopLevelElementFromUIElement(element, new TopLevelElement(), true);
                    window = AddTopLevelElement(window, excludeTopLevelId);
                    AddToElementDictionary(element, window);
                    return window;
                }
                TaskActivityElement element3 = FrameworkUtilities.TopLevelElement(element);
                TaskActivityElement key = element3 == null ? ZappyTaskService.Instance.RootElement : element3;
                if (key == null)
                {
                    return obj2;
                }
                TaskActivityObject obj3 = null;
                if (!ElementDictionary.TryGetValue(key, out obj3))
                {
                    TopLevelElement element5 = ScreenMapUtil.FillTopLevelElementFromUIElement(key, new TopLevelElement(), true);
                    element5 = AddTopLevelElement(element5, excludeTopLevelId);
                    AddToElementDictionary(key, element5);
                    obj3 = element5;
                }
                foreach (ITaskActivityElement element6 in GetElementsInOrder(element))
                {
                    if (!ElementDictionary.TryGetValue(element6, out obj2))
                    {
                        obj2 = ScreenMapUtil.FromUIElement(element6);
                        obj2 = obj3.Add(obj2, ReuseID);
                        obj2.SpecialControlType = ScreenMapUtil.GetSpecialControlType(element6);
                        AddToElementDictionary(element6, obj2);
                        obj2.Ancestor = obj3;
                    }
                    obj3 = obj2;
                }
            }
            return obj2;
        }

        public void AddUIObjects(IEnumerable<ITaskActivityElement> uiElements)
        {
            foreach (ITaskActivityElement element in uiElements)
            {
                                
                TaskActivityObject uiObject = AddUIObject(element);
                if (uiObject != null && !UniqueNameDictionary.ContainsKey(element))
                {
                    string uniqueName = ScreenMapUtil.GetUniqueName(uiObject, Id);
                    if (!uniqueNameHelper.TryAdd(element, uniqueName))
                    {
                                                
                    }
                }
            }
        }

        public void BindParameters(ValueMap valueMap)
        {
            if (TopLevelWindows != null)
            {
                foreach (TopLevelElement element in TopLevelWindows)
                {
                    element.BindParameters(valueMap);
                }
            }
        }

        private void BuildIdDictionary()
        {
            foreach (TopLevelElement element in TopLevelWindows)
            {
                BuildIdDictionaryInternal(element);
            }
        }

        private void BuildIdDictionaryInternal(TaskActivityObject uiObject)
        {
            if (!idDictionary.ContainsKey(uiObject.Id))
            {
                idDictionary.Add(uiObject.Id, uiObject);
            }
            foreach (TaskActivityObject obj2 in uiObject.Descendants)
            {
                BuildIdDictionaryInternal(obj2);
            }
        }

        public void Clear()
        {
            topLevelElements = new Collection<TopLevelElement>();
            idDictionary = null;
            elementDictionary = null;
        }


        public void Reset()
        {
            if (topLevelElements != null)
                topLevelElements.Clear();


            if (idDictionary != null)
                idDictionary.Clear();

            if (elementDictionary != null)
                elementDictionary.Clear();
        }

        public bool Contains(TaskActivityObject uiObject) =>
            GetTopLevelElement(uiObject) != null;

        internal bool ContainsExpandableAncestor(string uiObjectId)
        {
            string str;
            ScreenMapUtil.GetLastId(uiObjectId, out str);
            string nextId = ScreenMapUtil.GetNextId(str, out str);
            if (!string.IsNullOrEmpty(str))
            {
                nextId = ScreenMapUtil.GetNextId(str, out str);
                TaskActivityObject topLevelElementFromId = GetTopLevelElementFromId(nextId);
                while (!string.IsNullOrEmpty(nextId = ScreenMapUtil.GetNextId(str, out str)))
                {
                    topLevelElementFromId = topLevelElementFromId.GetUIObjectFromId(nextId);
                    if (topLevelElementFromId.Condition.Conditions == null)
                    {
                        if (IsRoleOrControlTypeOfExpandableControl(topLevelElementFromId.Condition))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        foreach (QueryCondition condition in topLevelElementFromId.Condition.Conditions)
                        {
                            if (IsRoleOrControlTypeOfExpandableControl(condition))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal IList<TaskActivityObject> GetAllUIElements(string uiObjectId)
        {
            List<TaskActivityObject> list = new List<TaskActivityObject>();
            string remainingUIObjectId = string.Empty;
            TaskActivityObject topLevelElementFromScreenMapId = GetTopLevelElementFromScreenMapId(uiObjectId, out remainingUIObjectId);
            string str2 = string.Empty;
            while (!string.IsNullOrEmpty(str2 = ScreenMapUtil.GetNextId(remainingUIObjectId, out remainingUIObjectId)))
            {
                topLevelElementFromScreenMapId = topLevelElementFromScreenMapId.GetUIObjectFromId(str2);
                list.Add(topLevelElementFromScreenMapId);
            }
            return list;
        }

        internal static string GetContainerFriendlyName(string uiObjectName)
        {
            string previousScreenMapId = string.Empty;
            string lastId = ScreenMapUtil.GetLastId(uiObjectName, out previousScreenMapId);
            return previousScreenMapId;
        }


        public int GetCount()
        {
            int num = 0;
            foreach (TopLevelElement element in TopLevelWindows)
            {
                num++;
                num += element.Descendants.Count;
            }
            return num;
        }

        private static ITaskActivityElement[] GetElementsInOrder(ITaskActivityElement element)
        {
            Stack<ITaskActivityElement> stack = new Stack<ITaskActivityElement>();
            stack.Push(element);
            ITaskActivityElement ancestor = element.QueryId?.Ancestor;
            if (ScreenMapUtil.GetSpecialControlType(ancestor) == SpecialControlType.DocumentWindow)
            {
                ancestor = ancestor.QueryId.Ancestor;
            }
            while (ancestor != null && !FrameworkUtilities.IsTopLevelElement(ancestor))
            {
                stack.Push(ancestor);
                ancestor = ancestor.QueryId.Ancestor;
                if (ScreenMapUtil.GetSpecialControlType(ancestor) == SpecialControlType.DocumentWindow)
                {
                    ancestor = ancestor.QueryId?.Ancestor;
                }
            }
            return stack.ToArray();
        }

        public IEnumerator<TaskActivityObject> GetEnumerator() =>
            new Enumerator(this);

        private TopLevelElement GetExistingTopLevelElement(TopLevelElement newWindow)
        {
            foreach (TopLevelElement element in TopLevelWindows)
            {
                if (ScreenMapUtil.CompareUIObjects(newWindow, element))
                {
                    return element;
                }
            }
            return null;
        }

        public string GetQueryIdFromId(string uiObjectId)
        {
            string partialId = string.Empty;
            return GetTopLevelElementFromScreenMapId(uiObjectId, out partialId).GetQueryString(partialId);
        }

        public TopLevelElement GetTopLevelElement(TaskActivityObject uiObject)
        {
            TopLevelElement element = uiObject as TopLevelElement;
            if (element != null)
            {
                return element;
            }
            foreach (TopLevelElement element2 in TopLevelWindows)
            {
                if (element2.Descendants.Contains(uiObject))
                {
                    return element2;
                }
            }
            return null;
        }

        internal static string GetTopLevelElementFriendlyName(string uiObjectName)
        {
            string remainingScreenMapId = string.Empty;
            string nextId = ScreenMapUtil.GetNextId(uiObjectName, out remainingScreenMapId);
            if (string.IsNullOrEmpty(remainingScreenMapId))
            {
                object[] args = { uiObjectName };
                            }
            string str3 = ScreenMapUtil.GetNextId(remainingScreenMapId, out remainingScreenMapId);
            return nextId + ScreenMapIdSeperator + str3;
        }

        private TopLevelElement GetTopLevelElementFromId(string elementId)
        {
            TopLevelElement element = TryGetTopLevelElementFromId(elementId);
            if (element == null)
            {
                object[] args = { elementId };
                            }
            return element;
        }

        private TopLevelElement GetTopLevelElementFromScreenMapId(string uiObjectId, out string remainingUIObjectId)
        {
            remainingUIObjectId = string.Empty;
            string nextId = ScreenMapUtil.GetNextId(uiObjectId, out remainingUIObjectId);
            if (string.IsNullOrEmpty(remainingUIObjectId))
            {
                return null;
            }
            nextId = ScreenMapUtil.GetNextId(remainingUIObjectId, out remainingUIObjectId);
            return GetTopLevelElementFromId(nextId);
        }

        internal TopLevelElement externalGetTopLevelElementFromScreenMapId(string uiObjectId, out string remainingUIObjectId)
        {
            try
            {
                return GetTopLevelElementFromScreenMapId(uiObjectId, out remainingUIObjectId);
            }
            catch
            {
                remainingUIObjectId = string.Empty;
                return null;
            }
        }

        public TaskActivityObject GetUIObjectFromUIObjectId(string uiObjectId)
        {
            string remainingUIObjectId = string.Empty;
            TopLevelElement topLevelElementFromScreenMapId = GetTopLevelElementFromScreenMapId(uiObjectId, out remainingUIObjectId);
            if (string.IsNullOrEmpty(remainingUIObjectId))
            {
                return topLevelElementFromScreenMapId;
            }
            return topLevelElementFromScreenMapId.GetUIObjectFromPartialUIObjectId(remainingUIObjectId);
        }

                
                                                                                        
                                
        private static bool IsRoleOrControlTypeOfExpandableControl(QueryCondition qCond)
        {
            PropertyCondition condition = qCond as PropertyCondition;
            if (condition != null && condition.PropertyName.Equals("Role", StringComparison.OrdinalIgnoreCase))
            {
                string a = condition.Value.ToString();
                if (!string.Equals(a, "menu item", StringComparison.OrdinalIgnoreCase) && !string.Equals(a, "outline item", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Equals(a, "check box", StringComparison.OrdinalIgnoreCase);
                }
                return true;
            }
            if (condition == null || !condition.PropertyName.Equals("ControlType", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            string controlName = condition.Value.ToString();
            if (!ControlType.MenuItem.NameEquals(controlName) && !ControlType.TreeItem.NameEquals(controlName))
            {
                return ControlType.CheckBoxTreeItem.NameEquals(controlName);
            }
            return true;
        }

        internal static bool IsTopLevelElementId(string uiObjectName)
        {
            string topLevelElementFriendlyName = GetTopLevelElementFriendlyName(uiObjectName);
            return TaskActivityObject.IdComparer.Equals(uiObjectName, topLevelElementFriendlyName);
        }

        public TaskActivityObject MergeUIObject(ScreenIdentifier screenMap, string objectName)
        {
            string uiObjectId = objectName;
            string nextId = ScreenMapUtil.GetNextId(uiObjectId, out uiObjectId);
            nextId = ScreenMapUtil.GetNextId(uiObjectId, out uiObjectId);
            TopLevelElement topLevelElementFromId = screenMap.GetTopLevelElementFromId(nextId);
            TaskActivityObject obj2 = AddTopLevelElement(topLevelElementFromId, null);
            TaskActivityObject uiObject = topLevelElementFromId;
            while (!string.IsNullOrEmpty(nextId = ScreenMapUtil.GetNextId(uiObjectId, out uiObjectId)))
            {
                uiObject = uiObject.GetUIObjectFromId(nextId);
                if (uiObject == null)
                    return obj2;
                TaskActivityObject obj4 = obj2.Add(uiObject, ReuseID);
                obj4.Ancestor = obj2;
                obj2 = obj4;
            }
            return obj2;
        }

        public static string ParentElementId(string uiObjectName)
        {
            string previousScreenMapId = string.Empty;
            ScreenMapUtil.GetLastId(uiObjectName, out previousScreenMapId);
            if (!string.IsNullOrEmpty(previousScreenMapId) && previousScreenMapId.Contains(ScreenMapIdSeperator))
            {
                return previousScreenMapId;
            }
            return string.Empty;
        }

        public bool Remove(TaskActivityObject uiObject)
        {
            TopLevelElement item = uiObject as TopLevelElement;
            if (item != null)
            {
                if (TopLevelWindows.Remove(item))
                {
                    IdDictionary.Remove(item.Id);
                    return true;
                }
            }
            else
            {
                foreach (TopLevelElement element2 in TopLevelWindows)
                {
                    if (element2.Remove(uiObject))
                    {
                        return true;
                    }
                }
            }
            return TopLevelWindows.Count < 1 && !IdDictionary.ContainsKey(item.Id);
        }

        private TopLevelElement TryGetTopLevelElementFromId(string elementId)
        {
            foreach (TopLevelElement element in topLevelElements)
            {
                if (TaskActivityObject.IdComparer.Equals(elementId, element.Id))
                {
                    return element;
                }
            }
            return null;
        }

                
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public Dictionary<ITaskActivityElement, TaskActivityObject> ElementDictionary
        {
            get
            {
                if (elementDictionary == null)
                {
                    elementDictionary = new Dictionary<ITaskActivityElement, TaskActivityObject>(new ElementQueryComparer());
                }
                return elementDictionary;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public string FileName
        {
            get =>
                fileName;
            set
            {
                fileName = value;
            }
        }

        [XmlAttribute]
        [Browsable(false)]
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
                    BuildIdDictionary();
                }
                return idDictionary;
            }
        }

        [XmlArrayItem("TopLevelWindow")]
        public Collection<TopLevelElement> TopLevelWindows
        {
            get => topLevelElements;
            set => topLevelElements = value;
        }

        public IDictionary<ITaskActivityElement, string> UniqueNameDictionary =>
            uniqueNameHelper.UniqueNameDictionary;

        private class Enumerator : IEnumerator<TaskActivityObject>, IDisposable, IEnumerator
        {
            private int objectIndex;
            private bool topLevelElementEnum;
            private ScreenIdentifier screenmap;
            private int windowIndex;

            public Enumerator(ScreenIdentifier map)
            {
                screenmap = map;
                windowIndex = -1;
                objectIndex = -1;
                topLevelElementEnum = true;
            }

            public bool MoveNext()
            {
                if (topLevelElementEnum)
                {
                    if (MoveToNextTopLevelElement())
                    {
                        return true;
                    }
                    windowIndex = 0;
                    topLevelElementEnum = false;
                }
                return MoveToNextUIObject();
            }

            private bool MoveToNextTopLevelElement()
            {
                windowIndex++;
                return windowIndex < screenmap.TopLevelWindows.Count;
            }

            private bool MoveToNextUIObject()
            {
                objectIndex++;
                if (screenmap.TopLevelWindows.Count > windowIndex)
                {
                    while (screenmap.topLevelElements[windowIndex].Descendants.Count <= objectIndex)
                    {
                        windowIndex++;
                        objectIndex = 0;
                        if (screenmap.TopLevelWindows.Count <= windowIndex)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public TaskActivityObject Current
            {
                get
                {
                    if (topLevelElementEnum)
                    {
                        return screenmap.topLevelElements[windowIndex];
                    }
                    return screenmap.topLevelElements[windowIndex].Descendants[objectIndex];
                }
            }

            object IEnumerator.Current =>
                Current;
        }
    }
}
