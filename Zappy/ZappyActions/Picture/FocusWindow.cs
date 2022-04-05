using System;
using System.ComponentModel;
using System.Diagnostics;
using Zappy.Decode.Helper;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Picture
{
    public class FocusWindow : TemplateAction
    {
        [Category("Input")]
        public DynamicProperty<string> TopLevelWindowName { get; set; }

        [Category("Optional")]
        [Description("Set this property if Top Level Window Name changes every time")]
        public DynamicProperty<string> ExeFileName { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            IntPtr handle = IntPtr.Zero;

            if (!string.IsNullOrEmpty(TopLevelWindowName))
            {
                handle = NativeMethods.FindWindow(null, TopLevelWindowName);
                if (handle == IntPtr.Zero)
                    throw new Exception("Window is NOT found");
            }
            else if (!string.IsNullOrEmpty(ExeFileName))
            {
                handle = GetExeWindowHandle(ExeFileName);
                if (handle == IntPtr.Zero)
                    throw new Exception("Window is NOT found");
            }
            WindowHelper.FocusWindow(handle);
        }
        public IntPtr GetExeWindowHandle(string Exe)
        {
            IntPtr handle = IntPtr.Zero;
            Process[] procsExe = Process.GetProcessesByName(Exe);

            foreach (Process process in procsExe)
            {
                                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                handle = process.MainWindowHandle;
                break;
            }
            return handle;
        }
    }  
}
