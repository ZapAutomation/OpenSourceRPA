using System.Threading;

namespace Zappy.SharedInterface.Helper
{
    public static class ActionIDRegister
    {
        private static paddedlong lastId = new paddedlong(WallClock.UtcNow.Ticks);

        public static long GetUniqueId()
        {
            return Interlocked.Increment(ref lastId.l);
        }
        public static long PeekUniqueId()
        {
            return Interlocked.CompareExchange(ref lastId.l, -1, -1);
        }
    }
}