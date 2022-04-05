﻿





using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zappy.ZappyActions.ElementPicker.Input
{
    public sealed partial class EditorPageInputDriver : IDisposable
    {
        #region Native

        private const Int32 HC_ACTION = 0;

        private const Int32 WH_KEYBOARD_LL = 13;

        private const Int32 WH_MOUSE_LL = 14;

        private delegate IntPtr LLProc(Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(Int32 idHook, LLProc lpfn, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern Boolean UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern Boolean GetPhysicalCursorPos(ref POINT pt);

        [DllImport("user32.dll")]
        private static extern Int16 GetKeyState(Int32 nVirtKey);

        private struct POINT
        {
            public int x;
            public int y;
        }

        private IntPtr keyboardHook;

        private LLProc keyboardProc;

        private IntPtr mouseHook;

        private LLProc mouseProc;

        private const Int32 VK_EXTENDED = 0x100;

        private const Int32 VK_LSHIFT = 0xA0;

        private const Int32 VK_RSHIFT = 0xA1;

        private const Int32 VK_LCONTROL = 0xA2;

        private const Int32 VK_RCONTROL = 0xA3;

        private const Int32 VK_LMENU = 0xA4;

        private const Int32 VK_RMENU = 0xA5;

        private const Int32 VK_LWIN = 0x5B;

        private const Int32 VK_RWIN = 0x5C;

        private const Int32 VK_PACKET = 0xE7;

        private const Int32 WM_KEYUP = 0x0101;

        private const Int32 WM_SYSKEYUP = 0x0105;

        private const Int32 WM_KEYDOWN = 0x0100;

        private const Int32 WM_SYSKEYDOWN = 0x0104;

        private const Int32 WM_LBUTTONUP = 0x0202;

        private const Int32 WM_LBUTTONDOWN = 0x0201;

        private const Int32 WM_MBUTTONUP = 0x0208;

        private const Int32 WM_MBUTTONDOWN = 0x0207;

        private const Int32 WM_MOUSEMOVE = 0x0200;

        private const Int32 WM_RBUTTONUP = 0x0205;

        private const Int32 WM_RBUTTONDOWN = 0x0204;

        private const Int32 LLKHF_EXTENDED = 0x01;

        private struct KBDLLHOOKSTRUCT
        {
#pragma warning disable 649
            public UInt32 vkCode;
            public UInt32 scanCode;
            public LLKHF flags;
            public UInt32 time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        private enum LLKHF : UInt32
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80
        }

        private static Int32 LOBYTE(Int32 x) => x & 0xFF;

        private static Int32 HIBYTE(Int32 x) => x >> 8;

        #endregion

        private IntPtr LowLevelKeyboardProc(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= HC_ACTION)
            {
                var e = new InputEventArgs();
                var pt = new POINT();
                GetPhysicalCursorPos(ref pt);
                e.X = pt.x;
                e.Y = pt.y;
                e.AltKey = HIBYTE(GetKeyState(VK_LMENU) | GetKeyState(VK_RMENU)) != 0;
                e.CtrlKey = HIBYTE(GetKeyState(VK_LCONTROL) | GetKeyState(VK_RCONTROL)) != 0;
                e.ShiftKey = HIBYTE(GetKeyState(VK_LSHIFT) | GetKeyState(VK_RSHIFT)) != 0;
                e.WinKey = HIBYTE(GetKeyState(VK_LWIN) | GetKeyState(VK_RWIN)) != 0;

                var k = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                e.Key = (KeyboardKey)k.vkCode;                 if (VK_PACKET == (Int32)e.Key)
                {
                    e.Key = (KeyboardKey)(-k.scanCode);                 }
                else if (k.flags.HasFlag(LLKHF.LLKHF_EXTENDED))
                {
                    e.Key += VK_EXTENDED;                 }

                switch ((Int32)wParam)
                {
                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        e.Type = InputEventType.KeyUp;
                        this.OnKeyUp(e);
                        this.OnInput(e);
                        break;
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        e.Type = InputEventType.KeyDown;
                        this.OnKeyDown(e);
                        this.OnInput(e);
                        break;
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private IntPtr LowLevelMouseProc(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= HC_ACTION)
            {
                var e = new InputEventArgs();
                var pt = new POINT();
                GetPhysicalCursorPos(ref pt);
                e.X = pt.x;
                e.Y = pt.y;
                e.AltKey = HIBYTE(GetKeyState(VK_LMENU) | GetKeyState(VK_RMENU)) != 0;
                e.CtrlKey = HIBYTE(GetKeyState(VK_LCONTROL) | GetKeyState(VK_RCONTROL)) != 0;
                e.ShiftKey = HIBYTE(GetKeyState(VK_LSHIFT) | GetKeyState(VK_RSHIFT)) != 0;
                e.WinKey = HIBYTE(GetKeyState(VK_LWIN) | GetKeyState(VK_RWIN)) != 0;

                switch ((Int32)wParam)
                {
                    case WM_LBUTTONUP:
                        e.Type = InputEventType.MouseUp;
                        e.Button = MouseButton.Left;
                        this.OnMouseUp(e);
                        this.OnInput(e);
                        break;
                    case WM_RBUTTONUP:
                        e.Type = InputEventType.MouseUp;
                        e.Button = MouseButton.Right;
                        this.OnMouseUp(e);
                        this.OnInput(e);
                        break;
                    case WM_MBUTTONUP:
                        e.Type = InputEventType.MouseUp;
                        e.Button = MouseButton.Middle;
                        this.OnMouseUp(e);
                        this.OnInput(e);
                        break;
                    case WM_LBUTTONDOWN:
                        e.Type = InputEventType.MouseDown;
                        e.Button = MouseButton.Left;
                        this.OnMouseDown(e);
                        this.OnInput(e);
                        break;
                    case WM_RBUTTONDOWN:
                        e.Type = InputEventType.MouseDown;
                        e.Button = MouseButton.Right;
                        this.OnMouseDown(e);
                        this.OnInput(e);
                        break;
                    case WM_MBUTTONDOWN:
                        e.Type = InputEventType.MouseDown;
                        e.Button = MouseButton.Middle;
                        this.OnMouseDown(e);
                        this.OnInput(e);
                        break;
                    case WM_MOUSEMOVE:
                        e.Type = InputEventType.MouseMove;
                        this.OnMouseMove(e);
                        this.OnInput(e);
                        break;
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        public EditorPageInputDriver()
        {
            Task.Run(() =>
            {
                this.keyboardProc = LowLevelKeyboardProc;
                this.keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, IntPtr.Zero, 0);
                this.mouseProc = LowLevelMouseProc;
                this.mouseHook = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, IntPtr.Zero, 0);
                Application.Run();
                this.Dispose();
            });
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(this.keyboardHook);
            UnhookWindowsHookEx(this.mouseHook);
        }
    }
}
