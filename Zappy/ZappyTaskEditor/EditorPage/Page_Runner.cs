using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyTaskEditor.Nodes;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage
    {
        PageState _State;
        public PageState State
        {
            get { return _State; }
            set
            {
                if (value != _State)
                {
                    _State = value;
                                                                                                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnStateChanged;

        private void Initialize_Runner()
        {
            this.State = PageState.Stopped;
        }

        public string ZappytaskFullFileName { get; set; }

        public bool _AutoStep = false;
        public void Run()
        {
            _AutoStep = true;
            RunDebug(true);
        }

        public void RunDebug()
        {
            _AutoStep = false;
            RunDebug(true);
        }
        public void RunWithoutDebug()
        {
            _AutoStep = true;
            RunDebug(false);
        }

        internal void ContinueDebug()
        {
            _AutoStep = true;
            Step();
        }

        public void RunDebug(bool AttachDebugger)
        {
            if (!Validate())
            {
                MessageBox.Show("Invalid Connection");
                return;
            }

            if (_PageZappyTask == null)
            {
                if (_runZappyTaskHelper != null)
                {
                    ClearDubugRelatedFlags();
                    SaveZappyTask(true);
                    _runZappyTaskHelper.ParentTaskEditorPage.RunDebug(AttachDebugger);
                }
                else
                {
                    MessageBox.Show("Part of Run Zappy Task: To run -> execute parent task");
                }
                return;
            }

            _PageZappyTask.ExecuteActivities.Activities.Clear();

            bool _HasTrigger = false;
            for (int i = 0; i < Nodes.Count; i++)
            {
                _PageZappyTask.ExecuteActivities.Activities.Add(Nodes[i].Activity);
                _HasTrigger |= Nodes[i].Activity is IZappyTrigger;
            }

            try
            {
                if (_HasTrigger)
                {
                    AttachDebugger = false;
                }
                CommonProgram.StartPlaybackFromFile(ZappytaskFullFileName, AttachDebugger);
                Canvas.Invalidate();
                ClearDubugRelatedFlags();
                if (!_HasTrigger)
                    State = PageState.Running;
                                            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }


        public void SaveZappyTask(bool useSameFileIfAvailable)
        {
            
                                                                        PageFormTabbed frmTabbed = ((System.Windows.Forms.ContainerControl)this.CanvasParentPanel.Parent.Parent).ParentForm as PageFormTabbed;

            if (_PageZappyTask == null)
            {
                _runZappyTaskHelper.runZappyTask.NestedExecuteActivities.Activities.Clear();
                for (int i = 0; i < Nodes.Count; i++)
                {
                                                                                                                                                                                    _runZappyTaskHelper.runZappyTask.NestedExecuteActivities.Activities.Add(Nodes[i].Activity);
                }
                
                if (frmTabbed != null)
                    frmTabbed.RenameZappyTaskHelperTabs();
                _runZappyTaskHelper.ParentTaskEditorPage.SaveZappyTask(true);
                IsDirty = false;
            }
            else
            {
                _PageZappyTask.ExecuteActivities.Activities.Clear();
                for (int i = 0; i < Nodes.Count; i++)
                {
                                                            _PageZappyTask.ExecuteActivities.Activities.Add(Nodes[i].Activity);
                }

                if (_PageZappyTask != null && _PageZappyTask.ExecuteActivities.Count > 0)
                {
                    if (useSameFileIfAvailable && !string.IsNullOrEmpty(ZappytaskFullFileName))
                    {
                                                var bytes = File.ReadAllBytes(ZappytaskFullFileName);
                        try
                        {
                            using (Stream fs = File.Open(ZappytaskFullFileName, FileMode.Create))
                            {
                                _PageZappyTask.Save(fs, ZappytaskFullFileName, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                File.WriteAllBytes(ZappytaskFullFileName, bytes);
                            }
                            catch
                            {

                            }
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                        {
                            saveFileDialog1.Filter = "Zappy Files|*.zappy";
                            saveFileDialog1.FilterIndex = 2;
                            saveFileDialog1.RestoreDirectory = true;
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                using (Stream myStream = saveFileDialog1.OpenFile())
                                    _PageZappyTask.Save(myStream, saveFileDialog1.FileName, false);
                                ZappytaskFullFileName = saveFileDialog1.FileName;
                            }
                        }
                        if (frmTabbed != null)
                        {
                            try
                            {
                                                                frmTabbed.tabControl1.SelectedTab.Text =
                                    Path.GetFileNameWithoutExtension(ZappytaskFullFileName);
                                frmTabbed.tabControl1.SelectedTab.Name = ZappytaskFullFileName;
                                frmTabbed.fileNameTabPageDict[ZappytaskFullFileName] = frmTabbed.tabControl1.SelectedTab;
                            }
                            catch
                            {
                                                            }
                        }
                    }
                    IsDirty = false;
                }
                                                else
                    MessageBox.Show("No Action to Export!!");
            }
        }

        private int iterationCountForStackOverFlow = 0;
        public bool Validate()
        {
            List<StartNode> _StartNodes = new List<StartNode>();
            Dictionary<Guid, Node> _NodeDict = new Dictionary<Guid, Node>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                _NodeDict[Nodes[i].Id] = Nodes[i];
                if (Nodes[i] is StartNode)
                    _StartNodes.Add(Nodes[i] as StartNode);
            }

            for (int i = 0; i < _StartNodes.Count; i++)
            {
                StartNode _StartNode = _StartNodes[i];
                iterationCountForStackOverFlow = 0;
                                if (!TraverseTree(_StartNode, _NodeDict))
                    return false;
            }
            return true;
        }

        bool TraverseTree(Node StartNode, Dictionary<Guid, Node> NodeDict)
        {
            iterationCountForStackOverFlow++;
                        if (iterationCountForStackOverFlow >= 5000)
                return false;

            bool valid = false;
            if (StartNode is EndNode)
                valid = true;
            if (StartNode is DecisionNode)
            {
                                return true;
            }
            for (int i = 0; i < StartNode.Ports.Count; i++)
            {
                if (StartNode.Ports[i].NextNodeId == StartNode.Id)
                    return false;
                if (NodeDict.TryGetValue(StartNode.Ports[i].NextNodeId, out Node NextNode))
                    valid = TraverseTree(NextNode, NodeDict);
                else if ((StartNode.Ports[i] is Port.ErrorPort) && StartNode.Ports[i].NextNodeId == Guid.Empty)
                    valid = true;
                else
                    valid = false;
            }

            return valid;
        }

        public void Step()
        {
            try
            {
                PubSubService.Instance.Publish(PubSubTopicRegister.DebugNextStep, "{Next}");
            }
            catch
            {
                State = PageState.Stopped;
            }
        }


                                                                                                                
                        
                                                
        public void Stop(bool buttonClicked = false)
        {
            if (State == PageState.Running)
            {
                try
                {
                    if (buttonClicked)
                        PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.CancelZappyExecutionMessage);
                }
                catch
                {

                }
                finally
                {
                    State = PageState.Stopped;
                }
            }
        }

        public bool ExecutionProgress(Guid Current, string Result, Guid Next)
        {
            bool nodeFound = false;
            CrapyLogger.log.InfoFormat("{0},{1},{2}", Current, Result, Next);

            if (Result == "ContextLoaded")
                return nodeFound;

            bool _Invalidate = false;

            Node node = null;
            bool isFailedExecution = false;

            if (Current != Guid.Empty)
            {

                node = GetNodeById(Current);
                                
                if (node != null)                 {
                    if (State != PageState.Running)
                        State = PageState.Running;
                    ApplyFlag(node, NodeState.Debug, false);
                    if (!string.IsNullOrEmpty(Result) && Result.StartsWith("Success"))
                        ApplyFlag(node, NodeState.ExecutionPassed, true);
                    else
                    {
                        ApplyFlag(node, NodeState.ExecutionFailed, true);
                        node.ExecutionResult = Result;
                        isFailedExecution = true;
                    }
                    if (!CanvasParentPanel.ClientRectangle.Contains(CanvasParentPanel.PointToClient(node.Bounds.Center)))
                    {
                        CanvasParentPanel.AutoScrollPosition = node.Bounds.Center;
                    }
                    _Invalidate = true;
                    if (node.Activity is RunZappyTask runZappyTask)
                    {
                        PageFormTabbed frmTabbed = ((System.Windows.Forms.ContainerControl)this.CanvasParentPanel.Parent.Parent).ParentForm as PageFormTabbed;
                                                if (frmTabbed != null && frmTabbed.tabControl1.TabPages.Count < 5)
                            openRunZapyTaskAction(node, runZappyTask);
                        if (isFailedExecution)
                            openRunZapyTaskAction(node, runZappyTask);
                    }
                    else
                    {
                        nodeFound = true;
                    }
                }
            }

            if (Next != Guid.Empty)
            {
                node = GetNodeById(Next);

                if (node != null)
                {
                    if (_AutoStep && (node.NodeState & NodeState.BreakPoint) == 0)
                        Step();
                    ApplyFlag(node, NodeState.Debug, true);
                    if (node.Activity is EndNodeAction)
                    {
                        Step();
                        State = PageState.Stopped;
                        ApplyFlag(Nodes, NodeState.Selected, false);
                                                                                            }
                    _Invalidate = true;
                }

            }
            else
            {
                if (Result.StartsWith("Fail"))
                {
                    SetState(node, NodeState.ExecutionFailed);
                    Stop();
                }
                                State = PageState.Stopped;
                _Invalidate = true;
                                                                                            }

            if (_Invalidate)
                Canvas.Invalidate();
            return nodeFound;
        }

        public void ActionExecutionTrace(IZappyAction Current, string Result, Guid Next)
        {

        }

        void ClearDubugRelatedFlags()
        {
            ApplyFlag(Nodes, NodeState.Debug, false);
            ApplyFlag(Nodes, NodeState.ExecutionFailed, false);
            ApplyFlag(Nodes, NodeState.ExecutionPassed, false);
            this.Canvas.Invalidate();
        }
    }
}
