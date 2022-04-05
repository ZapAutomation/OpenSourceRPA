//#define TRACE_DATA
#define Enable_Trapy_Service
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.ManagedHooks;
using ZappyMessages;
using ZappyMessages.RecordPlayback;

namespace Zappy.Trapy
{
    public static class TrapyService
    {
        struct EventBuffer
        {
            public byte[] ByteArray { get; set; }
            public int Offset { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEHOOKSTRUCT
        {
            public System.Drawing.Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }


        static BlockingCollection<EventBuffer> _Messages;
        static ConcurrentQueue<EventBuffer> _BufferPool;

        static MouseHook _MouseHook;
        static KeyboardHook _KeyboardHook;
        static Dictionary<string, WinEventHook> _WinEventHooks;

        public static event Action<KeyBoardInfo> KeyBoardInfoEvent;
        public static event Action<MouseHookInfo> MouseHookInfoEvent;
        public static event Action<TrapyWinEvent> WinInfoEvent;

        public static bool _ProcessKeyboardMouseEvents = true;
        static int _BufferFragmentSize;

        static byte[] _MegaBuffer;

        public static void Init()
        {
            try
            {
                _BufferFragmentSize = MessageSizeCache.GetMessageSize(MessageType.Max);

                _MegaBuffer = new byte[128 * 1024];//128k

                _BufferPool = new ConcurrentQueue<EventBuffer>();
                for (int i = 0; i < _MegaBuffer.Length; i += 128)
                {
                    for (int j = 0; j < 128 - _BufferFragmentSize; j += _BufferFragmentSize)
                    {
                        EventBuffer eb = new EventBuffer() { ByteArray = _MegaBuffer, Offset = i + j };
                        ReturnBufferToPool(eb);
                    }
                }

                _Messages = new BlockingCollection<EventBuffer>();

                _MouseHook = new MouseHook();
                _KeyboardHook = new KeyboardHook();
                _WinEventHooks = new Dictionary<string, WinEventHook>(10);

                WinEventHook _WinEventHook = new WinEventHook();//AccessibleEvents.SystemMenuStart, AccessibleEvents.ParentChange
                _WinEventHooks[_WinEventHook.Start.ToString() + _WinEventHook.Stop.ToString()] = _WinEventHook;

                Task.Factory.StartNew(RunMessageLoop);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }


        unsafe static void InternalWinEventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            EventBuffer _Buffer = GetBuffer();
            fixed (byte* ptr = &_Buffer.ByteArray[_Buffer.Offset])
            {
                TrapyWinEvent* _Kbptr = (TrapyWinEvent*)ptr;
                _Kbptr[0].MessageType = MessageType.WinHookEvent;
                _Kbptr[0].Event = (AccessibleEvents)eventType;
                _Kbptr[0].Hwnd = hwnd;
                _Kbptr[0].idChild = idChild;
                _Kbptr[0].idObject = idObject;
                _Kbptr[0].EventThreadID = dwEventThread;
            }
            _Messages.Add(_Buffer);
        }

        unsafe static void KeyboardHookCallback(int code, UIntPtr wparam, IntPtr lparam)
        {
            EventBuffer _Buffer = GetBuffer();
            fixed (byte* ptr = &_Buffer.ByteArray[_Buffer.Offset])
            {
                KeyBoardInfo* _Kbptr = (KeyBoardInfo*)ptr;
                _Kbptr->EventTime = Environment.TickCount;
                _Kbptr->MessageType = MessageType.KeyBoardHookEvent;
                _Kbptr->Event = (TrapyLowLevelHookMessage)wparam.ToUInt32();
                //KeyBoardInfo.lparam = lparam;
                KBDLLHOOKSTRUCT _Ptr = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lparam, typeof(KBDLLHOOKSTRUCT));
                _Kbptr->Flags = _Ptr.flags;
                _Kbptr->ScanCode = _Ptr.scanCode;
                _Kbptr->VirtualKeyCode = _Ptr.vkCode;
                _Kbptr->dwExtraInfo = _Ptr.dwExtraInfo;
            }
            _Messages.Add(_Buffer);
        }

