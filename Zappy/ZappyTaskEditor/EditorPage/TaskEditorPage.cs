
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyActions.Loops;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage
    {
        internal LinkedList<List<IZappyAction>> undoLinkedList = new LinkedList<List<IZappyAction>>();
        internal LinkedList<List<IZappyAction>> redoLinkedList = new LinkedList<List<IZappyAction>>();

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    this.DeleteNode();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
                {
                    this.SelectAllNodes();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
                {
                    this.PerformUndo();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
                {
                    this.PerformRedo();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                {
                    this.Copy();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.X)
                {
                    this.Cut();
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                {
                    this.Paste();
                }
                                                
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        internal void PerformUndo()
        {
            if (undoLinkedList.Count > 0)
            {
                IsViaUndo = true;
                this.Nodes = new List<Node>();
                CreateNodesAndConnections(undoLinkedList.First.Value, true);
                redoLinkedList.AddFirst(undoLinkedList.First.Value);
                undoLinkedList.RemoveFirst();
                this.Canvas.Invalidate();
                IsViaUndo = false;
            }
        }

        internal void PerformRedo()
        {
            if (redoLinkedList.Count > 0)
            {
                this.Nodes = new List<Node>();
                CreateNodesAndConnections(redoLinkedList.First.Value, true);
                redoLinkedList.RemoveFirst();
                this.Canvas.Invalidate();
            }
        }

        internal void Cut()
        {
            Copy();
                        DeleteNode();
        }

        internal void zoomIn(int offset)
        {
            if (PageRenderOptions.GridSize >= 22 && offset > 0)
            {
                return;
            }
            PageRenderOptions.GridSize = PageRenderOptions.GridSize + offset;
            List<IZappyAction> ActionList = new List<IZappyAction>();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                ActionList.Add(this.Nodes[i].Activity);
                                                this.Nodes[i].Activity.EditorLocationX
                    = this.Nodes[i].Activity.EditorLocationX + offset + 8;                this.Nodes[i].Activity.EditorLocationY
                    = this.Nodes[i].Activity.EditorLocationY + offset + 8;                             }
            this.Nodes = new List<Node>();
            CreateNodesAndConnections(ActionList, true);
            this.Canvas.Invalidate();
        }

        internal void zoomOut(int offset)
        {
            if (PageRenderOptions.GridSize <= 8 && offset < 0)
            {
                return;
            }
            PageRenderOptions.GridSize = PageRenderOptions.GridSize + offset;
            List<IZappyAction> ActionList = new List<IZappyAction>();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                ActionList.Add(this.Nodes[i].Activity);
                                                this.Nodes[i].Activity.EditorLocationX
                    = this.Nodes[i].Activity.EditorLocationX + offset - 4;                this.Nodes[i].Activity.EditorLocationY
                    = this.Nodes[i].Activity.EditorLocationY + offset - 4;                             }
            this.Nodes = new List<Node>();
            CreateNodesAndConnections(ActionList, true);
            this.Canvas.Invalidate();
        }

        public static List<Node> CopiedNodes = new List<Node>();

        internal void Copy()
        {
            try
            {

                CopiedNodes.Clear();
                List<Node> _SelectedNodes = null;
                foreach (var selectedNode in this.Nodes)
                {
                    if ((selectedNode.NodeState & NodeState.Selected) == 0 || selectedNode.Activity is StartNodeAction)
                        continue;
                    if (_SelectedNodes == null) _SelectedNodes = new List<Node>();
                    _SelectedNodes.Add(selectedNode);
                }
                if (_SelectedNodes != null)
                {
                    CopiedNodes.AddRange(_SelectedNodes);
                                                                                                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }
        internal void Paste()
        {
            try
            {
                List<Node> _PastedNodes = CopiedNodes;
                if (_PastedNodes != null && _PastedNodes.Count > 0)
                {
                    ApplyFlag(Nodes, NodeState.Selected, false);
                    int _PosX = 0, _PosY = 0;
                    
                    _PosX = LastMouseLocation.X;
                    _PosY = LastMouseLocation.Y;

                                                                                                    

                    if (_PastedNodes.Count > 1)
                    {

                        List<IZappyAction> actions = new List<IZappyAction>(_PastedNodes.Count);
                        for (int i = 0; i < _PastedNodes.Count; i++)
                            actions.Add(_PastedNodes[i].Activity);


                        List<IZappyAction> clonedActionsWithUpdatedLink = CloneActionsWithUpdatedLink(actions);
                                                                                                                        CreateNodesAndConnections(clonedActionsWithUpdatedLink, false, _PosX, _PosY,  true);
                        
                                            }
                    else
                    {
                        IZappyAction za = _PastedNodes[0].Activity.DeepCloneAction();
                        za.SelfGuid = Guid.NewGuid();
                        if (za is RunZappyTask zappyTask)
                        {
                            UpdateRunZappyTaskGuid(zappyTask);
                        }
                        Node nd = AddNode(za, _PosX, _PosY, true);
                        this.Canvas.Invalidate();
                        ApplyFlag(nd, NodeState.Selected, true);
                                            }
                                    }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        internal void ExecuteSelectedActions()
        {
            try
            {
                List<Node> _SelectedNodes = new List<Node>();
                foreach (var selectedNode in this.Nodes)
                {
                    if(selectedNode.Activity is IVariableAction)
                        _SelectedNodes.Add(selectedNode);
                    else if (!((selectedNode.NodeState & NodeState.Selected) == 0 || selectedNode.Activity is StartNodeAction))
                        _SelectedNodes.Add(selectedNode);
                }
                if (_SelectedNodes.Count > 0)
                {
                    int startnodeid = -1, endnodeid = -1;
                                        for (int i = 0; i < _SelectedNodes.Count; i++)
                    {
                        bool validstartnode = true, validendnode = true;
                        if (!(_SelectedNodes[i].Activity is IVariableAction))
                        {
                            for (int j = 0; j < _SelectedNodes.Count; j++)
                            {
                                if (_SelectedNodes[i].Activity.SelfGuid.Equals(_SelectedNodes[j].Activity.NextGuid))
                                {
                                    validstartnode = false;
                                }
                                if (_SelectedNodes[i].Activity.NextGuid.Equals(_SelectedNodes[j].Activity.SelfGuid))
                                {
                                    validendnode = false;
                                }
                            }

                            if (validstartnode)
                            {
                                if (startnodeid == -1)
                                    startnodeid = i;
                                else
                                {
                                    MessageBox.Show("Multiple Start Nodes");
                                    return;
                                }
                            }
                            if (validendnode)
                            {
                                if (endnodeid == -1)
                                    endnodeid = i;
                                else
                                {
                                    MessageBox.Show("Multiple End Nodes");
                                    return;
                                }
                            }
                        }
                    }
                                        IZappyAction startZappyAction = new StartNodeAction();
                    startZappyAction.NextGuid = _SelectedNodes[startnodeid].Activity.SelfGuid;
                    IZappyAction endZappyAction = new EndNodeAction();
                                        Guid resetGuid = _SelectedNodes[endnodeid].Activity.NextGuid;
                    _SelectedNodes[endnodeid].Activity.NextGuid = endZappyAction.SelfGuid;
                                                            ZappyTask runselectedAcivities = new ZappyTask();
                    for (int i = 0; i < _SelectedNodes.Count; i++)
                    {
                                                                        runselectedAcivities.ExecuteActivities.Activities.Add(_SelectedNodes[i].Activity);
                    }
                    runselectedAcivities.ExecuteActivities.Activities.Insert(0, startZappyAction);
                    runselectedAcivities.ExecuteActivities.Activities.Add(endZappyAction);

                    string ZappyFile = CommonProgram.GetZappyTempFile();
                                        runselectedAcivities.SaveUnencrypted(ZappyFile);

                                        _SelectedNodes[endnodeid].Activity.NextGuid = resetGuid;
                                        CommonProgram.StartPlaybackFromFile(ZappyFile);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable To Execue selected actions");
                CrapyLogger.log.Error(ex);
            }
        }

        private void UpdateRunZappyTaskGuid(RunZappyTask zappyTask)
        {
            List<IZappyAction> tempActivityList = CloneActionsWithUpdatedLink(zappyTask.NestedExecuteActivities.Activities);
            zappyTask.NestedExecuteActivities = new ActionList();
            zappyTask.NestedExecuteActivities.AddRange(tempActivityList);
        }

        private List<IZappyAction> CloneActionsWithUpdatedLink(List<IZappyAction> _PastedActivities)
        {
            List<IZappyAction> _PastedActivityCopyList = new List<IZappyAction>(_PastedActivities.Count);
                        Dictionary<Guid, Guid> OldToNewGuidMapping = new Dictionary<Guid, Guid>();
            
            foreach (IZappyAction nd in _PastedActivities)
            {
                IZappyAction newNodeAction = CommonProgram.DeepClone<IZappyAction>(nd);
                _PastedActivityCopyList.Add(newNodeAction);
                newNodeAction.SelfGuid = Guid.NewGuid();
                                OldToNewGuidMapping[nd.SelfGuid] = newNodeAction.SelfGuid;
                                if (nd is RunZappyTask zappyTask)
                {
                    UpdateRunZappyTaskGuid(zappyTask);
                }
            }

                                                
            foreach (IZappyAction ndcopy in _PastedActivityCopyList)
            {
                Guid _tempguid;

                if (!OldToNewGuidMapping.TryGetValue(ndcopy.NextGuid, out _tempguid))
                    ndcopy.NextGuid = Guid.Empty;
                else
                    ndcopy.NextGuid = _tempguid;

                if (ndcopy is DecisionNodeAction)
                {
                    DecisionNodeAction decisionCopyActivity = ndcopy as DecisionNodeAction;

                    if (!OldToNewGuidMapping.TryGetValue(decisionCopyActivity.TrueGuid, out _tempguid))
                        decisionCopyActivity.TrueGuid = Guid.Empty;
                    else
                        decisionCopyActivity.TrueGuid = _tempguid;

                    if (!OldToNewGuidMapping.TryGetValue(decisionCopyActivity.FalseGuid, out _tempguid))
                        decisionCopyActivity.FalseGuid = Guid.Empty;
                    else
                        decisionCopyActivity.FalseGuid = _tempguid;

                                                                                                }
                else if (ndcopy is IZappyLoopStartAction)
                {
                    IZappyLoopStartAction copyActivity = ndcopy as IZappyLoopStartAction;
                    if (!OldToNewGuidMapping.TryGetValue(copyActivity.LoopEndGuid, out _tempguid))
                        copyActivity.LoopEndGuid = Guid.Empty;
                    else
                        copyActivity.LoopEndGuid = _tempguid;
                                                        }
                else if (ndcopy is IZappyLoopEndAction)
                {
                    IZappyLoopEndAction copyActivity = ndcopy as IZappyLoopEndAction;
                    if (!OldToNewGuidMapping.TryGetValue(copyActivity.LoopStartGuid, out _tempguid))
                        copyActivity.LoopStartGuid = Guid.Empty;
                    else
                        copyActivity.LoopStartGuid = _tempguid;
                                                        }

                Type type = typeof(IDynamicProperty);
                PropertyInfo[] properties = ndcopy.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (type.IsAssignableFrom(property.PropertyType))
                    {

                        IDynamicProperty dp = property.GetValue(ndcopy) as IDynamicProperty;
                        if (dp != null && dp.DymanicKeySpecified)
                        {
                            string oldGuidString = ZappyExecutionContext.GetGuid(dp.DymanicKey);
                            Guid oldGuid;
                            if (Guid.TryParse(oldGuidString, out oldGuid))
                                if (OldToNewGuidMapping.ContainsKey(oldGuid))
                                    dp.DymanicKey = dp.DymanicKey.Replace(oldGuidString,
                                        OldToNewGuidMapping[oldGuid].ToString());
                        }
                    }
                }

            }
                        return _PastedActivityCopyList;
        }

        internal void DeleteNode()
        {
            List<Node> _RemovalNodes = null;
            foreach (var selectedNode in this.Nodes)
            {
                if ((selectedNode.NodeState & NodeState.Selected) == 0)
                    continue;
                if (_RemovalNodes == null) _RemovalNodes = new List<Node>();
                _RemovalNodes.Add(selectedNode);
            }
            if (_RemovalNodes != null)
            {
                for (int i = 0; i < _RemovalNodes.Count; i++)
                    RemoveNode(_RemovalNodes[i]);
                ApplyFlag(Nodes, NodeState.Selected, false);
                this.Canvas.Invalidate();
            }
        }

        internal void SetBreakpoint()
        {
            Node _Node = null;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if ((Nodes[i].NodeState & NodeState.Selected) == NodeState.Selected)
                {
                    _Node = Nodes[i];
                    ApplyFlag(Nodes[i], NodeState.BreakPoint,
                        (Nodes[i].NodeState & NodeState.BreakPoint) != NodeState.BreakPoint);
                    break;
                }
            }

            if (_Node != null)
                Canvas.Invalidate();         }

        internal void SelectAllNodes()
        {
            ApplyFlag(Nodes, NodeState.Selected, true);
            this.Canvas.Invalidate();
        }
    }
}