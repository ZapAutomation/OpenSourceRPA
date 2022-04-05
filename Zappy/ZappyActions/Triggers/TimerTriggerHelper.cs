using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyActions.Triggers.Trigger;

namespace Zappy.ZappyActions.Triggers
{
    public abstract class TimerTriggerHelper : TemplateAction, IZappyTimerTrigger
    {
        public TimerTriggerHelper() : base()
        {
            IsDisabled = false;
        }

        [Description("The First  / Initial time delay in Seconds when this event will be fired.")]
        [Category("Input")]
        public virtual DynamicProperty<int> DueTimeInSeconds { get; set; }

        [Description("True, if user confirms to execute the trigger;otherwise, false")]
        [Category("Optional")]
        public DynamicProperty<bool> AskBeforeExecuting { get; set; }

        [XmlIgnore]
        [Category("Optional")]
        public ZappyTriggerType ZappyTriggerType { get { return ZappyTriggerType.Timer; } }

        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        { }

        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this);
        }

        private IDisposable disposableTimerTrigger;

        public IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            TimerTriggerManager timerTriggerManager = new TimerTriggerManager();
            disposableTimerTrigger = timerTriggerManager.ConfigureTimerTrigger(this);
            return disposableTimerTrigger;
        }

        public void UnRegisterTrigger()
        {
            disposableTimerTrigger.Dispose();
        }
    }
}