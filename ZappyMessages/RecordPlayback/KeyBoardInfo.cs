using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZappyMessages.RecordPlayback
{

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
    public struct KeyBoardInfo
    {
        [FieldOffset(0)]
        public MessageType MessageType;
        [FieldOffset(4)]
        public TrapyLowLevelHookMessage Event;
        [FieldOffset(8)]
        public int VirtualKeyCode;
        [FieldOffset(12)]
        public int ScanCode;
        [FieldOffset(16)]
        public int Flags;
        [FieldOffset(20)]
        public int EventTime;
        [FieldOffset(24)]
        public IntPtr dwExtraInfo;

        public override string ToString()
        {
            return string.Format("KeyboardEvent: {0}, Event:(VirtKeyCode:{1} ScanCode:{2} Flags:{3} dwExtraInfo:{4}, Keys:{5}) Time:{6}",
                Event.ToString(),
                VirtualKeyCode,
                ScanCode, Flags,
                dwExtraInfo,
                ((Keys)VirtualKeyCode).ToString(),//Marshal.ReadInt32(lparam)
                 EventTime);
        }
    }
}
