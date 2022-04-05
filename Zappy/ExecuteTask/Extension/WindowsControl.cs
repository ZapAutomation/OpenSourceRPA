using System;
using System.Runtime.InteropServices;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension
{
    internal class WindowsControl
    {
        private IntPtr hwnd;
        private ZappyTaskControl uiControl;

        public WindowsControl(ZappyTaskControl uiControl)
        {
            this.uiControl = uiControl;
            this.hwnd = uiControl.TechnologyElement.WindowHandle;
        }

        public int GetExtendedStyle() =>
            NativeMethods.GetWindowLong(this.hwnd, NativeMethods.GWLParameter.GWL_EXSTYLE);

        public int GetGeneric<T>(int flag, ref T obj)
        {
            object lParam = (T)obj;
            IntPtr ptr = WinNativeMethods.XProcSendMessageByRef(this.hwnd, flag, IntPtr.Zero, ref lParam);
            obj = (T)lParam;
            return ptr.ToInt32();
        }

        public IntPtr GetHandle(int flag) =>
            WinNativeMethods.SendMessage(this.hwnd, flag, IntPtr.Zero, IntPtr.Zero);

        public int GetInt(int flag) =>
            this.GetInt(flag, 0, 0);

        public int GetInt(int flag, int wParam, int lParam) =>
            WinNativeMethods.SendMessage(this.hwnd, flag, new IntPtr(wParam), new IntPtr(lParam)).ToInt32();

        public bool GetScrollInfo(int fnBar, ref WinNativeMethods.SCROLLINFO scrollInfo) =>
            WinNativeMethods.GetScrollInfo(this.hwnd, fnBar, ref scrollInfo);

        public WinNativeMethods.CHARRANGE GetSelectionRange(int richTextFlag, int textBoxFlag)
        {
            WinNativeMethods.CHARRANGE charrange = new WinNativeMethods.CHARRANGE
            {
                cpMax = -1,
                cpMin = 0
            };
            this.GetGeneric<WinNativeMethods.CHARRANGE>(richTextFlag, ref charrange);
            if (charrange.cpMax == -1)
            {
                int num = WinNativeMethods.SendMessage(this.hwnd, textBoxFlag, IntPtr.Zero, IntPtr.Zero).ToInt32();
                charrange.cpMax = (num - (num & 0x7fff)) / 0x10000;
                charrange.cpMin = num & 0x7fff;
            }
            return charrange;
        }

        public int GetStyle() =>
            NativeMethods.GetWindowLong(this.hwnd, NativeMethods.GWLParameter.GWL_STYLE);

        public string GetText()
        {
            IntPtr lParam = Marshal.AllocCoTaskMem(0x800);
            IntPtr ptr2 = WinNativeMethods.SendMessage(this.hwnd, 13, new IntPtr(0x800), lParam);
            if (ptr2.ToInt32() > 0)
            {
                char[] destination = new char[ptr2.ToInt32()];
                Marshal.Copy(lParam, destination, 0, destination.Length);
                return new string(destination);
            }
            return string.Empty;
        }

        public bool HasExtendedStyle(int flag) =>
            ((this.GetExtendedStyle() & flag) == flag);

        public bool HasOwnerWindow() =>
            (NativeMethods.GetWindowLong(this.hwnd, NativeMethods.GWLParameter.GWL_HWNDPARENT) > 0);

        public bool HasStyle(int flag) =>
            ((this.GetStyle() & flag) == flag);

        public bool IsCheckedList()
        {
            ZappyTaskControl firstChild = this.uiControl.FirstChild;
            return ((firstChild != null) && (firstChild.ControlType == ControlType.CheckBox));
        }

        public bool IsListView() =>
            (this.uiControl.ClassNameValue.IndexOf("listview", StringComparison.OrdinalIgnoreCase) >= 0);

        private uint MakeDword(int low, int high) =>
            ((uint)((low & 0xffff) | ((high & 0xffff) << 0x10)));

        public IntPtr SetInt(int flag, int wParam, int lParam) =>
            WinNativeMethods.SendMessage(this.hwnd, flag, new IntPtr(wParam), new IntPtr(lParam));

        public void SetScrollInfo(int style, ref WinNativeMethods.SCROLLINFO scrollInfo, int msg, bool fRedraw)
        {
            WinNativeMethods.SetScrollInfo(this.hwnd, style, ref scrollInfo, fRedraw);
            int nPos = scrollInfo.nPos;
            WinNativeMethods.SendMessage(this.hwnd, msg, new IntPtr((long)this.MakeDword(nPos, 4)), IntPtr.Zero);
            WinNativeMethods.SendMessage(this.hwnd, msg, new IntPtr((long)this.MakeDword(nPos, 5)), IntPtr.Zero);
        }
    }
}