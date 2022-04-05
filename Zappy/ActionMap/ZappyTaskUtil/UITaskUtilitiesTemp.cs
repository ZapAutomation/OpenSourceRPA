using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using Accessibility;
using Crapy.ActionMap.Enums;
using Crapy.ActionMap.HelperClasses;
using Crapy.ActionMap.UITechnology;
using Crapy.LogManager;
using Microsoft.Win32;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

//namespace Crapy.ActionMap.UITask
//{
//    internal static class UITaskUtilities
//    {
//        internal static string DateFormat = "dd-MMM-yyyy";
//        internal static string DateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
//        internal const string InternetExplorerFrameworkId = "InternetExplorer";
//        internal static bool s_useManagedMSAA = false;
//        internal static bool UserInteractive = UITaskUtilities.UserInteractive;

//        internal static void AddAssemblyResolveHandler()
//        {
//            AppDomain.CurrentDomain.AssemblyResolve += UITaskUtilitiesStore.OnResolve;
//        }




//        internal static void BringStartMenuToFocus()
//        {
//            UITaskUtilities.BringStartMenuToFocus();
//        }

//        internal static bool CanResetSkipStep() =>
//            UITaskUtilities.CanResetSkipStep();

//        public static void CheckForNull(IntPtr parameter, string parameterName)
//        {
//            UITaskUtilities.CheckForNull(parameter, parameterName);
//        }

//        public static void CheckForNull(object parameter, string parameterName)
//        {
//            UITaskUtilities.CheckForNull(parameter, parameterName);
//        }

//        public static void CheckForNull(string parameter, string parameterName)
//        {
//            UITaskUtilities.CheckForNull(parameter, parameterName);
//        }

//        public static void CheckForPointWithinControlBounds(int offsetX, int offsetY, Rectangle controlBound, string message)
//        {
//            UITaskUtilities.CheckForPointWithinControlBounds(offsetX, offsetY, controlBound, message);
//        }

//        internal static void CleanUpTempFiles()
//        {
//            UITaskUtilities.CleanUpTempFiles();
//        }

//        internal static void CloseProcess(object process)
//        {
//            UITaskUtilities.CloseProcess(process);
//        }

//        public static object ConvertIUnknownToTypedObject(object iunknownObject, System.Type type)
//        {
//            IntPtr ptr2;
//            IntPtr iUnknownForObjectInContext = Marshal.GetIUnknownForObjectInContext(iunknownObject);
//            Guid iid = Marshal.GenerateGuidForType(type);
//            Marshal.ThrowExceptionForHR(Marshal.QueryInterface(iUnknownForObjectInContext, ref iid, out ptr2));
//            return Marshal.GetTypedObjectForIUnknown(ptr2, type);
//        }

//        public static ControlStates ConvertState(AccessibleStates accState)
//        {
//            if (accState == AccessibleStates.None)
//            {
//                return (ControlStates.None | ControlStates.Normal);
//            }
//            return (ControlStates)((long)accState);
//        }

//        internal static string ConvertTo32BitString(string fileName) =>
//            UITaskUtilities.ConvertTo32BitString(fileName);

//        internal static T ConvertToType<T>(object value) =>
//            ConvertToType<T>(value, true);

//        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
//        internal static T ConvertToType<T>(object value, bool throwIfValueIsNull)
//        {
//            T local;
//            if (throwIfValueIsNull && (value == null))
//            {
//                RethrowException exception = new RethrowException(new ArgumentNullException(), true)
//                {
//                    DataType = typeof(T)
//                };
//                throw exception;
//            }
//            if (!TryConvertToType<T>(value, out local))
//            {
//                RethrowException exception2 = new RethrowException(new InvalidCastException(), true)
//                {
//                    DataType = typeof(T),
//                    Value = value
//                };
//                throw exception2;
//            }
//            return local;
//        }

