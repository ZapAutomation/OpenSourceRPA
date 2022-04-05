using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinListItem : WinControl
    {
        public WinListItem() : this(null)
        {
        }

        public WinListItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ListItem.Name);
            SearchConfigurations.Remove(SearchConfiguration.VisibleOnly);
        }

        public string[] GetColumnValues()
        {
            WinList parent = GetParent() as WinList;
            if (parent == null)
            {
                throw new InvalidOperationException(Resources.NotAListViewItemControl);
            }
            if (!parent.IsReportView)
            {
                throw new InvalidOperationException(Resources.NotInReportViewMode);
            }
            string accessibleDescription = AccessibleDescription;
            string[] columnNames = parent.GetColumnNames();
            if (columnNames == null || columnNames.Length == 0)
            {
                return null;
            }
            string[] strArray2 = new string[columnNames.Length];
            strArray2[0] = DisplayText;
            if (!string.IsNullOrEmpty(accessibleDescription))
            {
                accessibleDescription = accessibleDescription.Substring(columnNames[1].Length + 2);
                int index = 2;
                while (index < columnNames.Length)
                {
                    int num2 = accessibleDescription.IndexOf(columnNames[index], StringComparison.Ordinal);
                    if (num2 == -1)
                    {
                        break;
                    }
                    strArray2[index - 1] = accessibleDescription.Substring(0, num2 - 2);
                    accessibleDescription = accessibleDescription.Substring(num2 + columnNames[index].Length + 2);
                    index++;
                }
                strArray2[index - 1] = accessibleDescription;
            }
            return strArray2;
        }

        public void Select()
        {
            ZappyTaskControl parent = GetParent();
            if (parent is WinList)
            {
                ZappyTaskControl control2 = parent.GetParent();
                if (control2 is WinWindow)
                {
                    control2 = control2.GetParent();
                }
                if (control2 is WinComboBox)
                {
                    parent = control2;
                }
            }
            if (parent is WinComboBox)
            {
                WinComboBox box = parent as WinComboBox;
                if (box.IsEditable)
                {
                    box.EditableItem = DisplayText;
                }
                else
                {
                    box.SelectedItem = DisplayText;
                }
            }
            else
            {
                if (!(parent is WinList))
                {
                    throw new InvalidOperationException(Resources.InvalidListItemOperation);
                }
                WinList list = parent as WinList;
                list.SelectedItems = new[] { DisplayText };
            }
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Selected = "Selected";
        }
    }
}

