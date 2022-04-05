using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Mssa
{
    internal class SFICommunicationChannel
    {
        private const string AGENT_ACK_SYNC_EVENT_NAME = "VSTLM_AGENT_CUIT_SFI_INPUT_PROCESSED";
        private const string AGENT_NOTIFY_SYNC_EVENT_NAME = "VSTLM_AGENT_CUIT_SFI_INPUT_NOTIFY";
        private const string AGENT_PING_SYNC_EVENT_NAME = "VSTLM_AGENT_CUIT_SFI_PING_NOTIFY";
        private const string AGENT_TO_SFI_SHARED_MEMORY_NAME = "VSTLM_AGENT_IMMERSIVEACCESS_CHANNEL_SHARED_MEMORY";
        private const int BUFFER_SIZE = 0x800;
        internal const uint WAIT_TIMEOUT = 0x102;

        internal SFICommunicationChannel(bool createNewChannel)
        {
            Memory = new SharedMemoryManager();
            bool flag = false;
            if (createNewChannel)
            {
                flag = Memory.Create("VSTLM_AGENT_IMMERSIVEACCESS_CHANNEL_SHARED_MEMORY", 0x800);
            }
            else
            {
                flag = Memory.OpenExisting("VSTLM_AGENT_IMMERSIVEACCESS_CHANNEL_SHARED_MEMORY", 0x800);
            }
            if (!flag)
            {
                throw new ArgumentException("Incorrect Arguments provided for Creating SFI Communication Channel");
            }
            SharedMemoryUtility.EventAccess dwDesiredAccess = SharedMemoryUtility.EventAccess.EventModifyState | SharedMemoryUtility.EventAccess.Synchronize;
            NotifyHandle = SharedMemoryUtility.CreateEventEx(IntPtr.Zero, "VSTLM_AGENT_CUIT_SFI_INPUT_NOTIFY", SharedMemoryUtility.EventCreateFlag.Default, dwDesiredAccess);
            AckHandle = SharedMemoryUtility.CreateEventEx(IntPtr.Zero, "VSTLM_AGENT_CUIT_SFI_INPUT_PROCESSED", SharedMemoryUtility.EventCreateFlag.Default, dwDesiredAccess);
        }

        internal void CloseChannel()
        {
            if (NotifyHandle != null)
            {
                NotifyHandle.Dispose();
                NotifyHandle = null;
            }
            if (AckHandle != null)
            {
                AckHandle.Dispose();
                AckHandle = null;
            }
        }

        internal void DestroyMemory()
        {
            if (Memory != null)
            {
                Memory.Destroy();
                Memory = null;
            }
        }

        internal SFI_REQUEST ReadRequestData()
        {
            SFI_REQUEST data = new SFI_REQUEST();
            Memory.GetData(ref data);
            return data;
        }

        internal bool SendAck(SFI_REQUEST channelData)
        {
            Memory.WriteData(channelData);
            return SharedMemoryUtility.SetEvent(AckHandle);
        }

        internal void SendExitSignal()
        {
            if (Memory != null && Memory.IsMemoryInitialized())
            {
                SFI_REQUEST data = new SFI_REQUEST
                {
                    RequestType = SFI_REQUEST_TYPE.EXIT,
                    ProcessId = Process.GetCurrentProcess().Id
                };
                Memory.WriteData(data);
                SharedMemoryUtility.SetEvent(NotifyHandle);
            }
        }

        internal void SendRequest(SFI_REQUEST data)
        {
            try
            {
                Memory.WriteData(data);
            }
            catch (Exception exception)
            {
                object[] args = new object[1];
                object[] objArray2 = { data.RequestType.ToString(), exception };
                args[0] = string.Format(CultureInfo.InvariantCulture, "SFIRequestError", objArray2);
                CrapyLogger.log.ErrorFormat("SFICommunicationChannel: {0}", args);
            }
            SharedMemoryUtility.ResetEvent(AckHandle);
            SharedMemoryUtility.SetEvent(NotifyHandle);
        }

        internal bool WaitForAck(int timeout, out SFI_ERROR_CODE ackError)
        {
            ackError = SFI_ERROR_CODE.NONE;
            if (SharedMemoryUtility.WaitForSingleObject(AckHandle, timeout) == 0)
            {
                SFI_REQUEST data = new SFI_REQUEST();
                Memory.GetData(ref data);
                ackError = data.ErrorCode;
                return true;
            }
            return false;
        }

        internal int WaitForRequest(int timeout) =>
            SharedMemoryUtility.WaitForSingleObject(NotifyHandle, timeout);

        private SharedMemoryUtility.EventHandle AckHandle { get; set; }

        private SharedMemoryManager Memory { get; set; }

        private SharedMemoryUtility.EventHandle NotifyHandle { get; set; }

        internal enum SFI_ERROR_CODE
        {
            NONE,
            UNKNOWN,
            INVALID_WINDOW_HANDLE
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SFI_REQUEST
        {
            internal SFI_REQUEST_TYPE RequestType;
            internal int WindowHandle;
            internal int ProcessId;
            internal SFI_ERROR_CODE ErrorCode;
            internal Rectangle Rectangle;
        }

        internal enum SFI_REQUEST_TYPE
        {
            REGISTER,
            SWITCH_WINDOW,
            DRAW_HIGHLIGHT_START,
            DRAW_HIGHLIGHT_STOP,
            EXIT
        }
    }
}