//        [Conditional("DEBUG")]
//        public static void DisableDebugUIMessages()
//        {
//        }

//        internal static void DisposeSkipEventObject()
//        {
//            UITaskUtilities.DisposeSkipEventObject();
//        }

//        internal static bool ExistsRegistryKey(RegistryKey hive, string subKeyPath) =>
//            UITaskUtilities.ExistsRegistryKey(hive, subKeyPath);

//        internal static bool ExistsRegistryKey(RegistryKey hive, string subKeyPath, string valueName) =>
//            UITaskUtilities.ExistsRegistryKey(hive, subKeyPath, valueName);

//        public static IAccessible GetAccessibleFromServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
//        {
//            IAccessible objectForIUnknown = null;
//            if (serviceProvider != null)
//            {
//                try
//                {
//                    Guid gUID = typeof(IAccessible).GUID;
//                    IntPtr zero = IntPtr.Zero;
//                    if ((serviceProvider.QueryService(ref gUID, ref gUID, out zero) == 0) && (zero != IntPtr.Zero))
//                    {
//                        objectForIUnknown = Marshal.GetObjectForIUnknown(zero) as IAccessible;
//                    }
//                }
//                catch (Exception exception)
//                {
//                    //EqtTrace.Error(exception);
//                }
//            }
//            return objectForIUnknown;
//        }

//        internal static IDictionary<string, string> GetAppSettings() =>
//            UITaskUtilities.GetAppSettings();

//        public static string GetAssemblyFileVersion(Assembly assembly) =>
//            UITaskUtilities.GetAssemblyFileVersion(assembly);

//        public static DateTimeFormatInfo GetDateFormat(CultureInfo culture) =>
//            UITaskUtilities.GetDateFormat(culture);

//        public static string GetDateTimeToString(DateTime dateTime, bool includeTime) =>
//            UITaskUtilities.GetDateTimeToString(dateTime, includeTime);

//        public static Assembly GetExecutingAssembly() =>
//            Assembly.GetCallingAssembly();

//        public static string GetExpandedLongPath(string path) =>
//            UITaskUtilities.GetExpandedLongPath(path);

//        public static Stream GetFileStreamWithCreateReadWriteAccess(string file) =>
//            UITaskUtilities.GetFileStreamWithCreateReadWriteAccess(file);

//        public static Uri GetIEHomepage() =>
//            UITaskUtilities.GetIEHomepage();

//        public static int GetLcidFromWindowHandle(IntPtr windowHandle) =>
//            UITaskUtilities.GetLcidFromWindowHandle(windowHandle);

//        public static int[] GetLocaleIdentifiers(IDictionary<string, string> appSettings) =>
//            UITaskUtilities.GetLocaleIdentifiers(appSettings);

//        public static bool GetManagedMsaaPropertyValue(IUITechnologyElement element)
//        {
//            bool flag = false;
//            try
//            {
//                object propertyValue = element.GetPropertyValue("_isManagedMsaaElement");
//                if (propertyValue is bool)
//                {
//                    flag = (bool)propertyValue;
//                }
//            }
//            catch (NotSupportedException)
//            {
//            }
//            catch (NotImplementedException)
//            {
//            }
//            catch (InvalidCastException)
//            {
//            }
//            return flag;
//        }

//        public static string GetOSInfo(string propertyName) =>
//            UITaskUtilities.GetOSInfo(propertyName);

//        internal static object GetProcessById(int pid) =>
//            UITaskUtilities.GetProcessById(pid);

//        internal static int GetProcessId(object process) =>
//            UITaskUtilities.GetProcessId(process);

//        public static int GetProcessIdForWindow(IntPtr windowHandle) =>
//            UITaskUtilities.GetProcessIdForWindow(windowHandle);

//        internal static string GetProcessMainWindowTitle(object process) =>
//            UITaskUtilities.GetProcessMainWindowTitle(process);

