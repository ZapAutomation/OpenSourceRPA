using Accessibility;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;


namespace Zappy.ActionMap.ZappyTaskUtil
{
    public static class ZappyTaskUtilities
    {
        private static IDictionary<string, string> appSettings;
        private static int currentProcessId;
        internal const string DateFormat = "dd-MMM-yyyy";
        private static Regex dateRangeValueRegex = new Regex("\"(?<mindate>(\\d|\\W|\\w|\\s|\\S^\")+)\"-\"(?<maxdate>(\\d|\\W|\\w|\\s|\\S^\")+)\"", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        internal const string DateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
        private const uint EventModifyState = 2;
        private const string IEPluginResourceAssemblyName = "IE.Resources.dll";
        private static int ieVersion = -2147483648;
        private static bool? isIE10OrHigher;
        private static bool isImageActionLogEnabled;
        private static volatile bool isSkipStepOn;
        private static bool? isWin7;
        private static bool? isWin8OrGreaterOs;
        private static readonly object lockObject;
        private static readonly RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static string programFilesIDEFolderPath = string.Empty;
        private static readonly Regex programW6432 = new Regex("%ProgramW6432%", options);
        private static readonly Regex programX86 = new Regex(@"%ProgramFiles\(x86\)%", options);
        private static SafeWaitHandle skipStepEventHandle;
        private static readonly Regex sysWow64;
        private const string UIAPluginResourceAssemblyName = "Uia.Resources.dll";
        internal static bool UserInteractive = Environment.UserInteractive;
        private static readonly string windowsFolderPath;
        private static readonly string windowsPathForRegex;
        internal static string WindowsSpecialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        internal static bool s_useManagedMSAA = false;


        static ZappyTaskUtilities()
        {
            object[] args = { windowsPathForRegex };
            sysWow64 = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}", args), options);
            windowsFolderPath = WindowsSpecialFolderPath;
            object[] objArray2 = { windowsFolderPath };
            windowsPathForRegex = Regex.Escape(string.Format(CultureInfo.InvariantCulture, @"{0}\SysWow64", objArray2));
            lockObject = new object();
            appSettings = null;
        }

        public static bool IsManagedMsaaElement(ITaskActivityElement element)
        {
            bool managedMsaaPropertyValue = false;
            if ((element != null) && UITechnologyManager.AreEqual("MSAA", element.Framework))
            {
                managedMsaaPropertyValue = GetManagedMsaaPropertyValue(element);
            }
            return managedMsaaPropertyValue;
        }

        public static bool UseManagedMSAA
        {
            get
            {
                return s_useManagedMSAA;
            }
            set
            {
                s_useManagedMSAA = value;
            }
        }

        public static ControlStates ConvertState(AccessibleStates accState)
        {
            if (accState == AccessibleStates.None)
            {
                return (ControlStates.None | ControlStates.Normal);
            }
            return (ControlStates)((long)accState);
        }

        internal static void AddAssemblyResolveHandler()
        {
                    }

        internal static void BringStartMenuToFocus()
        {
            PressWindowsKey(true);
            PressWindowsKey(false);
        }

        internal static bool CanResetSkipStep()
        {
                        object lockObject = ZappyTaskUtilities.lockObject;
            lock (lockObject)
            {
                isSkipStepOn = false;
                if (skipStepEventHandle != null && !skipStepEventHandle.IsInvalid)
                {
                    return true;
                }
            }
            return false;
        }

        public static void CheckForNull(IntPtr parameter, string parameterName)
        {
            if (parameter == IntPtr.Zero)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void CheckForNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void CheckForNull(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void CheckForPointWithinControlBounds(int offsetX, int offsetY, Rectangle controlBound, string message)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX > controlBound.Width || offsetY > controlBound.Height)
            {
                throw new ArgumentOutOfRangeException(message);
            }
        }

        internal static void CleanUpTempFiles()
        {
            foreach (string str2 in Directory.GetFiles(Path.GetTempPath(), "*.xml"))
            {
                if (File.Exists(str2) && str2.Contains("OmniLogRawXml"))
                {
                    try
                    {
                        File.Delete(str2);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        internal static void CloseProcess(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                process2.Close();
            }
        }

        public static object ConvertIUnknownToTypedObject(object iunknownObject, Type type)
        {
            IntPtr ptr2;
            IntPtr iUnknownForObjectInContext = Marshal.GetIUnknownForObjectInContext(iunknownObject);
            Guid iid = Marshal.GenerateGuidForType(type);
            Marshal.ThrowExceptionForHR(Marshal.QueryInterface(iUnknownForObjectInContext, ref iid, out ptr2));
            return Marshal.GetTypedObjectForIUnknown(ptr2, type);
        }

        internal static string ConvertTo32BitString(string fileName)
        {
            fileName = programW6432.Replace(fileName, "%ProgramFiles%");
            fileName = programX86.Replace(fileName, "%ProgramFiles%");
            fileName = Environment.ExpandEnvironmentVariables(fileName);
            object[] args = { windowsFolderPath };
            fileName = sysWow64.Replace(fileName, string.Format(CultureInfo.InvariantCulture, @"{0}\System32", args));
            return fileName;
        }

        internal static T ConvertToType<T>(object value) =>
            ConvertToType<T>(value, true);


        internal static T ConvertToType<T>(object value, bool throwIfValueIsNull)
        {
            T local;
            if (throwIfValueIsNull && value == null)
            {
                throw new SystemException(string.Empty, new ArgumentNullException());
            }
            if (!TryConvertToType(value, out local))
            {
                throw new SystemException(string.Empty, new InvalidCastException());
            }
            return local;
        }

        [Conditional("DEBUG")]
        public static void DisableDebugUIMessages()
        {
            Debug.Listeners.Clear();
        }

        internal static void DisposeSkipEventObject()
        {
            if (skipStepEventHandle != null && !skipStepEventHandle.IsInvalid)
            {
                skipStepEventHandle.Dispose();
            }
            skipStepEventHandle = null;
        }

        public static void DrawHighlight(int x, int y, int width, int height, int highlightTime)
        {
            SwitchFromImmersive.Instance.DrawHighlight(x, y, width, height, highlightTime);
        }

        internal static bool ExistsRegistryKey(RegistryKey hive, string subKeyPath) =>
            ExistsRegistryKey(hive, subKeyPath, string.Empty);

        internal static bool ExistsRegistryKey(RegistryKey hive, string subKeyPath, string valueName)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(subKeyPath))
                {
                    if (key != null)
                    {
                        if (!string.IsNullOrEmpty(valueName))
                        {
                            return key.GetValue(valueName) != null;
                        }
                        return true;
                    }
                }
            }
            catch (SecurityException exception)
            {
                object[] args = { hive.Name, subKeyPath, exception };
                            }
            return false;
        }

