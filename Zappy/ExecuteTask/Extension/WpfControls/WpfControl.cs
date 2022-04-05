using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfControl : ZappyTaskControl
    {
        internal static readonly string s_FrameworkId = "Wpf";
        private static readonly Dictionary<string, bool> s_ValidProperties = InitializeValidSearchProperties();

        public WpfControl() : this(null)
        {
        }

        public WpfControl(ZappyTaskControl parent) : base(parent)
        {
            TechnologyName = "UIA";
            SearchProperties.Add(PropertyNames.FrameworkId, s_FrameworkId);
        }

        internal WpfControl(ZappyTaskControl parent, bool noFrameworkId) : base(parent)
        {
            TechnologyName = "UIA";
            if (!noFrameworkId)
            {
                SearchProperties.Add(PropertyNames.FrameworkId, s_FrameworkId);
            }
        }

        protected override ZappyTaskControl[] GetZappyTaskControlsForSearch()
        {
            ValidateSearchProperties();
            return base.GetZappyTaskControlsForSearch();
        }

        protected virtual Dictionary<string, bool> GetValidSearchProperties() =>
            s_ValidProperties;

        internal static Dictionary<string, bool> InitializeValidSearchProperties() =>
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.ControlType,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Name,
                    true
                },
                {
                    PropertyNames.LabeledBy,
                    true
                },
                {
                    PropertyNames.HelpText,
                    true
                },
                {
                    PropertyNames.AcceleratorKey,
                    true
                },
                {
                    PropertyNames.AccessKey,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.ClassName,
                    true
                },
                {
                    PropertyNames.AutomationId,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Instance,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Value,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.MaxDepth,
                    true
                },
                {
                    PropertyNames.FrameworkId,
                    true
                }
            };

        internal virtual void ValidateSearchProperties()
        {
            Dictionary<string, bool> validSearchProperties = GetValidSearchProperties();
            if (SearchPropertiesSetExplicitly && validSearchProperties != null)
            {
                foreach (PropertyExpression expression in SearchProperties)
                {
                    if (!validSearchProperties.ContainsKey(expression.PropertyName))
                    {
                        object[] args = { expression.PropertyName };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidSearchProperty, args));
                    }
                }
            }
        }

        public virtual string AcceleratorKey =>
            (string)GetProperty(PropertyNames.AcceleratorKey);

        public virtual string AccessKey =>
            (string)GetProperty(PropertyNames.AccessKey);

        public virtual string AutomationId =>
            (string)GetProperty(PropertyNames.AutomationId);

        public virtual string Font =>
            (string)GetProperty(PropertyNames.Font);

        public virtual string FrameworkId =>
            (string)GetProperty(PropertyNames.FrameworkId);

        public virtual string HelpText =>
            (string)GetProperty(PropertyNames.HelpText);

        public virtual string ItemStatus =>
            (string)GetProperty(PropertyNames.ItemStatus);

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

        public virtual string ToolTipText
        {
            get
            {
                string name = null;
                WpfToolTip parent = new WpfToolTip(TopParent);
                WpfText text = new WpfText(parent);
                Mouse.Hover(this);
                for (int i = 0; i < 5; i++)
                {
                    if (parent.TryFind())
                    {
                        try
                        {
                            name = parent.Name;
                            ZappyTaskControlCollection children = parent.GetChildren();
                            if (children != null && children.Count > 0)
                            {
                                name = text.DisplayText;
                            }
                        }
                        catch (ZappyTaskException)
                        {
                                                                                }
                        return name;
                    }
                    Thread.Sleep(100);
                }
                return name;
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : ZappyTaskControl.PropertyNames
        {
            public static readonly string AcceleratorKey = "AcceleratorKey";
            public static readonly string AccessKey = "AccessKey";
            public static readonly string AutomationId = "AutomationId";
            public static readonly string Font = "Font";
            public static readonly string FrameworkId = "FrameworkId";
            public static readonly string HelpText = "HelpText";
            public static readonly string ItemStatus = "ItemStatus";
            public static readonly string LabeledBy = "LabeledBy";
        }
    }
}

