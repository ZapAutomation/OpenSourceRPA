using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
    internal static class ScreenElementUtility
    {
        private const int CLICKABLE_POINT_OFFSET = 5;
        private const string IETitleBarClassName = "IEFrame";
        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        internal const int SWP_NOACTIVATE = 0x10;
        internal const int SWP_NOCOPYBITS = 0x100;
        internal const int SWP_NOMOVE = 2;
        internal const int SWP_NOOWNERZORDER = 0x200;
        internal const int SWP_NOREDRAW = 8;
        internal const int SWP_NOREPOSITION = 0x200;
        internal const int SWP_NOSENDCHANGING = 0x400;
        internal const int SWP_NOSIZE = 1;
        internal const int SWP_NOZORDER = 4;
        internal const int SWP_SHOWWINDOW = 0x40;

        internal static bool ControlWindowUsesConfiguredIMELanguage(IScreenElement element)
        {
            if (ScreenElement.ImeLanguageList.Count > 0 && element.TechnologyElement != null && element.TechnologyElement.WindowHandle != IntPtr.Zero)
            {
                int lcidFromWindowHandle = ZappyTaskUtilities.GetLcidFromWindowHandle(element.TechnologyElement.WindowHandle);
                return ScreenElement.ImeLanguageList.Contains(lcidFromWindowHandle);
            }
            return false;
        }

        internal static IRPFPlayback CreatePlaybackInstance() =>
            new CRPFPlaybackClass();

        internal static string GetKeyboardLayout() =>
            CultureInfo.CurrentUICulture.KeyboardLayoutId.ToString("X", CultureInfo.InvariantCulture);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int keyCode);
                        internal static ILastInvocationInfo GetStepInfo(IRPFPlayback playback)
        {
#if !COMENABLED
            return null;
#else
            if (playback == null)
            {
                return null;
            }
            object lastActionInfo = playback.GetLastActionInfo();
            ILastInvocationInfo typedObjectForIUnknown = null;
            if (lastActionInfo != null)
            {
                IntPtr ptr2;
                IntPtr iUnknownForObjectInContext = Marshal.GetIUnknownForObjectInContext(lastActionInfo);
                Guid iid = Marshal.GenerateGuidForType(typeof(ILastInvocationInfo));
                if (Marshal.QueryInterface(iUnknownForObjectInContext, ref iid, out ptr2) == 0)
                {
                    typedObjectForIUnknown = (ILastInvocationInfo)Marshal.GetTypedObjectForIUnknown(ptr2, typeof(ILastInvocationInfo));
                }
            }
            return typedObjectForIUnknown;
#endif
        }

        public static void PerformActionOnIETitleBar(ScreenElement element, MouseActionType actionType, MouseButtons button, ModifierKeys modifierKeys, IntPtr windowHandle, int duration = 0, int speed = -1)
        {
            int num;
            int num2;
            int num3;
            int num4;
            NativeMethods.SetForegroundWindow(windowHandle);
            IntPtr ancestor = NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT);
            element.TechnologyElement.GetBoundingRectangle(out num, out num2, out num3, out num4);
            if (num3 <= 0)
            {
                
            }
            else
            {
                ExecuteNativeMethods.RectStruct struct2;
                ExecuteNativeMethods.RectStruct struct3;
                if (ExecuteNativeMethods.GetWindowRect(ancestor, out struct2))
                {
                    if (struct2.Right - struct2.Left <= 0 || struct2.Bottom - struct2.Top <= 0)
                    {
                        
                        return;
                    }
                }
                else
                {
                    
                    return;
                }
                struct3.Left = struct2.Left;
                struct3.Top = struct2.Top;
                struct3.Right = struct2.Right;
                struct3.Bottom = struct2.Bottom;
                Point point = new Point((int)0.0, (int)0.0);
                IntPtr hMonitor = ExecuteNativeMethods.MonitorFromRect(ref struct3, 1);
                ExecuteNativeMethods.MonitorInfo structure = new ExecuteNativeMethods.MonitorInfo();
                structure.Size = Marshal.SizeOf(structure);
                if (ExecuteNativeMethods.GetMonitorInfo(hMonitor, ref structure))
                {
                    if (structure.WorkArea.Left + 5 > num)
                    {
                        point.X = num - (structure.WorkArea.Left + 5);
                    }
                    else if (structure.WorkArea.Right - 5 < num + num3)
                    {
                        point.X = num + num3 - (structure.WorkArea.Right - 5);
                    }
                    if (structure.WorkArea.Top + 5 > num2)
                    {
                        point.Y = num2 - (structure.WorkArea.Top + 5);
                    }
                    else if (structure.WorkArea.Bottom - 5 < num2 + num4)
                    {
                        point.Y = num2 + num4 - (structure.WorkArea.Bottom - 5);
                    }
                    if (point.X != 0.0 || point.Y != 0.0)
                    {
                        ExecuteNativeMethods.SetWindowPos(ancestor, IntPtr.Zero, struct2.Left - point.X, struct2.Top - point.Y, 0, 0, 0x215);
                        element.TechnologyElement.GetBoundingRectangle(out num, out num2, out num3, out num4);
                    }
                }
                else
                {
                    
                }
                switch (actionType)
                {
                    case MouseActionType.Click:
                        ScreenElement.Desktop.MouseButtonClick(num + num3 / 2, num2 + 5, button, modifierKeys, 0);
                        return;

                    case MouseActionType.DoubleClick:
                        ScreenElement.Desktop.DoubleClick(num + num3 / 2, num2 + 5, button, modifierKeys, 0);
                        return;

                    case MouseActionType.Drag:
                    case MouseActionType.WheelRotate:
                        break;

                    case MouseActionType.Hover:
                        ScreenElement.Desktop.MouseHover(num + num3 / 2, num2 + 5, 0, speed, duration);
                        break;

                    default:
                        return;
                }
            }
        }
    }
}