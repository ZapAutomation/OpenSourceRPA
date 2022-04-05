using System;
using System.Collections.Generic;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinRow : WinControl
    {
        private static readonly Dictionary<string, bool> ValidProperties = InitializeValidProperties();

        public WinRow() : this(null)
        {
        }

        public WinRow(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Row.Name);
        }

        public WinCell GetCell(int cellIndex)
        {
            WinCell cell = new WinCell(this)
            {
                SearchProperties = { [WinCell.PropertyNames.ColumnIndex] = cellIndex.ToString(CultureInfo.InvariantCulture) }
            };
            cell.Find();
            return cell;
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection cells = Cells;
            if (cells != null)
            {
                            }
            return null;
        }

        protected override ZappyTaskControl[] GetZappyTaskControlsForSearch()
        {
            ValidateSearchProperties();
            if (!SearchProperties.Contains(PropertyNames.RowIndex))
            {
                return null;
            }
            if (ALUtility.GetControlTypeUsingSearchProperties(Container) != ControlType.Table)
            {
                ALUtility.ThrowDataGridRelatedException(Resources.NoTableSpecifiedAsContainer, "SearchProperties");
            }
            ZappyTaskControl control = ALUtility.CreateNewZappyTaskControlAndCopyFrom(this);
            control.SearchProperties[ZappyTaskControl.PropertyNames.Instance] = ALUtility.GetModifiedInstanceFromIndex(control.SearchProperties[PropertyNames.RowIndex], PropertyNames.RowIndex);
            control.SearchProperties.Remove(PropertyNames.RowIndex);
            return new[] { control };
        }

        protected override Dictionary<string, bool> GetValidSearchProperties() =>
            ValidProperties;

        private static Dictionary<string, bool> InitializeValidProperties()
        {
            Dictionary<string, bool> dictionary = InitializeValidSearchProperties();
            dictionary.Add(PropertyNames.RowIndex, true);
            return dictionary;
        }

        public virtual ZappyTaskControlCollection Cells =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Cells);

        public virtual int RowIndex =>
            (int)GetProperty(PropertyNames.RowIndex);

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        public virtual string Value =>
            (string)GetProperty(PropertyNames.Value);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Cells = "Cells";
            public static readonly string RowIndex = "RowIndex";
            public static readonly string Selected = "Selected";
            public static readonly string Value = "Value";
        }
    }
}

