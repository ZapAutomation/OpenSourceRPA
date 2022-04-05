using System;

namespace Zappy.ZappyTaskEditor.Nodes
{
    [Flags]
    public enum NodeState
    {
        General = 0,
        Selected = 1,
        Debug = 2,
        ExecutionPassed = 4,
        ExecutionFailed = 8,
        BreakPoint = 16

    }
}