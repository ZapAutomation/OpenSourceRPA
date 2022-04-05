using System;
using System.Runtime.InteropServices;

namespace ZappyChromeNativeConsole
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WinProcessEntry : Toolhelp32.IEntry
    {
        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(Toolhelp32.Snapshot snap, ref WinProcessEntry entry);

        public bool TryMoveNext(Toolhelp32.Snapshot snap, out Toolhelp32.IEntry entry)
        {
            var x = new WinProcessEntry { dwSize = Marshal.SizeOf(typeof(WinProcessEntry)) };
            var b = Process32Next(snap, ref x);
            entry = x;
            return b;
        }

        public int dwSize;
        public int cntUsage;
        public int th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public int th32ModuleID;
        public int cntThreads;
        public int th32ParentProcessID;
        public int pcPriClassBase;
        public int dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public String fileName;
        //byte fileName[260];
        //public const int sizeofFileName = 260;
    }
}
