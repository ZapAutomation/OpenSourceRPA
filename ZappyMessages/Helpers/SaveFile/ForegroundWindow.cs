using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZappyMessages.Helpers.SaveFile
{
    class ForegroundWindow : IWin32Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        static ForegroundWindow obj = null;
        public static ForegroundWindow CurrentWindow
        {
            get
            {
                if (obj == null)
                    obj = new ForegroundWindow();
                return obj;
            }
        }
        public IntPtr Handle
        {
            get { return GetForegroundWindow(); }
        }
    }
}