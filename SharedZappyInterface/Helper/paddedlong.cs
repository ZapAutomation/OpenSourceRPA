using System.Runtime.InteropServices;

namespace Zappy.SharedInterface.Helper
{
    [StructLayout(LayoutKind.Explicit, Size = 256)]
    struct paddedlong
    {
        [FieldOffset(128)] public long l;

        public paddedlong(long ticks)
        {
            l = ticks;
        }
    }
}