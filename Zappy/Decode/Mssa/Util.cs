using System;

namespace Zappy.Decode.Mssa
{
    internal class Util
    {
        public static int HIWORD(int n) =>
            (n >> 0x10) & 0xffff;

        public static int HIWORD(IntPtr n) =>
            HIWORD((int)(long)n);

        public static int LOWORD(int n) =>
            n & 0xffff;

        public static int LOWORD(IntPtr n) =>
            LOWORD((int)(long)n);

        public static int MAKELONG(int low, int high) =>
            (high << 0x10) | (low & 0xffff);

        public static IntPtr MAKELPARAM(int low, int high) =>
            (IntPtr)((high << 0x10) | (low & 0xffff));

        public static int SignedHIWORD(int n) =>
            (short)((n >> 0x10) & 0xffff);

        public static int SignedHIWORD(IntPtr n) =>
            SignedHIWORD((int)(long)n);

        public static int SignedLOWORD(int n) =>
            (short)(n & 0xffff);

        public static int SignedLOWORD(IntPtr n) =>
            SignedLOWORD((int)(long)n);
    }
}