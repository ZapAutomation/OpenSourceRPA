using System;
using Zappy.SharedInterface;

namespace Zappy.ZappyActions.Loops
{
                public interface IZappyLoopEndAction : IZappyAction
    {
        Guid LoopStartGuid { get; set; }
    }
}
