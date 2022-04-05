using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ZappyMessages.RecordPlayback
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 42)]
    public struct MouseHookInfo
    {
        [FieldOffset(0)]
        public MessageType MessageType;
        [FieldOffset(2)]
        public TrapyLowLevelHookMessage Event;
        [FieldOffset(6)]
        public Point Point;
        [FieldOffset(14)]
        public UIntPtr Hwnd;
        [FieldOffset(22)]
        public int mouseData;
        [FieldOffset(26)]
        public int flags;
        [FieldOffset(30)]
        public IntPtr dwExtraInfo;
        [FieldOffset(38)]
        public int EventTime;
        public override string ToString()

        {
            return string.Format("MouseEvent: {0}, Pt:({1},{2}) Hwnd:{3}, flags:{4} extrainfo:{5}, Time:{6}, mousedata:{7}",
                Event.ToString(),
                Point.X,
                Point.Y,
                Hwnd,
                flags,
                dwExtraInfo, EventTime, mouseData);
        }
    }

}
