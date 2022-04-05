using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.Properties;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Helper
{
    internal static class RecorderUtilities
    {
        private const string AutoSuggestClassName = "Auto-Suggest Dropdown";
        private const string DesktopClassName = "#32769";
        private const string IE9NotificationBarClassName = "DirectUIHWND";
        private const string IEClassName = "IEFrame";
        private const string imeLanguageTag = "ImeHandlingNeeded";
        private const string TaskBarClassName = "MSTaskSwWClass";
        private const string TaskListThumbnailWnd = "TaskListThumbnailWnd";
        private const string WinSevenClassName = "MSTaskListWClass";

        public static bool AncestorMatchedForCombo(IZappyAction zaction, SetValueAction valueAction)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction inputAction = zaction as ZappyTaskAction;
                if (inputAction != null && inputAction.ActivityElement != null && valueAction != null &&
                    valueAction.ActivityElement != null)
                {
                    TaskActivityElement uIElement = inputAction.ActivityElement;
                    for (int i = 0; i < 3; i++)
                    {
                        TaskActivityElement parent = GetParent(uIElement);
                        if (parent == null)
                        {
                            break;
                        }

                        if (parent.Equals(valueAction.ActivityElement))
                        {
                            return true;
                        }

                        if (string.Equals(parent.ClassName, "Auto-Suggest Dropdown", StringComparison.Ordinal))
                        {
                            return true;
                        }

                        uIElement = parent;
                    }
                }
            }

            return false;
        }

        public static void CheckIfRebootRequired(bool isRemoteTestingEnabled)
        {
            if (isRemoteTestingEnabled)
            {
                
            }
            else
            {
                object[] args = { @"Software\Microsoft\VisualStudio\15.0" };
                string subKeyPath = string.Format(CultureInfo.InvariantCulture, @"{0}\TeamTest\ZappyTask\RebootNotRequired", args);
                try
                {
                    if (ZappyTaskUtilities.ExistsRegistryKey(Registry.LocalMachine, subKeyPath) || ZappyTaskUtilities.ExistsRegistryKey(Registry.CurrentUser, subKeyPath))
                    {
                        
                    }
                    else
                    {
                        PerformRebootCheck(GetLastRebootTime());
                        if (ZappyTaskUtilities.TryCreateRegistryKey(Registry.LocalMachine, subKeyPath) && ZappyTaskUtilities.TryCreateRegistryKey(Registry.CurrentUser, subKeyPath))
                        {
                            object[] objArray2 = { subKeyPath };
                            
                        }
                    }
                }
                catch (ZappyTaskException)
                {
                                        throw;
                }
                catch (SecurityException exception)
                {
                    CrapyLogger.log.Error(exception);
                                        throw new ZappyTaskException(Resources.RebootMessage, exception);
                }
                catch (UnauthorizedAccessException exception2)
                {
                    CrapyLogger.log.Error(exception2);
                                        throw new ZappyTaskException(Resources.RebootMessage, exception2);
                }
            }
        }

        public static bool ClickTimeWithinDoubleClickRange(int currentActionTime, int lastActionTime)
        {
            long num = currentActionTime - lastActionTime;
            bool flag = num <= SystemInformation.DoubleClickTime;
            object[] args = { currentActionTime, lastActionTime, flag };
            
            return flag;
        }

                                                        
        private static bool ExistsInstallTimeInRegistry(RegistryKey registryKey, string subKeyPath) =>
            ZappyTaskUtilities.ExistsRegistryKey(registryKey, subKeyPath, "InstallLCID") && ZappyTaskUtilities.ExistsRegistryKey(registryKey, subKeyPath, "InstallDate") && ZappyTaskUtilities.ExistsRegistryKey(registryKey, subKeyPath, "InstallTime");

        public static Point GetDefaultInteractionPoint(TaskActivityElement element)
        {
                                    if (element == null)
            {
                return new Point();
            }
            try
            {
                int num;
                int num2;
                int num3;
                int num4;
                Screen screen = Screen.FromHandle(element.WindowHandle);
                bool isRightToLeft = false;
                try
                {
                    isRightToLeft = element.GetRightToLeftProperty(RightToLeftKind.Text);
                }
                catch (SystemException)
                {
                    
                }
                element.GetBoundingRectangle(out num, out num2, out num3, out num4);
                Point point2 = GetDefaultInteractionPointInternal(isRightToLeft, num, num2, num3, num4);
                return new Point(point2.X - screen.Bounds.X, point2.Y - screen.Bounds.Y);
            }
            catch (Exception exception)
            {
                object[] args = { exception };
                CrapyLogger.log.ErrorFormat("RecorderUtilities: Exception {0} occurred during DefaultInteractionPointForSnapShot", args);
                                return new Point();
            }
        }

        internal static Point GetDefaultInteractionPointInternal(bool isRightToLeft, int left, int top, int width, int height)
        {
            int x = isRightToLeft ? left + width - 5 : left + 5;
            return new Point(x, top + 5);
        }

        public static DateTime GetLastRebootTime()
        {
            string oSInfo = ZappyTaskUtilities.GetOSInfo("LastBootUpTime");
            if (oSInfo != null)
            {
                return ManagementDateTimeConverter.ToDateTime(oSInfo);
            }
            CrapyLogger.log.ErrorFormat("RecorderUtilities: Error in GetLastRebootTime, please check for WMI issues, otherwise certain features may not work");
            return WallClock.Now;
        }

        public static TaskActivityElement GetParent(TaskActivityElement element)
        {
            TaskActivityElement parentFast = null;
            try
            {
                if (element != null)
                {
                    parentFast = ZappyTaskService.Instance.GetParentFast(element);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            return parentFast;
        }

        public static Point GetUpdatedInteractionPointforActiveScreen(TaskActivityElement element, Point point)
        {
                                    if (element != null)
            {
                try
                {
                    if (ElementExtension.GetBoundingRectangle(element).Contains(point.X, point.Y))
                    {
                        Screen screen = Screen.FromHandle(element.WindowHandle);
                        return new Point(point.X - screen.Bounds.X, point.Y - screen.Bounds.Y);
                    }
                }
                catch (Exception)
                {
                }
            }
            return point;
                    }

                                                                
        public static bool IsIENotificationBar(TaskActivityElement element)
        {
            if (element != null && string.Equals("MSAA", element.Framework, StringComparison.OrdinalIgnoreCase))
            {
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                if (element2 != null && string.Equals(element2.ClassName, "IEFrame", StringComparison.Ordinal))
                {
                    for (TaskActivityElement element3 = element; element3 != null && element3 != element2; element3 = ZappyTaskService.Instance.GetParent(element3))
                    {
                        if (!string.Equals(element3.ClassName, "DirectUIHWND", StringComparison.Ordinal))
                        {
                            break;
                        }
                        if (string.Equals(element3.Name, LocalizedSystemStrings.Instance.IE9NotificationBar, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsIEWindow(IntPtr hwnd) =>
            ZappyTaskUtilities.IsIEWindow(hwnd);

        public static bool IsValidUtf32String(string testString)
        {
            try
            {
                new UTF32Encoding(true, true, true).GetBytes(testString);
            }
            catch (EncoderFallbackException exception)
            {
                object[] args = { exception };
                CrapyLogger.log.ErrorFormat("IsValidUtf32String: The inputstring has invalid encoding: {0}", args);
                return false;
            }
            return true;
        }

        public static bool IsWinSevenGroupedTaskBarButton(IZappyAction zaction)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;
                if (action == null || !AggregatorUtilities.IsWindows7OrHigher)
                {
                    return false;
                }

                TaskActivityElement uIElement = action.ActivityElement;
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(uIElement);
                TaskActivityElement parent = GetParent(uIElement);
                if (uIElement == null || parent == null || element2 == null)
                {
                    return false;
                }

                return ControlType.ListItem.NameEquals(uIElement.ControlTypeName) &&
                       string.Equals(uIElement.ClassName, "TaskListThumbnailWnd", StringComparison.Ordinal) &&
                       ControlType.List.NameEquals(parent.ControlTypeName) &&
                       string.Equals(parent.ClassName, "TaskListThumbnailWnd", StringComparison.Ordinal) &&
                       ControlType.Window.NameEquals(element2.ControlTypeName) &&
                       string.Equals(element2.ClassName, "#32769", StringComparison.Ordinal);
            }
            return false;

        }

        public static bool IsWinSevenTaskBarButton(IZappyAction zaction)
        {
            ZappyTaskAction action = zaction as ZappyTaskAction;
            if (action != null && AggregatorUtilities.IsWindows7OrHigher)
            {
                TaskActivityElement uIElement = action.ActivityElement;
                TaskActivityElement parent = GetParent(uIElement);
                TaskActivityElement element3 = FrameworkUtilities.TopLevelElement(uIElement);
                if (parent != null && element3 != null && (ControlType.Button.NameEquals(uIElement.ControlTypeName) || ControlType.NameComparer.Equals("MenuButton", uIElement.ControlTypeName)) && (ControlType.ToolBar.NameEquals(parent.ControlTypeName) && string.Equals(uIElement.ClassName, "MSTaskListWClass", StringComparison.Ordinal)) && string.Equals(parent.ClassName, "MSTaskListWClass", StringComparison.Ordinal) && string.Equals(element3.ClassName, "MSTaskSwWClass", StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        public static void PerformRebootCheck(DateTime rebootTime)
        {
            object[] args = { @"Software\Microsoft\VisualStudio\15.0" };
            string subKeyPath = string.Format(CultureInfo.InvariantCulture, @"{0}\TeamTest\ZappyTask\ModifiedLowLevelHooksTimeout", args);
            bool flag = ExistsInstallTimeInRegistry(Registry.LocalMachine, subKeyPath);
            bool flag2 = ExistsInstallTimeInRegistry(Registry.CurrentUser, subKeyPath);
            DateTime now = WallClock.Now;
            if (flag)
            {
                DateTime time2 = ReadInstallTime(Registry.LocalMachine, subKeyPath);
                object[] objArray2 = { time2 };
                
                if (time2 < now)
                {
                    now = time2;
                }
            }
            if (flag2)
            {
                DateTime time3 = ReadInstallTime(Registry.CurrentUser, subKeyPath);
                object[] objArray3 = { time3 };
                
                if (time3 < now)
                {
                    now = time3;
                }
            }
            if (now > rebootTime)
            {
                DateTime time4 = RegKeyLastModifiedTime(Registry.LocalMachine, subKeyPath);
                object[] objArray4 = { time4 };
                
                if (time4 < now)
                {
                    now = time4;
                }
                time4 = RegKeyLastModifiedTime(Registry.CurrentUser, subKeyPath);
                object[] objArray5 = { time4 };
                
                if (time4 < now)
                {
                    now = time4;
                }
            }
            if (!flag)
            {
                WriteInstallTime(Registry.LocalMachine, subKeyPath, now);
            }
            if (!flag2)
            {
                WriteInstallTime(Registry.CurrentUser, subKeyPath, now);
            }
            if (rebootTime < now)
            {
                object[] objArray6 = { rebootTime, now };
                CrapyLogger.log.ErrorFormat("PerformRebootCheck(): User needs to reboot. Last reboot time {0}, Install time {1}." + objArray6);
                throw new ZappyTaskException(Resources.RebootMessage);
            }
        }

        public static bool PointsWithinRange(Point point1, Point point2, Size range)
        {
            bool flag = Math.Abs((long)(point1.X - point2.X)) <= range.Width && Math.Abs((long)(point1.Y - point2.Y)) <= range.Height;
            object[] args = { flag };
            
            return flag;
        }

        private static DateTime ReadInstallTime(RegistryKey registryRoot, string subKeyPath)
        {
            int num;
            DateTime time2;
            DateTime now = WallClock.Now;
            string str = ZappyTaskUtilities.GetRegistryValue<string>(registryRoot, subKeyPath, "InstallLCID", false);
            if (string.IsNullOrEmpty(str) || !TryParseIntOrHex(str, out num))
            {
                object[] args = { str };
                CrapyLogger.log.ErrorFormat("ReadInstallTime(): Unable to parse string '{0}' for lcid", args);
                return now;
            }
            CultureInfo info = new CultureInfo(num);
            string str2 = ZappyTaskUtilities.GetRegistryValue<string>(registryRoot, subKeyPath, "InstallDate", false);
            if (string.IsNullOrEmpty(str2) || !DateTime.TryParse(str2, info.DateTimeFormat, DateTimeStyles.None, out time2))
            {
                object[] objArray2 = { str2 };
                CrapyLogger.log.ErrorFormat("ReadInstallTime(): Unable to parse string '{0}' for install date" + objArray2);
                return now;
            }
            string str3 = ZappyTaskUtilities.GetRegistryValue<string>(registryRoot, subKeyPath, "InstallTime", false);
            if (string.IsNullOrEmpty(str3) || !DateTime.TryParse(str3, info.DateTimeFormat, DateTimeStyles.None, out now))
            {
                object[] objArray3 = { str3 };
                CrapyLogger.log.ErrorFormat("ReadInstallTime(): Unable to parse string '{0}' for install time", objArray3);
                return now;
            }
            return new DateTime(time2.Year, time2.Month, time2.Day, now.Hour, now.Minute, now.Second);
        }

        public static DateTime RegKeyLastModifiedTime(RegistryKey root, string subkey)
        {
            UIntPtr ptr = (UIntPtr)(-2147483647);
            UIntPtr ptr2 = (UIntPtr)(-2147483646);
            UIntPtr hKey = root == Registry.LocalMachine ? ptr2 : ptr;
            int samDesired = 1;
            UIntPtr zero = UIntPtr.Zero;
            uint num2 = 0;
            num2 = NativeMethods.RegOpenKeyEx(hKey, subkey, 0, samDesired, out zero);
            switch (num2)
            {
                case 0:
                    DateTime time;
                    try
                    {
                        FILETIME lpftLastWriteTime = new FILETIME();
                        uint lpcClass = 0;
                        num2 = NativeMethods.RegQueryInfoKey(zero, null, ref lpcClass, IntPtr.Zero, out lpcClass, out lpcClass, out lpcClass, out lpcClass, out lpcClass, out lpcClass, IntPtr.Zero, out lpftLastWriteTime);
                        long fileTime = (lpftLastWriteTime.dwHighDateTime << 0x20) + lpftLastWriteTime.dwLowDateTime;
                        if (num2 != 0 || fileTime == 0)
                        {
                            object[] args = { num2 };
                            CrapyLogger.log.ErrorFormat("Could not find out the lastwrite time for the registry key 'ModifiedLowLevelHooksTimeout': Error :{0}", args);
                            return WallClock.Now;
                        }
                        time = DateTime.FromFileTime(fileTime);
                    }
                    finally
                    {
                        num2 = NativeMethods.RegCloseKey(zero);
                        if (num2 != 0)
                        {
                            object[] objArray5 = { num2 };
                            CrapyLogger.log.ErrorFormat("Could not close the registry key 'ModifiedLowLevelHooksTimeout': Error :{0}" + objArray5);
                        }
                    }
                    return time;

                case 2:
                    {
                        object[] objArray1 = { subkey };
                        
                        if (!ZappyTaskUtilities.TryCreateRegistryKey(root, subkey))
                        {
                            object[] objArray2 = { subkey };
                            
                        }
                        break;
                    }
                default:
                    {
                        object[] objArray3 = { subkey, num2 };
                        CrapyLogger.log.ErrorFormat("Could not open the registry key '{0}': Error :{1}", objArray3);
                        break;
                    }
            }
            return WallClock.Now;
        }

        private static bool TryParseIntOrHex(string intString, out int parsedNumber)
        {
            if (intString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return int.TryParse(intString.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out parsedNumber);
            }
            return int.TryParse(intString, out parsedNumber);
        }

        private static void WriteInstallTime(RegistryKey registryKey, string subKeyPath, DateTime installTime)
        {
            ZappyTaskUtilities.TryCreateRegistryKey(registryKey, subKeyPath, "InstallLCID", CultureInfo.CurrentCulture.LCID.ToString(CultureInfo.InvariantCulture));
            ZappyTaskUtilities.TryCreateRegistryKey(registryKey, subKeyPath, "InstallDate", installTime.ToShortDateString());
            ZappyTaskUtilities.TryCreateRegistryKey(registryKey, subKeyPath, "InstallTime", installTime.ToShortTimeString());
        }

        public static string ImeLanguageTag =>
            "ImeHandlingNeeded";

        internal static bool IsPointerEventCaptureApplicable =>
            ZappyTaskUtilities.IsWin8OrGreaterOs();
    }
}