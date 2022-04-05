using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper.Enums;

namespace Zappy.Decode.Helper
{

    internal static class ProcessManager
    {
        private static bool hasIncludeProcessSpecified;
        private static readonly object lockObject = new object();
        private static Dictionary<int, bool> processIdExcludeList = new Dictionary<int, bool>();
        private static Dictionary<int, int> processIdIncludeList = new Dictionary<int, int>();
        private static Dictionary<string, string> processNameExcludeList = new Dictionary<string, string>();
        private static Dictionary<string, string> processNameIncludeList = new Dictionary<string, string>();
        internal static Dictionary<string, string> transparentProcessList = new Dictionary<string, string>();


        static ProcessManager()
        {
                                                                                    
                                                        }

        public static bool GetHigherPrivelegeFlagForProcessId(int processId)
        {
            object lockObject = ProcessManager.lockObject;
            lock (lockObject)
            {
                if (processIdExcludeList.ContainsKey(processId))
                {
                    return processIdExcludeList[processId];
                }
            }
            return false;
        }

        public static int GetProcessIdForWindow(IntPtr windowHandle) =>
            ZappyTaskUtilities.GetProcessIdForWindow(windowHandle);

        public static ProcessType IgnoreAction(int processId, bool throwIfAppFromNetworkPath)
        {
            bool processListsModified = false;
            return IgnoreAction(processId, throwIfAppFromNetworkPath, out processListsModified);
        }

        public static ProcessType IgnoreAction(IntPtr windowHandle, bool throwIfAppFromNetworkPath)
        {
            int processId = 0;
            return IgnoreAction(windowHandle, throwIfAppFromNetworkPath, out processId);
        }

        public static ProcessType IgnoreAction(Point location, bool throwIfAppFromNetworkPath, out int processId)
        {
            NativeMethods.POINT pt = new NativeMethods.POINT(location.X, location.Y);
            return IgnoreAction(NativeMethods.WindowFromPoint(pt), throwIfAppFromNetworkPath, out processId);
        }

        public static ProcessType IgnoreAction(int processId, bool throwIfAppFromNetworkPath, out bool processListsModified)
        {
            processListsModified = false;
            if (processId == 0 || processId == ZappyTaskUtilities.CurrentProcessId)
            {
                return ProcessType.IgnoredProcess;
            }
            if (processIdExcludeList.ContainsKey(processId))
            {
                return ProcessType.IgnoredProcess;
            }
            if (!processIdIncludeList.ContainsKey(processId))
            {
                string fileName = string.Empty;
                bool flag = ZappyTaskUtilities.IsHigherPrivilegeProcess(processId, out fileName);
                if (transparentProcessList.ContainsKey(fileName) || transparentProcessList.ContainsKey(Path.GetFileName(fileName)))
                {
                    return ProcessType.TransparentProcess;
                }
                if (flag || string.IsNullOrEmpty(fileName) || processNameExcludeList.ContainsKey(fileName) || processNameExcludeList.ContainsKey(Path.GetFileName(fileName)))
                {
                    object obj2 = ProcessManager.lockObject;
                    lock (obj2)
                    {
                        if (!processIdExcludeList.ContainsKey(processId))
                        {
                            if (flag)
                            {
                                object[] args = { processId, fileName };
                                
                            }
                            processIdExcludeList.Add(processId, flag);
                            processListsModified = true;
                        }
                    }
                    return ProcessType.IgnoredProcess;
                }
                if (throwIfAppFromNetworkPath && IsUNCPath(fileName))
                {
                    if (!processIdIncludeList.ContainsKey(processId))
                    {
                        processIdIncludeList.Add(processId, processId);
                        processListsModified = true;
                        if (!processNameIncludeList.ContainsKey(fileName))
                        {
                            processNameIncludeList.Add(fileName, fileName);
                        }
                        object[] objArray2 = { fileName };
                        throw new ApplicationFromNetworkShareException(string.Format(CultureInfo.CurrentCulture, "Properties.Resources.ApplicationFromNetworkShareException", objArray2));
                    }
                }
                if (!hasIncludeProcessSpecified)
                {

                    if (!processIdIncludeList.ContainsKey(processId))                    {
                        processListsModified = true;
                        processIdIncludeList.Add(processId, processId);
                    }
                    return ProcessType.None;
                }
                if (!processNameIncludeList.ContainsKey(fileName) && !processNameIncludeList.ContainsKey(Path.GetFileName(fileName)))
                {
                    return ProcessType.IgnoredProcess;
                }
                object lockObject = ProcessManager.lockObject;
                lock (lockObject)
                {
                    if (throwIfAppFromNetworkPath && !processIdIncludeList.ContainsKey(processId))
                    {
                        processListsModified = true;
                        processIdIncludeList.Add(processId, processId);
                    }
                }
            }
            return ProcessType.None;
        }

