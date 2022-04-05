namespace Zappy.ActionMap.Enums
{
    public enum ControlStates : long
    {
        AlertHigh = 0x10000000L,
        AlertLow = 0x4000000L,
        AlertMedium = 0x8000000L,
        Animated = 0x4000L,
        Busy = 0x800L,
        Checked = 0x10L,
        Collapsed = 0x400L,
        Default = 0x100L,
        Expanded = 0x200L,

        ExtSelectable = 0x2000000L,
        Floating = 0x1000L,
        Focusable = 0x100000L,
        Focused = 4L,
        HasPopup = 0x40000000L,
        HotTracked = 0x80L,
        Indeterminate = 0x20L,
        Invalid = 0x80000000L,
        Invisible = 0x8000L,
        Linked = 0x400000L,

        Marqueed = 0x2000L,
        Maximized = 0x200000000L,
        Minimized = 0x400000000L,
        Mixed = 0x20L,
        Moveable = 0x40000L,

        MultiSelectable = 0x1000000L,
        None = 0L,
        Normal = 0x800000000L,
        Off = 0x2000000000L,

        Offscreen = 0x10000L,
        On = 0x1000000000L,
        Pressed = 8L,
        Protected = 0x20000000L,
        ReadOnly = 0x40L,
        Restored = 0x100000000L,
        Selectable = 0x200000L,
        Selected = 2L,
        SelfVoicing = 0x80000L,
        Sizeable = 0x20000L,
        Traversed = 0x800000L,
        Unavailable = 1L
    }
}