using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Zappy.Decode.Hooks.PointerEvent;

namespace Zappy.Decode.Helper
{
    internal class SharedMemoryManager
    {
        private PointerBuffer SharedMemBuffer;
        private uint SharedMemBufferSize;
        private SafeMemoryMappedFileHandle SharedMemMapFileHandle;
        private string SharedMemName;

        internal bool Create(string sharedMemoryName, uint bufferSize)
        {
            SharedMemName = sharedMemoryName;
            SharedMemBufferSize = bufferSize;
            SharedMemMapFileHandle = OpenFileMapping(FileMapAccess.FileMapWrite, false, SharedMemName);
            if (SharedMemMapFileHandle.IsInvalid)
            {
                SharedMemMapFileHandle = CreateFileMapping(IntPtr.Zero, IntPtr.Zero, FileMapProtection.PageReadWrite, 0, SharedMemBufferSize, SharedMemName);
            }
            if (!SharedMemMapFileHandle.IsInvalid)
            {
                SharedMemBuffer = MapViewOfFile(SharedMemMapFileHandle, FileMapAccess.FileMapWrite, 0, 0, (UIntPtr)SharedMemBufferSize);
            }
            return IsMemoryInitialized();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeMemoryMappedFileHandle CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, FileMapProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        internal void Destroy()
        {
            if (IsMemoryInitialized())
            {
                SharedMemBuffer.Dispose();
                SharedMemMapFileHandle.Dispose();
                SharedMemBuffer = null;
                SharedMemMapFileHandle = null;
            }
        }


        internal unsafe bool GetData<T>(ref T data)
        {
            if (!IsMemoryInitialized())
            {
                return false;
            }
            bool flag = false;
            bool success = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                SharedMemBuffer.DangerousAddRef(ref success);
                if (!success)
                {
                    return flag;
                }
                byte* pointer = (byte*)SharedMemBuffer.DangerousGetHandle().ToPointer();
                if (pointer == null)
                {
                    return flag;
                }
                using (UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, SharedMemBufferSize, SharedMemBufferSize, FileAccess.Read))
                {
                    byte[] buffer = new byte[SharedMemBufferSize];
                    stream.Read(buffer, 0, (int)SharedMemBufferSize);
                    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                    handle.Free();
                }
                flag = true;
            }
            finally
            {
                if (success)
                {
                    SharedMemBuffer.DangerousRelease();
                }
            }
            return flag;
        }

        internal bool IsMemoryInitialized() =>
            SharedMemBuffer != null && !SharedMemBuffer.IsInvalid;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern PointerBuffer MapViewOfFile(SafeMemoryMappedFileHandle hFileMapping, FileMapAccess dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);
        internal bool OpenExisting(string sharedMemoryName, uint bufferSize)
        {
            SharedMemName = sharedMemoryName;
            SharedMemBufferSize = bufferSize;
            SharedMemMapFileHandle = OpenFileMapping(FileMapAccess.FileMapWrite, false, SharedMemName);
            if (SharedMemMapFileHandle.IsInvalid)
            {
                return false;
            }
            SharedMemBuffer = MapViewOfFile(SharedMemMapFileHandle, FileMapAccess.FileMapWrite, 0, 0, (UIntPtr)SharedMemBufferSize);
            return IsMemoryInitialized();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeMemoryMappedFileHandle OpenFileMapping(FileMapAccess DesiredAccess, bool bInheritHandle, string lpName);
        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll")]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        internal bool WriteData<T>(T data)
        {
            if (!IsMemoryInitialized())
            {
                return false;
            }
            bool success = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                SharedMemBuffer.DangerousAddRef(ref success);
                if (success)
                {
                    Marshal.StructureToPtr(data, SharedMemBuffer.DangerousGetHandle(), true);
                }
            }
            finally
            {
                if (success)
                {
                    SharedMemBuffer.DangerousRelease();
                }
            }
            return true;
        }

        [Flags]
        private enum FileMapAccess : uint
        {
            FileMapAllAccess = 0x1f,
            FileMapCopy = 1,
            fileMapExecute = 0x20,
            FileMapRead = 4,
            FileMapWrite = 2
        }

        [Flags]
        private enum FileMapProtection : uint
        {
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            PageReadonly = 2,
            PageReadWrite = 4,
            PageWriteCopy = 8,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000
        }
    }
}