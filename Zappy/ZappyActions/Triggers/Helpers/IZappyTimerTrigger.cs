using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Triggers.Helpers
{
                public interface IZappyTimerTrigger : IZappyTrigger
    {
        DynamicProperty<int> DueTimeInSeconds { get; }
                DynamicProperty<bool> AskBeforeExecuting { get; set; }
    }
}