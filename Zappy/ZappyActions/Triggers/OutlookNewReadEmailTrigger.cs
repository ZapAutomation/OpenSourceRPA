using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Helpers;
using Zappy.Plugins.Outlook;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using ZappyMessages;
using ZappyMessages.OutlookMessages;
using ZappyMessages.PubSub;

namespace Zappy.ZappyActions.Triggers
{
        [Description("Check for new email in outlook")]
    public class OutlookNewReadEmailTrigger : TemplateAction, IZappyTrigger
    {
        public OutlookNewReadEmailTrigger()
        {
            IsDisabled = false;

                    }

        [RunScriptFunctionType(typeof(Func<string, bool>))]
        [Description("Receiver mail id")]
        [Category("Input")]
        public DynamicProperty<string> To { get; set; }

        [RunScriptFunctionType(typeof(Func<string, bool>))]
        [Description("Sender mail id")]
        [Category("Input")]
        public DynamicProperty<string> From { get; set; }

        [RunScriptFunctionType(typeof(Func<string, bool>))]
        [Description("Subject of mail")]
        [Category("Input")]
        public DynamicProperty<string> Subject { get; set; }

        [RunScriptFunctionType(typeof(Func<string, bool>))]
        [Description("Name of the attachment")]
        [Category("Input")]
        public DynamicProperty<string> AttachmentName { get; set; }

        [RunScriptFunctionType(typeof(Func<string, bool>))]
        [Description("Message body content")]
        [Category("Input")]
        public DynamicProperty<string> Body { get; set; }

        [Description("Mail save directory")]
        [Category("Input")]
        public DynamicProperty<string> SaveDirectory { get; set; }

        [XmlIgnore]
        [Category("Optional")]
        public ZappyTriggerType ZappyTriggerType
        {
            get { return ZappyTriggerType.Outlook; }
        }
        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
        }

        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this) + " TO:" + this.To + " From:" + this.From + " Subject:" + this.Subject + " AttachmentName:" + this.AttachmentName;
        }

                                                        
                                        
        public IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            
            ZappyTask runningZappyTask = context.ContextData[CrapyConstants.ZappyTaskKey] as ZappyTask;

            OutlookNewEmailTriggerInfo triggerInfo = new OutlookNewEmailTriggerInfo
            {
                To = this.To,
                From = this.From,
                Subject = this.Subject,
                Body = this.Body,
                TriggerId = this.SelfGuid,
                ParentTaskId = runningZappyTask.Id,
                SaveDirPath = this.SaveDirectory,
                RemoveTrigger = false
            };

            OutlookCommunicator.Instance.RegisterNewEmailTriggerOutlook(triggerInfo);
            return null;
        }

        public void UnRegisterTrigger()
        {
            OutlookNewEmailTriggerInfo triggerInfo = new OutlookNewEmailTriggerInfo
            {
                To = this.To,
                From = this.From,
                Subject = this.Subject,
                Body = this.Body,
                TriggerId = this.SelfGuid,
                SaveDirPath = this.SaveDirectory,
                RemoveTrigger = true
            };
            OutlookCommunicator.Instance.RegisterNewEmailTriggerOutlook(triggerInfo);
        }
    }
}

