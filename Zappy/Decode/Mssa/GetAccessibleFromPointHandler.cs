using Accessibility;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    internal delegate int GetAccessibleFromPointHandler(NativeMethods.POINT pt, ref IAccessible pAcc, ref object childId);
}