        private static string GetAncestorClassName(IntPtr windowHandle, NativeMethods.GetAncestorFlag flag)
        {
            string className = null;
            IntPtr ancestor = NativeMethods.GetAncestor(windowHandle, flag);
            if (ancestor != IntPtr.Zero)
            {
                className = NativeMethods.GetClassName(ancestor);
            }
            return className;
        }

        internal static IDictionary<string, string> GetAppSettings()
        {
            if (appSettings == null)
            {
                ZappyTaskUtilities.appSettings = new Dictionary<string, string>();
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                foreach (string str in appSettings.AllKeys)
                {
                    ZappyTaskUtilities.appSettings[str] = appSettings[str];
                }
            }
            return appSettings;
        }

        public static string GetAssemblyFileVersion(Assembly assembly)
        {
            if (assembly != null)
            {
                try
                {
                    return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;

                }
                catch (Exception)
                {
                    return string.Empty;

                }

            }
            return string.Empty;
        }

        public static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            if (culture.Calendar is GregorianCalendar)
            {
                return culture.DateTimeFormat;
            }
            GregorianCalendar calendar = null;
            DateTimeFormatInfo dateTimeFormat = null;
            foreach (Calendar calendar2 in culture.OptionalCalendars)
            {
                if (calendar2 is GregorianCalendar)
                {
                    if (calendar == null)
                    {
                        calendar = calendar2 as GregorianCalendar;
                    }
                    else if (((GregorianCalendar)calendar2).CalendarType == GregorianCalendarTypes.Localized)
                    {
                        calendar = calendar2 as GregorianCalendar;
                        break;
                    }
                }
            }
            if (calendar == null)
            {
                dateTimeFormat = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
                dateTimeFormat.Calendar = new GregorianCalendar();
                return dateTimeFormat;
            }
            dateTimeFormat = ((CultureInfo)culture.Clone()).DateTimeFormat;
            dateTimeFormat.Calendar = calendar;
            return dateTimeFormat;
        }

