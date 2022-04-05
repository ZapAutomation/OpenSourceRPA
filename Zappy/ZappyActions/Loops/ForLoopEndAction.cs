using System.ComponentModel;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
                [Description(" Action Of ForLoop End")]
    public class ForLoopEndAction : LoopEndAction
    {
        public ForLoopEndAction() : base()
        {

        }
                                        
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            this.Initialize(context);
            (this._LoopStartAction as ForLoopStartAction).PerformStep(context);
            base.Invoke(context, actionInvoker);
        }
    }

}