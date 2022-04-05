using System;
using System.Runtime.InteropServices;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Hooks.PointerEvent
{
    internal sealed class PointerBuffer : SafeHandle
    {
        public PointerBuffer() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                return SharedMemoryManager.UnmapViewOfFile(handle);
            }
            return true;
        }

        public override bool IsInvalid =>
            handle == IntPtr.Zero;
    }
}