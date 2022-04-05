using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Kill The Running Process")]
    public class KillProcess : TemplateAction
    {
        [Category("Input")]
        [Description("ProcessName for Killing the process")]
        public DynamicProperty<string> ProcessName { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            try
            {

                {
                    List<Exception> innerExceptions = new List<Exception>();
                    foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName(this.ProcessName))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception exception)
                        {
                            innerExceptions.Add(exception);
                        }
                    }
                    if (innerExceptions.Count > 0)
                    {
                        throw new AggregateException("Encountered errors while trying to kill a process", innerExceptions);
                    }
                }
            }
            catch
            {
                if (!this.ContinueOnError)
                {
                    throw;
                }
            }
        }


                


        public override string AuditInfo()
        {
            return base.AuditInfo() + " ProcessName:" + this.ProcessName;
        }
    }
}

