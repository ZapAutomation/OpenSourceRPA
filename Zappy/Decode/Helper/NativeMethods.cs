using Accessibility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Zappy.Decode.Helper
{
                

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal static class NativeMethods
    {
        internal static readonly Guid AccessibleGuid = typeof(IAccessible).GUID;
        internal const int DefaultSendMessageTimeout = 0x3e8;
        private static Dictionary<string, string> deviceToDriveMap;
        private static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture);
        internal const int ERROR_ACCESS_DENIED = 5;
        internal const int GCL_HICONSM = -34;
        internal const string IEControlClassName = "Internet Explorer_Server";
        private static bool? is64BitOperatingSystem;
        internal const int KeyStatePressed = 0x8000;
        private static readonly object lockObject = new object();
        private const int MaxPath = 0x100;
        internal const int MaxWindowTextLength = 0x400;
        internal const int OBJID_CLIENT = -4;
        internal const int OBJID_WINDOW = 0;
                internal const uint SWP_NOMOVE = 2;
        internal const uint SWP_NOSIZE = 1;
        private const int TOKEN_QUERY = 8;
        private const int TokenElevationTypeClass = 0x12;
        private const int TokenElevationTypeFull = 2;
        internal const int ToolBarCloseBtnIndex = 5;
        internal const int ToolBarHelpBtnIndex = 4;
        internal const int ToolBarMaxBtnIndex = 3;
        internal const int ToolBarMinBtnIndex = 2;
        private const string WindowsHtmlMessage = "WM_HTML_GETOBJECT";
        private static readonly uint WindowsHtmlMessageId = RegisterWindowMessage("WM_HTML_GETOBJECT");
        internal const int WindowsLowLevelKeyboardHookId = 13;
        internal const int WindowsLowLevelMouseHookId = 14;
        public const int WM_HOTKEY = 0x312;
        private const int WM_NULL = 0;
        internal const int WS_CAPTION = 0xc00000;
        internal const int WS_EX_APPWINDOW = 0x40000;
        internal const int WS_EX_LAYERED = 0x80000;
        internal const int WS_EX_LAYOUTRTL = 0x400000;
        internal const int WS_EX_RIGHT = 0x1000;
        internal const int WS_EX_RTLREADING = 0x2000;
        internal const int WS_EX_TOOLWINDOW = 0x80;
        internal const int WS_EX_TOPMOST = 8;
        internal const int WS_EX_TRANSPARENT = 0x20;
        internal const int WS_MINIMIZE = 0x20000000;
        public const int TVM_GETEDITCONTROL = 0x110F;
        public const int HWND_TOPMOST = -1;

        internal const int MOUSEEVENTF_LEFTDOWN = 0x02;
        internal const int MOUSEEVENTF_LEFTUP = 0x04;
        internal const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        internal const int MOUSEEVENTF_RIGHTUP = 0x0010;

        public const int WM_SETTEXT = 0xC;

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromEvent(IntPtr windowHandle, int objectID, int childID, ref IAccessible accessibleObject, ref object childObject);
        internal static IAccessible AccessibleObjectFromWindow(IntPtr windowHandle) =>
            AccessibleObjectFromWindow(windowHandle, 0);
        internal static bool IsWpfClassName(IntPtr hWnd) =>
            ZappyTaskUtilities.IsWpfClassName(GetClassName(hWnd));

        internal static IAccessible AccessibleObjectFromWindow(IntPtr windowHandle, int objectId)
        {
            Guid accessibleGuid = AccessibleGuid;
            IAccessible pAcc = null;
            if (AccessibleObjectFromWindow(windowHandle, objectId, ref accessibleGuid, ref pAcc) != 0)
            {
                object[] args = { windowHandle };
                CrapyLogger.log.ErrorFormat("AccessibleObjectFromWindow failed for window handle {0}", args);
            }
            return pAcc;
        }




        [DllImport("SHCore.dll", SetLastError = true)]
        internal static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        [DllImport("user32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hWnd);

        public enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

        public enum DPI_AWARENESS_CONTEXT
        {
            DPI_AWARENESS_CONTEXT_UNAWARE = -1,
            DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
            DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5
        }



        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetProcessDpiAwarenessContext(int nFlags);
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetProcessDPIAware();

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hWnd, int dwObjectID, ref Guid riid, ref IAccessible pAcc);
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(LowLevelHookHandle idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
        private static string ConvertDevicePathToWindowsPath(string processName)
        {
            if (string.IsNullOrEmpty(processName))
            {
                CrapyLogger.log.ErrorFormat("ConvertDevicePathToWindowsPath(): Invalid input process name.");
                return string.Empty;
            }
            string str = string.Empty;
            foreach (KeyValuePair<string, string> pair in DeviceToDriveMap)
            {
                if (processName.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    str = processName.Replace(pair.Key, pair.Value);
                }
            }
            if (string.IsNullOrEmpty(str))
            {
                object[] args = { processName };
                CrapyLogger.log.ErrorFormat("ConvertDevicePathToWindowsPath(): No windowspath mapping exists for devicepath '{0}'", args);
            }
            return str;
        }

        private static KeyStates ConvertToKeyStates(short keyStateMask)
        {
            KeyStates none = KeyStates.None;
            if ((keyStateMask & 0x8000) != 0)
            {
                none |= KeyStates.Down;
            }
            if ((keyStateMask & 1) != 0)
            {
                none |= KeyStates.None | KeyStates.Toggled;
            }
            return none;
        }

        [DllImport("Gdi32.dll")]
        internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DestroyIcon(IntPtr handle);
        [DllImport("user32.dll")]
        internal static extern IntPtr DispatchMessage(ref MSG msg);
        [DllImport("msdrm.dll")]
        public static extern int DRMIsWindowProtected(IntPtr windowHandle, [MarshalAs(UnmanagedType.Bool)] ref bool isProtected);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, ref IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc callBack, IntPtr param);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr FindResource(IntPtr hModule, uint lpName, uint lpType);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr windowHandle1, IntPtr windowHandle2, [MarshalAs(UnmanagedType.LPWStr)] string lpszClass, [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlag gaFlags);
        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(int vKey);
        internal static IntPtr GetClassLongPtr(IntPtr windowHandle, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetClassLongPtr64(windowHandle, nIndex);
            }
            return new IntPtr(GetClassLongPtr32(windowHandle, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern int GetClassLongPtr32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);
        internal static string GetClassName(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return string.Empty;
            }
            StringBuilder lpClassName = new StringBuilder(0x100);
            if (GetClassName(windowHandle, lpClassName, lpClassName.Capacity) <= 0)
            {
                
            }
            return lpClassName.ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        internal static Image GetCurrenProcessIcon()
        {
            Image currenProcessIconAsIs = GetCurrenProcessIconAsIs();
            if (currenProcessIconAsIs != null && currenProcessIconAsIs.Size.Height != 0x10 && currenProcessIconAsIs.Size.Width != 0x10)
            {
                currenProcessIconAsIs = new Bitmap(currenProcessIconAsIs, new Size(0x10, 0x10));
            }
            return currenProcessIconAsIs;
        }

        internal static Image GetCurrenProcessIconAsIs()
        {
            Bitmap bitmap = null;
            using (Process process = Process.GetCurrentProcess())
            {
                IntPtr zero = IntPtr.Zero;
                Icon icon = null;
                try
                {
                    zero = GetClassLongPtr(process.MainWindowHandle, -34);
                    if (zero != IntPtr.Zero)
                    {
                        icon = Icon.FromHandle(zero);
                    }
                    if (icon == null)
                    {
                        icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                    }
                    if (icon != null)
                    {
                        bitmap = icon.ToBitmap();
                    }
                }
                finally
                {
                    if (icon != null)
                    {
                        icon.Dispose();
                    }
                    if (zero != IntPtr.Zero)
                    {
                        DestroyIcon(zero);
                    }
                }
            }
            return bitmap;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDesktopWindow();
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int index);
        internal static T GetDocumentFromWindowHandle<T>(IntPtr windowHandleOfIE) where T : class
        {
            IntPtr zero = IntPtr.Zero;
            if (SendMessageTimeout(windowHandleOfIE, WindowsHtmlMessageId, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 0x3e8, out zero) == IntPtr.Zero)
            {
                int num = Marshal.GetLastWin32Error();
                if (num == 5)
                {
                    throw new Exception("AccessDenied");
                }
                object[] args = { num };
                
                return default(T);
            }
            if (zero == IntPtr.Zero)
            {
                
                return default(T);
            }
            try
            {
                return (T)ObjectFromLresult(zero, typeof(T).GUID, IntPtr.Zero);
            }
            catch (COMException exception)
            {
                object[] objArray2 = { exception.ErrorCode };
                
            }
            catch (InvalidCastException)
            {
                object[] objArray3 = { typeof(T).Name };
                
            }
            return default(T);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);


        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);


        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetGUIThreadInfo(uint idThread, out GUITHREADINFO lpgui);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetKeyboardLayout(uint idThread);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        private static KeyStates GetKeyState(Keys virtualKeyCode) =>
            ConvertToKeyStates(GetKeyState((int)virtualKeyCode));

        [DllImport("kernel32.dll")]
        internal static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);
        private static string[] GetNetworkDevicePaths()
        {
            List<string> list = new List<string>();
            object[] args = { DirectorySeparator };
            string item = string.Format(CultureInfo.InvariantCulture, "{0}Device{0}Mup", args);
            list.Add(item);
            string str2 = ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\NetworkProvider\Order", "ProviderOrder") as string;
            if (!string.IsNullOrEmpty(str2))
            {
                foreach (string str3 in str2.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    object[] objArray2 = { str3.Trim() };
                    string subKeyPath = string.Format(CultureInfo.InvariantCulture, @"SYSTEM\CurrentControlSet\Services\{0}\NetworkProvider", objArray2);
                    string str5 = ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, subKeyPath, "DeviceName") as string;
                    if (!string.IsNullOrEmpty(str5))
                    {
                        list.Add(str5.Trim());
                    }
                }
            }
            return list.ToArray();
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetParent(IntPtr hwnd);
        internal static string GetProcessFileName(int processId)
        {
            IntPtr hProcess = OpenProcess(ProcessAccessRights.PROCESS_QUERY_LIMITED_INFORMATION, false, (uint)processId);
            if (hProcess == IntPtr.Zero)
            {
                object[] args = { processId, Marshal.GetLastWin32Error() };
                CrapyLogger.log.WarnFormat("GetProcessFileName(): Error in OpenProcess for processId {0}: {1}", args);
                return string.Empty;
            }
            StringBuilder lpImageFileName = new StringBuilder(0x101);
            if (GetProcessImageFileName(hProcess, lpImageFileName, 0x101) == 0)
            {
                object[] objArray2 = { processId, Marshal.GetLastWin32Error() };
                CrapyLogger.log.WarnFormat("GetProcessFileName(): Error in GetProcessImageFileName for processId {0}: {1}", objArray2);
                CloseHandle(hProcess);
                return string.Empty;
            }
            CloseHandle(hProcess);
            return ConvertDevicePathToWindowsPath(lpImageFileName.ToString());
        }

        [DllImport("psapi.dll", CharSet = CharSet.Auto)]
        internal static extern uint GetProcessImageFileName(IntPtr hProcess, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpImageFileName, uint nSize);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetTitleBarInfo(IntPtr hwnd, ref TITLEBARINFO pti);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr tokenHandle, uint tokenInformationClass, out uint tokenInformation, uint tokenInformationLength, out uint returnLength);
        [DllImport("kernel32.dll")]
        public static extern ushort GetUserDefaultUILanguage();
        [DllImport("kernel32.dll")]
        public static extern uint GetVersionEx(ref OSVERSIONINFO lpVersionInfo);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hwnd, GetWindowFlag uCmd);
        public static int GetWindowLong(IntPtr hWnd, GWLParameter index)
        {
            try
            {
                if (IntPtr.Size == 8)
                {
                    return (int)GetWindowLongPtr64(hWnd, index);
                }
                return GetWindowLong32(hWnd, index);
            }
            catch (OverflowException)
            {
                return 0;
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr windowHandle, GWLParameter nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr windowHandle, GWLParameter nIndex);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT ptr);

        internal static uint GetWindowProcessId(IntPtr windowHandle)
        {
            uint num;
            GetWindowThreadProcessId(windowHandle, out num);
            return num;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        internal static string GetWindowText(IntPtr winHandle)
        {
            string str = String.Empty;
            try
            {
                StringBuilder windowText = new StringBuilder(0x400);
                int resultInt32 = GetWindowText(winHandle, windowText, windowText.Capacity);
                if (resultInt32 > 0)
                {
                    str = windowText.ToString();
                }
                                                                
                                                                                            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                                throw;
            }

            return str;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr windowHandle, StringBuilder windowText, int maxCharCount);
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint dwProcessId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern ushort GlobalAddAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern ushort GlobalDeleteAtom(ushort atom);
        [DllImport("ieframe.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr IELaunchURL(string url, ref ProcessInformation pinfo, IntPtr ptr);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);
        internal static bool IsKeyStateDown(Keys virtualKeyCode) =>
            (GetKeyState(virtualKeyCode) & KeyStates.Down) == KeyStates.Down;

        internal static bool IsProcess64Bit(uint processId)
        {
            if (!Is64BitOperatingSystem)
            {
                return false;
            }
            IntPtr hProcess = OpenProcess(ProcessAccessRights.PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                object[] args = { processId, Marshal.GetLastWin32Error() };
                CrapyLogger.log.ErrorFormat("IsProcess64Bit(): Error in OpenProcess for processId {0}: {1}", args);
                return false;
            }
            bool lpSystemInfo = false;
            if (!IsWow64Process(hProcess, out lpSystemInfo))
            {
                object[] objArray2 = { processId, Marshal.GetLastWin32Error() };
                CrapyLogger.log.ErrorFormat("IsProcess64Bit(): Error in isWow64Process for processId {0}: {1}", objArray2);
                CloseHandle(hProcess);
                return false;
            }
            CloseHandle(hProcess);
            return !lpSystemInfo;
        }

        internal static bool IsProcessElevated(IntPtr hwnd)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return false;
            }
            return IsProcessElevated(GetWindowProcessId(hwnd));
        }

        internal static bool IsProcessElevated(uint processId)
        {
            bool flag;
            IntPtr processHandle = OpenProcess(ProcessAccessRights.PROCESS_QUERY_INFORMATION, false, processId);
            IntPtr zero = IntPtr.Zero;
            if (processHandle == IntPtr.Zero)
            {
                object[] args = { processId, Marshal.GetLastWin32Error() };
                CrapyLogger.log.ErrorFormat("IsProcessElevated: OpenProcess failed for processid {0} with error: {1}", args);
                return true;
            }
            try
            {
                uint num;
                if (!OpenProcessToken(processHandle, 8, out zero))
                {
                    object[] objArray2 = { processId, Marshal.GetLastWin32Error() };
                    CrapyLogger.log.ErrorFormat("IsProcessElevated: OpenProcessToken failed for processid {0} with error: {1}", objArray2);
                    return true;
                }
                uint returnLength = 0;
                if (!GetTokenInformation(zero, 0x12, out num, 4, out returnLength))
                {
                    object[] objArray3 = { processId, Marshal.GetLastWin32Error() };
                    CrapyLogger.log.ErrorFormat("IsProcessElevated: GetTokenInformation failed for processid {0} with error: {1}", objArray3);
                    return true;
                }
                if (num == 2)
                {
                    return true;
                }
                flag = false;
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    CloseHandle(zero);
                }
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
            return flag;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsWindowEnabled(IntPtr hWnd);
        public static bool IsWindowResponding(IntPtr windowHandle)
        {
            bool flag = false;
            if (IsWindow(windowHandle))
            {
                IntPtr ptr;
                IntPtr ptr2 = SendMessageTimeout(windowHandle, 0, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 0x3e8, out ptr);
                if (IsWindow(windowHandle) && ptr2 != IntPtr.Zero)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                object[] args = { windowHandle, Marshal.GetLastWin32Error() };
                            }
            return flag;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsZoomed(IntPtr hWnd);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint code, uint mapType);
        internal static uint MapVirtualKey(Keys keys, VirtualKeyMapType mapType) =>
            MapVirtualKey((uint)keys, (uint)mapType);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);
        [return: MarshalAs(UnmanagedType.IDispatch)]
        [DllImport("oleacc.dll", PreserveSig = false)]
        private static extern object ObjectFromLresult(IntPtr msgcallResult, [MarshalAs(UnmanagedType.LPStruct)] Guid refGuid, IntPtr resultRef);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccessRights dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool PeekMessage(ref MSG msg, IntPtr hwnd, int nMsgFilterMin, int nMsgFilterMax, int wRemoveMsg);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern uint QueryDosDevice([MarshalAs(UnmanagedType.LPWStr)] string lpDeviceName, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpTargetPath, uint ucchMax);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern uint RegCloseKey(UIntPtr hkey);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, Keys vk);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint RegisterWindowMessage(string messageString);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint RegOpenKeyEx(UIntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, uint ulOptions, int samDesired, out UIntPtr phkResult);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint RegQueryInfoKey(UIntPtr hKey, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpClass, ref uint lpcClass, IntPtr lpReserved, out uint lpcSubKeys, out uint lpcbMaxSubKeyLen, out uint lpcbMaxClassLen, out uint lpcValues, out uint lpcMaxValueNameLen, out uint lpcMaxValueLen, IntPtr lpcbSecurityDescriptor, out FILETIME lpftLastWriteTime);
        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, StringBuilder builder);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, string builder);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        internal static int SetWindowLong(IntPtr windowHandle, GWLParameter nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return (int)SetWindowLongPtr64(windowHandle, nIndex, new IntPtr(dwNewLong));
            }
            return SetWindowLong32(windowHandle, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr windowHandle, GWLParameter nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);
        internal static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr windowHandle, GWLParameter nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("User32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool redraw);
        [DllImport("user32.dll")]
        internal static extern LowLevelHookHandle SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(AccessibleEvents eventMin, AccessibleEvents eventMax, IntPtr eventHookAssemblyHandle, WinEventProc eventHookHandle, uint processId, uint threadId, SetWinEventHookParameter parameterFlags);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        [DllImport("kernel32.dll")]
        internal static extern uint SizeofResource(IntPtr hModule, IntPtr hResource);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(IntPtr idHook);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr eventHookHandle);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);
        internal static IntPtr WindowFromAccessibleObject(IAccessible pAcc)
        {
            uint num;
            IntPtr zero = IntPtr.Zero;
            try
            {
                num = WindowFromAccessibleObject(pAcc, ref zero);
            }
            catch (InvalidCastException)
            {
                return IntPtr.Zero;
            }
            if (num == 0x8001010d)
            {
                zero = IntPtr.Zero;
            }
            return zero;
        }

        [DllImport("oleacc.dll")]
        private static extern uint WindowFromAccessibleObject(IAccessible pAcc, ref IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(POINT pt);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern bool Wow64DisableWow64FsRedirection(out IntPtr oldValue);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern bool Wow64RevertWow64FsRedirection(IntPtr oldValue);

        internal static Dictionary<string, string> DeviceToDriveMap
        {
            get
            {
                object lockObject = NativeMethods.lockObject;
                lock (lockObject)
                {
                    if (deviceToDriveMap == null)
                    {
                        deviceToDriveMap = new Dictionary<string, string>();
                        string[] networkDevicePaths = GetNetworkDevicePaths();
                        string str = DirectorySeparator + DirectorySeparator;
                        foreach (string str2 in networkDevicePaths)
                        {
                            string key = str2 + DirectorySeparator;
                            if (!deviceToDriveMap.ContainsKey(key))
                            {
                                deviceToDriveMap.Add(key, str);
                                object[] args = { key, str };
                                
                            }
                        }
                        DriveInfo[] drives = DriveInfo.GetDrives();
                        if (drives != null)
                        {
                            foreach (DriveInfo info in drives)
                            {
                                if (!string.IsNullOrEmpty(info.Name) && info.Name.EndsWith(DirectorySeparator, StringComparison.Ordinal))
                                {
                                    string lpDeviceName = info.Name.Remove(info.Name.Length - 1);
                                    StringBuilder lpTargetPath = new StringBuilder(0x101);
                                    uint num3 = QueryDosDevice(lpDeviceName, lpTargetPath, 0x101);
                                    int num4 = Marshal.GetLastWin32Error();
                                    string str5 = lpTargetPath.ToString();
                                    if (num3 <= 0 || string.IsNullOrEmpty(str5))
                                    {
                                        object[] objArray2 = { num4, info.Name };
                                        CrapyLogger.log.ErrorFormat("ConstructDeviceToDriveMap(): QueryDosDevice failed with error '{0}' for {1}", objArray2);
                                    }
                                    else
                                    {
                                        string str6 = str5 + DirectorySeparator;
                                        if (!deviceToDriveMap.ContainsKey(str6))
                                        {
                                            deviceToDriveMap.Add(str6, info.Name);
                                            object[] objArray3 = { str6, info.Name };
                                            
                                        }
                                        else
                                        {
                                            object[] objArray4 = { str6 };
                                            CrapyLogger.log.ErrorFormat("ConstructDeviceToDriveMap(): Duplicate device path '{0}'", objArray4);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return deviceToDriveMap;
            }
        }

        internal static bool Is64BitOperatingSystem
        {
            get
            {
                if (!is64BitOperatingSystem.HasValue)
                {
                    is64BitOperatingSystem = false;
                    SYSTEM_INFO lpSystemInfo = new SYSTEM_INFO();
                    GetNativeSystemInfo(ref lpSystemInfo);
                    ProcessorArchitecture wProcessorArchitecture = (ProcessorArchitecture)lpSystemInfo.wProcessorArchitecture;
                    if (wProcessorArchitecture == ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL)
                    {
                        is64BitOperatingSystem = false;
                    }
                    else if (wProcessorArchitecture == ProcessorArchitecture.PROCESSOR_ARCHITECTURE_IA64 || wProcessorArchitecture == ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64)
                    {
                        is64BitOperatingSystem = true;
                    }
                    else
                    {
                        is64BitOperatingSystem = Environment.GetEnvironmentVariable("%ProgramFiles(x86)") != null;
                    }
                }
                return is64BitOperatingSystem.Value;
            }
        }

        [Flags]
        internal enum DIALOG_STYLES
        {
            DS_SETFONT = 0x40,
            DS_SHELLFONT = 0x48
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DLGITEMTEMPLATE
        {
            public uint style;
            public uint exStyle;
            public short x;
            public short y;
            public short cx;
            public short cy;
            public ushort id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DLGITEMTEMPLATEEX
        {
            public uint helpID;
            public uint exStyle;
            public uint style;
            public short x;
            public short y;
            public short cx;
            public short cy;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DLGTEMPLATE
        {
            public uint dwStyle;
            public uint dwExStyle;
            public ushort cDlgItems;
            public short x;
            public short y;
            public short cx;
            public short cy;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DLGTEMPLATEEX
        {
            public ushort wDlgVer;
            public ushort wSignature;
            public uint dwHelpID;
            public uint dwExStyle;
            public uint dwStyle;
            public ushort cDlgItems;
            public short x;
            public short y;
            public short cx;
            public short cy;
        }

        internal delegate bool EnumWindowsProc(IntPtr windowHandle, ref IntPtr lParam);

        internal enum ExtendedWindowStyles
        {
            WS_EX_LAYERED = 0x80000,
            WS_EX_TRANSPARENT = 0x20
        }

        internal enum GetAncestorFlag : uint
        {
            GA_PARENT = 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3
        }

        internal enum GetWindowFlag : uint
        {
            GW_OWNER = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        internal enum GWLParameter
        {
            GWL_EXSTYLE = -20,
            GWL_HINSTANCE = -6,
            GWL_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KeyboardHookStruct
        {
            internal int vkCode;
            internal int scanCode;
            internal int flags;
            internal int time;
            internal IntPtr dwExtraInfo;
        }

        internal class LowLevelHookHandle : SafeHandle
        {
            public LowLevelHookHandle() : base(IntPtr.Zero, true)
            {
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    return UnhookWindowsHookEx(handle);
                }
                return true;
            }

            public override bool IsInvalid =>
                handle == IntPtr.Zero;
        }

        internal delegate IntPtr LowLevelHookProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseLLHookStruct
        {
            internal POINT pt;
            internal int mouseData;
            internal int flags;
            internal int time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            internal IntPtr hwnd;
            internal uint message;
            internal IntPtr wParam;
            internal IntPtr lParam;
            internal uint time;
            internal int ptX;
            internal int ptY;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct OSVERSIONINFO
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int x;
            internal int y;
            internal POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Flags]
        internal enum ProcessAccessRights : uint
        {
            PROCESS_QUERY_INFORMATION = 0x400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_VM_ALL = 0x38,
            PROCESS_VM_OPERATION = 8,
            PROCESS_VM_READ = 0x10,
            PROCESS_VM_WRITE = 0x20
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcessInformation
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        internal enum ProcessorArchitecture
        {
            PROCESSOR_ARCHITECTURE_AMD64 = 9,
            PROCESSOR_ARCHITECTURE_IA64 = 6,
            PROCESSOR_ARCHITECTURE_INTEL = 0
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        [Flags]
        internal enum SendMessageTimeoutFlags
        {
            SMTO_ABORTIFHUNG = 2,
            SMTO_BLOCK = 1,
            SMTO_NORMAL = 0,
            SMTO_NOTIMEOUTIFNOTHUNG = 8
        }

        [Flags]
        internal enum SetWinEventHookParameter
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
            public DateTime ToDateTime() =>
                new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);

            public static SYSTEMTIME FromDateTime(DateTime dateTime) =>
                new SYSTEMTIME
                {
                    wDay = (ushort)dateTime.Day,
                    wDayOfWeek = (ushort)dateTime.DayOfWeek,
                    wHour = (ushort)dateTime.Hour,
                    wMilliseconds = (ushort)dateTime.Millisecond,
                    wMinute = (ushort)dateTime.Minute,
                    wMonth = (ushort)dateTime.Month,
                    wSecond = (ushort)dateTime.Second,
                    wYear = (ushort)dateTime.Year
                };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class SYSTEMTIMEARRAY
        {
            public short wYearmin;
            public short wMonthmin;
            public short wDayOfWeekmin;
            public short wDaymin;
            public short wHourmin;
            public short wMinutemin;
            public short wSecondmin;
            public short wMillisecondsmin;
            public short wYearmax;
            public short wMonthmax;
            public short wDayOfWeekmax;
            public short wDaymax;
            public short wHourmax;
            public short wMinutemax;
            public short wSecondmax;
            public short wMillisecondsmax;
            public void ToDateTimeRange(out DateTime minDate, out DateTime maxDate)
            {
                minDate = DateTime.MinValue;
                maxDate = DateTime.MaxValue;
                try
                {
                    minDate = new DateTime(wYearmin, wMonthmin, wDaymin, wHourmin, wMinutemin, wSecondmin);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                try
                {
                    maxDate = new DateTime(wYearmax, wMonthmax, wDaymax, wHourmax, wMinutemax, wSecondmax);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            public static SYSTEMTIMEARRAY FromDateTimeRange(DateTime minDate, DateTime maxDate) =>
                new SYSTEMTIMEARRAY
                {
                    wDaymin = (short)minDate.Day,
                    wDayOfWeekmin = (short)minDate.DayOfWeek,
                    wHourmin = (short)minDate.Hour,
                    wMillisecondsmin = (short)minDate.Millisecond,
                    wMinutemin = (short)minDate.Minute,
                    wMonthmin = (short)minDate.Month,
                    wSecondmin = (short)minDate.Second,
                    wYearmin = (short)minDate.Year,
                    wDaymax = (short)maxDate.Day,
                    wDayOfWeekmax = (short)maxDate.DayOfWeek,
                    wHourmax = (short)maxDate.Hour,
                    wMillisecondsmax = (short)maxDate.Millisecond,
                    wMinutemax = (short)maxDate.Minute,
                    wMonthmax = (short)maxDate.Month,
                    wSecondmax = (short)maxDate.Second,
                    wYearmax = (short)maxDate.Year
                };
        }

        [Flags]
        internal enum tagOLECONTF : uint
        {
            OLECONTF_EMBEDDINGS = 1,
            OLECONTF_LINKS = 2,
            OLECONTF_ONLYIFRUNNING = 0x10,
            OLECONTF_ONLYUSER = 8,
            OLECONTF_OTHERS = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TITLEBARINFO
        {
            public const int CCHILDREN_TITLEBAR = 5;
            public uint cbSize;
            public RECT rcTitleBar;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public AccessibleStates[] rgstate;
        }

        internal enum VirtualKeyMapType : uint
        {
            ScanCodeToVirtualKey = 1,
            ScanCodeToVirtualKeyEx = 3,
            VirtualKeyToChar = 2,
            VirtualKeyToScanCode = 0,
            VirtualKeyToScanCodeEx = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        internal enum WindowShowStyle : uint
        {
            ForceMinimized = 11,
            Hide = 0,
            Maximize = 3,
            Minimize = 6,
            Restore = 9,
            Show = 5,
            ShowDefault = 10,
            ShowMaximized = 3,
            ShowMinimized = 2,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            ShowNormal = 1,
            ShowNormalNoActivate = 4
        }

        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOAOWNERZORDER = 0x0200;

        internal delegate void WinEventProc(IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds);

        internal static IAccessible AccessibleClientObjectFromWindow(IntPtr windowHandle) =>
            AccessibleObjectFromWindow(windowHandle, -4);


        #region "Refresh Notification Area Icons"


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        public static void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }


        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }
        #endregion

    }



}
