using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum SetValueAsEditBoxOptions
    {
        All = 0x3f00,
        DeleteContent = 0x100,
        DoNotVerify = 0x2000,
        None = 0,
        UseCopyPaste = 0x800,
        UseProgrammatic = 0x200,
        UseSendKeys = 0x1000,
        UseWindowMessage = 0x400
    }
}