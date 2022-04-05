using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinMenuBar : WinControl
    {
        public WinMenuBar() : this(null)
        {
        }

        public WinMenuBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.MenuBar.Name);
        }

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(WinMenu.PropertyNames.Items);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Items = "Items";
        }
    }
}

