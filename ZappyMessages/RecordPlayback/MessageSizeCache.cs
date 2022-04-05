using System.Linq;
using System.Runtime.InteropServices;

namespace ZappyMessages.RecordPlayback
{
    public static class MessageSizeCache
    {
        static int[] _Size;
        static MessageSizeCache()
        {
            _Size = new int[(int)MessageType.Max + 1];
            _Size[(int)MessageType.WinHookEvent] = Marshal.SizeOf(typeof(TrapyWinEvent));
            _Size[(int)MessageType.KeyBoardHookEvent] = Marshal.SizeOf(typeof(KeyBoardInfo));
            _Size[(int)MessageType.MouseHookEvent] = Marshal.SizeOf(typeof(MouseHookInfo));
            _Size[(int)MessageType.Max] = _Size.Max();
        }

        public static int GetMessageSize(MessageType MessageType)
        {
            return _Size[(int)MessageType];
        }
    }
}
