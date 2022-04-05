using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public static class Extensions
    {


        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public static void SetWindowTheme(this Control control, string pszSubAppName)
        {
            SetWindowTheme(control.Handle, pszSubAppName, null);
        }

    }
}
