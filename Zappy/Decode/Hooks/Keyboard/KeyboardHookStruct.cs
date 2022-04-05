using System.Runtime.InteropServices;

namespace Zappy.Decode.Hooks.Keyboard
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardHookStruct
    {
        internal int vkCode;
        internal int scanCode;
        internal int flags;
        internal int time;
        internal int dwExtraInfo;
    }
}