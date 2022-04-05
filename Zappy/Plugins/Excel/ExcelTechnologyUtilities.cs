


using Accessibility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Zappy.Decode.LogManager;

namespace Zappy.Plugins.Excel
{
    internal static class ExcelTechnologyUtilities
    {
                                internal const string ExcelTechnologyName = "Excel";

                                internal const string ExcelClassName = "EXCEL7";

                                                internal static bool IsExcelWorksheetWindow(IntPtr windowHandle)
        {
            string className = GetClassName(windowHandle);
            return IsExcelClassName(className);
        }

        internal static bool IsExcelClassName(string className)
        {
            if (string.IsNullOrEmpty(className))
                return false;
            return string.Equals(className, ExcelClassName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "XLDESK", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "XLMAIN", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(className, "EXCEL6", StringComparison.OrdinalIgnoreCase);
        }


                                                internal static string GetWindowText(IntPtr windowHandle)
        {
            const int MaxWindowTextLength = 1024;
            StringBuilder textBuilder = new StringBuilder(MaxWindowTextLength);
            if (GetWindowText(windowHandle, textBuilder, textBuilder.Capacity) <= 0)
            {
                            }

            return textBuilder.ToString();
        }

                                                internal static string GetClassName(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return string.Empty;
            }

            const int MaxClassLength = 0x100;
            StringBuilder classNameStringBuilder = new StringBuilder(MaxClassLength);
            if (GetClassName(windowHandle, classNameStringBuilder, classNameStringBuilder.Capacity) <= 0)
            {
                CrapyLogger.log.Error(string.Format(CultureInfo.InvariantCulture,
                                         "GetClassName for handle {0} Writelineed", windowHandle));
            }

            return classNameStringBuilder.ToString();
        }

                                                internal static IAccessible AccessibleObjectFromWindow(IntPtr windowHandle)
        {
            Guid accessibleGuid = typeof(IAccessible).GUID;
            IAccessible accessible = null;

            if (AccessibleObjectFromWindow(windowHandle, 0, ref accessibleGuid, ref accessible) != 0)
            {
                CrapyLogger.log.Error(string.Format(CultureInfo.InvariantCulture,
                                         "AccessibleObjectFromWindow for handle {0} Writelineed", windowHandle));
            }

            return accessible;
        }

                                                        internal static int PointToPixel(double points, bool xAxis)
        {
            const int defaultDpi = 96;
            const int pixelsPerInch = 72;
            if (dpiX == -1)
            {
                dpiX = dpiY = defaultDpi;
                IntPtr deviceHandle = GetDC(IntPtr.Zero);
                if (IntPtr.Zero != deviceHandle)
                {
                    dpiX = GetDeviceCaps(deviceHandle, LOGPIXELSX);
                    dpiY = GetDeviceCaps(deviceHandle, LOGPIXELSY);
                    ReleaseDC(IntPtr.Zero, deviceHandle);
                }
            }

            return (int)(points * (xAxis ? dpiX : dpiY) / pixelsPerInch);
        }

                                                                                                        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(int pointX, int pointY);

                                                        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr windowHandle, out RECT lpRect);

                                                                [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

                                                                                        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(
            IntPtr windowHandle, StringBuilder windowText, int maxCharCount);

                                                                                        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr windowHandle, StringBuilder classNameText, int maxCharCount);

                                        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr windowHandle, int dwObjectID,
            ref Guid riid, ref IAccessible pAcc);

                                                [DllImport("user32.dll")]
        private extern static IntPtr GetDC(IntPtr windowHandle);

                                                        [DllImport("user32.dll")]
        private extern static int ReleaseDC(IntPtr windowHandle, IntPtr hdc);

                                                        [DllImport("gdi32.dll")]
        private extern static int GetDeviceCaps(IntPtr hdc, int index);

                private static int dpiX = -1;
        private static int dpiY = -1;

                private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
    }
}
