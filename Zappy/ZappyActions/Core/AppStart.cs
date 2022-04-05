using System;
using System.ComponentModel;
using System.Diagnostics;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("Start new application")]
    public sealed class AppStart : TemplateAction
    {
        public AppStart()
        {
            Arguments = AppPath = string.Empty;
        }

        [Category("Input")]
        [Description("Application path")]
        public DynamicProperty<string> AppPath { get; set; }

        [Category("Optional")]
        [Description("Arguments as application name")]
        public string Arguments { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string path = AppPath;
            var p = Process.Start(path, Arguments);
            while (p.MainWindowHandle == IntPtr.Zero)
            {
                           try
                {
                    p.WaitForInputIdle(1000);
                }
                catch (InvalidOperationException)
                {
                    break;
                }

                p.Refresh();
            }

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " AppPath:" + this.AppPath + " Arguments:" + this.Arguments;
        }
    }

}
