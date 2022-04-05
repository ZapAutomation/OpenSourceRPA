using System;

namespace Zappy.ActionMap.Enums
{
    [Flags]
    public enum SetValueAsComboBoxOptions
    {
        DoNotVerifyMirrorLanguage = 0x10,
        DoNotVerifySendKeys = 8,
        ExpandProgrammatically = 0x80,
        None = 0,
        PressEnterAfterTyping = 0x40,
        UseLeftDropDownButton = 1,
        UseQueryId = 0x20,
        UseSelect = 2,
        UseSetAsEdit = 4
    }
}