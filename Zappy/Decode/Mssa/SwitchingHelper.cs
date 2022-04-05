using System;
using System.Collections;
using System.Collections.Generic;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Mssa
{

    internal static class SwitchingHelper
    {

        private static ITaskActivityElement DoContainerBasedSwitchDown(UITechnologyManager plugin, ITaskActivityElement element, out UITechnologyManager switchedPlugin)
        {
            switchedPlugin = plugin;
            if (FrameworkUtilities.IsWindowlessSwitchContainer(element))
            {
                int num = 0;
                ITaskActivityElement element2 = null;
                foreach (UITechnologyManager manager in ZappyTaskService.Instance.PluginManager.TechnologyManagers)
                {
                    if (!UITechnologyManager.AreEqual(manager, plugin) && manager != ZappyTaskService.Instance.PluginManager.DefaultTechnologyManager)
                    {
                        int supportLevel = -1;
                        ITaskActivityElement element3 = null;
                        try
                        {
                            element3 = manager.ConvertToThisTechnology(element, out supportLevel);
                        }
                        catch (ZappyTaskException exception)
                        {
                            CrapyLogger.log.Error(exception);
                        }
                        if (element3 != null && supportLevel > num)
                        {
                            num = supportLevel;
                            element2 = element3;
                            switchedPlugin = manager;
                        }
                    }
                }
                try
                {
                    int num3 = -1;
                    ITaskActivityElement element4 = ZappyTaskService.Instance.PluginManager.DefaultTechnologyManager.ConvertToThisTechnology(element, out num3);
                    if (element4 != null && num3 > num)
                    {
                        element2 = element4;
                        num = num3;
                        switchedPlugin = ZappyTaskService.Instance.PluginManager.DefaultTechnologyManager;
                    }
                }
                catch (Exception)                 {
                    if (num < 1)
                    {
                        throw;
                    }
                }
                if (num < 1 || element2 == null)
                {
                    throw new Exception("TechnologyNotSupportedException");                }
                element2.SwitchingElement = element;
                element = element2;
                object[] args = { plugin.TechnologyName, switchedPlugin.TechnologyName, num };
                
            }
            return element;
        }

        internal static ITaskActivityElement DoSwitchingForElementFromPoint(ITaskActivityElement element, IntPtr windowHandle, int pointX, int pointY)
        {
            IntPtr ptr;
            UITechnologyManager manager;
            if (element == null)
            {
                return null;
            }
            IntPtr ptr2 = GetWindowLessSwitchingHandle(windowHandle, out ptr, out manager);
            ITaskActivityElement elementLeft = null;
            UITechnologyManager switchedPlugin = null;
            ITaskActivityElement elementFromPoint = element;
            try
            {
                if (!UITechnologyManager.AreEqual(manager.TechnologyName, element.Framework))
                {
                    elementFromPoint = manager.GetElementFromPoint(pointX, pointY);
                    if (elementFromPoint == null)
                    {
                        return null;
                    }
                }
                elementLeft = DoContainerBasedSwitchDown(manager, elementFromPoint, out switchedPlugin);
            }
                                                            catch (SystemException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            if (switchedPlugin != null && !UITechnologyManager.AreEqual(manager, switchedPlugin))
            {
                ITaskActivityElement elementRight = element;
                if (!UITechnologyManager.AreEqual(element.Framework, switchedPlugin.TechnologyName))
                {
                    elementRight = switchedPlugin.GetElementFromPoint(pointX, pointY);
                    if (elementRight == null)
                    {
                        return null;
                    }
                }
                if (!AbstractZappyTaskService.ElementEqualsIgnoreContainer(elementLeft, elementRight))
                {
                    elementRight.SwitchingElement = elementLeft;
                    elementFromPoint = elementRight;
                }
            }
            HackForWindowsFormsHostIssue(elementFromPoint);
            return elementFromPoint;
        }

        internal static ITaskActivityElement DoSwitchingForFocusedElement(ITaskActivityElement element, IntPtr windowHandle)
        {
            IntPtr ptr;
            UITechnologyManager manager;
            if (element == null || element.SwitchingElement != null)
            {
                return element;
            }
            IntPtr handle = GetWindowLessSwitchingHandle(windowHandle, out ptr, out manager);
            ITaskActivityElement focusedElement = element;
            UITechnologyManager switchedPlugin = null;
            try
            {
                if (!UITechnologyManager.AreEqual(element.Framework, manager.TechnologyName))
                {
                    focusedElement = manager.GetFocusedElement(handle);
                    if (focusedElement == null)
                    {
                        return null;
                    }
                }
                focusedElement = DoContainerBasedSwitchDown(manager, focusedElement, out switchedPlugin);
            }
            catch (SystemException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            if (switchedPlugin != null && !UITechnologyManager.AreEqual(manager, switchedPlugin))
            {
                ITaskActivityElement elementRight = element;
                if (!UITechnologyManager.AreEqual(element.Framework, switchedPlugin.TechnologyName))
                {
                    elementRight = switchedPlugin.GetFocusedElement(windowHandle);
                    if (elementRight == null)
                    {
                        return null;
                    }
                }
                if (AbstractZappyTaskService.ElementEqualsIgnoreContainer(focusedElement, elementRight))
                {
                    focusedElement = focusedElement.SwitchingElement;
                }
                else
                {
                    elementRight.SwitchingElement = focusedElement;
                    focusedElement = elementRight;
                }
            }
            HackForWindowsFormsHostIssue(focusedElement);
            return focusedElement;
        }

        internal static IEnumerator<TaskActivityElement> GetChildrenEnumerator(ITaskActivityElement element, object parsedQueryIdCookie)
        {
            UITechnologyManager manager2;
            if (element == null)
            {
                return null;
            }
            ITaskActivityElement element2 = element;
            element = ZappyTaskService.Instance.ConvertToMappedTechnologyElement(element);
            bool flag = false;
            if (element == null)
            {
                return null;
            }
            if (!element2.Equals(element) && GetTechnologyBoundaryHandle(element2.Framework, element2.WindowHandle) != element2.WindowHandle)
            {
                flag = true;
            }
            UITechnologyManager plugin = ZappyTaskService.Instance.TechnologyManagerByName(element.Framework);
            element = DoContainerBasedSwitchDown(plugin, element, out manager2);
            ITaskActivityElement switchingElement = null;
            if (!UITechnologyManager.AreEqual(plugin, manager2))
            {
                switchingElement = element;
            }
            else if (flag)
            {
                element.SwitchingElement = element2;
                switchingElement = element;
            }
            else
            {
                switchingElement = element2.SwitchingElement;
            }
            IEnumerator children = manager2.GetChildren(element, parsedQueryIdCookie);
            if (children != null)
            {
                if (!children.MoveNext() && !Equals(element.Framework, element2.Framework))
                {
                    switchingElement = element2;
                    children = new[] { element }.GetEnumerator();
                }
                else
                {
                    children.Reset();
                }
            }
                        return new ChildrenEnumerator(children, element, switchingElement);
        }

        private static UITechnologyManager GetHostingTechnologyByWindowWalkUp(IntPtr windowHandle, out IntPtr hostingWindowHandle, out IntPtr boundaryWindowHandle, out UITechnologyManager technologyManager)
        {
            string technologyName = string.Empty;
            UITechnologyManager manager = ZappyTaskService.Instance.TechnologyManagerFromHandle(windowHandle);
            if (manager != null)
            {
                technologyName = manager.TechnologyName;
            }
            return GetHostingTechnologyByWindowWalkUp(technologyName, windowHandle, out hostingWindowHandle, out boundaryWindowHandle, out technologyManager);
        }

        private static UITechnologyManager GetHostingTechnologyByWindowWalkUp(string technologyName, IntPtr windowHandle, out IntPtr hostingWindowHandle, out IntPtr boundaryWindowHandle, out UITechnologyManager technologyManager)
        {
            technologyManager = ZappyTaskService.Instance.TechnologyManagerByName(technologyName);
            UITechnologyManager manager = technologyManager;
            boundaryWindowHandle = windowHandle;
            hostingWindowHandle = NativeMethods.GetAncestor(boundaryWindowHandle, NativeMethods.GetAncestorFlag.GA_PARENT);
            while (hostingWindowHandle != IntPtr.Zero && NativeMethods.IsWindow(hostingWindowHandle))
            {
                manager = ZappyTaskService.Instance.TechnologyManagerFromHandle(hostingWindowHandle);
                if (!UITechnologyManager.AreEqual(manager, technologyManager))
                {
                    return manager;
                }
                boundaryWindowHandle = hostingWindowHandle;
                hostingWindowHandle = NativeMethods.GetAncestor(boundaryWindowHandle, NativeMethods.GetAncestorFlag.GA_PARENT);
            }
            return manager;
        }

        internal static IntPtr GetTechnologyBoundaryHandle(string technologyName, IntPtr windowHandle)
        {
            IntPtr ptr;
            IntPtr ptr2;
            UITechnologyManager manager;
            GetHostingTechnologyByWindowWalkUp(technologyName, windowHandle, out ptr, out ptr2, out manager);
            if (ptr != IntPtr.Zero && ptr2 != IntPtr.Zero)
            {
                windowHandle = ptr2;
            }
            return windowHandle;
        }

        private static IntPtr GetWindowLessSwitchingHandle(IntPtr windowHandle, out IntPtr boundaryWindowHandle, out UITechnologyManager pluginToUse)
        {
            IntPtr ptr;
            UITechnologyManager manager;
            UITechnologyManager techManager = GetHostingTechnologyByWindowWalkUp(windowHandle, out ptr, out boundaryWindowHandle, out manager);
            if (UITechnologyManager.GetTechnologyManagerProperty<bool>(manager, UITechnologyManagerProperty.WindowLessTreeSwitchingSupported) || !UITechnologyManager.GetTechnologyManagerProperty<bool>(techManager, UITechnologyManagerProperty.WindowLessTreeSwitchingSupported))
            {
                pluginToUse = manager;
                return windowHandle;
            }
            pluginToUse = techManager;
            return ptr;
        }

        internal static void HackForWindowsFormsHostIssue(ITaskActivityElement element)
        {
            if (element != null && element.SwitchingElement == null && NativeMethods.IsWindow(element.WindowHandle))
            {
                IntPtr ptr;
                IntPtr ptr2;
                UITechnologyManager manager;
                UITechnologyManager manager2 = GetHostingTechnologyByWindowWalkUp(element.Framework, element.WindowHandle, out ptr2, out ptr, out manager);
                if (ptr2 != IntPtr.Zero && ptr != ptr2)
                {
                    IntPtr ptr3;
                    IntPtr ptr4;
                    UITechnologyManager manager3;
                    GetHostingTechnologyByWindowWalkUp(ptr2, out ptr4, out ptr3, out manager3);
                    if (ptr4 != IntPtr.Zero)
                    {
                        element.SwitchingElement = manager.GetElementFromWindowHandle(ptr);
                        if (element.SwitchingElement != null)
                        {
                            element.SwitchingElement.SwitchingElement = manager2.GetElementFromWindowHandle(ptr);
                        }
                    }
                }
            }
        }
    }
}