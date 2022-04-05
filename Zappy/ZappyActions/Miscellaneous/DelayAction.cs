using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Execute;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Dealy Time Duration")]
    public class DelayAction : ZappyTaskAction
    {
        private int duration;

        public DelayAction()
        {
        }

        public DelayAction(int duration)
        {
            if (duration <= 0 && duration != -1)
            {
                throw new ArgumentOutOfRangeException("duration");
            }
            Duration = duration;
        }

        internal override string GetParameterString() =>
            Duration.ToString(CultureInfo.InvariantCulture);

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                    
                                    Thread.Sleep(Duration);
        }

        [Category("Input")]
        [Description("Delay time in milliseconds")]
        public int Duration
        {
            get =>
                duration;
            set
            {
                duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Duration:" + this.Duration;
        }
    }
}