
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Invoker
{
    public class ZappyInvoker
    {
                                
        private static Dictionary<IntPtr, frmZappy> _InvokeRegister = new Dictionary<IntPtr, frmZappy>(128);

        static ZappyInvoker()
        {
                                                                                                                                                                                                            
        }


        public static void RemoveZappy(IntPtr hwnd)
        {
            lock (_InvokeRegister)
            {
                _InvokeRegister.Remove(hwnd);
            }
        }

        public static void Invoke(string DirPath, IntPtr ParentWindow, IntPtr InvokingHwnd,
            ZappyTask PredictedTask = null)
        {

            frmZappy _Zappy = null;
            if (_InvokeRegister.TryGetValue(ParentWindow, out _Zappy) && !_Zappy.IsDisposed)
            {
                                                
                
                                if (!string.IsNullOrEmpty(DirPath) && string.IsNullOrEmpty(_Zappy.TaskDirPath))
                {
                                        _Zappy.TaskDirPath = DirPath;
                    _Zappy.LoadTasks(DirPath);
                }

                return;
            }

                        if (NativeMethods.IsWindow(ParentWindow) && NativeMethods.IsWindowVisible(InvokingHwnd)
                && (ParentWindow != IntPtr.Zero))
            {

                                                                                                if (Program.UI_Instance.InvokeRequired)
                    Program.UI_Instance.BeginInvoke(new Action(() =>
                        ShowZappy(ParentWindow, DirPath, InvokingHwnd, PredictedTask)));
                else
                    ShowZappy(ParentWindow, DirPath, InvokingHwnd, PredictedTask);
            }
        }


        static void ShowZappy(IntPtr ParentWindow, string DirPath, IntPtr InvokingHwnd,
            ZappyTask PredictedTask = null)
        {
                                    frmZappy _Zappy = new frmZappy(ParentWindow, DirPath, InvokingHwnd, PredictedTask);
            lock (_InvokeRegister)
            {
                _InvokeRegister[ParentWindow] = _Zappy;
            }

            _Zappy.ShowInactiveTopmost();
                                            }


                                                                                                
                
        

                                                                                                                                                                

        public static void ExecuteFirstZappyTask()
        {
            IntPtr _Handle = NativeMethods.GetActiveWindow();
            IntPtr ancestor = _Handle;
            if (!NativeMethods.IsWindow(ancestor))
                ancestor = NativeMethods.GetAncestor(_Handle, NativeMethods.GetAncestorFlag.GA_ROOTOWNER);
            frmZappy _Zappy = null;
            if (_InvokeRegister.TryGetValue(ancestor, out _Zappy) && !_Zappy.IsDisposed)
            {
                _Zappy.PlayFirstTask();
            }
        }


                                                                                                                                                                                                                                                                                                                                                                                                                                

        public static void AddTask(string DirPath, string TaskFilePath, ZappyTask Task, IZappyAction firstAction)
        {
            bool isZappyAvailable = false;
            foreach (KeyValuePair<IntPtr, frmZappy> kvp in _InvokeRegister)
            {
                if (kvp.Value.TaskDirPath.ToUpper() == DirPath.ToUpper())
                {
                                        isZappyAvailable = true;
                                        kvp.Value.BeginInvoke(
                        new Action(() => kvp.Value.RemoveInsertNewNode(TaskFilePath)));
                    if (TaskFilePath != null && Path.GetFileName(TaskFilePath).Equals(CrapyConstants.AutoLearnActivityName))
                        kvp.Value.ChangeColor();
                }
            }

            if (!isZappyAvailable)
            {
                if (firstAction is ChromeAction)
                {
                                        List<Process> processes = Process.GetProcessesByName("chrome").ToList();
                    processes.AddRange(Process.GetProcessesByName("firefox"));
                    if (processes != null && processes.Count > 0)
                    {
                        foreach (Process chrome in processes)
                        {
                                                        if (chrome.MainWindowHandle == IntPtr.Zero)
                            {
                                continue;
                            }
                            IntPtr windowHandle = chrome.MainWindowHandle;
                            Invoke(DirPath,
                                windowHandle,
                                windowHandle);
                        }
                    }

                }
                else if (firstAction is ZappyTaskAction)
                    Invoke(DirPath, (firstAction as ZappyTaskAction).TopLevelWindowHandle, (firstAction as ZappyTaskAction).ElementWindowHandle);
            }
        }

                                                                                                                                                                        
                                                                        
                        
                        
        public static void ShowNotificationZappy(string Notification)
        {
            PubSubService.Instance.Publish(PubSubTopicRegister.Notification, Notification);
        }
                                    }
}
