using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum ScrollOptions
    {
        None = 0,
        UseClickOnScrollBar = 8,
        UseKeyboard = 0x10,
        UseMouseWheel = 2,
        UseProgrammatic = 1,
        UseScrollBar = 4
    }
}