        public static ProcessType IgnoreAction(IntPtr windowHandle, bool throwIfAppFromNetworkPath, out int processId)
        {
            processId = 0;
            if (windowHandle != IntPtr.Zero)
            {
                processId = GetProcessIdForWindow(windowHandle);
                return IgnoreAction(processId, throwIfAppFromNetworkPath);
            }
            return ProcessType.IgnoredProcess;
        }

        public static bool IgnoredOrTransparentProcess(ProcessType processType)
        {
            if (processType != ProcessType.IgnoredProcess)
            {
                return processType == ProcessType.TransparentProcess;
            }
            return true;
        }

        private static Dictionary<string, string> InitializeTransparentProcessList() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { {
                "Tabtip.exe",
                "Tabtip.exe"
            }, {
                    "Zappy.exe",
                    "Zappy.exe"
                }
                                
                                                                                                                                                
            };
                                
        public static bool IsUNCPath(int processId)
        {
            string fileName = string.Empty;
            ZappyTaskUtilities.IsHigherPrivilegeProcess(processId, out fileName);
            return IsUNCPath(fileName);
        }

        public static bool IsUNCPath(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    Uri uri = new Uri(fileName);
                    return uri.IsUnc;
                }
                catch (UriFormatException)
                {
                }
            }
            return false;
        }

        public static void SetHigherPrivelegeFlagForProcessId(int processId, bool higherPrivelegeProcessFlag)
        {
            object lockObject = ProcessManager.lockObject;
            lock (lockObject)
            {
                if (processIdExcludeList.ContainsKey(processId))
                {
                    processIdExcludeList[processId] = higherPrivelegeProcessFlag;
                }
            }
        }

        public static void SetRecorderOptions(RecorderOptions recorderOptions)
        {
            object lockObject = ProcessManager.lockObject;
            lock (lockObject)
            {
                processIdExcludeList = new Dictionary<int, bool>();
                processNameExcludeList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                processIdIncludeList = new Dictionary<int, int>();
                processNameIncludeList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                transparentProcessList = InitializeTransparentProcessList();
                hasIncludeProcessSpecified = false;
                foreach (int num in recorderOptions.ExcludeProcess)
                {
                    if (!processIdExcludeList.ContainsKey(num))
                    {
                        processIdExcludeList.Add(num, false);
                    }
                }
                foreach (string str in recorderOptions.ExcludeProcessName)
                {
                    string expandedLongPath = ZappyTaskUtilities.GetExpandedLongPath(str);
                    if (!string.IsNullOrEmpty(expandedLongPath) && !processNameExcludeList.ContainsKey(expandedLongPath))
                    {
                        processNameExcludeList.Add(expandedLongPath, expandedLongPath);
                    }
                }
                foreach (int num2 in recorderOptions.IncludeProcess)
                {
                    if (!IsUNCPath(num2) && !processIdIncludeList.ContainsKey(num2))
                    {
                        processIdIncludeList.Add(num2, num2);
                    }
                }
                foreach (string str3 in recorderOptions.IncludeProcessName)
                {
                    string str4 = ZappyTaskUtilities.GetExpandedLongPath(str3);
                    if (!string.IsNullOrEmpty(str4) && !processNameIncludeList.ContainsKey(str4))
                    {
                        processNameIncludeList.Add(str4, str4);
                    }
                }
                if (recorderOptions.RecordOnDesktopProcess)
                {
                    IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
                    if (desktopWindow != IntPtr.Zero)
                    {
                        int windowProcessId = (int)NativeMethods.GetWindowProcessId(desktopWindow);
                        processIdIncludeList.Add(windowProcessId, windowProcessId);
                    }
                    processNameIncludeList.Add("explorer.exe", "explorer.exe");
                }
                if (processIdIncludeList.Count != 0 || processNameIncludeList.Count != 0)
                {
                    hasIncludeProcessSpecified = true;
                }
            }
        }
    }



}
