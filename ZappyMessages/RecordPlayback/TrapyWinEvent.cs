using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZappyMessages.RecordPlayback
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 30)]
    public struct TrapyWinEvent
    {
        [FieldOffset(0)]
        public MessageType MessageType;
        [FieldOffset(2)]
        public AccessibleEvents Event;
        [FieldOffset(6)]
        public IntPtr Hwnd;
        [FieldOffset(14)]
        public int idObject;
        [FieldOffset(18)]
        public int idChild;
        [FieldOffset(22)]
        public uint EventThreadID;
        [FieldOffset(26)]
        public int EventTime;

        public override string ToString()
        {
            return string.Format("WinEvent: {0}, Hwnd:{1} idObject:{2}, idChild:{3} EventThreadID:{4} Time:{5}",
                Event.ToString(),
                Hwnd,
                idObject,
                idChild,
                EventThreadID,
                EventTime);
        }
    }
}
