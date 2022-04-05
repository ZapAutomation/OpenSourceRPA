using System;

namespace Zappy.SharedInterface.Helper
{
    public static class WallClock
    {
        static DateTime _BaseDateTimeUtc, _BaseDateTime;

        static int _BaseMillisecs;
        static WallClock()
        {
            Refresh();
        }

        public static void Refresh()
        {

            _BaseDateTimeUtc = DateTime.UtcNow;
            _BaseMillisecs = Environment.TickCount;
            _BaseDateTime = _BaseDateTimeUtc.ToLocalTime();

        }

        public static DateTime Now { get { return _BaseDateTime.AddMilliseconds(Environment.TickCount - _BaseMillisecs); } }
        public static DateTime UtcNow { get { return _BaseDateTimeUtc.AddMilliseconds(Environment.TickCount - _BaseMillisecs); } }

    }
}
