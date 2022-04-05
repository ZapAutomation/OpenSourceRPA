using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask;
using Zappy.Helpers;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyActions.Triggers.Trigger;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;
using ZappyMessages.Triggers;

namespace Zappy.ZappyActions.Triggers
{
    [Description("Trigger invoked when system is idle for sometime")]
    public class SystemIdleTrigger : TemplateAction, IZappyTrigger
    {
        public SystemIdleTrigger()
        {
            TriggerLastRaised = DateTime.MinValue;
        }

        [Category("Output")]
        public DateTime TriggerLastRaised { get; set; }

                
        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }

        [Category("Input")]
        [Description("Idle time in milliseconds")]
        public DynamicProperty<double> IdleTime { get; set; }

        
                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            TriggerLastRaised = DateTime.Now;
        }

        private Timer SystemIdleTrackTimer;

        public IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            

            return RegisterIdleTracking(IdleTime);
        }

        public void UnRegisterTrigger()
        {
                        SystemIdleTrackTimer.Dispose();
        }

        private IDisposable RegisterIdleTracking(double interval)
        {
            SystemIdleTrackTimer = new System.Timers.Timer();
            SystemIdleTrackTimer.Interval = interval;
                        SystemIdleTrackTimer.AutoReset = true;
            SystemIdleTrackTimer.Elapsed += SystemIdleTrackTimer_Elapsed;
            SystemIdleTrackTimer.Start();
            return SystemIdleTrackTimer;
        }

        private void SystemIdleTrackTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            DateTime lastActivityTime = DateTime.Parse(ZappyPlaybackCommunicator.Instance.GetLastActivityTime(PubSubTopicRegister.ZappyPlayback2Zappy, PubSubTopicRegister.Zappy2ZappyPlayback));

            if (DateTime.Now.Subtract(lastActivityTime).TotalMilliseconds
                > SystemIdleTrackTimer.Interval)
            {
                                                ZappyTriggerManager.ValidateAndRaiseTrigger(this, null);

            }
                       


        }

    }
}