//        public static object GetRegistryValue(RegistryKey regKey, string subKeyPath, string registryValue) =>
//            UITaskUtilities.GetRegistryValue(regKey, subKeyPath, registryValue);

//        public static T GetRegistryValue<T>(RegistryKey regKey, string subKeyPath, string registryValue, bool throwException) =>
//            UITaskUtilities.GetRegistryValue<T>(regKey, subKeyPath, registryValue, throwException);

//        public static string GetShortDateRangeString(DateTime minRange, DateTime maxRange) =>
//            UITaskUtilities.GetShortDateRangeString(minRange, maxRange);

//        public static string GetShortDateRangeStringInCurrentCulture(DateTime minRange, DateTime maxRange) =>
//            UITaskUtilities.GetShortDateRangeStringInCurrentCulture(minRange, maxRange);

//        public static StreamReader GetStreamReader(string file) =>
//            UITaskUtilities.GetStreamReader(file);

//        internal static Assembly GetTypeAssembly(System.Type type) =>
//            UITaskUtilities.GetTypeAssembly(type);

//        public static string GetUniqueName(string path, string basename, string extension) =>
//            UITaskUtilities.GetUniqueName(path, basename, extension);

//        public static string GetUniqueName(string path, string basename, string extension, int basecount) =>
//            UITaskUtilities.GetUniqueName(path, basename, extension, basecount);

//        public static bool GetValue<T>(IDictionary<string, string> appSettings, string key, out T value) =>
//            UITaskUtilities.GetValue<T>(appSettings, key, out value);

//        public static bool HasObjectTime(string value) =>
//            UITaskUtilities.HasObjectTime(value);

//        internal static bool HasProcessExited(object process) =>
//            UITaskUtilities.HasProcessExited(process);

//        internal static void InitLogFiles()
//        {
//            UITaskUtilities.InitLogFiles();
//        }

//        internal static void InitSkipEventObject(string skipPlayBackEventName)
//        {
//            UITaskUtilities.InitSkipEventObject(skipPlayBackEventName);
//        }

//        public static bool IsCharmsBar(IntPtr windowHandle) =>
//            UITaskUtilities.IsCharmsBar(windowHandle);

//        public static bool IsCharmsBar(IntPtr windowHandle, string windowClassName) =>
//            UITaskUtilities.IsCharmsBar(windowHandle, windowClassName);

//        public static bool IsDesktopWindowAndNotLegacy(IntPtr windowHandle) =>
//            false;

//        public static bool IsDesktopWindowAndNotLegacy(AutomationElement element) =>
//            false;

//        public static bool IsFirefoxWindowClassName(string className) =>
//            UITaskUtilities.IsFirefoxWindowClassName(className);

//        public static bool IsHigherPrivilegeProcess(IntPtr windowHandle) =>
//            UITaskUtilities.IsHigherPrivilegeProcess(windowHandle);

//        public static bool IsHigherPrivilegeProcess(int processId, out string fileName) =>
//            UITaskUtilities.IsHigherPrivilegeProcess(processId, out fileName);

//        public static bool IsIEWindow(IntPtr windowHandle) =>
//            UITaskUtilities.IsIEWindow(windowHandle);

//        public static bool IsIEWindowClassName(string className) =>
//            UITaskUtilities.IsIEWindowClassName(className);

//        public static bool IsImmersiveBrowserWindow(IntPtr windowHandle) =>
//            UITaskUtilities.IsImmersiveBrowserWindow(windowHandle);

//        public static bool IsImmersiveModeActive() =>
//            UITaskUtilities.IsImmersiveModeActive();

//        public static bool IsImmersiveWindow(IntPtr windowHandle) =>
//            UITaskUtilities.IsImmersiveWindow(windowHandle);

//        public static bool IsImmersiveWindowClassName(string className) =>
//            UITaskUtilities.IsImmersiveWindowClassName(className);

