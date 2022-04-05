using System.Collections.Generic;
using Zappy.SharedInterface;

namespace Zappy.Decode.Hooks.Keyboard
{
    public class ZappyTaskActionTimeStampComparer : IComparer<IZappyAction>
    {
        public static readonly ZappyTaskActionTimeStampComparer Instance = new ZappyTaskActionTimeStampComparer();

        private ZappyTaskActionTimeStampComparer()
        {

        }
        int IComparer<IZappyAction>.Compare(IZappyAction x, IZappyAction y)
        {
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
}