using System.ComponentModel;
using System.Threading;
using Zappy.Properties;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("Pause the Action")]
    public class PauseAction : TemplateAction
    {
        public PauseAction()
        {
            SleepTimeInMilliseconds = 3000;
        }

        [Category("Input")]
        [Description("Sets sleep time in Milliseconds")]
        public int SleepTimeInMilliseconds { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Thread.Sleep(SleepTimeInMilliseconds);
        }
    }
}