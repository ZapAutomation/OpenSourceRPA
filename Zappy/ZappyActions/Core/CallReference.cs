using System;
using System.ComponentModel;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Core
{
    public class CallReference : TemplateAction
    {
        [Description("Specify the Guid (SelfGuid) for reference activity")]
        [Category("Input")]
        public DynamicProperty<Guid> ActivityReferenceGuid { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyTask runningZappyTask = context.ContextData[CrapyConstants.ZappyTaskKey] as ZappyTask;
            runningZappyTask.ActionDictionary.TryGetValue(ActivityReferenceGuid, out IZappyAction action);
            action.Invoke(context, actionInvoker);
        }
    }
}
