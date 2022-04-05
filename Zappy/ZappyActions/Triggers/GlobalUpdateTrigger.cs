using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyActions.Triggers.Trigger;

namespace Zappy.ZappyActions.Triggers
{
        [Description("Global Updation Tigger - For Robot Management - Action According to Certain String")]
    public class GlobalUpdateTrigger : TemplateAction, IZappyTrigger
    {


        public GlobalUpdateTrigger() : base()
        {
            IsDisabled = false;
        }

        [Description("FilterText for global updation text")]
        [Category("Input")]
        public DynamicProperty<string> FilterText { get; set; }

        [Category("Optional")]
        public int NotificationDelay { get; set; }

        [XmlIgnore]
        [Category("Optional")]
        public ZappyTriggerType ZappyTriggerType { get { return ZappyTriggerType.GlobalUpdate; } }

        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }
     
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        { }

        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this) + " FilterText:" + this.FilterText;
        }

        internal IDisposable ConfigureGlobalUpdateTrigger(IZappyAction TriggerAction)
        {
            if (TriggerAction is GlobalUpdateTrigger && !string.IsNullOrEmpty((TriggerAction as GlobalUpdateTrigger).FilterText.Value))
            {
                lock (ZappyTriggerManager._globalUpdateTriggers)
                {
                    ZappyTriggerManager._globalUpdateTriggers[TriggerAction.SelfGuid] = TriggerAction as GlobalUpdateTrigger;
                }
            }
            return null;
        }

        public IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            return ConfigureGlobalUpdateTrigger(this);
        }

      
        void IZappyTrigger.UnRegisterTrigger()
        {
                    }
    }
}
