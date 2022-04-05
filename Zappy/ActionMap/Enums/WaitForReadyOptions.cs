using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum WaitForReadyOptions
    {
        None,
        EnablePlaybackWaitForReady,
        EnableTechnologyWaitForReady
    }
}