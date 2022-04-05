using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.LowLevelHookEvent;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask;
using Zappy.Properties;
using Zappy.Trapy;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Triggers;
using ZappyMessages;
using ZappyMessages.RecordPlayback;
using Timer = System.Threading.Timer;

namespace Zappy.Decode.Hooks.Window
{
    internal class WindowLaunchEventCapture : EventCaptureBase, IEventCapture
    {
        private Dictionary<IntPtr, ZappyTaskAction> actionsWithMissingElement;
        private static readonly string clickOnceAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps");
        private Dictionary<string, string> ddeCommandLines;
        private volatile bool ddeCommandLinesInitialized;

        private Dictionary<int, string> _ProcessFileNameDictionary;
        public Dictionary<int, Process> _RecordedProcesses;
        public Dictionary<IntPtr, int> _RecordedWindowHandles;
        public Dictionary<int, int> _RecordedProcessesTiming;

                        private AccessibleEvents lastAccEvent;
        private uint lastEventTimeStamp;
        private IntPtr lastEventWindowHandle;
        private TaskActivityElement previousElement;
                        private Dictionary<string, string> specialProcessesToIgnore;
        private readonly object syncObject;
                                        private int _TickCount;
        private int _LastActiveProcess;
        private IntPtr _DesktopHandle;

        public event Action<int, int, string> ProcessMainWindowDestroyed;
        Timer _tmr;

        public WindowLaunchEventCapture(IEventCapture accessoryEventCapture) : base(accessoryEventCapture)
        {
                                    _LastActiveProcess = -1;
            _TickCount = Environment.TickCount;
            syncObject = new object();
            
            _RecordedWindowHandles = new Dictionary<IntPtr, int>();
            _RecordedProcessesTiming = new Dictionary<int, int>();
            _RecordedProcesses = new Dictionary<int, Process>();
            _ProcessFileNameDictionary = new Dictionary<int, string>();

            actionsWithMissingElement = new Dictionary<IntPtr, ZappyTaskAction>();
            _tmr = new Timer(TimerCallback_tmr, null, 1000, 1000);

            Instance = this;
        }

        private void AddProcessToExistingProcesses(Process process)
        {
            MonitorProcess(process);
        }

        private static Process[] GetAllProcesses()
        {
            Process[] processes = new Process[0];
            try
            {
                processes = Process.GetProcesses();
            }
            catch (Win32Exception exception)
            {
                CrapyLogger.log.Error(exception);
            }
            return processes;
        }

        public static Process GetProcess(IntPtr windowHandle) =>
            GetProcessById(ProcessManager.GetProcessIdForWindow(windowHandle));

        private static Process GetProcessById(int pid)
        {
            Process processById;
            try
            {
                processById = Process.GetProcessById(pid);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(string.Empty, exception);
            }
            return processById;
        }

        public static string GetProcessCommandLine(Process process)
        {
            string str = string.Empty;
            object[] args = { process.Id };
            string[] selectedProperties = { "CommandLine" };
            SelectQuery query = new SelectQuery("Win32_Process", string.Format(CultureInfo.InvariantCulture, "ProcessId={0}", args), selectedProperties);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                using (ManagementObjectCollection objects = searcher.Get())
                {
                    if (objects != null)
                    {
                        foreach (ManagementObject obj2 in objects)
                        {
                            return obj2["CommandLine"] as string;
                        }
                        return str;
                    }
                    return str;
                }
            }
        }

