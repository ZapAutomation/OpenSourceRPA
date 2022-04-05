using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Zappy.Decode.Helper
{
    internal static class SharedMemoryUtility
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern EventHandle CreateEventEx(IntPtr lpEventAttributes, string lpName, EventCreateFlag dwFlags, EventAccess dwDesiredAccess);
        [DllImport("kernel32.dll")]
        internal static extern bool ResetEvent(EventHandle hEvent);
        [DllImport("kernel32.dll")]
        internal static extern bool SetEvent(EventHandle hEvent);
        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern int WaitForSingleObject(EventHandle handle, int milliseconds);

        [Flags]
        internal enum EventAccess : uint
        {
            EventAllAccess = 0x1f0003,
            EventModifyState = 2,
            Synchronize = 0x100000
        }

        [Flags]
        internal enum EventCreateFlag : uint
        {
            Default = 0,
            InitialSet = 2,
            ManualReset = 1
        }

        internal sealed class EventHandle : SafeHandle
        {
            public EventHandle() : base(IntPtr.Zero, true)
            {
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    return CloseHandle(handle);
                }
                return true;
            }

            public override bool IsInvalid =>
                handle == IntPtr.Zero;
        }
    }
}