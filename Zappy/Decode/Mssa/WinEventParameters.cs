using System;
using System.Windows.Forms;

namespace Zappy.Decode.Mssa
{
    internal class WinEventParameters
    {
        internal WinEventParameters(AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId)
        {
            AccEvent = accEvent;
            WindowHandle = windowHandle;
            ObjectId = objectId;
            ChildId = childId;
        }

        internal AccessibleEvents AccEvent { get; private set; }

        internal int ChildId { get; private set; }

        internal int ObjectId { get; private set; }

        internal IntPtr WindowHandle { get; private set; }
    }
}