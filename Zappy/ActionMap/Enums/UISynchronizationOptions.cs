using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum UISynchronizationOptions
    {
        None,
        DisableMouseSynchronization,
        DisableKeyboardSynchronization
    }
}