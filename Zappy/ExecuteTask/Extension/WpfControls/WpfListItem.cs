using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfListItem : WpfControl
    {
        public WpfListItem() : this(null)
        {
        }

        public WpfListItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ListItem.Name);
        }

        public void Select()
        {
            ZappyTaskControl parent = GetParent();
            if (!(parent is WpfComboBox) && !(parent is WpfList))
            {
                throw new InvalidOperationException(Resources.InvalidListItemOperation);
            }
            PropertyExpression expression = SearchProperties.Find(ZappyTaskControl.PropertyNames.Instance);
            int instance = 1;
            if (expression != null && int.TryParse(expression.PropertyValue, out instance) && instance <= 0)
            {
                object[] args = { instance, ControlType.ListItem, ZappyTaskControl.PropertyNames.Instance };
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
            }
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                TechnologyElementPropertyProvider.SelectUsingInstanceAndName(parent, DisplayText, instance);
                return null;
            }, this, true, true);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Selected = "Selected";
        }
    }
}

