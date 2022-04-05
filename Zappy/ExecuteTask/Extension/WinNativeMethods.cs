using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Zappy.Decode.Helper;
using Zappy.Decode.Mssa;

namespace Zappy.ExecuteTask.Extension
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal static class WinNativeMethods
    {
        private const int MEM_COMMIT = 0x1000;
        private const int MEM_RELEASE = 0x8000;
        private const int MEM_RESERVE = 0x2000;
        private const int PAGE_READWRITE = 4;

        private static bool AllocateInProcMemory(uint processId, int size, out IntPtr structMem, out IntPtr hProcess)
        {
            structMem = IntPtr.Zero;
            hProcess = IntPtr.Zero;
            if (processId == 0)
            {
                return false;
            }
            hProcess = NativeMethods.OpenProcess(NativeMethods.ProcessAccessRights.PROCESS_VM_ALL, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                return false;
            }
            structMem = VirtualAllocEx(hProcess, IntPtr.Zero, new IntPtr(size), 0x3000, 4);
            if (structMem == IntPtr.Zero)
            {
                NativeMethods.CloseHandle(hProcess);
                hProcess = IntPtr.Zero;
                return false;
            }
            return true;
        }

        [DllImport("user32.dll")]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO ScrollInfo);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] IntPtr lpBuffer, IntPtr dwSize, out int nBytesRead);
        [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO ScrollInfo, bool fRedraw);
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int flAllocationType, int flProtect);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int dwFreeType);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr dwSize, out int nBytesWritten);

        public static IntPtr XProcSendMessageByRef(IntPtr windowHandle, int msg, IntPtr wParam, ref object lParam)
        {
            IntPtr ptr;
            IntPtr ptr2;
            IntPtr ptr4;
            uint windowProcessId = NativeMethods.GetWindowProcessId(windowHandle);
            int size = Marshal.SizeOf(lParam);
            if (!AllocateInProcMemory(windowProcessId, size, out ptr2, out ptr))
            {
                                throw new ZappyTaskException(Messages.FailedToSendMessage);
            }
            IntPtr ptr3 = Marshal.AllocCoTaskMem(size);
            Marshal.StructureToPtr(lParam, ptr3, false);
            try
            {
                int num3;
                if (!WriteProcessMemory(ptr, ptr2, ptr3, new IntPtr(size), out num3) || (num3 != size))
                {
                    throw new ZappyTaskException(Messages.FailedToSendMessage);
                }
                ptr4 = SendMessage(windowHandle, msg, wParam, ptr2);
                if (!ReadProcessMemory(ptr, ptr2, ptr3, new IntPtr(size), out num3) || (num3 != size))
                {
                    throw new ZappyTaskException(Messages.FailedToSendMessage);
                }
                lParam = Marshal.PtrToStructure(ptr3, lParam.GetType());
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr3);
                VirtualFreeEx(ptr, ptr2, IntPtr.Zero, 0x8000);
                NativeMethods.CloseHandle(ptr);
            }
            return ptr4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
    }
}