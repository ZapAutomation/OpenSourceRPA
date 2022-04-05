using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;


namespace Zappy.Decode.Helper
{
    internal abstract class AbstractZappyTaskService : IZappyTaskServiceOptimized, IZappyTaskService, IDisposable
    {
        protected bool eatNavigationTimeoutException = true;
        protected IZappyTaskExtensionPackageManager extensionPackageManager;
        protected Dictionary<ZappyTaskEventType, List<UITechnologyManager>> globalEventMap;
        protected static AbstractZappyTaskService instance;
        protected bool isSessionStarted;
        protected static readonly object lockObject = new object();
        protected IUITechnologyPluginManager pluginManager;
        protected Dictionary<string, string> proxyToCoreMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        protected TaskActivityElement rootElement;

        protected AbstractZappyTaskService()
        {
        }

        public bool AddEventHandler(TaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            bool flag;
            ZappyTaskUtilities.CheckForNull(element, "element");
            ZappyTaskUtilities.CheckForNull(eventSink, "eventSink");
            object[] args = new object[] { element.Name };
            
            try
            {
                flag = this.TechnologyManagerFromElement(element).AddEventHandler(element, eventType, eventSink);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, element);
                throw;
            }
            return flag;
        }

        public void AddExtensionToCoreTechnologyMapping(string extensionTechnologyName, string coreTechnologyName)
        {
            if ((!string.IsNullOrEmpty(extensionTechnologyName) && !string.IsNullOrEmpty(coreTechnologyName)) && !this.proxyToCoreMap.ContainsKey(extensionTechnologyName))
            {
                this.proxyToCoreMap.Add(extensionTechnologyName, coreTechnologyName);
            }
        }