        private void HandleNewApplication(IntPtr windowHandle)
        {
            

            if (Paused)
            {
                
                return;
            }
            Process process = GetProcess(windowHandle);
            string processFileName = NativeMethods.GetProcessFileName(process.Id);

                        if(ClientUI.WindowLaunchTriggers.Count >0)
            {
                HandleWindowLaunchTrgger(processFileName, windowHandle);
            }

            if (string.IsNullOrEmpty(processFileName))
                return;

            if (specialProcessesToIgnore.ContainsKey(processFileName))
            {
                return;
            }
            if (processFileName.StartsWith(clickOnceAppPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            string processCommandLine = null;
                        try
            {
                processCommandLine = GetProcessCommandLine(process);
            }
            catch (Exception exception)
            {
                if (!(exception is COMException) && !(exception is ManagementException))
                {
                    throw;
                }
                                            }
            if (!string.IsNullOrEmpty(processCommandLine))
            {
                if (ddeCommandLines.ContainsKey(processCommandLine))
                {
                                        
                    return;
                }
                processCommandLine = StripPathFromCommandLine(processCommandLine, processFileName);
            }
                                    MonitorProcess(process);
            RecordLaunchApplication(process, processFileName, processCommandLine);

        }


        private void HandleWindowLaunchTrgger(string processFileName, IntPtr windowHandle)
        {
            string exeName = Path.GetFileName(processFileName);
            lock (ClientUI.WindowLaunchTriggers)
            {
                for (int i = 0; i < ClientUI.WindowLaunchTriggers.Count; i++)
                {
                    if(exeName.ToUpper().Equals
                        (ClientUI.WindowLaunchTriggers[i].ApplicationExeName.ToUpper()))
                    {
                        if(!string.IsNullOrEmpty(ClientUI.WindowLaunchTriggers[i].WindowTitle))
                        {
                            if (ClientUI.WindowLaunchTriggers[i].WindowTitle.Equals(NativeMethods.GetWindowText(windowHandle)))
                            {
                                                                ZappyPlaybackCommunicator.Instance.
                                    PublishWindowLaunchRequestToZappyPlayback(ClientUI.WindowLaunchTriggers[i]);
                            }
                        }
                        else
                        {
                                                        ZappyPlaybackCommunicator.Instance.
                                    PublishWindowLaunchRequestToZappyPlayback(ClientUI.WindowLaunchTriggers[i]);
                        }
                    }
                }
            }
        }

        private void HandleWindowDestroy(IntPtr windowHandle)
        {

            int _ProcessID = -1;
            
            if (_RecordedWindowHandles.TryGetValue(windowHandle, out _ProcessID))
            {
                
                _RecordedWindowHandles.Remove(windowHandle);
                Process _DestroyedProcess = null;
                _RecordedProcesses.TryGetValue(_ProcessID, out _DestroyedProcess);
                Process_Exited(_DestroyedProcess, EventArgs.Empty);

            }
        }

        private void HandleWindowShow(IntPtr windowHandle)
        {
            try
            {


                if (IsNewMainWindow(windowHandle))
                {
                    HandleNewApplication(windowHandle);
                    return;
                }
            }
            catch (Win32Exception exception2)
            {
                CrapyLogger.log.Error(exception2);
            }
            catch (InvalidOperationException exception3)
            {
                CrapyLogger.log.Error(exception3);
            }
        }

        private bool IgnoreEvent(AccessibleEvents accEvent, IntPtr windowHandle, uint dwmsEventTime)
        {
            bool returnVal = false;

            if (accEvent != AccessibleEvents.Destroy && accEvent != AccessibleEvents.Show)
                returnVal = true;

            if (accEvent == lastAccEvent && windowHandle == lastEventWindowHandle && dwmsEventTime == lastEventTimeStamp)
            {
                returnVal = true;
            }

            if (!returnVal)
            {
                lastAccEvent = accEvent;
                lastEventTimeStamp = dwmsEventTime;
                lastEventWindowHandle = windowHandle;
            }
            
            return returnVal;
        }

        private void InitializeDdeCommandLines(object dummy)
        {
                                    if (!ddeCommandLinesInitialized)
            {
                ddeCommandLines = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string str in Registry.ClassesRoot.GetSubKeyNames())
                {
                    if (str[0] != '.')
                    {
                        try
                        {
                            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(str + @"\shell"))
                            {
                                if (key != null)
                                {
                                    foreach (string str2 in key.GetSubKeyNames())
                                    {
                                        using (RegistryKey key2 = key.OpenSubKey(str2 + @"\ddeexec"))
                                        {
                                            if (key2 != null)
                                            {
                                                using (RegistryKey key3 = key.OpenSubKey(str2 + @"\command"))
                                                {
                                                    if (key3 != null)
                                                    {
                                                        string str3 = key3.GetValue(string.Empty) as string;
                                                        if (!string.IsNullOrEmpty(str3))
                                                        {
                                                            str3 = Environment.ExpandEnvironmentVariables(str3);
                                                            if (!ddeCommandLines.ContainsKey(str3))
                                                            {
                                                                ddeCommandLines.Add(str3, str3);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (IOException exception)
                        {
                                                        CrapyLogger.log.ErrorFormat("Recorder : InitializeDdeCommandLines : {0}", exception);
                        }
                        catch (UnauthorizedAccessException exception2)
                        {
                                                        CrapyLogger.log.ErrorFormat("Recorder : InitializeDdeCommandLines : {0}", exception2);
                        }
                        catch (ObjectDisposedException exception3)
                        {
                                                        CrapyLogger.log.ErrorFormat("Recorder : InitializeDdeCommandLines : {0}", exception3);
                        }
                        catch (SecurityException exception4)
                        {
                                                        CrapyLogger.log.ErrorFormat("Recorder : InitializeDdeCommandLines : {0}", exception4);
                        }
                    }
                }
                InitializeSpecialProcessToIgnoredList();
                ddeCommandLinesInitialized = true;
                            }
        }

        private void InitializeProcessList(object processes)
        {

            try
            {
                Process[] processArray = (Process[])processes;
                foreach (Process process in processArray)
                {
                    AddProcessToExistingProcesses(process);
                }
                object syncObject = this.syncObject;
                lock (syncObject)
                {
                    IntPtr desktopWindow = _DesktopHandle;
                    if (desktopWindow != IntPtr.Zero)
                    {
                        int processIdForWindow = ProcessManager.GetProcessIdForWindow(desktopWindow);
                        if (!this._RecordedProcesses.ContainsKey(processIdForWindow))
                        {
                            string processFileName =
                                Path.GetFileNameWithoutExtension(NativeMethods.GetProcessFileName(processIdForWindow));
                            this._RecordedProcesses.Add(processIdForWindow, Process.GetProcessById(processIdForWindow));
                            this._RecordedWindowHandles[desktopWindow] = processIdForWindow;

                        }
                        MonitorProcess(desktopWindow);

                    }
                    if (!_RecordedProcesses.ContainsKey(0))
                    {
                        _RecordedProcesses.Add(0, null);
                    }
                }
            }
            catch (Win32Exception exception)
            {
                CrapyLogger.log.Error(exception);
            }
            catch (InvalidOperationException exception2)
            {
                CrapyLogger.log.Error(exception2);
            }
                    }

        private void InitializeSpecialProcessToIgnoredList()
        {
            specialProcessesToIgnore = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string key = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\Dwm.exe");
            specialProcessesToIgnore.Add(key, key);
            foreach (string str3 in Directory.GetFiles(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET"), "dfsvc.exe", SearchOption.AllDirectories))
            {
                specialProcessesToIgnore.Add(str3, str3);
            }
        }

        private bool IsNewMainWindow(IntPtr windowHandle)
        {
            object syncObject = this.syncObject;
            lock (syncObject)
            {
                int processIdForWindow = ProcessManager.GetProcessIdForWindow(windowHandle);

                if (processIdForWindow != _LastActiveProcess)
                {
                    int _PrevTickCount = 0;
                    _RecordedProcessesTiming.TryGetValue(_LastActiveProcess, out _PrevTickCount);
                    _PrevTickCount += Environment.TickCount - _TickCount;
                    _RecordedProcessesTiming[_LastActiveProcess] = _PrevTickCount;
                    _TickCount = Environment.TickCount;
                    _LastActiveProcess = processIdForWindow;
                }

                if (!_RecordedProcesses.ContainsKey(processIdForWindow))
                {
                    Process _Process = GetProcessById(processIdForWindow);
                    IntPtr mainWindowHandle = _Process.MainWindowHandle;
                    if (mainWindowHandle == windowHandle || mainWindowHandle == NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT))
                    {
                        string processFileName = NativeMethods.GetProcessFileName(processIdForWindow);
                        _ProcessFileNameDictionary[processIdForWindow] = processFileName;
                        _RecordedProcesses.Add(processIdForWindow, _Process);
                        _RecordedWindowHandles[mainWindowHandle] = processIdForWindow;
                                                

                        return true;
                    }
                                                                                                                        
                                        
                }
                return false;
            }
        }



        bool IEventCapture.SetTrackingElement(TaskActivityElement element, Point interactionPoint, bool alwaysTakeSnapshot)
        {
            object[] args = { element };
            
            bool flag = false;
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                try
                {
                                        if (element != null && actionsWithMissingElement != null && element != previousElement && actionsWithMissingElement.Count > 0)
                    {
                        previousElement = element;
                        TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                        if (element2 != null)
                        {
                            IntPtr windowHandle = element2.WindowHandle;
                            if (windowHandle != IntPtr.Zero && actionsWithMissingElement.ContainsKey(windowHandle))
                            {
                                ZappyTaskAction action = actionsWithMissingElement[windowHandle];
                                actionsWithMissingElement.Remove(windowHandle);
                                action.ActivityElement = element2;
                                flag = true;
                            }
                        }
                    }
                }
                catch (ZappyTaskControlNotAvailableException exception)
                {
                    CrapyLogger.log.Error(exception);
                }
                if (accessoryEventCapture != null)
                {
                    accessoryEventCapture.SetTrackingElement(element, interactionPoint, alwaysTakeSnapshot);
                }
                return flag;
            }
        }

        public void Start()
        {
                        _DesktopHandle = NativeMethods.GetDesktopWindow();
            InitializeProcessList(GetAllProcesses());
            InitializeDdeCommandLines(null);
            EventCaptureException = null;
            OnStarted(this, null);
            TrapyService.WinInfoEvent += TrapyRemoteControl_WinInfoEvent;
        }

        public void TrapyRemoteControl_WinInfoEvent(TrapyWinEvent obj)
        {
            WindowEventCallback(IntPtr.Zero, obj.Event, obj.Hwnd, obj.idObject, obj.idChild, obj.EventThreadID, (uint)obj.EventTime);
        }

        public void Stop()
        {
            TrapyService.WinInfoEvent -= TrapyRemoteControl_WinInfoEvent;

        }

        private void RecordLaunchApplication(Process process, string processName, string arguments)
        {
            bool _Monitored = MonitorProcess(process);
                                    
            if (!string.IsNullOrEmpty(processName) && _Monitored)
            {
                LaunchApplicationAction element = new LaunchApplicationAction(processName);
                if (!string.IsNullOrEmpty(arguments))
                    element.Arguments = arguments;
                actions.Push(element);
                if (!actionsWithMissingElement.ContainsKey(process.MainWindowHandle))
                {
                    actionsWithMissingElement.Add(process.MainWindowHandle, element);
                }
            }
        }

        private static string StripFirstArgumentFromCommandLine(string arguments)
        {
            int startIndex = -1;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == '"')
                {
                    i++;
                    while (i < arguments.Length)
                    {
                        if (arguments[i] == '"')
                        {
                            break;
                        }
                        i++;
                    }
                }
                else if (char.IsWhiteSpace(arguments, i))
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex != -1)
            {
                return arguments.Substring(startIndex);
            }
            return string.Empty;
        }

        public static string StripPathFromCommandLine(string arguments, string fileName)
        {
            arguments = arguments.Trim();
            if (arguments.StartsWith(fileName, StringComparison.OrdinalIgnoreCase))
            {
                arguments = arguments.Substring(fileName.Length);
            }
            else
            {
                object[] args = { fileName };
                string str = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", args);
                if (arguments.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                {
                    arguments = arguments.Substring(str.Length);
                }
                else
                {
                    arguments = StripFirstArgumentFromCommandLine(arguments);
                }
            }
            arguments = arguments.TrimStart();
            if (string.Equals(arguments, "\"", StringComparison.Ordinal))
            {
                arguments = string.Empty;
            }
            return arguments;
        }

                                void WindowEventCallback(IntPtr hWinEventHook, AccessibleEvents accEvent, IntPtr windowHandle, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (!IgnoreEvent(accEvent, windowHandle, dwmsEventTime))
            {
                WindowEventCallbackWorker(accEvent, windowHandle);
            }
        }

        private void WindowEventCallbackWorker(AccessibleEvents accEvent, IntPtr windowHandle)
        {
            AccessibleEvents events = accEvent;
            IntPtr hWnd = windowHandle;
            try
            {
                if (hWnd != IntPtr.Zero &&
                  hWnd != _DesktopHandle)
                {

                    if (events == AccessibleEvents.Destroy && !NativeMethods.IsWindow(hWnd))
                        HandleWindowDestroy(hWnd);
                                        else if (events == AccessibleEvents.Show && NativeMethods.IsWindow(hWnd))
                    {
                                                if (!ProcessManager.IgnoredOrTransparentProcess(ProcessManager.IgnoreAction(hWnd, false)))
                        {
                            HandleWindowShow(hWnd);
                                                                                }
                        else
                        {
                                                                                                                                                                    }
                    }
                }


            }
            catch (Exception exception)
            {
                EventCaptureException = exception;
            }
        }


        private void MonitorProcess(IntPtr Hwnd)
        {
            if (!_RecordedWindowHandles.ContainsKey(Hwnd))
            {
                int _ProcessID = (int)NativeMethods.GetWindowProcessId(Hwnd);
                MonitorProcess(_ProcessID);
            }
        }

        private void MonitorProcess(int ProcessID)
        {
            if (!_RecordedProcesses.ContainsKey(ProcessID))
            {
                MonitorProcess(Process.GetProcessById(ProcessID));
            }
        }

        private bool MonitorProcess(Process Process)
        {
            if (Process.MainWindowHandle == IntPtr.Zero || ProcessManager.IgnoredOrTransparentProcess(ProcessManager.IgnoreAction(Process.Id, false)))
            {
                
                return false;
            }

            if (!_RecordedProcesses.ContainsKey(Process.Id))
            {
                _RecordedProcesses[Process.Id] = Process;
                _RecordedWindowHandles[Process.MainWindowHandle] = Process.Id;
                string _ProcessFile = NativeMethods.GetProcessFileName(Process.Id);
                _ProcessFileNameDictionary[Process.Id] = _ProcessFile;
                
                return true;
            }
            return true;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process proc = sender as Process;
            int _ProcessID = -1;

            lock (_RecordedProcesses)
            {
                if (proc != null && _RecordedProcesses.ContainsKey(proc.Id))
                {
                    _ProcessID = proc.Id;
                    _RecordedProcesses.Remove(_ProcessID);

                }
            }


            if (_ProcessID > 0)
            {
                string _ProcessFilePath = string.Empty;
                int _TimeSpent = -1;
                _RecordedProcessesTiming.TryGetValue(_ProcessID, out _TimeSpent);
                _ProcessFileNameDictionary.TryGetValue(_ProcessID, out _ProcessFilePath);
                ProcessMainWindowDestroyed?.BeginInvoke(_ProcessID, _TimeSpent, _ProcessFilePath, null, null);
                            }

        }

        public static WindowLaunchEventCapture Instance { get; private set; }

        void TimerCallback_tmr(object o)
        {
            try
            {
                foreach (KeyValuePair<IntPtr, int> item in _RecordedWindowHandles)
                {
                    if (!NativeMethods.IsWindow(item.Key))
                    {
                        Process proc = null;
                        if (_RecordedProcesses.TryGetValue(item.Value, out proc))
                            Process_Exited(proc, EventArgs.Empty);
                    }
                }
            }
            catch { }
        }
    }
}