using Accessibility;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal static class MsaaNativeMethods
    {
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] object[] rgvarChildren, out int pcObtained);
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromPoint(NativeMethods.POINT pt, [In, Out] ref IAccessible pAcc, [In, Out] ref object childId);
        public static bool AllocateInProcMemory(uint processId, int size, out IntPtr structMem, out IntPtr hProcess)
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
            structMem = VirtualAllocEx(hProcess, IntPtr.Zero, new IntPtr(size), AllocationType.MEM_RESERVE | AllocationType.MEM_COMMIT, MemoryProtectionConstant.PAGE_READWRITE);
            if (structMem == IntPtr.Zero)
            {
                NativeMethods.CloseHandle(hProcess);
                hProcess = IntPtr.Zero;
                return false;
            }
            return true;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr ChildWindowFromPointEx(IntPtr parentWindowHandle, NativeMethods.POINT pt, ChildWindowFromPointParameter flags);
        [DllImport("Gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int x, int y, int width, int height);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hGDIObject);
        public static void FreeInProcAllocatedMemory(IntPtr structMem, IntPtr hProcess)
        {
            if (structMem != IntPtr.Zero && !VirtualFreeEx(hProcess, structMem, IntPtr.Zero, AllocationType.MEM_RELEASE))
            {
                
            }
            if (hProcess != IntPtr.Zero && !NativeMethods.CloseHandle(hProcess))
            {
                
            }
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetParent(IntPtr windowHandle);
        public static int GetResponseOfSendMessage<T>(IntPtr hwnd, uint msg, ref T obj)
        {
            object lParam = obj;
            IntPtr ptr = XProcSendMessageByRef(hwnd, msg, IntPtr.Zero, ref lParam, Marshal.SizeOf(obj));
            obj = (T)lParam;
            return ptr.ToInt32();
        }

        [DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetRoleText(int dwRole, StringBuilder lpszRole, uint cchRoleMax);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr windowHandle, GWParameter nIndex);
        [DllImport("user32.dll")]
        internal static extern int GetWindowRgn(IntPtr windowHandle, IntPtr hRegion);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Gdi32.dll")]
        internal static extern bool PtInRegion(IntPtr hRegion, int x, int y);
        public static bool ReadProcessMemory(IntPtr hProcess, IntPtr lpAddress, byte[] buffer)
        {
            int num;
            if (!ReadProcessMemory(hProcess, lpAddress, buffer, new IntPtr(buffer.Length), out num))
            {
                return false;
            }
            return num == buffer.Length;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, IntPtr dwSize, out int nBytesRead);
        [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtectionConstant flProtect);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationType flFreeType);

        public static IntPtr XProcSendMessageByRef(IntPtr windowHandle, uint msg, IntPtr wParam, ref object lParam, int numBytes)
        {
            IntPtr ptr;
            IntPtr ptr2;
            IntPtr ptr3;
            if (!AllocateInProcMemory(NativeMethods.GetWindowProcessId(windowHandle), numBytes, out ptr2, out ptr))
            {
                
                throw new ZappyTaskException("Messages.FailedToSendMessage");
            }
            try
            {
                if (!NativeMethods.IsWindowResponding(windowHandle))
                {
                    
                    throw new ZappyTaskException("Messages.FailedToSendMessage");
                }
                ptr3 = SendMessage(windowHandle, msg, wParam, ptr2);
                int num2 = Marshal.GetLastWin32Error();
                object[] args = { num2 };
                                byte[] buffer = new byte[numBytes];
                if (!ReadProcessMemory(ptr, ptr2, buffer))
                {
                    
                    throw new ZappyTaskException("Messages.FailedToSendMessage");
                }
                IntPtr destination = Marshal.AllocCoTaskMem(numBytes);
                Marshal.Copy(buffer, 0, destination, numBytes);
                lParam = Marshal.PtrToStructure(destination, lParam.GetType());
                Marshal.FreeCoTaskMem(destination);
            }
            finally
            {
                FreeInProcAllocatedMemory(ptr2, ptr);
            }
            return ptr3;
        }

        internal enum AllocationType
        {
            MEM_COMMIT = 0x1000,
            MEM_DECOMMIT = 0x4000,
            MEM_RELEASE = 0x8000,
            MEM_RESERVE = 0x2000
        }

        internal enum ChildWindowFromPointParameter : uint
        {
            CWP_ALL = 0,
            CWP_SKIPDISABLED = 2,
            CWP_SKIPINVISIBLE = 1,
            CWP_SKIPTRANSPARENT = 4
        }

        internal static class Constants
        {
            internal const uint GETTEXT = 13;
            internal const uint GETTEXTLENGTH = 14;
            internal const int MaxRoleLength = 0x100;
            internal const string WM_GETCONTROLNAME = "WM_GETCONTROLNAME";
            internal const int WM_NULL = 0;
        }

        internal enum GWParameter
        {
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6,
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_MAX = 6,
            GW_OWNER = 4
        }

        internal enum MemoryProtectionConstant
        {
            PAGE_READWRITE = 4
        }
    }
}