using System;
using System.Runtime.InteropServices;

namespace Zappy.ExecuteTask.Execute
{
    internal static class ExecuteNativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo monitorInfo);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RectStruct rect);
        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromRect([In] ref RectStruct rect, uint flags);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        internal struct MonitorInfo
        {
            internal int Size;
            internal RectStruct Monitor;
            internal RectStruct WorkArea;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RectStruct
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}