using System;
using System.Collections.Generic;
using System.IO;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Graph;
using Zappy.Invoker;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel;

#if !LEGACY4

#endif

namespace Zappy.Helpers
{
    internal static class InternalNodeGenerator
    {

        public static Dictionary<uint, InternalNode> ProcessNodes { get; private set; }

        private static Dictionary<IntPtr, InternalNode> _WindowHandles;
        private static InternalNode _cachedProcessNode, _DesktopNode, _IdleProcess, _IdleWindow;
        //public static DateTime _LastAnalyticsWrittenDate;
        private static List<IntPtr> _RemovalHandles;

        public static GraphBuilder NodeGraphBuilder { get; private set; }

        public static DateTime LastActivity
        {
            get
            {
                return NodeGraphBuilder?.LastActivity ?? DateTime.MaxValue;
            }
        }
        //public static ZappyTask _MegaTask = new ZappyTask();

        internal static void Initialize()
        {

            ProcessNodes = new Dictionary<uint, InternalNode>();
            _WindowHandles = new Dictionary<IntPtr, InternalNode>();
            _RemovalHandles = new List<IntPtr>();
            _IdleWindow = new InternalNode("Idle Window");
            _IdleProcess = new InternalNode("Idle Process");
            _IdleProcess.Nodes.Add(_IdleWindow);
            _IdleWindow.Parent = _IdleProcess;

            NodeGraphBuilder = new GraphBuilder();
            NodeGraphBuilder.BatchedActivities += TimerHelper;

        }

        internal static void stop()
        {
            if (NodeGraphBuilder != null)
                NodeGraphBuilder.Stop();
        }

        public static void AddAction(IZappyAction Action)
        {
            //CrapyLogger.log.Error(Action.ToString());
            if (NodeGraphBuilder != null)
                NodeGraphBuilder.EditAction(Action, ActionMap.Enums.RecordedEventType.NewAction);
        }

        public static void DeleteAction(ZappyTaskAction Action)
        {
            if (NodeGraphBuilder != null)
                NodeGraphBuilder.EditAction(Action, ActionMap.Enums.RecordedEventType.DeletedAction);
        }

        internal static void TimerHelper(List<IZappyAction> BatchedActivities)
        {
            try
            {
                CreateDisplayNode(BatchedActivities);

                List<InternalNode> _ProcessToCheck = null;
                foreach (KeyValuePair<IntPtr, InternalNode> kvp in _WindowHandles)
                {
                    if (!NativeMethods.IsWindowVisible(kvp.Key))
                    {
                        if (kvp.Value.Tag != null)
                        {
                            DateTime _MarkedTime = Convert.ToDateTime(kvp.Value.Tag);
                            if ((WallClock.Now - _MarkedTime).TotalSeconds >= 3)
                            {
                                //save file , remove handle
                                _RemovalHandles.Add(kvp.Key);
                                List<IZappyAction> _RecordedActivities = new List<IZappyAction>(kvp.Value.Nodes.Count);
                                foreach (InternalNode TaskNode in kvp.Value.Nodes)
                                {
                                    IZappyAction action = TaskNode.Tag as IZappyAction;
                                    if (action != null)
                                    {
                                        try
                                        {
                                            if (NodeGraphBuilder.RecordedEvents.IndexFromEnd(action, 0) >= 0)
                                                _RecordedActivities.Add(TaskNode.Tag as IZappyAction);
                                        }
                                        catch { }
                                    }
                                }
                                if (_ProcessToCheck == null)
                                    _ProcessToCheck = new List<InternalNode>();
                                if (!_ProcessToCheck.Contains(kvp.Value.Parent))
                                    _ProcessToCheck.Add(kvp.Value.Parent);

                                //kvp.Value.Parent.Nodes.Remove(kvp.Value);
                            }
                        }
                        else
                        {
                            kvp.Value.Tag = WallClock.Now;
                        }
                    }
                }

                for (int i = 0; i < _RemovalHandles.Count; i++)
                {
                    _WindowHandles.Remove(_RemovalHandles[i]);
                }

                if (_ProcessToCheck != null)
                    for (int i = 0; i < _ProcessToCheck.Count; i++)
                    {
                        for (int j = 0; j < _ProcessToCheck[i].Nodes.Count; j++)
                        {
                            List<IZappyAction> _RecordedActivities =
                                new List<IZappyAction>(_ProcessToCheck[i].Nodes[j].Nodes.Count);
                            foreach (InternalNode TaskNode in _ProcessToCheck[i].Nodes[j].Nodes)
                                _RecordedActivities.Add(TaskNode.Tag as IZappyAction);
                        }
                    }

                _RemovalHandles.Clear();

            }
            catch (Exception e)
            {
                CrapyLogger.log.Error(e);
                //throw;
            }

        }

        private static string _ActionString;
        private static InternalNode _ProcessNode, _WindowNode;

