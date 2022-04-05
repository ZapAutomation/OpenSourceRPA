using System;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    [Serializable]
    public static class DateTimePickerUtilities
    {
        private const uint DTM_GETSYSTEMTIME = 0x1001;
        private const int DTS_TIMEFORMAT = 8;

        private static bool GetSelectedDateTime(IntPtr windowHandle, out DateTime dateTime)
        {
            NativeMethods.SYSTEMTIME systemtime = new NativeMethods.SYSTEMTIME();
            if (MsaaNativeMethods.GetResponseOfSendMessage(windowHandle, 0x1001, ref systemtime) == 0)
            {
                dateTime = systemtime.ToDateTime();
                return true;
            }
            dateTime = new DateTime();
            return false;
        }

        internal static object GetValue(MsaaElement element)
        {
            DateTime time;
            if (GetSelectedDateTime(element.WindowHandle, out time))
            {
                return ZappyTaskUtilities.GetDateTimeToString(time, ShouldGenerateTime(element.WindowHandle));
            }
            return null;
        }

        internal static bool IsDateTimePickerClassName(string className) =>
            !string.IsNullOrEmpty(className) && className.Contains("SysDateTimePick32");

        private static bool ShouldGenerateTime(IntPtr datePickerWindowHandle) =>
            (NativeMethods.GetWindowLong(datePickerWindowHandle, NativeMethods.GWLParameter.GWL_STYLE) & 8) == 8;
    }
}