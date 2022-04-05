using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfWindow : WpfControl
    {
        private static readonly Dictionary<string, bool> ValidProperties = InitializeValidProperties();

        public WpfWindow() : this(null)
        {
        }

        public WpfWindow(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Window.Name);
        }

        protected override Dictionary<string, bool> GetValidSearchProperties() =>
            ValidProperties;

        private static Dictionary<string, bool> InitializeValidProperties()
        {
            Dictionary<string, bool> dictionary = InitializeValidSearchProperties();
            dictionary.Add(PropertyNames.OrderOfInvocation, true);
            return dictionary;
        }

        public virtual bool AlwaysOnTop =>
            (bool)GetProperty(PropertyNames.AlwaysOnTop);

        public virtual bool HasTitleBar =>
            (bool)GetProperty(PropertyNames.HasTitleBar);

        public virtual bool Maximized
        {
            get =>
                (bool)GetProperty(PropertyNames.Maximized);
            set
            {
                SetProperty(PropertyNames.Maximized, value);
            }
        }

        public virtual bool Minimized
        {
            get =>
                (bool)GetProperty(PropertyNames.Minimized);
            set
            {
                SetProperty(PropertyNames.Minimized, value);
            }
        }

        public virtual bool Popup =>
            (bool)GetProperty(PropertyNames.Popup);

        public virtual bool Resizable =>
            (bool)GetProperty(PropertyNames.Resizable);

        public virtual bool Restored
        {
            get =>
                (bool)GetProperty(PropertyNames.Restored);
            set
            {
                SetProperty(PropertyNames.Restored, value);
            }
        }

        public virtual bool ShowInTaskbar =>
            (bool)GetProperty(PropertyNames.ShowInTaskbar);

        public virtual bool TabStop =>
            (bool)GetProperty(PropertyNames.TabStop);

        public virtual bool Transparent =>
            (bool)GetProperty(PropertyNames.Transparent);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string AlwaysOnTop = "AlwaysOnTop";
            public static readonly string HasTitleBar = "HasTitleBar";
            public static readonly string Maximized = "Maximized";
            public static readonly string Minimized = "Minimized";
            public static readonly string OrderOfInvocation = "OrderOfInvocation";
            public static readonly string Popup = "Popup";
            public static readonly string Resizable = "Resizable";
            public static readonly string Restored = "Restored";
            public static readonly string ShowInTaskbar = "ShowInTaskbar";
            public static readonly string TabStop = "TabStop";
            public static readonly string Transparent = "Transparent";
        }
    }
}