        public static string GetDateTimeToString(DateTime dateTime, bool includeTime)
        {
            if (includeTime)
            {
                return dateTime.ToString("dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            return dateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        internal static Type GetExceptionTargetSiteType(Exception ex)
        {
            if (ex.TargetSite != null)
            {
                return ex.TargetSite.DeclaringType;
            }
            return null;
        }

        public static Assembly GetExecutingAssembly() =>
            Assembly.GetCallingAssembly();

        public static string GetExpandedLongPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            string fullPath = Environment.ExpandEnvironmentVariables(path);
            try
            {
                if (fullPath.IndexOf(Path.DirectorySeparatorChar) == -1 && fullPath.IndexOf(Path.AltDirectorySeparatorChar) == -1)
                {
                    return fullPath;
                }
                fullPath = Path.GetFullPath(fullPath);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (PathTooLongException)
            {
            }
            return fullPath;
        }

        public static string[] GetFiles(string customPluginDirectoryPath)
        {
            if (Directory.Exists(customPluginDirectoryPath))
            {
                return Directory.GetFiles(customPluginDirectoryPath, "*.dll");
            }
            return null;
        }

        public static Stream GetFileStreamWithCreateReadWriteAccess(string file)
        {
            var _DirName = Path.GetDirectoryName(file);
            if (!Directory.Exists(_DirName))
            {
                Directory.CreateDirectory(_DirName);
            }

            return new FileStream(file, FileMode.Create, FileAccess.ReadWrite);


        }

        public static Uri GetIEHomepage()
        {
            string str = GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Main", "Start Page") as string;
            Uri uri = new Uri("about:blank");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    uri = new Uri(str);
                }
                catch (UriFormatException exception)
                {
                    CrapyLogger.log.Error(exception);
                }
            }
            return uri;
        }

        public static string GetKeyboardLayout() =>
            CultureInfo.CurrentUICulture.KeyboardLayoutId.ToString("X", CultureInfo.InvariantCulture);

        public static int GetLcidFromWindowHandle(IntPtr windowHandle)
        {
            int num = 0;
            if (windowHandle != IntPtr.Zero)
            {
                uint dwProcessId = 0;
                num = (int)NativeMethods.GetKeyboardLayout(NativeMethods.GetWindowThreadProcessId(windowHandle, out dwProcessId)) & 0xffff;
            }
            return num;
        }

        public static int[] GetLocaleIdentifiers(IDictionary<string, string> appSettings)
        {
            List<int> list = new List<int>();
            string str = null;
            if (GetValue(appSettings, "IMELanguages", out str))
            {
                object[] args = { str };
                                if (!string.IsNullOrEmpty(str))
                {
                    char[] separator = { ',' };
                    foreach (string str2 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrEmpty(str2))
                        {
                            int num2;
                            if (int.TryParse(str2, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out num2))
                            {
                                list.Add(num2);
                            }
                            else
                            {
                                object[] objArray2 = { str2 };
                                                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        private static ManagementObject GetManagementObjectFromQuery(SelectQuery query)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    using (ManagementObjectCollection objects = searcher.Get())
                    {
                        if (objects.Count == 1)
                        {
                            ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator();
                            enumerator.Reset();
                            enumerator.MoveNext();
                            return (ManagementObject)enumerator.Current;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (!(exception is COMException) && !(exception is ManagementException))
                {
                    throw;
                }
                object[] args = { exception.Message };
                CrapyLogger.log.ErrorFormat("An error occured while querying for WMI data:{0}", args);
            }
            return null;
        }

        public static string GetOSInfo(string propertyName)
        {
            string propertyValueAsString = null;
            SelectQuery query = new SelectQuery("Win32_OperatingSystem");
            ManagementObject managementObjectFromQuery = GetManagementObjectFromQuery(query);
            if (managementObjectFromQuery != null)
            {
                propertyValueAsString = GetPropertyValueAsString(managementObjectFromQuery, propertyName);
            }
            return propertyValueAsString;
        }

        internal static object GetProcessById(int id) =>
            Process.GetProcessById(id);

        internal static int GetProcessId(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                return process2.Id;
            }
            return -1;
        }

        public static int GetProcessIdForWindow(IntPtr windowHandle) =>
            (int)NativeMethods.GetWindowProcessId(windowHandle);

        internal static string GetProcessMainWindowTitle(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                return process2.MainWindowTitle;
            }
            return null;
        }

        private static string GetPropertyValueAsString(ManagementObject managementObject, string propertyName)
        {
            string str = string.Empty;
            PropertyData data = managementObject.Properties[propertyName];
            if (data != null)
            {
                object obj2 = data.Value;
                if (obj2 != null)
                {
                    str = obj2.ToString();
                }
            }
            return str;
        }

        public static object GetRegistryValue(RegistryKey regKey, string subKeyPath, string registryValue) =>
            GetRegistryValue<object>(regKey, subKeyPath, registryValue, false);

        public static T GetRegistryValue<T>(RegistryKey regKey, string subKeyPath, string registryValue, bool throwException)
        {
            T local = default(T);
            try
            {
                using (RegistryKey key = regKey.OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ExecuteKey))
                {
                    if (key != null)
                    {
                        object obj2 = key.GetValue(registryValue);
                        if (obj2 == null)
                        {
                            return local;
                        }
                        if (Equals(typeof(string), typeof(T)) || Equals(typeof(object), typeof(T)))
                        {
                            switch (key.GetValueKind(registryValue))
                            {
                                case RegistryValueKind.Binary:
                                    {
                                        byte[] buffer = key.GetValue(registryValue) as byte[];
                                        if (buffer != null)
                                        {
                                            obj2 = BitConverter.ToString(buffer, 0);
                                        }
                                        break;
                                    }
                                case RegistryValueKind.DWord:
                                case RegistryValueKind.QWord:
                                    obj2 = obj2.ToString();
                                    break;
                            }
                        }
                        try
                        {
                            local = (T)obj2;
                        }
                        catch (InvalidCastException)
                        {
                        }
                    }
                    return local;
                }
            }
            catch (SecurityException exception)
            {
                CrapyLogger.log.Error(exception);
                if (throwException)
                {
                    throw;
                }
            }
            return local;
        }

        public static string GetShortDateRangeString(DateTime minRange, DateTime maxRange)
        {
            object[] args = { minRange.Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture), maxRange.Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) };
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\"-\"{1}\"", args);
        }

        public static string GetShortDateRangeStringInCurrentCulture(DateTime minRange, DateTime maxRange)
        {
            object[] args = { minRange.Date.ToShortDateString(), maxRange.Date.ToShortDateString() };
            return string.Format(CultureInfo.CurrentCulture, "\"{0}\"-\"{1}\"", args);
        }

        public static StreamReader GetStreamReader(string file) =>
            new StreamReader(file);

        internal static Assembly GetTypeAssembly(Type type) =>
            type.Assembly;

        public static string GetUniqueName(string path, string basename, string extension) =>
            GetUniqueName(path, basename, extension, 1);

