using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;

namespace Zappy.ActionMap.ElementManager
{
    [DebuggerDisplay("Count = {Count}")]
    internal class UIElementDictionary
    {
        private Dictionary<TaskActivityElement, TaskActivityElement> ancestorDictionary = new Dictionary<TaskActivityElement, TaskActivityElement>();
        private Dictionary<TaskActivityElement, TaskActivityElement> innerDictionary = new Dictionary<TaskActivityElement, TaskActivityElement>();
        private readonly object lockObject = new object();

        internal TaskActivityElement Add(TaskActivityElement element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (element.SwitchingElement != null)
                {
                    TaskActivityElement element2 = TaskActivityElement.Cast(element.SwitchingElement.SwitchingElement);
                    if (element2 != null)
                    {
                        AddHierarchy(element2, ancestorDictionary);
                        if (FrameworkUtilities.IsWindowlessSwitchContainer(element2))
                        {
                            AddHierarchy(TaskActivityElement.Cast(element.SwitchingElement), ancestorDictionary);
                        }
                    }
                }
                return AddHierarchy(element, innerDictionary);
            }
        }
        private int AddHierarchyRecursiveMapper = 0;
        private TaskActivityElement AddHierarchy(TaskActivityElement element, Dictionary<TaskActivityElement, TaskActivityElement> dictionary)
        {
            element = AddInternal(element, dictionary);
            if (!ReferenceEquals(element, null))
            {
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                if (!ReferenceEquals(element2, null))
                {
                    element2 = AddInternal(element2, innerDictionary);
                    element.TopLevelElement = element2;
                    object lockObject = this.lockObject;
                    lock (lockObject)
                    {
                        string windowText = NativeMethods.GetWindowText(element2.WindowHandle);
                        if (!string.IsNullOrEmpty(windowText))
                        {
                            UpdateWindowTitleData(element2, windowText);
                            UpdateWindowTitleData(element, windowText);
                        }
                    }
                }
                if (!FrameworkUtilities.IsTopLevelElement(element) && element.QueryId != null && element.QueryId.Ancestor != null)
                {
                    AddHierarchyRecursiveMapper++;
                    if (AddHierarchyRecursiveMapper < 1000)
                        element.QueryId.Ancestor = AddHierarchy(TaskActivityElement.Cast(element.QueryId.Ancestor), ancestorDictionary);
                }
                AddHierarchyRecursiveMapper = 0;
            }
            return element;
        }

        private TaskActivityElement AddInternal(TaskActivityElement element, Dictionary<TaskActivityElement, TaskActivityElement> dictionary)
        {
            TaskActivityElement element2 = null;
            if (dictionary.TryGetValue(element, out element2))
            {
                return element2;
            }
            object[] args = { element };
            
                                    element.CacheProperties();
                        object lockObject = this.lockObject;
            lock (lockObject)
            {
                try
                {
                    if (!dictionary.ContainsKey(element))
                    {
                        dictionary.Add(element, element);
                    }
                }
                catch (ArgumentException)
                {
                    object[] objArray2 = { element };
                    
                }
            }
            return element;
        }



        private static bool CompareTitles(string sourceTitle, string titleToCompare) =>
            string.Equals(sourceTitle, titleToCompare, StringComparison.OrdinalIgnoreCase);

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

        public List<TaskActivityElement> ToList()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                object[] args = { Count };
                
                return new List<TaskActivityElement>(innerDictionary.Keys);
            }
        }

        private void UpdateWindowTitleData(TaskActivityElement element, string windowTitle)
        {
            if (!TitleExists(element.WindowTitles, windowTitle))
            {
                element.WindowTitles.Add(windowTitle);
            }
        }

        public int Count
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return innerDictionary.Count;
                }
            }
        }
    }
}