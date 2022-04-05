using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinTree : WinControl
    {
        public WinTree() : this(null)
        {
        }

        public WinTree(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Tree.Name);
        }

        public virtual ZappyTaskControl HorizontalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.HorizontalScrollBar);

        public virtual ZappyTaskControlCollection Nodes =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Nodes);

        public virtual ZappyTaskControl VerticalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.VerticalScrollBar);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string HorizontalScrollBar = "HorizontalScrollBar";
            public static readonly string Nodes = "Nodes";
            public static readonly string VerticalScrollBar = "VerticalScrollBar";
        }
    }
}

