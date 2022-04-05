using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Plugins.Uia.Uia;


namespace Zappy.Decode.Hooks.Keyboard
{
    public static class FocusElement
    {
        private static Dictionary<string, ITaskActivityElement> dictAutomationElem = new Dictionary<string, ITaskActivityElement>();
        public static IntPtr GetMainWindowHandle(string mainWindowTitle)
        {
            IntPtr windowHandle = (IntPtr)0;
            windowHandle = NativeMethods.FindWindow(null, mainWindowTitle);
            if (windowHandle == null)
                throw new ArgumentNullException(mainWindowTitle, "is not found");
            else
                return windowHandle;
        }
        public static void ShowIfMinimized(IntPtr windowHandle)
        {
            if (NativeMethods.IsIconic(windowHandle) && NativeMethods.IsWindowVisible(windowHandle))
            {
                                NativeMethods.ShowWindow(windowHandle, NativeMethods.WindowShowStyle.Restore);
            }
        }
        public static List<string> GetWindowTitles(ScreenIdentifier WindowIdentifier)
        {
            List<string> windowTitles = new List<string>();
            string descendant, ControlType;
            int i;
            TaskActivityObject uIObjectFromUIObjectId = WindowIdentifier.TopLevelWindows[0];
            for (i = 1; i < uIObjectFromUIObjectId.WindowTitles.Count; i++) ;
            windowTitles.Add(uIObjectFromUIObjectId.WindowTitles[i - 1]);

            while (uIObjectFromUIObjectId != null)
            {
                if (uIObjectFromUIObjectId.Descendants == null || uIObjectFromUIObjectId.Descendants.Count <= 0)
                    break;
                uIObjectFromUIObjectId = uIObjectFromUIObjectId.Descendants[0];
                ControlType = uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlType") as string;
                                if (ControlType == "ToolBar" && uIObjectFromUIObjectId.Descendants.Count != 0)
                    continue;
                if ((windowTitles.Count > 1) && (ControlType != null)
                    && ((ControlType == "MenuItem") || (ControlType == "DropDownButton")
                         || (ControlType == "Button") || (ControlType == "ListItem")
                         || (ControlType == "TreeItem")))
                    windowTitles.RemoveAt(windowTitles.Count - 1);
                descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlName") as string;
                if (descendant == null)
                    descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlId") as string;
                if (descendant == null)
                    descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("Name") as string;
                if (!string.IsNullOrEmpty(descendant))
                {
                    windowTitles.Add(descendant);
                }
            }
            return windowTitles;
        }

        public static void FindElement(AutomationElement autoElement, string descendant)
        {
            if (autoElement == null)
                return;
            AutomationElement nativeElement = TreeWalker.ControlViewWalker.GetFirstChild(autoElement);
            if (nativeElement == null)
                return;
            UiaElement childee = UiaElementFactory.GetUiaElement(autoElement, false);
                        if (!dictAutomationElem.ContainsKey(childee.QueryId.Condition.ToString()))
                dictAutomationElem.Add(childee.Name, childee);
            while (nativeElement != null)
            {
                childee = UiaElementFactory.GetUiaElement(nativeElement, false);
                                                if (!dictAutomationElem.ContainsKey(childee.QueryId.Condition.ToString()))
                    dictAutomationElem.Add(childee.QueryId.Condition.ToString(), childee);

                FindElement(nativeElement, descendant);
                nativeElement = TreeWalker.ControlViewWalker.GetNextSibling(nativeElement);
            }
        }
        public static ITaskActivityElement GetFocusELement(ScreenIdentifier WindowIdentifier, IntPtr WindowHandle)
        {
            ITaskActivityElement focusElement = null;
            string descendant = null;
            TaskActivityObject uIObjectFromUIObjectId = WindowIdentifier.TopLevelWindows[0];

            while (uIObjectFromUIObjectId != null && uIObjectFromUIObjectId.Descendants.Count != 0)
            {
                uIObjectFromUIObjectId = uIObjectFromUIObjectId.Descendants[0];
                if (uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlType") as string == "ComboBox")
                    descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("AutomationId") as string;
                if (!string.IsNullOrEmpty(descendant))
                    break;
            }

                                                                                    
                                    
                                                                                    

                                                                        if (descendant == null)
                descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlName") as string;
            if (descendant == null)
                descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("ControlId") as string;
            if (descendant == null)
                descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("Name") as string;
            if (descendant == null)
                descendant = uIObjectFromUIObjectId.Condition.GetPropertyValue("AutomationId") as string;

                        AutomationElement ee = AutomationElement.FromHandle(WindowHandle);

                        descendant = uIObjectFromUIObjectId.Condition.ToString();

                                    
            if (!dictAutomationElem.ContainsKey(WindowIdentifier.TopLevelWindows[0].WindowTitles[0]) ||
                !dictAutomationElem.ContainsKey(descendant))
                dictAutomationElem.Clear();
            if (dictAutomationElem.Count == 0)
                FindElement(ee, descendant);
            foreach (KeyValuePair<string, ITaskActivityElement> currentElement in dictAutomationElem)
            {
                if (currentElement.Key == descendant)
                {
                    focusElement = currentElement.Value;
                    break;
                }   
            }
                        
            if (focusElement == null)
            {
                throw new ArgumentNullException("CONTROL IS NOT FOUND");
            }
            return focusElement;
        }

    }
}