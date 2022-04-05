using Accessibility;
using System;
using System.Diagnostics;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    internal class WindowInstanceGetter
    {
        private ElementInfo elementInfo;
        private int elementInstance = 1;
        private IntPtr elementWindowHandle = IntPtr.Zero;
        private bool isWindowVisible;
        private const int timeOut = 200;
        private Stopwatch timeOutWatch;

        private bool EnumChildWindows(IntPtr hWnd, ref IntPtr lParam)
        {
                                    if (timeOutWatch.ElapsedMilliseconds > 200L)
            {
                
                return false;
            }
            if (hWnd == elementWindowHandle)
            {
                lParam = hWnd;
                return false;
            }
            if (isWindowVisible && !NativeMethods.IsWindowVisible(hWnd))
            {
                return true;
            }
            if (elementInfo.MatchWin32ElementInfo(hWnd))
            {
                elementInstance++;
            }
                        return true;
        }

        internal int GetInstance(IntPtr parentWindowHandle, AccWrapper element, IQueryCondition[] elementQueryCondition, bool isVisibleOnly)
        {
            elementWindowHandle = element.WindowHandle;
            elementInfo = new ElementInfo(elementQueryCondition);
            isWindowVisible = isVisibleOnly;
            if (!elementInfo.IsInitialized())
            {
                return 0;
            }
            return GetInstanceInternal(parentWindowHandle);
        }

        private int GetInstanceInternal(IntPtr hwnd)
        {
            IntPtr zero = IntPtr.Zero;
            NativeMethods.EnumWindowsProc lpEnumFunc = EnumChildWindows;
            timeOutWatch = Stopwatch.StartNew();
            NativeMethods.EnumChildWindows(hwnd, lpEnumFunc, ref zero);
            timeOutWatch.Stop();
            if (zero != IntPtr.Zero)
            {
                return elementInstance;
            }
            return 0;
        }

        private class ElementInfo
        {
            private string accessibleName;
            private string className;
            private string controlId;
            private string controlName;
            private string name;

            public ElementInfo(IQueryCondition[] queryCondition)
            {
                foreach (QueryCondition condition in queryCondition)
                {
                    PropertyCondition condition2 = condition as PropertyCondition;
                    if (condition2 != null)
                    {
                        SetProperty(condition2.PropertyName, condition2.Value.ToString());
                    }
                }
            }

            public bool IsInitialized()
            {
                if (string.IsNullOrEmpty(controlId) && string.IsNullOrEmpty(controlName) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(accessibleName) && string.IsNullOrEmpty(name))
                {
                    return false;
                }
                return true;
            }

            public bool MatchWin32ElementInfo(IntPtr windowHandle)
            {
                int num;
                if (!string.IsNullOrEmpty(controlId) && int.TryParse(controlId, out num) && MsaaUtility.GetControlID(windowHandle) != num)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(controlName) && !string.Equals(MsaaUtility.GetControlName(windowHandle, (int)NativeMethods.GetWindowProcessId(windowHandle)), controlName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(this.className) && !string.Equals((NativeMethods.GetClassName(windowHandle)), this.className, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(name) && !string.Equals(NativeMethods.GetWindowText(windowHandle), name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(accessibleName))
                {
                    IAccessible accessibleObject = NativeMethods.AccessibleObjectFromWindow(windowHandle);
                    if (accessibleObject == null)
                    {
                        return false;
                    }
                    if (!string.Equals(MsaaUtility.NormalizeQueryPropertyValue(new AccWrapper(windowHandle, accessibleObject, 0).Name), accessibleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                return true;
            }

            private void SetProperty(string property, string propValue)
            {
                property = MsaaUtility.GetPropertyNameInCorrectCase(property);
                if (property != "ControlName")
                {
                    if (property != "ControlId")
                    {
                        if (property != "ClassName")
                        {
                            if (property != "AccessibleName")
                            {
                                if (property == "Name")
                                {
                                    name = propValue;
                                }
                                return;
                            }
                            accessibleName = propValue;
                            return;
                        }
                        className = propValue;
                        return;
                    }
                }
                else
                {
                    controlName = propValue;
                    return;
                }
                controlId = propValue;
            }
        }
    }
}