        unsafe static void MouseHookCallback(int code, UIntPtr wparam, IntPtr lparam)
        {
            EventBuffer _Buffer = GetBuffer();
            fixed (byte* ptr = &_Buffer.ByteArray[_Buffer.Offset])
            {
                MouseHookInfo* _Kbptr = (MouseHookInfo*)ptr;

                _Kbptr->MessageType = MessageType.MouseHookEvent;
                _Kbptr->Event = (TrapyLowLevelHookMessage)code;
                _Kbptr->EventTime = Environment.TickCount;
                _Kbptr->Hwnd = wparam;
                MOUSEHOOKSTRUCT* _Ptr = (MOUSEHOOKSTRUCT*)lparam;
                _Kbptr->dwExtraInfo = _Ptr->dwExtraInfo;
                _Kbptr->flags = _Ptr->flags;
                _Kbptr->mouseData = _Ptr->mouseData;
                _Kbptr->Point = Control.MousePosition;// _Ptr->pt;
            }
            _Messages.Add(_Buffer);
        }

        public unsafe static void Start()
        {
            Stop();
            foreach (KeyValuePair<string, WinEventHook> keyValuePair in _WinEventHooks)
            {
                keyValuePair.Value.InstallHook(InternalWinEventHookCallback);
            }
            _KeyboardHook.InstallHook(KeyboardHookCallback);
            _MouseHook.InstallHook(MouseHookCallback);
        }

        public unsafe static void Stop()
        {
            try
            {
                _KeyboardHook.UninstallHook();
                foreach (KeyValuePair<string, WinEventHook> keyValuePair in _WinEventHooks)
                {
                    keyValuePair.Value.UninstallHook();
                }
                //_WinEventHookShow.UninstallHook();
                _MouseHook.UninstallHook();

                //StopMouseWindowHook();

                NativeHookWrapper.DisposeCppLayer();
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        static EventBuffer GetBuffer()
        {
            EventBuffer _Buffer;
            if (!_BufferPool.TryDequeue(out _Buffer))
                _Buffer = new EventBuffer() { ByteArray = new byte[_BufferFragmentSize], Offset = 0 };
            return _Buffer;
        }

        static void ReturnBufferToPool(EventBuffer Buffer)
        {
            if (Buffer.ByteArray == _MegaBuffer)
                _BufferPool.Enqueue(Buffer);
        }

        static unsafe void RunMessageLoop()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            Thread.CurrentThread.Name = "Trapy";
        RESTART:
            foreach (EventBuffer _Buffer in _Messages.GetConsumingEnumerable())
            {
                try
                {
                    fixed (byte* ptr = &_Buffer.ByteArray[_Buffer.Offset])
                    {
                        MessageType mt = (MessageType)((ushort*)ptr)[0]; //GetMessageTypeFromBuffer(_Buffer);
                        switch (mt)
                        {
                            case MessageType.WinHookEvent:
                                WinInfoEvent?.Invoke(((TrapyWinEvent*)ptr)[0]);
                                break;
                            case MessageType.MouseHookEvent:
#if TRACE_DATA
                                CrapyLogger.log.Error(((MouseHookInfo*)ptr)[0].ToString());
#endif
                                if (_ProcessKeyboardMouseEvents)
                                    MouseHookInfoEvent?.Invoke(((MouseHookInfo*)ptr)[0]);
                                break;
                            case MessageType.KeyBoardHookEvent: //global keyboard events and ignore further processing
#if TRACE_DATA
                                CrapyLogger.log.Error(((KeyBoardInfo*)ptr)[0].ToString());
#endif
                                if (_ProcessKeyboardMouseEvents)
                                {
                                    KeyBoardInfoEvent?.Invoke(((KeyBoardInfo*)ptr)[0]);
                                }

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CrapyLogger.log.Error(ex);
#else
              CrapyLogger.log.Warn(ex);
#endif
                }
                finally
                {
                    ReturnBufferToPool(_Buffer);
                }
            }

            if (!_Messages.IsAddingCompleted)
                goto RESTART;
        }

    }
}
