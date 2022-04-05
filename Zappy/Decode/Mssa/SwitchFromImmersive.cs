using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Mssa
{
    internal sealed class SwitchFromImmersive : IDisposable
    {
        private static readonly object instance_lock = new object();
        private static IntPtr s_desktop = IntPtr.Zero;
        private static SwitchFromImmersive s_instance;

        private static IntPtr s_windowHandleAtTestStart = IntPtr.Zero;

        private SwitchFromImmersive()
        {
            bool flag = false;
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            if (mainWindowHandle != IntPtr.Zero)
            {
                            }
            if (!flag && ZappyTaskUtilities.IsWin8OrGreaterOs())
            {
                s_windowHandleAtTestStart = NativeMethods.GetForegroundWindow();
                if (s_windowHandleAtTestStart == IntPtr.Zero)
                {
                    s_windowHandleAtTestStart = Desktop;
                }
                try
                {
                    SFIClient.Instance.Initialise();
                    SwitchFromImmersiveActive = true;
                }
                catch (Exception exception)
                {
                    object[] args = { exception.Message };
                    CrapyLogger.log.ErrorFormat("SwitchFromImmersive: Error while initializing SFI component: '{0}'", args);
                    SwitchFromImmersiveActive = false;
                }
            }
            else
            {
                SwitchFromImmersiveActive = false;
            }
        }

        public void Cleanup()
        {
            if (SwitchFromImmersiveActive)
            {
                SwitchFromImmersiveToWindow(s_windowHandleAtTestStart);
            }
            object obj2 = instance_lock;
            lock (obj2)
            {
                if (s_instance != null)
                {
                    s_instance.Dispose();
                    s_instance = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && SwitchFromImmersiveActive)
            {
                SFIClient.Instance.Cleanup();
            }
        }

        public bool DrawHighlight(int x, int y, int width, int height, int highlightTime)
        {
            Rectangle rectangle = new Rectangle(x, y, width, height);
            if (SwitchFromImmersiveActive && SFIClient.Instance.DrawHighlightStart(rectangle))
            {
                Thread.Sleep(highlightTime);
                return SFIClient.Instance.DrawHighlightStop();
            }
            object[] args = { rectangle };
            CrapyLogger.log.ErrorFormat("SwitchFromImmersive: SFI not active. Unable to DrawHighlight on Immersive Window: '{0}'", args);
            return false;
        }

        public bool DrawHighlightStart(Rectangle rectangle) =>
            SwitchFromImmersiveActive && SFIClient.Instance.DrawHighlightStart(rectangle);

        public bool DrawHighlightStop() =>
            SwitchFromImmersiveActive && SFIClient.Instance.DrawHighlightStop();

        ~SwitchFromImmersive()
        {
            Dispose(false);
        }

        private bool IsSwitchRequired(IntPtr currentHwnd, IntPtr targetHwnd, IntPtr targetRootHwnd)
        {
            if (targetRootHwnd != IntPtr.Zero && currentHwnd != targetHwnd && currentHwnd != targetRootHwnd)
            {
                IntPtr ancestor = NativeMethods.GetAncestor(currentHwnd, NativeMethods.GetAncestorFlag.GA_ROOTOWNER);
                if (ancestor != targetHwnd && ancestor != targetRootHwnd)
                {
                    if (!ZappyTaskUtilities.IsImmersiveWindowClassName(NativeMethods.GetClassName(targetRootHwnd)) && !ZappyTaskUtilities.IsImmersiveWindow(currentHwnd))
                    {
                        return ZappyTaskUtilities.IsCharmsBar(currentHwnd);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool SwitchFromImmersiveToWindow(IntPtr hwnd)
        {
            if (SwitchFromImmersiveActive)
            {
                if (hwnd == IntPtr.Zero)
                {
                    
                    return false;
                }
                IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
                IntPtr ancestor = NativeMethods.GetAncestor(hwnd, NativeMethods.GetAncestorFlag.GA_ROOTOWNER);
                if (IsSwitchRequired(foregroundWindow, hwnd, ancestor))
                {
                    object[] args = { foregroundWindow, ancestor };
                    
                    return SFIClient.Instance.SwitchFromImmersiveToWindow(ancestor);
                }
            }
            return true;
        }

        private static IntPtr Desktop
        {
            get
            {
                IntPtr ptr1 = s_desktop;
                return s_desktop;
            }
        }

        public static SwitchFromImmersive Instance
        {
            get
            {
                object obj2 = instance_lock;
                lock (obj2)
                {
                    if (s_instance == null)
                    {
                        s_instance = new SwitchFromImmersive();
                    }
                    return s_instance;
                }
            }
        }

        private bool SwitchFromImmersiveActive { get; set; }
    }
}