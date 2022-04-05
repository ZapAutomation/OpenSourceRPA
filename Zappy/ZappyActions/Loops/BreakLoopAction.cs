using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
            
    [Description("Break Loop")]
    public class BreakLoopAction : TemplateAction
    {
                
                                        
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {         
            LoopStartAction _Start = context.ContextData[CrapyConstants.ZappyCurrentLoopStart] as LoopStartAction;
            _Start.TerminateLoop = true;
            NextGuid = _Start.SelfGuid;
        }
    }
}
