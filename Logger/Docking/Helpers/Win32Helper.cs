using System.Drawing;
using System.Windows.Forms;
using ZappyLogger.Docking.Win32;

namespace ZappyLogger.Docking.Helpers
{
    internal static class Win32Helper
    {
        #region Public methods

        public static Control ControlAtPoint(Point pt)
        {
            return Control.FromChildHandle(NativeMethods.WindowFromPoint(pt));
        }

        public static uint MakeLong(int low, int high)
        {
            return (uint)((high << 16) + low);
        }

        #endregion
    }
}