//        internal static bool IsInvalidComObjectException(Exception ex) =>
//            (ex is InvalidComObjectException);

//        public static bool IsManagedMsaaElement(IUITechnologyElement element)
//        {
//            bool managedMsaaPropertyValue = false;
//            if ((element != null) && UITechnologyManager.AreEqual("MSAA", element.TechnologyName))
//            {
//                managedMsaaPropertyValue = GetManagedMsaaPropertyValue(element);
//            }
//            return managedMsaaPropertyValue;
//        }

//        public static bool IsMfcClassName(string elementClassName) =>
//            UITaskUtilities.IsMfcClassName(elementClassName);

//        public static bool IsNativeMsaaElement(IUITechnologyElement element)
//        {
//            bool flag = false;
//            if ((element != null) && UITechnologyManager.AreEqual("MSAA", element.TechnologyName))
//            {
//                flag = true;
//                try
//                {
//                    flag = !GetManagedMsaaPropertyValue(element);
//                }
//                catch (NotSupportedException)
//                {
//                }
//                catch (NotImplementedException)
//                {
//                }
//                catch (InvalidCastException)
//                {
//                }
//            }
//            return flag;
//        }

//        internal static bool IsProtectedWindow() =>
//            UITaskUtilities.IsProtectedWindow();

//        internal static bool IsRootParentSupportedControl(IntPtr windowHandle, string className) =>
//            UITaskUtilities.IsRootParentSupportedControl(windowHandle, className);

//        public static bool IsSilverlightClassName(string elementClassName) =>
//            UITaskUtilities.IsSilverlightClassName(elementClassName);

//        internal static bool IsStartScreenVisible() =>
//            UITaskUtilities.IsStartScreenVisible();

//        public static bool IsWin8OrGreaterOs() =>
//            UITaskUtilities.IsWin8OrGreaterOs();

//        internal static bool IsWindows8OrLaterOs() =>
//            UITaskUtilities.IsWindows8OrLaterOs();

//        public static bool IsWindowsStartScreen(IntPtr windowHandle) =>
//            UITaskUtilities.IsWindowsStartScreen(windowHandle);

//        public static bool IsWinformsClassName(string elementClassName) =>
//            UITaskUtilities.IsWinformsClassName(elementClassName);

//        public static bool IsWpfClassName(string elementClassName) =>
//            UITaskUtilities.IsWpfClassName(elementClassName);

//        public static bool IsWWAWindow(IntPtr windowHandle) =>
//            UITaskUtilities.IsWWAWindow(windowHandle);

//        internal static void KillProcess(object process)
//        {
//            UITaskUtilities.KillProcess(process);
//        }

//        public static string NormalizeDynamicClassName(string elementClassName) =>
//            UITaskUtilities.NormalizeDynamicClassName(elementClassName);

//        internal static void RefreshProcess(object process)
//        {
//            UITaskUtilities.RefreshProcess(process);
//        }

//        [Conditional("DEBUG")]
//        public static void RestoreDebugUIMessages()
//        {
//        }

//        public static long RoundOffMillisecondToSecond(long milliSecondValue) =>
//            UITaskUtilities.RoundOffMillisecondToSecond(milliSecondValue);

//        public static void SafeThreadJoin(Thread threadToJoin)
//        {
//            UITaskUtilities.SafeThreadJoin(threadToJoin);
//        }

//        public static bool SafeThreadJoin(Thread threadToJoin, int timeout, bool abort) =>
//            UITaskUtilities.SafeThreadJoin(threadToJoin, timeout, abort);

//        internal static void SetCursorPosition(int x, int y)
//        {
//            Cursor.Position = new Point(x, y);
//        }

//        internal static void SkipStep()
//        {
//            UITaskUtilities.SkipStep();
//        }

//        public static void Sleep(int milliSeconds)
//        {
//            UITaskUtilities.Sleep(milliSeconds);
//        }

