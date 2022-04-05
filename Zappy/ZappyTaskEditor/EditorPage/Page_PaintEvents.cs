using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Nodes;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage
    {
        private Panel Canvas { get; set; }
        private Panel CanvasParentPanel { get; set; }


        public HashSet<Node> SelectedNodes { get; private set; }

        private Dictionary<Node, GraphicsPath> RenderedNodes { get; set; }

        internal Node GetNodeFromPoint(Point pt)
        {
            KeyValuePair<Node, GraphicsPath> first = new KeyValuePair<Node, GraphicsPath>();
            foreach (var x in this.RenderedNodes)
            {
                if (x.Value.IsVisible(pt.X, pt.Y))
                {
                    first = x;
                    break;
                }
            }

            if (first is KeyValuePair<Node, GraphicsPath> item)
            {
                return item.Key;
            }
            return null;
        }

        internal Node GetNodeForBounds(Point pt, Guid id)
        {
            foreach (var x in this.RenderedNodes)
            {
                if (x.Value.IsVisible(pt.X, pt.Y))
                {
                    if (x is KeyValuePair<Node, GraphicsPath> item)
                    {
                                                if (item.Key.Id != id)
                            return item.Key;
                    }
                }
            }
            return null;
        }

        public void Show(Panel parent)
        {
            parent.Controls.Clear();
            this.Canvas.Parent = parent;
            this.Canvas.Invalidate();
            CanvasParentPanel = parent;
        }

        private void Initialize_Events()
        {
            this.Canvas = new Panel();
            this.Canvas.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.Canvas, true);
            this.Canvas.Paint += Canvas_Paint;
            this.Canvas.MouseDown += Canvas_MouseDown;
            this.Canvas.MouseDoubleClick += Canvas_DoubleClick;
            this.Canvas.KeyDown += Canvas_KeyDown;
            this.Canvas.AllowDrop = true;
            this.Canvas.DragEnter += Canvas_DragEnter;
            this.Canvas.DragDrop += Canvas_DragDrop;
            this.Canvas.MouseMove += CanvasOnMouseMove;
            this.SelectedNodes = new HashSet<Node>();
            this.RenderedNodes = new Dictionary<Node, GraphicsPath>();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            this.RenderBackground(g, (sender as Control).ClientRectangle);

            this.RenderNodes(g);
            this.RenderLines(g);
            this.RenderPorts(g);

            if (this.SelectNodeRect != Rectangle.Empty)
            {
                e.Graphics.FillRectangle(PageRenderOptions.SelectionBackBrush, this.SelectNodeRect);
            }
        }

        private void RenderBackground(Graphics g, Rectangle r)
        {
            g.Clear(PageRenderOptions.BackColor);
            for (var y = 0; y < r.Height; y += PageRenderOptions.GridSize)
            {
                g.DrawLine(PageRenderOptions.GridPen, 0, y, r.Width, y);
            }
            for (var x = 0; x < r.Width; x += PageRenderOptions.GridSize)
            {
                g.DrawLine(PageRenderOptions.GridPen, x, 0, x, r.Height);
            }
        }

        private void RenderPorts(Graphics g)
        {
            if (this.MoveNodeOffsetPoint == Point.Empty)
            {
                NodeStyle o = NodeStyles.General;
                foreach (var node in this.Nodes)
                {
                    if ((node.NodeState & NodeState.Selected) != NodeState.Selected)
                        continue;

                    node.RenderedPorts.Clear();
                    var nodePath = this.RenderedNodes[node];
                    foreach (var port in node.Ports)
                    {
                        var portPath = port.Render(g, node.Bounds, o);
                        node.RenderedPorts.Add(port, portPath);
                        nodePath.FillMode = FillMode.Winding;
                        nodePath.AddPath(portPath, false);
                    }
                }
            }
        }

        private void RenderLines(Graphics g)
        {
            NodeStyle o = NodeStyles.General;
            var f = new PathFinder(this.Nodes);

            foreach (var node in this.Nodes)
            {
                foreach (var port in node.Ports)
                {
                    o.LinePenWithArrow.Brush = port.GetBackBrush();

                    if (port is ErrorPort)
                        o.LinePenWithArrow.DashStyle = DashStyle.Dash;
                    else
                        o.LinePenWithArrow.DashStyle = DashStyle.Solid;


                    if (this.LinkNodeEndPoint != Point.Empty && this.LinkNodeStartPort == port)
                    {
                        g.DrawLine(o.LinePenWithArrow, port.Bounds.Center.X, port.Bounds.Center.Y, this.LinkNodeEndPoint.X, this.LinkNodeEndPoint.Y);
                    }
                    else if (this.GetNodeById(port.NextNodeId) is Node linkedNode)
                    {
                        g.DrawPath(o.LinePenWithArrow, f.GetPath(port.Bounds.Center, linkedNode.Bounds.CenterTop));
                    }
                    o.LinePenWithArrow.DashStyle = DashStyle.Solid;

                }
            }
        }

        private void RenderNodes(Graphics g)
        {
            var width = this.Canvas.Parent.ClientSize.Width;
            var height = this.Canvas.Parent.ClientSize.Height;
            bool _SelectedNodesChanged = false;
            this.RenderedNodes.Clear();
            foreach (var node in this.Nodes)
            {
                var r = node.Bounds;
                NodeStyle o = NodeStyles.General;

                if ((node.NodeState & NodeState.Debug) > 0)
                { o = NodeStyles.Debug; }
                else if ((node.NodeState & NodeState.BreakPoint) > 0)
                {
                    o = NodeStyles.BreakPoint; r.X += this.MoveNodeOffsetPoint.X;
                    r.Y += this.MoveNodeOffsetPoint.Y;
                }
                else if ((node.NodeState & NodeState.ExecutionPassed) > 0)
                    o = NodeStyles.ExecutionPassed;
                else if ((node.NodeState & NodeState.ExecutionFailed) > 0)
                    o = NodeStyles.ExecutionFailed;
                else if ((node.NodeState & NodeState.Selected) > 0)                 {
                    o = NodeStyles.SelectedNode;
                    r.X += this.MoveNodeOffsetPoint.X;
                    r.Y += this.MoveNodeOffsetPoint.Y;
                }
                else
                    o = NodeStyles.General;

                if ((node.NodeState & NodeState.Selected) > 0)
                    _SelectedNodesChanged |= SelectedNodes.Add(node);
                else
                    _SelectedNodesChanged |= SelectedNodes.Remove(node);

                                                                
                this.RenderedNodes[node] = node.Render(g, r, o);
                node.RenderText(g, r, o);

                                width = Math.Max(width, r.Right + PageRenderOptions.GridSize * 5);
                height = Math.Max(height, r.Bottom + PageRenderOptions.GridSize * 5);
            }

            this.Canvas.Size = new Size(width, height);

            if (_SelectedNodesChanged)
            {
                if (SelectedNodes.Count == 1)
                    ActiveNodeChanged.Invoke(SelectedNodes.ElementAt(0));
                else
                    ActiveNodeChanged.Invoke(null);
            }
        }
    }
}