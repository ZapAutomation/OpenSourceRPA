using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml.Serialization;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public abstract class Node : INode
    {
        public NodeState NodeState { get; set; }

        [XmlAttribute] public Guid Id { get { return Activity.SelfGuid; } }

        [XmlAttribute] public string Name { get; protected set; }
        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                this.Activity.EditorLocationX = bounds.Center.X;
                this.Activity.EditorLocationY = bounds.Center.Y;
            }
        }

        public IZappyAction Activity { get; set; }

        public List<Port.Port> Ports { get; set; }

        public string ExecutionResult { get; set; }

        protected Node()
        {
            this.Ports = new List<Port.Port>();
            this.RenderedPorts = new Dictionary<Port.Port, GraphicsPath>();
        }

        public Node(IZappyAction activity) : this()
        {
            this.Activity = activity;
            string _NodeText = String.Empty;
                        _NodeText = CommonProgram.HumanizeNameForGivenType(activity.GetType());
            this.Name = _NodeText;
            this.Bounds = new Rectangle(
                PageRenderOptions.GridSize * 0,
                PageRenderOptions.GridSize * 0,
                PageRenderOptions.GridSize * EditorConstants.NodeWidth,
                Convert.ToInt32(PageRenderOptions.GridSize * EditorConstants.NodeHeight));
        }

        internal Dictionary<Port.Port, GraphicsPath> RenderedPorts { get; set; }

        public virtual bool CanStartLink => true;

        public virtual bool CanEndLink => true;

        internal void SetBounds(Rect rect, TaskEditorPage page)
        {
                                                Node n = page.GetNodeForBounds(rect.Center, this.Id);
            while (n != null)             {
                rect.X += PageRenderOptions.GridSize;
                rect.Y += PageRenderOptions.GridSize;
                n = page.GetNodeForBounds(rect.Center, this.Id);
            }
            rect.X = Math.Max(0, (rect.X / PageRenderOptions.GridSize) * PageRenderOptions.GridSize);
            rect.Y = Math.Max(0, (rect.Y / PageRenderOptions.GridSize) * PageRenderOptions.GridSize);
            foreach (var port in this.Ports)
            {
                port.UpdateBounds(rect);
            }

            this.Bounds = rect;
        }

        public Port.Port GetPortById(Guid id)
        {
            return this.Ports.FirstOrDefault(x => x.Id == id);
        }

        public Port.Port GetPortFromPoint(Point pt)
        {
            if (this.RenderedPorts.FirstOrDefault(x => x.Value.IsVisible(pt.X, pt.Y)) is
                KeyValuePair<Port.Port, GraphicsPath> item)
            {
                return item.Key;
            }

            return null;
        }

        public abstract GraphicsPath Render(Graphics g, Rect r, NodeStyle o);

        public virtual void RenderText(Graphics g, Rect r, NodeStyle o)
        {
            if (this is GlobalVariableNode)
            {
                Rectangle rect = r;
                rect.Inflate(-Convert.ToInt32(r.Width * 0.2), 0);
                r = rect;
            }
            string text;
            if (!string.IsNullOrEmpty(Activity.DisplayName))
            {
                text = Activity.DisplayName + Environment.NewLine + this.Name;
            }
            else
            {
                string window = DisplayHelper.GetActionElementWindow(Activity);
                if (!string.IsNullOrEmpty(window))
                    text = this.Name + Environment.NewLine + window;
                else
                {
                                                                                text = this.Name;
                }
            }
            g.DrawString(text,
                o.Font, o.FontBrush, r, o.StringFormat);
                                }
    }
}
