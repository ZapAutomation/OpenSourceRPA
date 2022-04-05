using System;
using System.Runtime.InteropServices;

namespace Zappy.Decode.Helper
{
    internal class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("GDI32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CreateDC(string driverName, string deviceName, string reserved, IntPtr initData);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        public enum TernaryRasterOperations
        {
            BLACKNESS = 0x42,
            CAPTUREBLT = 0x40000000,
            DSTINVERT = 0x550009,
            MERGECOPY = 0xc000ca,
            MERGEPAINT = 0xbb0226,
            NOTSRCCOPY = 0x330008,
            NOTSRCERASE = 0x1100a6,
            PATCOPY = 0xf00021,
            PATINVERT = 0x5a0049,
            PATPAINT = 0xfb0a09,
            SRCAND = 0x8800c6,
            SRCCOPY = 0xcc0020,
            SRCERASE = 0x440328,
            SRCINVERT = 0x660046,
            SRCPAINT = 0xee0086,
            WHITENESS = 0xff0062
        }
    }
}