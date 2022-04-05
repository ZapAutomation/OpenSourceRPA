using System;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    internal static class MonthCalendarUtilities
    {
        private const uint MCM_GETSELRANGE = 0x1005;
        private const string MonthCalendarClassName = "SysMonthCal32";

        private static bool GetSelectedRangeDateTime(IntPtr windowHandle, out DateTime minDateTime, out DateTime maxDateTime)
        {
            NativeMethods.SYSTEMTIMEARRAY systemtimearray = new NativeMethods.SYSTEMTIMEARRAY();
            if (MsaaNativeMethods.GetResponseOfSendMessage(windowHandle, 0x1005, ref systemtimearray) != 0)
            {
                systemtimearray.ToDateTimeRange(out minDateTime, out maxDateTime);
                return true;
            }
            minDateTime = new DateTime();
            maxDateTime = new DateTime();
            return false;
        }

        internal static object GetValue(AccessibleEvents accEvent, MsaaElement elementForEvent)
        {
            object shortDateRangeString = null;
            DateTime time;
            DateTime time2;
            if (IsSupportedEvent(accEvent) && GetSelectedRangeDateTime(elementForEvent.WindowHandle, out time, out time2))
            {
                shortDateRangeString = ZappyTaskUtilities.GetShortDateRangeString(time, time2);
            }
            return shortDateRangeString;
        }

        internal static bool IsMonthCalendarButton(MsaaElement element) =>
            element != null && ControlType.Button.NameEquals(element.ControlTypeName) && element.ClassName.StartsWith("SysMonthCal32", StringComparison.Ordinal);

        internal static bool IsMonthCalendarClassName(string className) =>
            !string.IsNullOrEmpty(className) && className.Contains("SysMonthCal32");

        private static bool IsSupportedEvent(AccessibleEvents accEvent)
        {
            if (accEvent != AccessibleEvents.Selection && accEvent != AccessibleEvents.SelectionAdd && accEvent != AccessibleEvents.SelectionRemove && accEvent != AccessibleEvents.SelectionWithin)
            {
                return accEvent == AccessibleEvents.SystemCaptureEnd;
            }
            return true;
        }
    }
}