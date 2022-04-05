using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZappyChromeNativeConsole
{
    public static class Toolhelp32
    {
        public const uint Inherit = 0x80000000;
        public const uint SnapModule32 = 0x00000010;
        public const uint SnapAll = SnapHeapList | SnapModule | SnapProcess | SnapThread;
        public const uint SnapHeapList = 0x00000001;
        public const uint SnapProcess = 0x00000002;
        public const uint SnapThread = 0x00000004;
        public const uint SnapModule = 0x00000008;

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateToolhelp32Snapshot(uint flags, int processId);

        public static IEnumerable<T> TakeSnapshot<T>(uint flags, int id) where T : IEntry, new()
        {
            using (var snap = new Snapshot(flags, id))
                for (IEntry entry = new T { }; entry.TryMoveNext(snap, out entry);)
                    yield return (T)entry;
        }

        public interface IEntry
        {
            bool TryMoveNext(Toolhelp32.Snapshot snap, out IEntry entry);
        }

        public struct Snapshot : IDisposable
        {
            void IDisposable.Dispose()
            {
                Toolhelp32.CloseHandle(m_handle);
            }
            public Snapshot(uint flags, int processId)
            {
                m_handle = Toolhelp32.CreateToolhelp32Snapshot(flags, processId);
            }
            IntPtr m_handle;
        }
    }
}
