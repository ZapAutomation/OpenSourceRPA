using System;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Triggers.Helpers
{
    public interface IZappyTrigger
    {
                DynamicProperty<bool> IsDisabled { get; set; }
        IDisposable RegisterTrigger(IZappyExecutionContext context);
        void UnRegisterTrigger();
    }

}