        /// <summary>
        /// For automatic prediction of Excel activities
        /// </summary>
        private static int rowIndex, actionCounter = 0;
        private static string sheetName, workbookName = string.Empty;
        private static bool learningExcelAction = false;

     
        static void CreateDisplayNode(List<IZappyAction> BatchedActivities)
        {
            try
            {
                //START:
                if (BatchedActivities.Count > 0)
                {
                    //_InactivityCount = CrapyConstants.MaxInactivitySecs;
                    int _IterationCount = BatchedActivities.Count;
                    for (int i = 0; i < _IterationCount; i++)
                    {
                        actionCounter++;
                        if (learningExcelAction && actionCounter > 50)
                        {
                            //stop excel learning
                            LearnedActions.CreateLearnedActions("AutoLearnedTask.zappy", false);
                            learningExcelAction = false;
                        }
                        _ActionString = string.Empty;
                        _ProcessNode = null;
                        _WindowNode = null;

                        IZappyAction _ZappyAction = BatchedActivities[i];
                        if (_ZappyAction is ChromeAction)
                        {
                            continue;
                        }
                        else if (_ZappyAction is ZappyTaskAction)
                        {
                            ZappyTaskAction uit = _ZappyAction as ZappyTaskAction;

                            if (uit.ActivityElement != null && uit.ActivityElement.WindowHandle != IntPtr.Zero &&
                                uit.ActivityElement.TopLevelElement != null)
                            {

                                generateLastActivitesList(uit, uit.ActivityElement.TopLevelElement.WindowHandle, uit.ActivityElement.WindowHandle,
                                    uit.ActivityElement.TopLevelElement.Name);
                            }
                            else if (uit.ElementWindowHandle != IntPtr.Zero)
                            {
                                generateLastActivitesList(uit, uit.TopLevelWindowHandle, uit.ElementWindowHandle, uit.ZappyWindowTitle);
                            }
                            else
                            {
                                if (_DesktopNode == null)
                                {
                                    InternalNode _UserLogon = new InternalNode("Desktop");
                                    _UserLogon.AltText = _UserLogon.Text;
                                    _DesktopNode = new InternalNode("Desktop");
                                    _UserLogon.Nodes.Add(_DesktopNode);
                                    _DesktopNode.Parent = _UserLogon;
                                    ProcessNodes[uint.MaxValue] = _UserLogon;
                                }

                                _WindowNode = _DesktopNode;
                            }
                        }

                        if (_WindowNode != null)
                        {
                            try
                            {
                                InternalNode _uitnode = new InternalNode(_ActionString);
                                if(string.IsNullOrEmpty(_ZappyAction.ExeName))
                                    _ZappyAction.ExeName = _WindowNode.Parent.AltText;
                                _uitnode.Tag = _ZappyAction;
                                _WindowNode.Nodes.Add(_uitnode);

                            }
                            catch (Exception ex)
                            {
                                CrapyLogger.log.Error(ex);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        static void generateLastActivitesList(ZappyTaskAction uit, IntPtr
            _WindowHandle, IntPtr
            _elementHandle, string topLevelElementName)
        {
            _ActionString = uit.ActionName + " " + (uit as ZappyTaskAction).ValueAsString;


            if (!_WindowHandles.TryGetValue(_WindowHandle, out _WindowNode))
            {
                uint _Pid;
                NativeMethods.GetWindowThreadProcessId(_WindowHandle, out _Pid);
                string _ProcessName = null;

                if (!ProcessNodes.TryGetValue(_Pid, out _ProcessNode))
                {
                    if (_Pid != 0)
                    {
                        _ProcessName = NativeMethods.GetProcessFileName((int)_Pid);

                        _ProcessNode = new InternalNode(_ProcessName);

                        _ProcessNode.AltText = Path.GetFileName(_ProcessName);

                        ProcessNodes[_Pid] = _cachedProcessNode = _ProcessNode;
                    }
                    else if (_cachedProcessNode != null)
                    {
                        _ProcessNode = _cachedProcessNode;
                    }
                    else
                    {
                        if (_DesktopNode == null)
                            _DesktopNode = new InternalNode("Desktop");
                        _ProcessNode = _DesktopNode;
                    }
                }

                _ProcessName = Path.GetFileNameWithoutExtension(_ProcessNode.Text);

                _WindowNode = new InternalNode(topLevelElementName);

                _WindowHandles[_WindowHandle] = _WindowNode;
                _WindowNode.Parent = _ProcessNode;
                _ProcessNode.Nodes.Add(_WindowNode);

                if (CrapyWriter._ProcessInvocationMap.TryGetValue(_ProcessName + _WindowNode.Text,
                    out string DirPath))
                    ZappyInvoker.Invoke(DirPath, _WindowHandle, _elementHandle);
            }
        }

    }
}
