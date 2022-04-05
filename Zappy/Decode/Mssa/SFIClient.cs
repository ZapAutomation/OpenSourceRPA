using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Mssa
{
    internal class SFIClient
    {
        private const int CHANNEL_TIMEOUT = 0x7d0;
        private const string DefaultLogDirectory = @"\ZappyTaskLogs\";
        private Process m_sfiProcess;
        private bool m_sfiProcessActive;
        private const string MtmLogDirectory = @"\RemoteTestRunner\";
        private const string REMOTE_RECORDER_PROCESS_NAME = "MTTA";
        private static bool s_initialised;
        private static SFIClient s_Instance = new SFIClient();

        private SFIClient()
        {
        }

        public void Cleanup()
        {
            s_initialised = false;
            CommunicationChannel.SendExitSignal();
            CommunicationChannel.CloseChannel();
            m_sfiProcessActive = false;
        }

        public bool DrawHighlightStart(Rectangle rectangle)
        {
            SFICommunicationChannel.SFI_REQUEST request = new SFICommunicationChannel.SFI_REQUEST
            {
                RequestType = SFICommunicationChannel.SFI_REQUEST_TYPE.DRAW_HIGHLIGHT_START,
                Rectangle = rectangle
            };
            object[] args = { rectangle };
            
            return SendRequest(request, 0x7d0);
        }

        public bool DrawHighlightStop()
        {
            SFICommunicationChannel.SFI_REQUEST request = new SFICommunicationChannel.SFI_REQUEST
            {
                RequestType = SFICommunicationChannel.SFI_REQUEST_TYPE.DRAW_HIGHLIGHT_STOP
            };
            
            return SendRequest(request, 0x7d0);
        }

        public void Initialise()
        {
            if (!s_initialised)
            {
                try
                {
                                                            InitialiseSFIServer();
                                    }
                catch (ArgumentException)
                {
                    CrapyLogger.log.ErrorFormat("SFIClient: Unable to Create SharedMemory for SFICommunication.");
                }
                if (!m_sfiProcessActive)
                {
                    CrapyLogger.log.ErrorFormat("SFIClient: Unable to activate SFI server. Closing the Channel.");
                    CommunicationChannel.CloseChannel();
                    throw new Exception("SFIInitError");
                }
                s_initialised = true;
            }
        }

        private void InitialiseSFIServer()
        {
            object[] args = { "CodedZappyTaskSwitchFromImmersive.EXE" };
            
            char[] separator = { '.' };
            Process[] processesByName = Process.GetProcessesByName("CodedZappyTaskSwitchFromImmersive.EXE".Split(separator)[0]);
            if (processesByName.Length > 1)
            {
                object[] objArray2 = { processesByName.Length };
                
                foreach (Process process in processesByName)
                {
                    try
                    {
                        process.Kill();
                        if (process.WaitForExit(20))
                        {
                            CrapyLogger.log.ErrorFormat("SFIClient: Attempt to close CodedZappyTaskSwitchFromImmersive process timedout.");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (SystemException exception)
                    {
                        object[] objArray3 = { exception };
                        CrapyLogger.log.ErrorFormat("SFIClient: Attempt to close CodedZappyTaskSwitchFromImmersive Server process failed with exception {0}.", objArray3);
                    }
                }
            }
            else if (processesByName.Length == 1)
            {
                object[] objArray4 = { processesByName[0].Id };
                
                CommunicationChannel = new SFICommunicationChannel(false);
                SFICommunicationChannel.SFI_REQUEST request = new SFICommunicationChannel.SFI_REQUEST
                {
                    ProcessId = Process.GetCurrentProcess().Id,
                    RequestType = SFICommunicationChannel.SFI_REQUEST_TYPE.REGISTER
                };
                if (SendRequest(request, 0x7d0))
                {
                    m_sfiProcess = processesByName[0];
                    m_sfiProcessActive = true;
                }
                else
                {
                    
                    m_sfiProcessActive = false;
                }
            }
            if (!m_sfiProcessActive)
            {
                CommunicationChannel = new SFICommunicationChannel(true);
                StartSFIProcess();
            }
        }

        private bool SendRequest(SFICommunicationChannel.SFI_REQUEST request, int timeout = 0x7d0)
        {
            bool flag = true;
            SFICommunicationChannel.SFI_ERROR_CODE nONE = SFICommunicationChannel.SFI_ERROR_CODE.NONE;
            if (!m_sfiProcessActive)
            {
                object[] args = { request.RequestType.ToString() };
                CrapyLogger.log.ErrorFormat("SFIClient: CodedZappyTaskSwitchFromImmsersive Process not Active. Unable to ServiceRequest of type: {0}", args);
                return false;
            }
            CommunicationChannel.SendRequest(request);
            if (!CommunicationChannel.WaitForAck(timeout, out nONE))
            {
                flag = false;
                switch (nONE)
                {
                    case SFICommunicationChannel.SFI_ERROR_CODE.NONE:
                        CrapyLogger.log.ErrorFormat("SFIClient: Timedout While waiting for ACK from SFIServer.");
                        return flag;

                    case SFICommunicationChannel.SFI_ERROR_CODE.UNKNOWN:
                        CrapyLogger.log.ErrorFormat("SFIClient: Unexpected Error Occurred while trying to SendRequest to SFIServer.");
                        return flag;

                    case SFICommunicationChannel.SFI_ERROR_CODE.INVALID_WINDOW_HANDLE:
                        {
                            object[] objArray2 = { request.WindowHandle };
                            
                            return flag;
                        }
                }
            }
            return flag;
        }

        private void StartSFIProcess()
        {
            string str = @"\ZappyTaskLogs\";
            string fileName = Process.GetCurrentProcess().MainModule.FileName;
            if (string.Equals(Path.GetFileNameWithoutExtension(fileName), "MTTA", StringComparison.OrdinalIgnoreCase))
            {
                str = @"\RemoteTestRunner\";
            }
            string str3 = Path.Combine(Path.GetDirectoryName(fileName), "CodedZappyTaskSwitchFromImmersive.EXE");
            object[] objArray1 = { Path.GetTempPath(), str, " ", Process.GetCurrentProcess().Id };
            string arguments = string.Concat(objArray1);
            object[] args = { str3, arguments };
            
            ProcessStartInfo startInfo = new ProcessStartInfo(str3, arguments)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                m_sfiProcess = Process.Start(startInfo);
                m_sfiProcessActive = true;
            }
            catch (SystemException exception)
            {
                object[] objArray3 = { exception };
                CrapyLogger.log.ErrorFormat("SFIClient: Failed to start the CodedZappyTaskSwitchFromImmersive Process. Exception: {0}", objArray3);
                m_sfiProcess = null;
                m_sfiProcessActive = false;
            }
        }

        public bool SwitchFromImmersiveToWindow(IntPtr windowHandle)
        {
            SFICommunicationChannel.SFI_REQUEST request = new SFICommunicationChannel.SFI_REQUEST
            {
                WindowHandle = windowHandle.ToInt32(),
                RequestType = SFICommunicationChannel.SFI_REQUEST_TYPE.SWITCH_WINDOW
            };
            object[] args = { windowHandle };
            
            return SendRequest(request, 0x7d0);
        }

        private SFICommunicationChannel CommunicationChannel { get; set; }

        public static SFIClient Instance =>
            s_Instance;
    }
}