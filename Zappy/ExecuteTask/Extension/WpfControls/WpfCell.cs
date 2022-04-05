using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfCell : WpfControl
    {
        private static readonly Dictionary<string, bool> ValidProperties = InitializeValidProperties();

        public WpfCell() : this(null)
        {
        }

        public WpfCell(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Cell.Name);
        }

        protected override ZappyTaskControl[] GetZappyTaskControlsForSearch()
        {
            List<ZappyTaskControl> list = new List<ZappyTaskControl>();
            ZappyTaskControl item = null;
            if (SearchProperties.Contains(PropertyNames.RowIndex))
            {
                if (ALUtility.GetControlTypeUsingSearchProperties(Container) == ControlType.Row)
                {
                    ALUtility.ThrowDataGridRelatedException(Resources.ExtraRowSpecifiedAsContainer, "SearchProperties");
                }
                if (ALUtility.GetControlTypeUsingSearchProperties(Container) != ControlType.Table)
                {
                    ALUtility.ThrowDataGridRelatedException(Resources.NoTableSpecifiedAsContainer, "SearchProperties");
                }
                item = ALUtility.CreateNewZappyTaskControlAndCopyFrom(this);
                ZappyTaskControl control2 = new ZappyTaskControl(Container)
                {
                    SearchProperties = {
                        [ZappyTaskControl.PropertyNames.Instance] = ALUtility.GetModifiedInstanceFromIndex(SearchProperties[PropertyNames.RowIndex], PropertyNames.RowIndex),
                        [ZappyTaskControl.PropertyNames.ControlType] = ControlType.Row.Name
                    },
                    TechnologyName = TechnologyName
                };
                item.SearchProperties.Remove(PropertyNames.RowIndex);
                list.Add(control2);
            }
            if (SearchProperties.Contains(PropertyNames.ColumnIndex) && list.Count == 0 && ALUtility.GetControlTypeUsingSearchProperties(Container) != ControlType.Row)
            {
                ALUtility.ThrowDataGridRelatedException(Resources.NoRowSpecifiedAsContainer, "SearchProperties");
            }
            if (item != null)
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        protected override Dictionary<string, bool> GetValidSearchProperties() =>
            ValidProperties;

        private static Dictionary<string, bool> InitializeValidProperties()
        {
            Dictionary<string, bool> dictionary = InitializeValidSearchProperties();
            dictionary.Add(PropertyNames.RowIndex, true);
            dictionary.Add(PropertyNames.ColumnIndex, true);
            dictionary.Add(PropertyNames.ColumnHeader, true);
            return dictionary;
        }

        public virtual bool Checked
        {
            get =>
                (bool)GetProperty(PropertyNames.Checked);
            set
            {
                SetProperty(PropertyNames.Checked, value);
            }
        }

        public virtual int ColumnIndex =>
            (int)GetProperty(PropertyNames.ColumnIndex);

        public virtual ControlType ContentControlType =>
            (ControlType)GetProperty(PropertyNames.ContentControlType);

        public virtual bool Indeterminate
        {
            get =>
                (bool)GetProperty(PropertyNames.Indeterminate);
            set
            {
                SetProperty(PropertyNames.Indeterminate, value);
            }
        }

        public virtual bool ReadOnly =>
            (bool)GetProperty(PropertyNames.ReadOnly);

        public virtual int RowIndex =>
            (int)GetProperty(PropertyNames.RowIndex);

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        public virtual string Value
        {
            get =>
                (string)GetProperty(PropertyNames.Value);
            set
            {
                SetProperty(PropertyNames.Value, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Checked = "Checked";
            public static readonly string ColumnHeader = "ColumnHeader";
            public static readonly string ColumnIndex = "ColumnIndex";
            public static readonly string ContentControlType = "ContentControlType";
            public static readonly string Indeterminate = "Indeterminate";
            public static readonly string ReadOnly = "ReadOnly";
            public static readonly string RowIndex = "RowIndex";
            public static readonly string Selected = "Selected";
            public static readonly string Value = "Value";
        }
    }
}