//        public static void SwitchFromImmersiveToWindow(IntPtr windowHandle)
//        {
//            UITaskUtilities.SwitchFromImmersiveToWindow(windowHandle);
//        }

//        internal static bool TryConvertToType<T>(object value, out T retValue) =>
//            UITaskUtilities.TryConvertToType<T>(value, out retValue);

//        public static bool TryCreateRegistryKey(RegistryKey hive, string subKeyPath) =>
//            UITaskUtilities.TryCreateRegistryKey(hive, subKeyPath);

//        public static bool TryCreateRegistryKey(RegistryKey hive, string subKeyPath, string valueName, string data) =>
//            UITaskUtilities.TryCreateRegistryKey(hive, subKeyPath, valueName, data);

//        public static bool TryGetDate(string value, out DateTime dateTime) =>
//            UITaskUtilities.TryGetDate(value, out dateTime);

//        public static bool TryGetDateString(string value, out string dateString) =>
//            UITaskUtilities.TryGetDateString(value, out dateString);

//        public static void TryGetDateTimeRangeString(string value, out string minDate, out string maxDate)
//        {
//            UITaskUtilities.TryGetDateTimeRangeString(value, out minDate, out maxDate);
//        }

//        public static bool TryGetShortDate(string value, out DateTime dateTime) =>
//            UITaskUtilities.TryGetShortDate(value, out dateTime);

//        public static bool TryGetShortDateAndLongTime(string value, out DateTime dateTime) =>
//            UITaskUtilities.TryGetShortDateAndLongTime(value, out dateTime);

//        public static bool TryGetVersion(string browserName, out Version version) =>
//            UITaskUtilities.TryGetVersion(browserName, out version);

//        public static bool TryParseDateTimeString(string dateTime, out DateTime dateTimeObject) =>
//            UITaskUtilities.TryParseDateTimeString(dateTime, out dateTimeObject);

//        public static void UpdateSqmForMsaaControl(string className)
//        {
//            UITaskUtilities.UpdateSqmForMsaaControl(className);
//        }

//        internal static void WaitForProcessExit(object process, int milliSecond)
//        {
//            UITaskUtilities.WaitForProcessExit(process, milliSecond);
//        }

//        public static string CommonDirectory =>
//            UITaskUtilities.CommonDirectory;

//        public static RegexOptions Compiled =>
//            UITaskUtilities.Compiled;

//        public static int CurrentProcessId =>
//            UITaskUtilities.CurrentProcessId;

//        internal static int CursorPositionX =>
//            Cursor.Position.X;

//        internal static int CursorPositionY =>
//            Cursor.Position.Y;

//        public static int IEVersion =>
//            UITaskUtilities.IEVersion;

//        public static bool IsIE10OrHigher =>
//            UITaskUtilities.IsIE10OrHigher;

//        public static bool IsImageActionLogEnabled
//        {
//            get
//            {
//                return UITaskUtilities.IsImageActionLogEnabled;
//            }
//            set
//            {
//                UITaskUtilities.IsImageActionLogEnabled = value;
//            }
//        }

//        public static bool IsPhone =>
//            UITaskUtilities.IsPhone;

//        public static bool IsRemoteTestingEnabled
//        {
//            get
//            {
//                return UITaskUtilities.IsRemoteTestingEnabled;
//            }
//            set
//            {
//                UITaskUtilities.IsRemoteTestingEnabled = value;
//            }
//        }

//        internal static bool IsSkipStepOn =>
//            UITaskUtilities.IsSkipStepOn;

//        public static bool IsWin7 =>
//            UITaskUtilities.IsWin7;

//        public static string ProgramFiles =>
//            UITaskUtilities.ProgramFiles;

//        public static bool UseManagedMSAA
//        {
//            get
//            {
//                return s_useManagedMSAA;
//            }
//            set
//            {
//                s_useManagedMSAA = value;
//            }
//        }
//    }

//}
