namespace Zappy.Decode.Hooks.LowLevelHookEvent
{
    internal enum LowLevelHookMessage
    {
        Char = 0x102,
        DeadChar = 0x103,
        KeyDown = 0x100,
        KeyUp = 0x101,
        LeftButtonDoubleClick = 0x203,
        LeftButtonDown = 0x201,
        LeftButtonUp = 0x202,
        MiddleButtonDoubleClick = 0x209,
        MiddleButtonDown = 0x207,
        MiddleButtonUp = 520,
        MouseMove = 0x200,
        MouseWheel = 0x20a,
        MouseHWheel = 0x20E,

        None = 0,
        RightButtonDoubleClick = 0x206,
        RightButtonDown = 0x204,
        RightButtonUp = 0x205,
        SysChar = 0x106,
        SysDeadChar = 0x107,
        SysKeyDown = 260,
        SysKeyUp = 0x105,

        UniChar = 0x109,
        XButtonDoubleClick = 0x20d,
        XButtonDown = 0x20b,
        XButtonUp = 0x20c
    }
}