using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Plugins.Excel;
using Zappy.Properties;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Decode.Helper
{
    [Guid("4EC67AB6-66B9-4954-B2DD-5B75DD6CBC19")]
    internal sealed class ZappyTaskService : AbstractZappyTaskService
    {
        private const string IEPluginAssemblyName = "IE";

        private ZappyTaskService()
        {
            
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }
        }

        public override TaskActivityElement ConvertTechnologyElement(ITaskActivityElement element)
        {
            ZappyTaskUtilities.CheckForNull(element, "element");
            ITaskActivityElement elementFromNativeElement = element;
            if (ZappyTaskUtilities.IsNativeMsaaElement(element))
            {
                elementFromNativeElement = GetElementFromNativeElement(element.Framework, element.NativeElement);
            }
            ITaskActivityElement switchingElement = element.SwitchingElement;
            for (ITaskActivityElement element4 = elementFromNativeElement; switchingElement != null; element4 = element4.SwitchingElement)
            {
                if (ZappyTaskUtilities.IsNativeMsaaElement(switchingElement))
                {
                    element4.SwitchingElement = GetElementFromNativeElement(switchingElement.Framework, switchingElement.NativeElement);
                }
                else
                {
                    element4.SwitchingElement = switchingElement;
                }
                switchingElement = switchingElement.SwitchingElement;
            }
            SwitchingHelper.HackForWindowsFormsHostIssue(elementFromNativeElement);
            return TaskActivityElement.Cast(elementFromNativeElement);
        }

        public override TaskActivityElement ConvertToMappedTechnologyElement(ITaskActivityElement element)
        {
            
            if (element != null && element.SwitchingElement == null && element.WindowHandle != IntPtr.Zero)
            {
                UITechnologyManager manager;
                TaskActivityElement element2 = element as TaskActivityElement;
                AutomationElement automationElement = null;
                if (element2 != null)
                {
                    automationElement = element2.AutomationElement;
                }
                if (automationElement != null)
                {
                    manager = TechnologyManagerFromAutomationElement(automationElement);
                }
                else
                {
                    manager = TechnologyManagerFromHandle(element.WindowHandle);
                }
                if (!UITechnologyManager.AreEqual(element.Framework, manager.TechnologyName))
                {
                    if (automationElement != null)
                    {
                        element = manager.GetElementFromAutomationElement(automationElement, null);
                    }
                    else
                    {
                        element = manager.GetElementFromWindowHandle(element.WindowHandle);
                    }
                }
            }
            return TaskActivityElement.Cast(element);
        }

        private ITaskActivityElement ConvertToTechnologyElement(UITechnologyManager technologyManager, ITaskActivityElement element)
        {
            if (element != null && technologyManager != null && !UITechnologyManager.AreEqual(element.Framework, technologyManager.TechnologyName))
            {
                element = technologyManager.GetElementFromWindowHandle(element.WindowHandle);
            }
            return element;
        }

                                                                        
        protected override void DisposeInternal()
        {
            if (!ZappyTaskUtilities.IsRemoteTestingEnabled)
            {
                DisposeMsaaTestPluginInstance();
            }
        }

        private void DisposeMsaaTestPluginInstance()
        {
            if (MsaaZappyPlugin.Instance != null)
            {
                MsaaZappyPlugin.Instance.Dispose();
            }
        }

        private ITaskActivityElement FocusedElementFromDefaultPlugin(ITaskActivityElement element)
        {
            if (element != null && (ControlType.ListItem.NameEquals(element.ControlTypeName) || ControlType.Empty.Equals(element.ControlTypeName)))
            {
                element = GetFocusedElementUsingThread();
            }
            return element;
        }

        public override IEnumerator<TaskActivityElement> GetChildren(ITaskActivityElement element, object parsedQueryIdCookie)
        {
            IEnumerator<TaskActivityElement> childrenEnumerator;
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                childrenEnumerator = SwitchingHelper.GetChildrenEnumerator(element, parsedQueryIdCookie);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, element);
                throw;
            }
            return childrenEnumerator;
        }

        public override TaskActivityElement GetElementFromWindowHandle(IntPtr handle)
        {
                        
            ZappyTaskUtilities.CheckForNull(handle, "handle");
            ThrowIfHigherPrivilegeProcess(handle);
            ITaskActivityElement elementFromWindowHandle = PluginManager.GetTechnologyManagerByAutomationElementOrWindowHandle(handle).GetElementFromWindowHandle(handle);
            SwitchingHelper.HackForWindowsFormsHostIssue(elementFromWindowHandle);
            return IfTechnologyRootElementThenConvertInParentTechnology(elementFromWindowHandle, false);
        }


        public override TaskActivityElement GetFocusedElement()
        {
            TaskActivityElement element3;
            
            try
            {
                ITaskActivityElement focusedElement = PluginManager.DefaultTechnologyManager.GetFocusedElement(IntPtr.Zero);
                if (ZappyTaskUtilities.IsRemoteTestingEnabled)
                {
                    return focusedElement as TaskActivityElement;
                }
                if (focusedElement == null)
                {
                    
                    return GetFocusedElementUsingThread();
                }
                IntPtr windowHandle = focusedElement.WindowHandle;
                ThrowIfHigherPrivilegeProcess(windowHandle);
                UITechnologyManager manager = TechnologyManagerFromHandle(windowHandle);
                ITaskActivityElement element = null;
                if (UITechnologyManager.AreEqual(manager, PluginManager.DefaultTechnologyManager))
                {
                    element = FocusedElementFromDefaultPlugin(focusedElement);
                }
                else
                {
                    try
                    {
                        element = TaskActivityElement.Cast(manager.GetFocusedElement(windowHandle));
                    }
                    catch (NotImplementedException)
                    {
                        element = FocusedElementFromDefaultPlugin(focusedElement);
                    }
                    catch (NotSupportedException)
                    {
                        element = FocusedElementFromDefaultPlugin(focusedElement);
                    }
                    if (element == null)
                    {
                        CrapyLogger.log.ErrorFormat("FocusedElement from plugin returned null, throwing ZappyTaskControlNotAvailableException exception");
                        throw new ZappyTaskControlNotAvailableException(Resources.FailedToGetUIElement);
                    }
                }
                element3 = ProcessTreeSwitchingFocusedElement(element, windowHandle);
            }
            catch (SystemException exception)
            {
                MapAndThrowException(exception, null);
                CrapyLogger.log.Error(exception);

                throw;
            }
            return element3;
        }


        private TaskActivityElement GetFocusedElementUsingThread()
        {
            TaskActivityElement element2;
            try
            {
                ITaskActivityElement focusedElement = null;
                NativeMethods.GUITHREADINFO structure = new NativeMethods.GUITHREADINFO();
                structure.cbSize = (uint)Marshal.SizeOf(structure);
                if (NativeMethods.GetGUIThreadInfo(0, out structure) && structure.hwndFocus != IntPtr.Zero)
                {
                    IntPtr hwndFocus = structure.hwndFocus;
                    ThrowIfHigherPrivilegeProcess(hwndFocus);
                    focusedElement = TechnologyManagerFromHandle(hwndFocus).GetFocusedElement(hwndFocus);
                    if (focusedElement != null)
                    {
                                                    focusedElement = ProcessTreeSwitchingFocusedElement(focusedElement, hwndFocus);
                    }
                }
                if (focusedElement == null)
                {
                    CrapyLogger.log.WarnFormat("GetFocusedElementUsingThread returned null");
                    throw new ZappyTaskControlNotAvailableException(Resources.FailedToGetUIElement);
                }
                element2 = TaskActivityElement.Cast(focusedElement);
            }
            catch (SystemException exception)
            {
                MapAndThrowException(exception, null);
                throw;
            }
            return element2;
        }

        private UITechnologyManager GetMsaaTestPluginInstance() =>
                        MsaaZappyPlugin.Instance;

        private TaskActivityElement IfTechnologyRootElementThenConvertInParentTechnology(ITaskActivityElement element, bool addAsSwitchingElement)
        {
            ITaskActivityElement element2 = element;
            if (element2 != null && element2.WindowHandle != NativeMethods.GetDesktopWindow() && element2.SwitchingElement == null)
            {
                UITechnologyManager manager = TechnologyManagerFromElement(element2);
                if (manager != null && manager.GetParent(element2) == null && !UITechnologyManager.AreEqual(manager, PluginManager.DefaultTechnologyManager))
                {
                    UITechnologyManager defaultTechnologyManager = PluginManager.DefaultTechnologyManager;
                    ITaskActivityElement element4 = ConvertToTechnologyElement(defaultTechnologyManager, element2);
                    if (element4 != null)
                    {
                        ITaskActivityElement parent = defaultTechnologyManager.GetParent(element4);
                        if (parent != null)
                        {
                            manager = TechnologyManagerFromHandle(parent.WindowHandle);
                            element2 = ConvertToTechnologyElement(manager, element2);
                        }
                    }
                }
            }
            if (addAsSwitchingElement && element2 != null && !Equals(element2.Framework, element.Framework) && !FrameworkUtilities.IsTopLevelElement(element2))
            {
                element.SwitchingElement = element2;
                return TaskActivityElement.Cast(element);
            }
            return TaskActivityElement.Cast(element2);
        }

        public override void Initialize()
        {
            
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                if (extensionPackageManager == null)
                {
                    extensionPackageManager = ZappyTaskExtensionPackageManager.Instance;
                                    }
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            object lockObject = AbstractZappyTaskService.lockObject;
            lock (lockObject)
            {
                AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
                Dispose();
            }
        }

        private TaskActivityElement ProcessTreeSwitchingFocusedElement(ITaskActivityElement element, IntPtr windowHandle)
        {
            if (element == null)
            {
                object[] args = { windowHandle };
                CrapyLogger.log.ErrorFormat("ProcessTreeSwitchingFocusedElement:: GetFocusedElement returned null for window handle {0}", args);
                throw new ZappyTaskControlNotAvailableException(Resources.FailedToGetUIElement);
            }
            element = SwitchingHelper.DoSwitchingForFocusedElement(element, windowHandle);
            return IfTechnologyRootElementThenConvertInParentTechnology(element, true);
        }

        public UITechnologyManager TechnologyManagerFromAutomationElement(AutomationElement element)
        {
            if (element != null)
            {
                return PluginManager.GetTechnologyManagerByAutomationElementOrWindowHandle(element);
            }
            return PluginManager.DefaultTechnologyManager;
        }

        private static void ThrowIfHigherPrivilegeProcess(IntPtr handle)
        {
            if (NativeMethods.GetDesktopWindow() != handle && ZappyTaskUtilities.IsHigherPrivilegeProcess(handle))
            {
                throw new ZappyTaskControlNotAvailableException(Resources.ErrorHigherPrivilegeProcess);
            }
        }

        private static void UpdateQueryIdForWindowLessSwitching(ITaskActivityElement element)
        {
                        
            ITaskActivityElement switchingElement = element.SwitchingElement.SwitchingElement;
            ITaskActivityElement element3 = switchingElement;
            if (switchingElement == null)
            {
                element3 = switchingElement = element.SwitchingElement;
            }
            else
            {
                if (FrameworkUtilities.IsWindowlessSwitchContainer(switchingElement))
                {
                    element3 = element.SwitchingElement;
                }
                else if (switchingElement.Equals(element.SwitchingElement.QueryId.Ancestor))
                {
                    return;
                }
                element.SwitchingElement.QueryId.Ancestor = switchingElement;
            }
            ITaskActivityElement element4 = element;
            ITaskActivityElement element5 = element;
        Label_006F:
            element4 = element4.QueryId.Ancestor;
            if (element4 == null)
            {
                element5.QueryId.Ancestor = element3;
            }
            else if (element4.Equals(element.SwitchingElement))
            {
                element5.QueryId.Ancestor = element3;
            }
            else
            {
                element5 = element4;
                goto Label_006F;
            }
        }

        public static AbstractZappyTaskService Instance
        {
            get
            {
                if (instance == null)
                {
                    object lockObject = AbstractZappyTaskService.lockObject;
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new ZappyTaskService();
                        }
                    }
                }
                return instance;
            }
            internal set
            {
                instance = value;
            }
        }

        public override IUITechnologyPluginManager PluginManager
        {
            get
            {
                if (ReferenceEquals(pluginManager, null))
                {
                    object lockObject = AbstractZappyTaskService.lockObject;
                    lock (lockObject)
                    {
                        if (ReferenceEquals(pluginManager, null) && !ReferenceEquals(extensionPackageManager, null))
                        {
                            IList<UITechnologyManager> extensions = extensionPackageManager.GetExtensions<UITechnologyManager>();
                                                                                    
                                                                                                                                                                        pluginManager = new ZappyTaskPluginManager(extensions, GetMsaaTestPluginInstance());
                                                    }
                    }
                }
                return pluginManager;
            }
        }
    }



}