        public bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            
            bool flag = false;
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (this.IsSessionStarted && !this.globalEventMap.ContainsKey(eventType))
                {
                    List<UITechnologyManager> list = new List<UITechnologyManager>();
                    this.globalEventMap.Add(eventType, list);
                    foreach (UITechnologyManager manager in this.PluginManager.TechnologyManagers)
                    {
                        if (manager.AddGlobalEventHandler(eventType, eventSink))
                        {
                            list.Add(manager);
                            flag = true;
                        }
                    }
                }
                return flag;
            }
            return flag;
        }

        public void Cleanup()
        {
            
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                this.StopSession();
            }
        }

        public virtual TaskActivityElement ConvertTechnologyElement(ITaskActivityElement unmanagedMsaaElement) =>
            TaskActivityElement.Cast(unmanagedMsaaElement);

        private ITaskActivityElement ConvertToCorrectTechnology(ITaskActivityElement element)
        {
            int num = 0;
            int supportLevel = 0;
            ITaskActivityElement element2 = element;
            foreach (UITechnologyManager manager in this.PluginManager.TechnologyManagers)
            {
                ITaskActivityElement element3 = null;
                try
                {
                    element3 = manager.ConvertToThisTechnology(element, out supportLevel);
                }
                catch (Exception)
                {
                    
                }
                if (supportLevel > num)
                {
                    element2 = element3;
                    num = supportLevel;
                }
            }
            return element2;
        }

        public virtual TaskActivityElement ConvertToMappedTechnologyElement(ITaskActivityElement element) =>
            TaskActivityElement.Cast(element);

        public virtual void Dispose()
        {
            
            this.Cleanup();
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (this.extensionPackageManager != null)
                {
                    this.extensionPackageManager.Dispose();
                    this.extensionPackageManager = null;
                }
                this.DisposeInternal();
                instance = null;
            }
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeInternal()
        {
        }

        internal static bool ElementEqualsIgnoreContainer(ITaskActivityElement elementLeft, ITaskActivityElement elementRight)
        {
            ZappyTaskUtilities.CheckForNull(elementLeft, "parent");
            ZappyTaskUtilities.CheckForNull(elementRight, "containerElement");
            if (elementRight.SwitchingElement != elementLeft.SwitchingElement)
            {
                ITaskActivityElement switchingElement = elementRight.SwitchingElement;
                ITaskActivityElement element2 = elementLeft.SwitchingElement;
                elementRight.SwitchingElement = null;
                elementLeft.SwitchingElement = null;
                bool flag = elementRight.Equals(elementLeft);
                elementRight.SwitchingElement = switchingElement;
                elementLeft.SwitchingElement = element2;
                return flag;
            }
            return elementRight.Equals(elementLeft);
        }

        private ITaskActivityElement GetBoundaryElementFromQueryID(ITaskActivityElement element)
        {
            while (((element != null) && (element.QueryId != null)) && (element.QueryId.Ancestor != null))
            {
                if (!string.Equals(element.Framework, element.QueryId.Ancestor.Framework, StringComparison.OrdinalIgnoreCase))
                {
                    return element.QueryId.Ancestor;
                }
                element = element.QueryId.Ancestor;
            }
            return null;
        }

        public virtual IEnumerator<TaskActivityElement> GetChildren(ITaskActivityElement element, object parsedQueryIdCookie)
        {
            throw new NotImplementedException();
        }

        public string GetCoreTechnologyName(string extensionTechnologyName)
        {
            if (this.proxyToCoreMap.ContainsKey(extensionTechnologyName))
            {
                return this.proxyToCoreMap[extensionTechnologyName];
            }
            return null;
        }

        public TaskActivityElement GetElementFromNativeElement(string technologyName, object nativeElement)
        {
            ZappyTaskUtilities.CheckForNull(technologyName, "technologyName");
            ZappyTaskUtilities.CheckForNull(nativeElement, "nativeElement");
            object[] args = new object[] { technologyName, nativeElement };
            
            ITaskActivityElement elementFromNativeElement = null;
            try
            {
                UITechnologyManager manager = this.TechnologyManagerByName(technologyName);
                if (manager != null)
                {
                    elementFromNativeElement = manager.GetElementFromNativeElement(nativeElement);
                }
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null);
                throw;
            }
            if (elementFromNativeElement == null)
            {
                            }
            return TaskActivityElement.Cast(elementFromNativeElement);
        }


        public virtual TaskActivityElement GetElementFromPoint(int pointX, int pointY)
        {
                        TaskActivityElement taskActivityElementFromPoint;
                        {

                try
                {

                    AutomationElement element = AutomationElement.FromPoint(new Point(pointX, pointY));
                    taskActivityElementFromPoint = TaskActivityElement.Cast(this.GetTechnologyElementFromAE(element, pointX, pointY, true));
                                                        }
                catch (Win32Exception exception)
                {
                    CrapyLogger.log.Error(exception);
                    throw new ZappyTaskControlNotAvailableException(exception.Message, exception);
                }
                catch (Exception exception2)
                {
                    
                    MapAndThrowException(exception2, null);
                    throw;
                }
            }
                                                                                                                                                return taskActivityElementFromPoint;
        }

        public virtual TaskActivityElement GetElementFromWindowHandle(IntPtr handle)
        {
            object[] args = new object[] { handle };
            
            ZappyTaskUtilities.CheckForNull(handle, "handle");
            if (ZappyTaskUtilities.IsHigherPrivilegeProcess(handle))
            {
                throw new ZappyTaskControlNotAvailableException(Resources.ErrorHigherPrivilegeProcess);
            }
            return TaskActivityElement.Cast(this.PluginManager.GetTechnologyManagerByAutomationElementOrWindowHandle(handle).GetElementFromWindowHandle(handle));
        }


        public IList<T> GetExtensions<T>() where T : class
        {
            if (this.extensionPackageManager != null)
            {
                return this.extensionPackageManager.GetExtensions<T>();
            }
            return null;
        }

        public TaskActivityElement GetFirstChild(TaskActivityElement element) =>
            this.GetFirstChildInternal(element, false);

        public TaskActivityElement GetFirstChildFast(TaskActivityElement element) =>
            this.GetFirstChildInternal(element, true);

        private TaskActivityElement GetFirstChildInternal(TaskActivityElement element, bool withoutTreeWalker)
        {
            TaskActivityElement element3;
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                IEnumerator children = null;
                ITaskActivityElement navigatedElement = null;
                UITechnologyManager manager = this.TechnologyManagerFromElement(element);
                if (manager != null)
                {
                    children = manager.GetChildren(element, null);
                }
                if ((children != null) && children.MoveNext())
                {
                    navigatedElement = children.Current as ITaskActivityElement;
                }
                if (withoutTreeWalker)
                {
                    return (navigatedElement as TaskActivityElement);
                }
                GetNavigationElemDelegate getNavigationElem = new GetNavigationElemDelegate(TreeWalker.ControlViewWalker.GetFirstChild);
                element3 = this.GetNavigationElementForBoundary(element, navigatedElement, getNavigationElem);
            }
            catch (Exception exception)
            {
                if (!(exception is TimeoutException) || this.EatNavigationTimeoutException)
                {
                    MapAndThrowException(exception, element);
                }
                throw;
            }
            return element3;
        }

        public virtual TaskActivityElement GetFocusedElement()
        {
            TaskActivityElement element3;
            
            try
            {
                ITaskActivityElement focusedElement = this.PluginManager.DefaultTechnologyManager.GetFocusedElement(IntPtr.Zero);
                if (ZappyTaskUtilities.IsRemoteTestingEnabled)
                {
                    return (focusedElement as TaskActivityElement);
                }
                IntPtr windowHandle = focusedElement.WindowHandle;
                if (ZappyTaskUtilities.IsHigherPrivilegeProcess(windowHandle))
                {
                    throw new ZappyTaskControlNotAvailableException(Resources.ErrorHigherPrivilegeProcess);
                }
                UITechnologyManager manager = this.TechnologyManagerFromHandle(windowHandle);
                ITaskActivityElement element = null;
                try
                {
                    element = TaskActivityElement.Cast(manager.GetFocusedElement(windowHandle));
                }
                catch (NotImplementedException)
                {
                }
                catch (NotSupportedException)
                {
                }
                if (element == null)
                {
                    CrapyLogger.log.Error("FocusedElement from plugin returned null, throwing ZappyTaskControlNotAvailableException exception");
                    throw new ZappyTaskControlNotAvailableException(Resources.FailedToGetUIElement);
                }
                element3 = TaskActivityElement.Cast(element);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null);
                throw;
            }
            return element3;
        }

        private TaskActivityElement GetNavigationElementForBoundary(TaskActivityElement currentElement, ITaskActivityElement navigatedElement, GetNavigationElemDelegate getNavigationElem)
        {
            TaskActivityElement element5;
            try
            {
                if (navigatedElement != null)
                {
                    if ((navigatedElement as TaskActivityElement).AutomationElement == null)
                    {
                        navigatedElement = this.ConvertToCorrectTechnology(navigatedElement);
                        ITaskActivityElement boundaryElementFromQueryID = this.GetBoundaryElementFromQueryID(currentElement);
                        ITaskActivityElement ancestor = navigatedElement;
                        ITaskActivityElement element3 = null;
                        while ((ancestor != null) && (ancestor.QueryId != null))
                        {
                            element3 = ancestor;
                            ancestor = ancestor.QueryId.Ancestor;
                            if ((ancestor != null) && (element3.Framework != ancestor.Framework))
                            {
                                break;
                            }
                        }
                        if (element3 != null)
                        {
                            element3.QueryId.Ancestor = boundaryElementFromQueryID;
                        }
                    }
                    else
                    {
                        navigatedElement = this.GetTechnologyElementFromAE((navigatedElement as TaskActivityElement).AutomationElement, -1, -1, false);
                    }
                }
                else if (currentElement.AutomationElement != null)
                {
                    AutomationElement element = null;
                    element = getNavigationElem(currentElement.AutomationElement);
                    if (element != null)
                    {
                        navigatedElement = this.GetTechnologyElementFromAE(element, -1, -1, false);
                    }
                    if (Automation.Compare(currentElement.AutomationElement, element))
                    {
                        navigatedElement = null;
                    }
                }
                if (navigatedElement != null)
                {
                    return TaskActivityElement.Cast(navigatedElement);
                }
                element5 = null;
            }
            catch (Exception exception)
            {
                if (!(exception is TimeoutException) || this.EatNavigationTimeoutException)
                {
                    MapAndThrowException(exception, navigatedElement);
                }
                throw;
            }
            return element5;
        }

        public virtual TaskActivityElement GetNextSibling(TaskActivityElement element) =>
            this.GetNextSiblingInternal(element, false);

        public virtual TaskActivityElement GetNextSiblingFast(TaskActivityElement element) =>
            this.GetNextSiblingInternal(element, true);

        private TaskActivityElement GetNextSiblingInternal(TaskActivityElement element, bool withoutTreeWalker)
        {
            TaskActivityElement element3;
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                ITaskActivityElement navigatedElement = null;
                UITechnologyManager manager = this.TechnologyManagerFromElement(element);
                if (manager != null)
                {
                    navigatedElement = manager.GetNextSibling(element);
                }
                if (withoutTreeWalker)
                {
                    return (navigatedElement as TaskActivityElement);
                }
                GetNavigationElemDelegate getNavigationElem = new GetNavigationElemDelegate(TreeWalker.ControlViewWalker.GetNextSibling);
                element3 = this.GetNavigationElementForBoundary(element, navigatedElement, getNavigationElem);
            }
            catch (Exception exception)
            {
                if (!(exception is TimeoutException) || this.EatNavigationTimeoutException)
                {
                    MapAndThrowException(exception, element);
                }
                throw;
            }
            return element3;
        }

        public virtual TaskActivityElement GetParent(TaskActivityElement element) =>
            this.GetParentInternal(element, false);

        public virtual TaskActivityElement GetParentFast(TaskActivityElement element) =>
            this.GetParentInternal(element, true);

        private TaskActivityElement GetParentInternal(TaskActivityElement element, bool withoutTreeWalker)
        {
            TaskActivityElement element3;
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                ITaskActivityElement navigatedElement = null;
                UITechnologyManager manager = this.TechnologyManagerFromElement(element);
                if (manager != null)
                {
                    navigatedElement = manager.GetParent(element);
                }
                if (withoutTreeWalker)
                {
                    return (navigatedElement as TaskActivityElement);
                }
                GetNavigationElemDelegate getNavigationElem = new GetNavigationElemDelegate(TreeWalker.ControlViewWalker.GetParent);
                element3 = this.GetNavigationElementForBoundary(element, navigatedElement, getNavigationElem);
            }
            catch (Exception exception)
            {
                if (!(exception is TimeoutException) || this.EatNavigationTimeoutException)
                {
                    MapAndThrowException(exception, element);
                }
                throw;
            }
            return element3;
        }

        public virtual TaskActivityElement GetPreviousSibling(TaskActivityElement element) =>
            this.GetPreviousSiblingInternal(element, false);

        public virtual TaskActivityElement GetPreviousSiblingFast(TaskActivityElement element) =>
            this.GetPreviousSiblingInternal(element, true);

        private TaskActivityElement GetPreviousSiblingInternal(TaskActivityElement element, bool withoutTreeWalker)
        {
            TaskActivityElement element3;
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                ITaskActivityElement navigatedElement = null;
                UITechnologyManager manager = this.TechnologyManagerFromElement(element);
                if (manager != null)
                {
                    navigatedElement = manager.GetPreviousSibling(element);
                }
                if (withoutTreeWalker)
                {
                    return (navigatedElement as TaskActivityElement);
                }
                GetNavigationElemDelegate getNavigationElem = new GetNavigationElemDelegate(TreeWalker.ControlViewWalker.GetPreviousSibling);
                element3 = this.GetNavigationElementForBoundary(element, navigatedElement, getNavigationElem);
            }
            catch (Exception exception)
            {
                if (!(exception is TimeoutException) || this.EatNavigationTimeoutException)
                {
                    MapAndThrowException(exception, element);
                }
                throw;
            }
            return element3;
        }

        private ITaskActivityElement GetTechnologyElementFromAE(AutomationElement element, int pointX, int pointY, bool usePoints)
        {
            if (element != null)
            {
                List<List<AutomationElement>> list;
                List<UITechnologyManager> technologyManagersForAutomationHierarchy = this.GetTechnologyManagersForAutomationHierarchy(element, out list);
                ITaskActivityElement[] elementArray = this.GetTechnologyElementsFromRawElementsHierachy(technologyManagersForAutomationHierarchy, list, pointX, pointY, usePoints);
                for (int i = 0; i < (elementArray.Length - 1); i++)
                {
                    ITaskActivityElement ancestor = elementArray[i];
                    ITaskActivityElement element3 = null;
                    while ((ancestor != null) && (ancestor.QueryId != null))
                    {
                        element3 = ancestor;
                        ancestor = ancestor.QueryId.Ancestor;
                        if ((ancestor != null) && (ancestor.Framework == elementArray[i + 1].Framework))
                        {
                            return elementArray[0];
                        }
                    }
                    if (element3 != null)
                    {
                        element3.QueryId.Ancestor = elementArray[i + 1];
                    }
                }
                if (elementArray.Length != 0)
                {
                    return elementArray[0];
                }
            }
            return null;
        }

        private ITaskActivityElement[] GetTechnologyElementsFromRawElementsHierachy(List<UITechnologyManager> managersHierarchy, List<List<AutomationElement>> rawElementsGroup, int pointX, int pointY, bool usePoints)
        {
            List<ITaskActivityElement> list = new List<ITaskActivityElement>();
            List<UITechnologyManager>.Enumerator enumerator = managersHierarchy.GetEnumerator();
            List<List<AutomationElement>>.Enumerator enumerator2 = rawElementsGroup.GetEnumerator();
            bool flag = true;
            while (enumerator.MoveNext())
            {
                enumerator2.MoveNext();
                AutomationElement ceilingElement = enumerator2.Current[enumerator2.Current.Count - 1];
                if (flag & usePoints)
                {
                    list.Add(enumerator.Current.GetElementFromPoint(pointX, pointY, ceilingElement));
                }
                else
                {
                    list.Add(enumerator.Current.GetElementFromAutomationElement(enumerator2.Current[0], ceilingElement));
                }
                if (flag)
                {
                    flag = false;
                }
            }
            return list.ToArray();
        }

        private List<UITechnologyManager> GetTechnologyManagersForAutomationHierarchy(AutomationElement element, out List<List<AutomationElement>> rawElementsGroup)
        {
            List<UITechnologyManager> list = new List<UITechnologyManager>();
            rawElementsGroup = new List<List<AutomationElement>>();
            List<string> list2 = new List<string>();
            List<string> list3 = new List<string>();
            IUITechnologyManager manager = null;
            List<AutomationElement> item = null;
            while ((element != null) && (AutomationElement.RootElement != element))
            {
                UITechnologyManager technologyManagerByAutomationElementOrWindowHandle = this.PluginManager.GetTechnologyManagerByAutomationElementOrWindowHandle(element);
                if (manager != technologyManagerByAutomationElementOrWindowHandle)
                {
                    if ((item != null) && (item.Count > 0))
                    {
                        rawElementsGroup.Add(item);
                    }
                    manager = technologyManagerByAutomationElementOrWindowHandle;
                    list.Add(technologyManagerByAutomationElementOrWindowHandle);
                    item = new List<AutomationElement>();
                }
                item.Add(element);
                list2.Add(element.Current.ClassName);
                list3.Add(element.Current.ControlType.ProgrammaticName);
                if ((element.Current.NativeWindowHandle != 0) && (NativeMethods.GetAncestor((IntPtr)element.Current.NativeWindowHandle, NativeMethods.GetAncestorFlag.GA_PARENT) == NativeMethods.GetDesktopWindow()))
                {
                    break;
                }
                AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);
                if ((parent != null) && ((parent == element) || (TreeWalker.ControlViewWalker.GetParent(parent) == element)))
                {
                    break;
                }
                element = parent;
            }
            if ((item != null) && (item.Count > 0))
            {
                rawElementsGroup.Add(item);
            }
            if ((list.Count == 0) && (element != null))
            {
                list.Add(this.PluginManager.DefaultTechnologyManager);
                item = new List<AutomationElement> {
                    element
                };
                rawElementsGroup.Add(item);
            }
            return list;
        }

        public virtual TaskActivityElement GetTopLevelElementUsingWindowTree(ITaskActivityElement element)
        {
            if (element == null)
            {
                return null;
            }
            IntPtr windowHandle = element.WindowHandle;
            IntPtr hwnd = windowHandle;
            IntPtr ancestor = windowHandle;
            IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
            while ((ancestor != IntPtr.Zero) && (ancestor != desktopWindow))
            {
                hwnd = ancestor;
                ancestor = NativeMethods.GetAncestor(hwnd, NativeMethods.GetAncestorFlag.GA_PARENT);
            }
            ITaskActivityElement elementFromWindowHandle = null;
            if (hwnd != IntPtr.Zero)
            {
                if ((windowHandle == hwnd) && UITechnologyManager.AreEqual(element.Framework, this.PluginManager.DefaultTechnologyManager.TechnologyName))
                {
                    elementFromWindowHandle = element;
                }
                else
                {
                    elementFromWindowHandle = this.PluginManager.DefaultTechnologyManager.GetElementFromWindowHandle(hwnd);
                }
            }
            else
            {
                object[] args = new object[] { element };
                CrapyLogger.log.ErrorFormat("GetTopLevelElementUsingWindowTree: parentWindow is null for element {0}", args);
            }
            if (elementFromWindowHandle == null)
            {
                object[] objArray2 = new object[] { element };
                CrapyLogger.log.ErrorFormat("GetTopLevelElementUsingWindowTree: TopLevelElement returned null for element {0}", objArray2);
            }
            return TaskActivityElement.Cast(elementFromWindowHandle);
        }

        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        protected static void MapAndThrowException(Exception ex, ITaskActivityElement element)
        {
            if (((ex is InvalidOperationException) || (ex is TimeoutException)) || ((ex is COMException) || (ex is ElementNotAvailableException)))
            {
                if (element == null)
                {
                    throw new ZappyTaskControlNotAvailableException(ex.Message, ex);
                }
                            }
        }

        public bool RemoveEventHandler(TaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            bool flag;
            ZappyTaskUtilities.CheckForNull(element, "element");
            ZappyTaskUtilities.CheckForNull(eventSink, "eventSink");
            object[] args = new object[] { element.Name };
            
            try
            {
                flag = this.TechnologyManagerFromElement(element).RemoveEventHandler(element, eventType, eventSink);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, element);
                throw;
            }
            return flag;
        }

        public bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            
            bool flag = true;
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (this.IsSessionStarted && this.globalEventMap.ContainsKey(eventType))
                {
                    List<UITechnologyManager> list = this.globalEventMap[eventType];
                    this.globalEventMap.Remove(eventType);
                    foreach (UITechnologyManager manager in list)
                    {
                        if (!manager.RemoveGlobalEventHandler(eventType, eventSink))
                        {
                            object[] args = new object[] { eventType, manager.TechnologyName };
                            
                            flag = false;
                        }
                    }
                }
                return flag;
            }
            return flag;
        }

        protected static ITaskActivityElement SkipHigherPrivilegeProcesses(UITechnologyManager plugin, ITaskActivityElement sibling, bool getNextSibling)
        {
                        {
                while (((sibling != null) && FrameworkUtilities.IsTopLevelElement(sibling)) && ZappyTaskUtilities.IsHigherPrivilegeProcess(sibling.WindowHandle))
                {
                    if (getNextSibling)
                    {
                        sibling = plugin.GetNextSibling(sibling);
                    }
                    else
                    {
                        sibling = plugin.GetPreviousSibling(sibling);
                    }
                }
            }
            return sibling;
        }

        public void StartSession(bool recordingSession)
        {
            
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (!this.IsSessionStarted)
                {
                    this.IsSessionStarted = true;
                    this.globalEventMap = new Dictionary<ZappyTaskEventType, List<UITechnologyManager>>();
                    this.PluginManager.StartSession(recordingSession);
                    this.IsRecording = recordingSession;
                }
            }
        }

        public void StopSession()
        {
            
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (this.IsSessionStarted)
                {
                    if (this.pluginManager != null)
                    {
                        this.pluginManager.StopSession();
                        this.pluginManager = null;
                    }
                    this.IsSessionStarted = false;
                    this.rootElement = null;
                }
            }
        }

        public UITechnologyManager TechnologyManagerByName(string technologyName)
        {
            if (this.PluginManager != null)
            {
                return this.PluginManager.GetTechnologyManagerByName(technologyName);
            }
            return null;
        }

        protected UITechnologyManager TechnologyManagerFromElement(ITaskActivityElement element) =>
            this.TechnologyManagerByName(element.Framework);

        public virtual UITechnologyManager TechnologyManagerFromHandle(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                return this.PluginManager.GetTechnologyManagerByAutomationElementOrWindowHandle(handle);
            }
            return this.PluginManager.DefaultTechnologyManager;
        }

        public virtual void UpdateQueryIdForTopLevelElement(TaskActivityElement element)
        {
        }

        public bool EatNavigationTimeoutException
        {
            get =>
                this.eatNavigationTimeoutException;
            set
            {
                this.eatNavigationTimeoutException = value;
            }
        }

        public bool IsRecording { get; set; }

        public bool IsSessionStarted
        {
            get
            {
                object lockObject = AbstractZappyTaskService.lockObject;
                lock (lockObject)
                {
                    return this.isSessionStarted;
                }
            }
            private set
            {
                object lockObject = AbstractZappyTaskService.lockObject;
                lock (lockObject)
                {
                    this.isSessionStarted = value;
                }
            }
        }

        public abstract IUITechnologyPluginManager PluginManager { get; }

        public virtual TaskActivityElement RootElement
        {
            get
            {
                
                if (this.rootElement == null)
                {
                    IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
                    if (desktopWindow != IntPtr.Zero)
                    {
                        this.rootElement = TaskActivityElement.Cast(this.PluginManager.DefaultTechnologyManager.GetElementFromWindowHandle(desktopWindow));
                    }
                    else
                    {
                        this.rootElement = this.PluginManager.DefaultTechnologyManager.GetRootElement();
                    }
                }
                return this.rootElement;
            }
        }

        private delegate AutomationElement GetNavigationElemDelegate(AutomationElement element);
    }



}
