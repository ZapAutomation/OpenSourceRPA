using System;
using System.Threading;
using System.Windows.Automation;
using Zappy.Decode.Helper;

namespace Zappy.ZappyActions.Picture
{
    public static class WindowHelper
    {
        public static void ShowIfMinimized(IntPtr windowHandle)
        {
            if (NativeMethods.IsIconic(windowHandle) && NativeMethods.IsWindowVisible(windowHandle))
            {
                                NativeMethods.ShowWindow(windowHandle, NativeMethods.WindowShowStyle.Restore);
            }
        }
        public static void FocusWindow(IntPtr handle)
        {
            ShowIfMinimized(handle);          
            AutomationElement element = AutomationElement.FromHandle(handle);
            if (element != null)
            {
                element.SetFocus();
            }
                                        NativeMethods.BringWindowToTop(handle);
                NativeMethods.SetForegroundWindow(handle);
                                        Thread.Sleep(500);
        }
    }
}