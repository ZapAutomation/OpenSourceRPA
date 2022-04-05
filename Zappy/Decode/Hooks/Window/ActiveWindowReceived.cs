using System;
using System.Runtime.InteropServices;
using System.Text;
using ZappyMessages;
using ZappyMessages.RecordPlayback;

namespace Zappy.Decode.Hooks.Window
{
    
                public class ActiveWindowReceived
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
            int nMaxCount);

        public string WindowTitle { get; protected set; }

        public readonly TrapyWinEvent ActiveWindowChangedInfo;
        public ActiveWindowReceived(TrapyWinEvent ActiveWindowChangedInfo)
        {
            this.ActiveWindowChangedInfo = ActiveWindowChangedInfo;
            if (ActiveWindowChangedInfo.Hwnd != IntPtr.Zero)
            {
                                                int _Len = 0;
                StringBuilder sb = new StringBuilder(_Len + 2);
                try
                {
                    _Len = GetWindowTextLength(ActiveWindowChangedInfo.Hwnd);
                    sb = new StringBuilder(_Len + 2);
                    sb.Append('a', _Len + 2);
                    _Len = GetWindowText(ActiveWindowChangedInfo.Hwnd, sb, _Len + 2);
                }
                catch { }
                WindowTitle = sb.ToString(0, _Len);
            }
        }



    }
}
