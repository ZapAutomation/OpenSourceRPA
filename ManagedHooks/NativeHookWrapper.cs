using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zappy.ManagedHooks
{
    //C# wrapper calling C++ dlls
    //Or using pinvoke API calls - preferred:
    //https://stackoverflow.com/questions/26277029/how-to-pinvoke-c-dll-function-from-c-sharp
    public static class NativeHookWrapper
    {
        //https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-keyboard-hook-in-c/
        //https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-mouse-hook-in-c/

        public enum HookTypes
        {
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            None = -100,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            Keyboard = 2,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            Mouse = 7,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            Hardware = 8,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            Shell = 10,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            KeyboardLL = 13,
            /// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
            MouseLL = 14
        }


        public delegate void HookProcessedHandler(int code, UIntPtr wparam, IntPtr lparam);

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        public enum SetCallBackResults
        {
            Success = 1,
            AlreadySet = -2,
            NotImplemented = -3,
            ArgumentError = -4
        }

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/InitializeHook/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "InitializeHook", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InitializeHook(HookTypes hookType, int threadID);


        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/InitializeHook/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "InitializeWinEvent", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr InitializeWinEvent(AccessibleEvents SysEventIDStart, AccessibleEvents SysEventIDStop, int threadID);

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/SetUserHookCallback/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "SetUserHookCallback", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern SetCallBackResults SetUserHookCallback(HookProcessedHandler hookCallback, HookTypes hookType);

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/SetUserHookCallback/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "SetWinEventCallback", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern SetCallBackResults SetWinEventCallback(WinEventDelegate hookCallback, IntPtr SysEventID);


        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninitializeHook/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "UninitializeHook", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern void UninitializeHook(HookTypes hookType);


        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninitializeHook/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "UninitializeWinEvent", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern void UninitializeWinEvent(IntPtr SysEventID);

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/DisposeCppLayer/*'/>
        [DllImport("ZappyDataHelper.dll", EntryPoint = "Dispose", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisposeCppLayer();
    }

}
