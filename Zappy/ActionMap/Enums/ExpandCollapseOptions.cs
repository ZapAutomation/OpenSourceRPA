using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum ExpandCollapseOptions
    {
        DoNotVerify = 0x20,
        None = 0,
        UseDoubleClick = 2,
        UseEnter = 8,

        UseNumpad = 4,
        UseProgrammatic = 0x10,
        UseWindowMessage = 1
    }
}