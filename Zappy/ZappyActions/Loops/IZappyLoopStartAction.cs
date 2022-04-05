using System;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
    public interface IZappyLoopStartAction : IZappyAction
    {
        Guid LoopEndGuid { get; set; }

        IZappyLoopEndAction GetEndLoopAction();

        ZappyLoopType LoopType { get; }

        DynamicProperty<bool> TerminateLoop { get; }

        bool CheckLoopTermination();
        void FinalizeLoop(IZappyExecutionContext context);

    }
}