        public static string GetUniqueName(string path, string basename, string extension, int basecount)
        {
            string str = Path.Combine(path, basename + basecount + extension);
            while (File.Exists(str) || Directory.Exists(str))
            {
                str = Path.Combine(path, basename + basecount + extension);
                basecount++;
            }
            return str;
        }

        public static bool GetValue<T>(IDictionary<string, string> appSettings, string key, out T value)
        {
            bool flag = false;
            value = default(T);
            if (appSettings.ContainsKey(key))
            {
                string str = appSettings[key];
                if (string.IsNullOrEmpty(str))
                {
                    return flag;
                }
                try
                {
                    value = (T)Convert.ChangeType(str, typeof(T), CultureInfo.CurrentCulture);
                    flag = true;
                }
                catch (ArgumentException exception)
                {
                                                            CrapyLogger.log.Error(exception);
                }
                catch (FormatException exception2)
                {
                                                            CrapyLogger.log.Error(exception2);
                }
            }
            return flag;
        }

        public static List<IntPtr> GetWebViewWindowHandles(IntPtr windowHandle)
        {
            List<IntPtr> list = new List<IntPtr>();
            foreach (IntPtr ptr in GetWindows(windowHandle, "XAMLWebViewHostWindowClass"))
            {
                IntPtr item = GetWindows(ptr, "Internet Explorer_Server").FirstOrDefault();
                if (item != IntPtr.Zero)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        internal static List<IntPtr> GetWindows(IntPtr parentHandle, string className)
        {
            List<IntPtr> handles = new List<IntPtr>();
            IntPtr lParam = new IntPtr();
            NativeMethods.EnumChildWindows(parentHandle, delegate (IntPtr hwnd, ref IntPtr param)
            {
                if (string.Equals(className, NativeMethods.GetClassName(hwnd), StringComparison.Ordinal))
                {
                    handles.Add(hwnd);
                }
                return true;
            }, ref lParam);
            return handles;
        }

        public static bool HasObjectTime(string value)
        {
            DateTime time;
            return !DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
        }

        internal static bool HasProcessExited(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                return process2.HasExited;
            }
            return true;
        }

                                                                        
        internal static void InitSkipEventObject(string skipPlayBackEventName)
        {
            isSkipStepOn = false;
            skipStepEventHandle = OpenEvent(2, false, skipPlayBackEventName);
            if (skipStepEventHandle == null || skipStepEventHandle.IsInvalid)
            {
                skipStepEventHandle = null;
                object[] args = { skipPlayBackEventName };
                CrapyLogger.log.ErrorFormat("Unable to open handle {0}. Cancel playback functionality will be affected.", args);
            }
        }

        public static bool IsCharmsBar(IntPtr windowHandle) =>
            IsCharmsBar(windowHandle, NativeMethods.GetClassName(windowHandle));

        public static bool IsCharmsBar(IntPtr windowHandle, string windowClassName)
        {
            if (!string.IsNullOrEmpty(windowClassName) && string.Equals(windowClassName, "NativeHWNDHost"))
            {
                string windowText = NativeMethods.GetWindowText(windowHandle);
                if (!string.IsNullOrEmpty(windowText) && string.Equals(windowText, "Charm Bar"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDesktopWindowAndNotLegacy(IntPtr windowHandle)
        {
            string className = NativeMethods.GetClassName(windowHandle);
            return !string.IsNullOrEmpty(className) && string.Equals(className, "#32769", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFirefoxWindowClassName(string className)
        {
            if (!string.Equals("MozillaUIWindowClass", className, StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals("MozillaDialogClass", className, StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        public static bool IsHigherPrivilegeProcess(IntPtr windowHandle)
        {
            string str;
            return windowHandle != IntPtr.Zero && IsHigherPrivilegeProcess(GetProcessIdForWindow(windowHandle), out str);
        }

        public static bool IsHigherPrivilegeProcess(int processId, out string fileName)
        {
            fileName = string.Empty;
            if (processId == 0 || processId == CurrentProcessId)
            {
                object[] args = { processId };
                                return true;
            }
            fileName = NativeMethods.GetProcessFileName(processId);
            try
            {
                IntPtr handle = Process.GetProcessById(processId).Handle;
            }
            catch (Win32Exception exception)
            {
                if (exception.NativeErrorCode == 5)
                {
                    object[] objArray2 = { processId };
                                        return true;
                }
            }
            catch (InvalidOperationException exception2)
            {
                object[] objArray3 = { processId, exception2 };
                                return true;
            }
            catch (ArgumentException exception3)
            {
                object[] objArray4 = { processId, exception3 };
                                return true;
            }
            return false;
        }

        public static bool IsIEWindow(IntPtr windowHandle) =>
            NativeMethods.IsWindow(windowHandle) && IsIEWindowClassName(NativeMethods.GetClassName(windowHandle));

        public static bool IsIEWindowClassName(string className)
        {
            if (!string.Equals("IEFrame", className, StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals("Internet Explorer_TridentDlgFrame", className, StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        public static bool IsImmersiveBrowserWindow(IntPtr windowHandle) =>
            "Internet Explorer_Server".Equals(NativeMethods.GetClassName(windowHandle)) && string.Equals("Shell DocObject View", GetAncestorClassName(windowHandle, NativeMethods.GetAncestorFlag.GA_PARENT), StringComparison.OrdinalIgnoreCase) && IsImmersiveWindowClassName(GetAncestorClassName(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOTOWNER));

        public static bool IsImmersiveModeActive()
        {
            if (!IsWin8OrGreaterOs())
            {
                return false;
            }
            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
            return foregroundWindow != IntPtr.Zero && IsImmersiveWindowClassName(NativeMethods.GetClassName(foregroundWindow));
        }

        public static bool IsImmersiveWindow(IntPtr windowHandle) =>
            IsImmersiveWindowClassName(GetAncestorClassName(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOTOWNER));

        public static bool IsImmersiveWindowClassName(string className)
        {
            if (string.IsNullOrEmpty(className) || !className.Contains("ApplicationFrameWindow") && !className.Contains(ZappyTaskCommonNames.ImmersiveAppWindowClassName) && !className.Contains("FileSearchAppWindowClass") && !className.Contains("SearchPane") && (!className.Contains("SearchResultsView") && !className.Contains(ZappyTaskCommonNames.StartMenuClassName)) && !className.Contains("Shell_CharmWindow") && !className.Contains("ContactCard"))
            {
                return false;
            }
            return true;
        }

        internal static bool IsInvalidComObjectException(Exception ex) =>
            ex is InvalidComObjectException;

        public static bool IsMfcClassName(string elementClassName) =>
            !string.IsNullOrEmpty(elementClassName) && elementClassName.StartsWith("Afx:", StringComparison.OrdinalIgnoreCase);


        internal static bool IsProtectedWindow()
        {
            bool protectedWindow = false;
            IntPtr param2 = new IntPtr();
            NativeMethods.EnumWindows(delegate (IntPtr wnd, ref IntPtr param)
            {
                protectedWindow = false;
                if (wnd != IntPtr.Zero && NativeMethods.IsWindow(wnd) && NativeMethods.IsWindowVisible(wnd) && !NativeMethods.IsIconic(wnd))
                {
                    try
                    {
                        NativeMethods.DRMIsWindowProtected(wnd, ref protectedWindow);
                    }
                    catch (DllNotFoundException)
                    {
                        return false;
                    }
                    catch (EntryPointNotFoundException)
                    {
                        return false;
                    }
                }
                return !protectedWindow;
            }, param2);
            return protectedWindow;
        }

        internal static bool IsRootParentSupportedControl(IntPtr windowHandle, string className)
        {
            string str = null;
            IntPtr ancestor = NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOTOWNER);
            if (ancestor != IntPtr.Zero)
            {
                str = NativeMethods.GetClassName(ancestor);
            }
            if (IsImmersiveWindowClassName(str) || IsCharmsBar(ancestor, str))
            {
                return true;
            }
            IntPtr zero = IntPtr.Zero;
            windowHandle = ancestor;
            while ((zero = NativeMethods.GetWindow(windowHandle, NativeMethods.GetWindowFlag.GW_OWNER)) != IntPtr.Zero && windowHandle != NativeMethods.GetDesktopWindow())
            {
                if (string.Equals(NativeMethods.GetClassName(zero), "ApplicationFrameWindow", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                windowHandle = zero;
            }
            return false;
        }

        public static bool IsSilverlightClassName(string elementClassName) =>
            !string.IsNullOrEmpty(elementClassName) && elementClassName.StartsWith("ATL:", StringComparison.OrdinalIgnoreCase);

        internal static bool IsStartScreenVisible()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle bounds = screen.Bounds;
                if (IsWindowsStartScreen(NativeMethods.WindowFromPoint(new NativeMethods.POINT(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2))))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWebView(IntPtr handle)
        {
            while (handle != IntPtr.Zero)
            {
                if (string.Equals(NativeMethods.GetClassName(handle), "XAMLWebViewHostWindowClass", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                handle = NativeMethods.GetParent(handle);
            }
            return false;
        }

        public static bool IsWin8OrGreaterOs()
        {
            return false;
        }
                                                                
        public static bool IsWindowOfSupportedTechnology(IntPtr windowHandle, out bool isWWAWindow, out bool isInternetExplorer)
        {
            string className = NativeMethods.GetClassName(windowHandle);
            isInternetExplorer = false;
            isWWAWindow = false;
            try
            {
                uint num;
                NativeMethods.GetWindowThreadProcessId(windowHandle, out num);
                Process processById = Process.GetProcessById((int)num);
                isInternetExplorer = processById != null && string.Equals(processById.ProcessName, "iexplore", StringComparison.OrdinalIgnoreCase);
                isWWAWindow = processById != null && string.Equals(processById.ProcessName, "wwahost", StringComparison.OrdinalIgnoreCase);
            }
            catch (ArgumentException exception)
            {
                object[] args = { exception.Message };
                CrapyLogger.log.ErrorFormat("UiaUtility: Exception {0} in IsWindowOfSupportedTechnology", args);
            }
            if (!isInternetExplorer && !isWWAWindow)
            {
                if (string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (IsRootParentSupportedControl(windowHandle, className) || string.Equals(className, ZappyTaskCommonNames.ImmersiveAppWindowClassName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (IsImmersiveWindowClassName(className))
                {
                    return true;
                }
                if (string.Equals(className, "Item Picker Window", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsWindows8OrLaterOs()
        {
            int major = Environment.OSVersion.Version.Major;
            int minor = Environment.OSVersion.Version.Minor;
            if (major <= 6 && (major != 6 || minor < 2))
            {
                return false;
            }
            return true;
        }

        public static bool IsWindowsStartScreen(IntPtr windowHandle)
        {
            windowHandle = NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT);
            return string.Equals(NativeMethods.GetClassName(windowHandle), ZappyTaskCommonNames.StartMenuClassName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsWinformsClassName(string elementClassName) =>
            !string.IsNullOrEmpty(elementClassName) && elementClassName.StartsWith("WindowsForms", StringComparison.OrdinalIgnoreCase);

        public static bool IsWpfClassName(string elementClassName) =>
            !string.IsNullOrEmpty(elementClassName) && elementClassName.StartsWith("HwndWrapper", StringComparison.OrdinalIgnoreCase);

        public static bool IsWWAWindow(IntPtr windowHandle)
        {
            uint dwProcessId = 0;
            NativeMethods.GetWindowThreadProcessId(windowHandle, out dwProcessId);
            if (dwProcessId != 0)
            {
                Process processById = Process.GetProcessById((int)dwProcessId);
                if (string.Equals("wwahost", processById.ProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal static void KillProcess(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                process2.Kill();
            }
        }


        internal static Assembly LoadAssembly(string assemblyFile) =>
            Assembly.LoadFrom(assemblyFile);

        public static string NormalizeDynamicClassName(string elementClassName)
        {
            if (string.IsNullOrEmpty(elementClassName))
            {
                return string.Empty;
            }
            string str = elementClassName;
            if (IsWinformsClassName(str) && str.Length > "WindowsForms".Length + 4)
            {
                int index = str.IndexOf('.', "WindowsForms".Length);
                if (index != -1)
                {
                    index = str.IndexOf('.', index + 1);
                    if (index != -1)
                    {
                        str = str.Substring(0, index);
                    }
                }
                return str;
            }
            if (IsMfcClassName(str))
            {
                return "Afx:";
            }
            if (IsWpfClassName(str))
            {
                return "HwndWrapper";
            }
            if (IsSilverlightClassName(str))
            {
                str = "ATL:";
            }
            return str;
        }


        public static Assembly OnResolve(object senderAppDomain, object arg)
        {
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                return null;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern SafeWaitHandle OpenEvent(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, [MarshalAs(UnmanagedType.LPWStr)] string lpName);
        private static void PressWindowsKey(bool isPress)
        {
            short num = 0x5b;
            SendInputHelper.INPUT mi = new SendInputHelper.INPUT
            {
                type = 1,
                union = { keyboardInput = { wVk = num } }
            };
            mi.union.keyboardInput.wScan = (short)SendInputHelper.MapVirtualKey(mi.union.keyboardInput.wVk, 0);
            int num2 = 0;
            if (mi.union.keyboardInput.wScan > 0)
            {
                num2 |= 8;
            }
            if (!isPress)
            {
                num2 |= 2;
            }
            mi.union.keyboardInput.dwFlags = num2;
            mi.union.keyboardInput.dwFlags |= 1;
            mi.union.keyboardInput.time = 0;
            mi.union.keyboardInput.dwExtraInfo = new IntPtr(0);
            SendInputHelper.SendInput(1, ref mi, Marshal.SizeOf(mi));
        }

        internal static void RefreshProcess(object process)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                process2.Refresh();
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool ResetEvent(SafeHandle hEvent);
        [Conditional("DEBUG")]
        public static void RestoreDebugUIMessages()
        {
            Debug.Listeners.Add(new DefaultTraceListener());
        }

        public static long RoundOffMillisecondToSecond(long milliSecondValue)
        {
            long num = 0x3e8L;
            if (milliSecondValue % 0x3e8L == 0)
            {
                num = 0L;
            }
            return (milliSecondValue + num) / 0x3e8L;
        }

        public static void SafeThreadJoin(Thread threadToJoin)
        {
            if (threadToJoin != null && threadToJoin.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                object[] args = { threadToJoin.ManagedThreadId };
                                while (!threadToJoin.Join(0x3e8))
                {
                    Application.DoEvents();
                }
                object[] objArray2 = { threadToJoin.ManagedThreadId };
                            }
            else if (threadToJoin == null)
            {
                CrapyLogger.log.Error("SafeThreadJoin called with null thread");
            }
            else
            {
                object[] objArray3 = { threadToJoin.ManagedThreadId };
                CrapyLogger.log.ErrorFormat("SafeThreadJoin called on the same thread - {0}", objArray3);
            }
        }

        public static bool SafeThreadJoin(Thread threadToJoin, int timeout, bool abort)
        {
            long elapsedMilliseconds;
            if (timeout <= 0)
            {
                SafeThreadJoin(threadToJoin);
                return true;
            }
            bool flag = true;
            if (threadToJoin == null || threadToJoin.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                if (threadToJoin == null)
                {
                    CrapyLogger.log.Error("SafeThreadJoin called with null thread");
                }
                else
                {
                    object[] objArray5 = { threadToJoin.ManagedThreadId };
                    CrapyLogger.log.ErrorFormat("SafeThreadJoin called on the same thread - {0}", objArray5);
                }
                return false;
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object[] args = { threadToJoin.ManagedThreadId };
                        for (int i = timeout < 0x3e8 ? timeout : 0x3e8; !threadToJoin.Join(i); i = timeout - elapsedMilliseconds < 0x3e8L ? timeout - (int)elapsedMilliseconds : 0x3e8)
            {
                Application.DoEvents();
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                if (elapsedMilliseconds > timeout)
                {
                    flag = false;
                    break;
                }
            }
            stopwatch.Stop();
            if (flag)
            {
                object[] objArray2 = { threadToJoin.ManagedThreadId };
                                return flag;
            }
            if (abort && threadToJoin.IsAlive)
            {
                object[] objArray3 = { threadToJoin.ManagedThreadId };
                                try
                {
                    threadToJoin.Abort();
                }
                catch (ThreadStartException exception)
                {
                    object[] objArray4 = { exception };
                                    }
            }
            return flag;
        }

        public static string SanitizeFriendlyName(string friendlyName, string ellipsisString)
        {
            if (!string.IsNullOrEmpty(friendlyName))
            {
                friendlyName = Regex.Replace(friendlyName, @"\s+", " ");
                if (friendlyName.Length > 50)
                {
                    friendlyName = friendlyName.Substring(0, 50) + ellipsisString;
                }
            }
            return friendlyName;
        }

        internal static void SetCursorPosition(int x, int y)
        {
            Cursor.Position = new Point(x, y);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(SafeHandle hEvent);
        internal static void SkipStep()
        {
                        object lockObject = ZappyTaskUtilities.lockObject;
            lock (lockObject)
            {
                isSkipStepOn = true;
                if (skipStepEventHandle != null && !skipStepEventHandle.IsInvalid)
                {
                    SetEvent(skipStepEventHandle);
                }
            }
        }

        public static void Sleep(int milliSeconds)
        {
            Thread.Sleep(milliSeconds);
        }

        public static void SwitchFromImmersiveToWindow(IntPtr windowHandle)
        {
            SwitchFromImmersive.Instance.SwitchFromImmersiveToWindow(windowHandle);
        }

        internal static bool TryConvertToType<T>(object value, out T retValue)
        {
            if (value != null)
            {
                try
                {
                    retValue = (T)value;
                }
                catch (InvalidCastException)
                {
                    if (!TryParseToType(value, out retValue))
                    {
                        return false;
                    }
                }
            }
            else
            {
                retValue = default(T);
            }
            return true;
        }

        public static bool TryCreateRegistryKey(RegistryKey hive, string subKeyPath) =>
            TryCreateRegistryKey(hive, subKeyPath, string.Empty, string.Empty);

        public static bool TryCreateRegistryKey(RegistryKey hive, string subKeyPath, string valueName, string data)
        {
            try
            {
                using (RegistryKey key = hive.CreateSubKey(subKeyPath))
                {
                    if (key != null)
                    {
                        object[] args = { hive.Name, subKeyPath };
                                                if (!string.IsNullOrEmpty(valueName))
                        {
                            key.SetValue(valueName, data, RegistryValueKind.String);
                            object[] objArray2 = { valueName, data };
                                                    }
                        return true;
                    }
                }
            }
            catch (SecurityException exception)
            {
                object[] objArray3 = { subKeyPath, valueName, data, exception };
                            }
            catch (UnauthorizedAccessException exception2)
            {
                object[] objArray4 = { subKeyPath, valueName, data, exception2 };
                            }
            catch (IOException exception3)
            {
                object[] objArray5 = { subKeyPath, valueName, data, exception3 };
                            }
            return false;
        }

        public static bool TryGetDate(string value, out DateTime dateTime)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat = GetDateFormat(culture);
            dateTime = new DateTime();
            return !string.IsNullOrEmpty(value) && DateTime.TryParseExact(value, culture.DateTimeFormat.LongDatePattern, culture, DateTimeStyles.None, out dateTime);
        }

        public static bool TryGetDateString(string value, out string dateString)
        {
            DateTime time;
            dateString = null;
            if (!TryGetDate(value, out time))
            {
                return false;
            }
            dateString = GetDateTimeToString(time, false);
            return true;
        }

        public static void TryGetDateTimeRangeString(string value, out string minDate, out string maxDate)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidDateRange), "value");
            }
            minDate = string.Empty;
            maxDate = string.Empty;
            Match match = dateRangeValueRegex.Match(value);
            minDate = match.Groups["mindate"].Value;
            maxDate = match.Groups["maxdate"].Value;
            if (string.IsNullOrEmpty(minDate))
            {
                minDate = value;
            }
        }

        public static bool TryGetShortDate(string value, out DateTime dateTime)
        {
            if (!DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return DateTime.TryParse(value, out dateTime);
            }
            return true;
        }

        public static bool TryGetShortDateAndLongTime(string value, out DateTime dateTime)
        {
            if (!TryGetShortDate(value, out dateTime))
            {
                CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.DateTimeFormat = GetDateFormat(culture);
                if (!DateTime.TryParseExact(value, culture.DateTimeFormat.ShortDatePattern, culture, DateTimeStyles.None, out dateTime))
                {
                    return DateTime.TryParseExact(value, culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern, culture, DateTimeStyles.None, out dateTime);
                }
            }
            return true;
        }

        public static bool TryGetVersion(string browserName, out Version version)
        {
            version = null;
            try
            {
                for (int i = 0; i < browserName.Length; i++)
                {
                    if (char.IsDigit(browserName[i]))
                    {
                        string str = browserName.Substring(i);
                        try
                        {
                            version = new Version(str);
                        }
                        catch (ArgumentException)
                        {
                            int major = int.Parse(str, CultureInfo.InvariantCulture);
                            version = new Version(major, 0);
                        }
                        return true;
                    }
                }
            }
            catch (SystemException)
            {
            }
            return false;
        }

        public static bool TryParseDateTimeString(string dateTime, out DateTime dateTimeObject)
        {
            if (!DateTime.TryParseExact(dateTime, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeObject) && !DateTime.TryParseExact(dateTime, "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeObject))
            {
                return DateTime.TryParse(dateTime, out dateTimeObject);
            }
            return true;
        }

        private static bool TryParseToType<T>(object value, out T retValue)
        {
            Type type = typeof(T);
            retValue = default(T);
            try
            {
                if (type == typeof(int))
                {
                    int num;
                    if (int.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
                    {
                        object obj2 = num;
                        retValue = (T)obj2;
                    }
                }
                else
                {
                    retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            catch (SystemException exception)
            {
                if (!(exception is InvalidCastException) && !(exception is FormatException))
                {
                    throw;
                }
                return false;
            }
            return true;
        }

        public static void UpdateSqmForMsaaControl(string className)
        {
                                                                                                                                                                                            }

        internal static void WaitForProcessExit(object process, int milliSecond)
        {
            Process process2 = process as Process;
            if (process2 != null)
            {
                if (milliSecond < 0)
                {
                    process2.WaitForExit();
                }
                else
                {
                    process2.WaitForExit(milliSecond);
                }
            }
        }

        public static string CommonDirectory
        {
            get
            {
                Environment.SpecialFolder folder = Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.CommonProgramFilesX86 : Environment.SpecialFolder.CommonProgramFiles;
                if (IsRemoteTestingEnabled)
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                }
                return Environment.GetFolderPath(folder);
            }
        }

        internal static RegexOptions Compiled =>
            RegexOptions.Compiled;

        public static int CurrentProcessId
        {
            get
            {
                if (currentProcessId == 0)
                {
                    currentProcessId = Process.GetCurrentProcess().Id;
                }
                return currentProcessId;
            }
        }

        internal static int CursorPositionX =>
            Cursor.Position.X;

        internal static int CursorPositionY =>
            Cursor.Position.Y;

        public static int IEVersion
        {
            get
            {
                if (ieVersion == -2147483648)
                {
                    ieVersion = -1;
                    string str = GetRegistryValue(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "svcVersion") as string;
                    if (str != null)
                    {
                        int index = str.IndexOf('.');
                        if (index != -1)
                        {
                            int.TryParse(str.Substring(0, index), out ieVersion);
                        }
                    }
                    else
                    {
                        str = GetRegistryValue(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "Version") as string;
                        if (str != null)
                        {
                            try
                            {
                                Version version = new Version(str);
                                ieVersion = version.Major;
                            }
                            catch (SystemException)
                            {
                            }
                        }
                    }
                    if (ieVersion == -1)
                    {
                                            }
                    else
                    {
                        object[] args = { ieVersion };
                                            }
                }
                return ieVersion;
            }
        }

        public static bool IsIE10OrHigher
        {
            get
            {
                if (!isIE10OrHigher.HasValue)
                {
                    isIE10OrHigher = IEVersion >= 10;
                }
                return isIE10OrHigher.Value;
            }
        }

        public static bool IsImageActionLogEnabled
        {
            get
            {
                return false;
                                                                                            }
            set
            {
                isImageActionLogEnabled = value;
            }
        }

        public static bool IsPhone =>
            false;

        public static bool IsRemoteTestingEnabled { get; set; }

        internal static bool IsSkipStepOn
        {
            get
            {
                object lockObject = ZappyTaskUtilities.lockObject;
                lock (lockObject)
                {
                    return isSkipStepOn;
                }
            }
        }

        public static bool IsWin7
        {
            get
            {
                if (!isWin7.HasValue || !isWin7.HasValue)
                {
                    isWin7 = Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1;
                }
                return isWin7.Value;
            }
        }

        public static string ProgramFiles
        {
            get
            {
                Environment.SpecialFolder folder = Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles;
                if (IsRemoteTestingEnabled)
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                }
                return Environment.GetFolderPath(folder);
            }
        }

                                                                                                        public static IAccessible GetAccessibleFromServiceProvider(IServiceProvider serviceProvider)
        {
            IAccessible objectForIUnknown = null;
            if (serviceProvider != null)
            {
                try
                {
                                                                                                                                                            }
                catch (Exception exception)
                {
                    CrapyLogger.log.Error(exception);
                }
            }
            return objectForIUnknown;
        }


        public static bool IsNativeMsaaElement(ITaskActivityElement element)
        {
            bool flag = false;
            if (element != null && UITechnologyManager.AreEqual("MSAA", element.Framework))
            {
                flag = true;
                try
                {
                    flag = !GetManagedMsaaPropertyValue(element);
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (InvalidCastException)
                {
                }
            }
            return flag;
        }

        public static bool GetManagedMsaaPropertyValue(ITaskActivityElement element)
        {
            bool flag = false;
            try
            {
                object propertyValue = element.GetPropertyValue("_isManagedMsaaElement");
                if (propertyValue is bool)
                {
                    flag = (bool)propertyValue;
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            catch (InvalidCastException)
            {
            }
            return flag;
        }

    }
}