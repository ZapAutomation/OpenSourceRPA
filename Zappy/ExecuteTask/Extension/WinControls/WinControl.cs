using System;
using System.Collections.Generic;
using System.Threading;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinControl : ZappyTaskControl
    {
        private static readonly Dictionary<string, bool> ValidProperties = InitializeValidSearchProperties();

        public WinControl() : this(null)
        {
        }

        public WinControl(ZappyTaskControl parent) : base(parent)
        {
            TechnologyName = "MSAA";
            SearchConfigurations.Add(SearchConfiguration.VisibleOnly);
        }

        protected override ZappyTaskControl[] GetZappyTaskControlsForSearch()
        {
            ValidateSearchProperties();
            return base.GetZappyTaskControlsForSearch();
        }

        protected virtual Dictionary<string, bool> GetValidSearchProperties() =>
            ValidProperties;

        internal static Dictionary<string, bool> InitializeValidSearchProperties() =>
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.ControlType,
                    true
                },
                {
                    PropertyNames.ControlId,
                    true
                },
                {
                    PropertyNames.ControlName,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Name,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Instance,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.MaxDepth,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.Value,
                    true
                },
                {
                    ZappyTaskControl.PropertyNames.ClassName,
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
                                            }
                }
            }
        }

        public string AccessibleDescription =>
            (string)GetProperty(PropertyNames.AccessibleDescription);

        public virtual string AccessKey =>
            (string)GetProperty(PropertyNames.AccessKey);

        public virtual int ControlId =>
            (int)GetProperty(PropertyNames.ControlId);

        public virtual string ControlName =>
            (string)GetProperty(PropertyNames.ControlName);

        public virtual string HelpText =>
            (string)GetProperty(PropertyNames.HelpText);

        public virtual string ToolTipText
        {
            get
            {
                WinWindow parent = new WinWindow
                {
                    SearchProperties = { {
                        ZappyTaskControl.PropertyNames.ClassName,
                        "tooltip",
                        PropertyExpressionOperator.Contains
                    } }
                };
                WinToolTip tip = new WinToolTip(parent);
                Mouse.Hover(this);
                for (int i = 0; i < 10; i++)
                {
                    if (tip.TryFind())
                    {
                        return tip.Name;
                    }
                    Thread.Sleep(100);
                }
                return null;
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : ZappyTaskControl.PropertyNames
        {
            public static readonly string AccessibleDescription = "AccessibleDescription";
            public static readonly string AccessKey = "AccessKey";
            public static readonly string ControlId = "ControlId";
            public static readonly string ControlName = "ControlName";
            public static readonly string HelpText = "HelpText";
        }
    }
}

