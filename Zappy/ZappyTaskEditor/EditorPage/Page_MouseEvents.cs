using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyTaskEditor.Nodes;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage
    {
        private Port.Port LinkNodeStartPort { get; set; }

        private Point LinkNodeEndPoint { get; set; }

        private Point MoveNodeStartPoint { get; set; }

        private Point MoveNodeOffsetPoint { get; set; }

        private Point SelectNodeStartPoint { get; set; }

        private Rectangle SelectNodeRect { get; set; }

        private Node StartPortNode;
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {

                                    var pagePanelScrollPosition = CanvasParentPanel.AutoScrollPosition;
            this.Canvas.Focus();
            StartPortNode = null;
            CanvasParentPanel.AutoScrollPosition = new Point(-pagePanelScrollPosition.X, -pagePanelScrollPosition.Y);

            if (State == PageState.Stopped)
            {
                                if (!Control.ModifierKeys.HasFlag(Keys.Control))
                    this.Canvas.MouseMove += this.SelectNodeFromPointCancel;
                this.Canvas.MouseUp += this.SelectNodeFromPointEnd;
                if (this.GetNodeFromPoint(e.Location) is Node node)
                {
                    
                    if (node.GetPortFromPoint(e.Location) is Port.Port port)
                    {
                                                if (node.CanStartLink)
                        {
                            this.Canvas.MouseMove += this.LinkNodeStart;
                            this.Canvas.MouseUp += this.LinkNodeCancel;
                            this.LinkNodeStartPort = port;
                            this.LinkNodeEndPoint = Point.Empty;
                            StartPortNode = node;
                        }
                    }
                    else
                    {
                                                if (!Control.ModifierKeys.HasFlag(Keys.Control))
                        {
                            this.Canvas.MouseMove += this.MoveNodeStart;
                            this.Canvas.MouseUp += this.MoveNodeCancel;
                            this.MoveNodeStartPoint = e.Location;
                            this.MoveNodeOffsetPoint = Point.Empty;
                        }
                    }
                }
                else
                {
                                                            this.Canvas.MouseMove += this.SelectNodesFromRectStart;
                    this.Canvas.MouseUp += this.SelectNodesFromRectCancel;
                    this.SelectNodeStartPoint = e.Location;
                    this.SelectNodeRect = Rectangle.Empty;
                }
            }
            else
            {
                if (this.GetNodeFromPoint(e.Location) is Node node)
                    ActiveNodeChanged?.Invoke(node);
            }
        }
        private Point LastMouseLocation;
        private void CanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            LastMouseLocation = e.Location;
        }
        private void Canvas_DoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.GetNodeFromPoint(e.Location) is Node node)
                {
                    if (node.Activity is RunZappyTask runZappyTask)
                    {
                        openRunZapyTaskAction(node, runZappyTask);
                    }
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }

        }

        private void openRunZapyTaskAction(Node node, RunZappyTask runZappyTask)
        {
            try
            {
                PageFormTabbed frmTabbed = ((System.Windows.Forms.ContainerControl)this.CanvasParentPanel.Parent.Parent).ParentForm as PageFormTabbed;
                if (node.Activity is RunZappyTaskFromFile runZappyTaskFromFile)
                {
                    if (!string.IsNullOrEmpty(runZappyTaskFromFile.LoadFromFilePath) &&
                        File.Exists(runZappyTaskFromFile.LoadFromFilePath))
                    {
                        frmTabbed.CreateNewTab(runZappyTaskFromFile.LoadFromFilePath);
                    }
                }
                else
                {
                    if (runZappyTask.NestedExecuteActivities == null)
                    {
                        runZappyTask.NestedExecuteActivities = new ActionList();
                        runZappyTask.NestedExecuteActivities.Add(new StartNodeAction());
                        runZappyTask.NestedExecuteActivities.Add(new EndNodeAction());
                    }
                    RunZappyTaskHelper runZappyTaskHelper = new RunZappyTaskHelper();
                    runZappyTaskHelper.runZappyTask = runZappyTask;
                    runZappyTaskHelper.ParentTaskEditorPage = this;
                                                            frmTabbed.CreateNewTab("", runZappyTaskHelper);
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        public event Action<Node> ActiveNodeChanged;

        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {
            if (State == PageState.Stopped)
            {
                var canvas = sender as Control;
                var location = canvas.PointToClient(new Point(e.X, e.Y));
                if (e.Data.GetData(typeof(TreeNode).FullName) is TreeNode treeNode)
                {

                    this.AddNode(treeNode.Name, location.X, location.Y);
                    this.Canvas.Invalidate();
                    
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    int _X = location.X, _Y = location.Y;

                    foreach (string file in files)
                    {
                                                PageFormTabbed frmTabbed = ((System.Windows.Forms.ContainerControl)this.CanvasParentPanel.Parent.Parent).ParentForm as PageFormTabbed;
                        frmTabbed.CreateNewTab(file);

                                                                                                                                            }
                                    }
            }
        }

        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (State == PageState.Stopped)
            {
                if (e.Data.GetData(typeof(TreeNode).FullName) is TreeNode treeNode)
                    e.Effect = DragDropEffects.Move;
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        #region Link Nodes

        private void LinkNodeCancel(object sender, MouseEventArgs e)
        {
            StartPortNode = null;
            this.Canvas.MouseMove -= this.LinkNodeStart;
            this.Canvas.MouseUp -= this.LinkNodeCancel;
        }

        private void LinkNodeStart(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.LinkNodeStart;
            this.Canvas.MouseUp -= this.LinkNodeCancel;
            this.Canvas.MouseMove += this.LinkingNode;
            this.Canvas.MouseUp += this.LinkNodeEnd;
        }

        private void LinkingNode(object sender, MouseEventArgs e)
        {
            this.LinkNodeEndPoint = e.Location;
            this.Canvas.Invalidate();
            ChangeAutoScrollPosition(e);
        }
        int _LastScrollChangeTime = 0;
        private void ChangeAutoScrollPosition(MouseEventArgs e)
        {

                                                                                                if (Environment.TickCount - _LastScrollChangeTime >= 500 && !CanvasParentPanel.ClientRectangle.Contains(CanvasParentPanel.PointToClient(Control.MousePosition)))
            {
                CanvasParentPanel.AutoScrollPosition = e.Location;                 _LastScrollChangeTime = Environment.TickCount;
                this.Canvas.Invalidate();
            }
        }

        private void LinkNodeEnd(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.LinkingNode;
            this.Canvas.MouseUp -= this.LinkNodeEnd;
            this.LinkNodeEndPoint = Point.Empty;
                        if (this.GetNodeFromPoint(e.Location) is Node node && node.CanEndLink && StartPortNode.Id != node.Id)
            {
                this.LinkNodeStartPort.NextNodeId = node.Id;
                if (StartPortNode != null)
                {
                    if (LinkNodeStartPort is TruePort)
                        (StartPortNode.Activity as DecisionNodeAction).TrueGuid = node.Activity.SelfGuid;
                    else if (LinkNodeStartPort is FalsePort)
                        (StartPortNode.Activity as DecisionNodeAction).FalseGuid = node.Activity.SelfGuid;
                    else if (LinkNodeStartPort is ErrorPort)
                        StartPortNode.Activity.ErrorHandlerGuid = node.Activity.SelfGuid;
                    else
                    {
                        StartPortNode.Activity.NextGuid = node.Activity.SelfGuid;
                                                LinkInputOutputProperties(StartPortNode.Activity, node.Activity);

                    }
                    
                }
            }
            else
            {
                this.LinkNodeStartPort.NextNodeId = Guid.Empty;
                                if (LinkNodeStartPort is TruePort)
                    (StartPortNode.Activity as DecisionNodeAction).TrueGuid = Guid.Empty;
                else if (LinkNodeStartPort is FalsePort)
                    (StartPortNode.Activity as DecisionNodeAction).FalseGuid = Guid.Empty;
                else if (LinkNodeStartPort is ErrorPort)
                    StartPortNode.Activity.ErrorHandlerGuid = Guid.Empty;
                else
                {
                    StartPortNode.Activity.NextGuid = Guid.Empty;
                }
            }

            IsDirty = true;
            StartPortNode = null;

            this.Canvas.Invalidate();
        }

        #endregion

        #region Move Nodes

        private void MoveNodeCancel(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.MoveNodeStart;
            this.Canvas.MouseUp -= this.MoveNodeCancel;
        }

        private void MoveNodeStart(object sender, MouseEventArgs e)
        {
            this.Canvas.Cursor = Cursors.SizeAll;
            this.Canvas.MouseMove -= this.MoveNodeStart;
            this.Canvas.MouseUp -= this.MoveNodeCancel;
            this.Canvas.MouseMove += this.MovingNode;
            this.Canvas.MouseUp += this.MoveNodeEnd;
                        var nodeId = this.GetNodeFromPoint(this.MoveNodeStartPoint);

            if ((nodeId.NodeState & NodeState.Selected) == NodeState.Selected)
            {
                ;
            }
            else
            {

                ApplyFlag(Nodes, NodeState.Selected, false);
                ApplyFlag(nodeId, NodeState.Selected, true);

            }
            this.Canvas.Invalidate();
        }

        private void MovingNode(object sender, MouseEventArgs e)
        {
            var offsetX = e.X - this.MoveNodeStartPoint.X;
            var offsetY = e.Y - this.MoveNodeStartPoint.Y;

            this.MoveNodeOffsetPoint = new Point(offsetX, offsetY);

            if (CanvasParentPanel.ClientRectangle.Contains(CanvasParentPanel.PointToClient(Control.MousePosition)))
            {
                this.Canvas.Invalidate();
            }
            ChangeAutoScrollPosition(e);
        }

        private void MoveNodeEnd(object sender, MouseEventArgs e)
        {
                                    this.Canvas.Cursor = Cursors.Default;
            this.Canvas.MouseMove -= this.MovingNode;
            this.Canvas.MouseUp -= this.MoveNodeEnd;
                        var offsetX = (int)Math.Round((double)(e.X - this.MoveNodeStartPoint.X) / PageRenderOptions.GridSize) * PageRenderOptions.GridSize;
            var offsetY = (int)Math.Round((double)(e.Y - this.MoveNodeStartPoint.Y) / PageRenderOptions.GridSize) * PageRenderOptions.GridSize;
            this.MoveNodeOffsetPoint = new Point(offsetX, offsetY);

                        
            foreach (var node in this.Nodes)
            {
                if ((node.NodeState & NodeState.Selected) != NodeState.Selected)
                    continue;
                var rect = node.Bounds;
                rect.X += this.MoveNodeOffsetPoint.X;
                rect.Y += this.MoveNodeOffsetPoint.Y;
                node.SetBounds(rect, this);
            }
            IsDirty = !MoveNodeOffsetPoint.IsEmpty;
            this.MoveNodeOffsetPoint = Point.Empty;
            this.Canvas.Invalidate();
        }

        bool _IsDirty = false;
        bool IsViaUndo = false;
        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty && !IsViaUndo)
                    UpdateUndoAction?.Invoke();
                                                _IsDirty = value;
                PageDirty?.Invoke();
                            }
        }

        public event Action PageDirty;

        public event Action UpdateUndoAction;
        #endregion

        #region Select Node From Point

        private void SelectNodeFromPointCancel(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.SelectNodeFromPointCancel;
            this.Canvas.MouseUp -= this.SelectNodeFromPointEnd;
        }

        private void SelectNodeFromPointEnd(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.SelectNodeFromPointCancel;
            this.Canvas.MouseUp -= this.SelectNodeFromPointEnd;
                        Node node = this.GetNodeFromPoint(e.Location);

            var ctrl = Control.ModifierKeys.HasFlag(Keys.Control);

            if (node == null)
            {
                if (ctrl)
                {
                    ;
                }
                else
                {
                    ApplyFlag(Nodes, NodeState.Selected, false);

                }

            }
            else
            {
                if (ctrl)
                    ApplyFlag(node, NodeState.Selected, (node.NodeState & NodeState.Selected) == 0);
                else
                {
                    ApplyFlag(Nodes, NodeState.Selected, false);
                    ApplyFlag(node, NodeState.Selected, true);
                }
            }
                                                                                                                        
                                                                                                                                                
                                    
                                    this.Canvas.Invalidate();

        }

        #endregion

        #region Select Nodes From Rect

        private void SelectNodesFromRectCancel(object sender, MouseEventArgs e)
        {
            this.Canvas.MouseMove -= this.SelectNodesFromRectStart;
            this.Canvas.MouseUp -= this.SelectNodesFromRectCancel;
        }

        private void SelectNodesFromRectStart(object sender, MouseEventArgs e)
        {
            this.Canvas.Cursor = Cursors.Cross;
            this.Canvas.MouseMove -= this.SelectNodesFromRectStart;
            this.Canvas.MouseUp -= this.SelectNodesFromRectCancel;
            this.Canvas.MouseMove += this.SelectingNodesFromRect;
            this.Canvas.MouseUp += this.SelectNodesFromRectEnd;
                        this.SelectNodeStartPoint = e.Location;
            this.SelectNodeRect = Rectangle.Empty;
            this.Canvas.Invalidate();
        }

        private void SelectingNodesFromRect(object sender, MouseEventArgs e)
        {
            var x = Math.Min(this.SelectNodeStartPoint.X, e.X);
            var y = Math.Min(this.SelectNodeStartPoint.Y, e.Y);
            var w = Math.Abs(this.SelectNodeStartPoint.X - e.X);
            var h = Math.Abs(this.SelectNodeStartPoint.Y - e.Y);
            this.SelectNodeRect = new Rectangle(x, y, w, h);
            this.Canvas.Invalidate();
        }

        private void SelectNodesFromRectEnd(object sender, MouseEventArgs e)
        {
            this.Canvas.Cursor = Cursors.Default;
            this.Canvas.MouseMove -= this.SelectingNodesFromRect;
            this.Canvas.MouseUp -= this.SelectNodesFromRectEnd;
                        if (!Control.ModifierKeys.HasFlag(Keys.Control))
            {
                                ApplyFlag(Nodes, NodeState.Selected, false);
            }
            foreach (var node in this.Nodes)
            {
                if ((node.NodeState & NodeState.Selected) == 0 &&
                     this.SelectNodeRect.IntersectsWith(node.Bounds))
                {

                    ApplyFlag(node, NodeState.Selected, true);

                }
            }
            this.SelectNodeRect = Rectangle.Empty;
            this.Canvas.Invalidate();
        }

        #endregion

        void ApplyFlag(IEnumerable<Node> Nodes, NodeState State, bool Add)
        {
            if (Add)
            {
                foreach (Node item in Nodes)
                {
                    item.NodeState = State;
                }
            }
            else
            {
                State = ~State;
                foreach (Node item in Nodes)
                {
                    item.NodeState &= State;
                }
            }
        }

        void ApplyFlag(Node item, NodeState State, bool Add)
        {
            if (Add)
            {
                item.NodeState |= State;
            }
            else
            {
                State = ~State;
                item.NodeState &= State;
            }
        }

        void SetState(IEnumerable<Node> Nodes, NodeState State)
        {
            foreach (Node item in Nodes)
                item.NodeState = State;
        }
        void SetState(Node item, NodeState State)
        {
            item.NodeState = State;
        }

    }

}