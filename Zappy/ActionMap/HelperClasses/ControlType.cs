using System;
using System.Collections.Generic;
using Zappy.Properties;

namespace Zappy.ActionMap.HelperClasses
{

    [Serializable]
    public sealed class ControlType
    {
        internal static Dictionary<string, ControlType> ControlTypes = new Dictionary<string, ControlType>(NameComparer);

        public static readonly ControlType ContextMenu = GetControlType("ContextMenu", Resources.ContextMenu);


        public static readonly ControlType Audio = GetControlType("Audio", Resources.Audio);

        public static readonly ControlType Button = GetControlType("Button", Resources.Button);

        public static readonly ControlType Calendar = GetControlType("Calendar", Resources.Calendar);

        public static readonly ControlType Cell = GetControlType("Cell", Resources.Cell);

        public static readonly ControlType CheckBox = GetControlType("CheckBox", Resources.CheckBox);

        public static readonly ControlType CheckBoxTreeItem = GetControlType("CheckBoxTreeItem", Resources.CheckBoxTreeItem);

        public static readonly ControlType Client = GetControlType("Client", Resources.Client);

        public static readonly ControlType ColumnHeader = GetControlType("ColumnHeader", Resources.ColumnHeader);

        public static readonly ControlType ComboBox = GetControlType("ComboBox", Resources.ComboBox);


        public static readonly ControlType Custom = GetControlType("Custom", Resources.Custom);

        public static readonly ControlType DatePicker = GetControlType("DatePicker", Resources.DatePicker);

        public static readonly ControlType DateTimePicker = GetControlType("DateTimePicker", Resources.DateTimePicker);

        public static readonly ControlType Document = GetControlType("Document", Resources.Document);

        public static readonly ControlType Edit = GetControlType("Edit", Resources.Edit);

        public static readonly ControlType Empty = GetControlType(string.Empty, string.Empty);

        public static readonly ControlType Expander = GetControlType("Expander", Resources.Expander);

        public static readonly ControlType FileInput = GetControlType("FileInput", Resources.FileInput);

        public static readonly ControlType FlipView = GetControlType("FlipView", Resources.FlipView);

        public static readonly ControlType FlipViewItem = GetControlType("FlipViewItem", Resources.FlipViewItem);

        public static readonly ControlType Frame = GetControlType("Frame", Resources.Frame);
        private readonly string friendlyName;

        internal static readonly ControlType Grip = GetControlType("Grip");

        public static readonly ControlType Group = GetControlType("Group", Resources.Group);

        public static readonly ControlType Hub = GetControlType("Hub", Resources.Hub);

        public static readonly ControlType HubSection = GetControlType("HubSection", Resources.HubSection);

        public static readonly ControlType Hyperlink = GetControlType("Hyperlink", Resources.Hyperlink);

        public static readonly ControlType Image = GetControlType("Image", Resources.Image);

        internal static readonly ControlType Indicator = GetControlType("Indicator");

        public static readonly ControlType Label = GetControlType("Label", Resources.Label);

        public static readonly ControlType List = GetControlType("List", Resources.List);

        public static readonly ControlType ListItem = GetControlType("ListItem", Resources.ListItem);

        public static readonly ControlType Media = GetControlType("Media", Resources.Media);

        public static readonly ControlType Menu = GetControlType("Menu", Resources.PopupMenu);

        public static readonly ControlType MenuBar = GetControlType("MenuBar", Resources.MenuBar);

        public static readonly ControlType MenuItem = GetControlType("MenuItem", Resources.MenuItem);
        private readonly string name;

        public static readonly ControlType Pane = GetControlType("Pane", Resources.Pane);

        public static readonly ControlType Pivot = GetControlType("Pivot", Resources.Pivot);

        public static readonly ControlType PivotItem = GetControlType("PivotItem", Resources.PivotItem);

        public static readonly ControlType ProgressBar = GetControlType("ProgressBar", Resources.ProgressBar);

        public static readonly ControlType RadioButton = GetControlType("RadioButton", Resources.RadioButton);

        public static readonly ControlType Rating = GetControlType("Rating", Resources.Rating);

        public static readonly ControlType Row = GetControlType("Row", Resources.Row);

        public static readonly ControlType RowHeader = GetControlType("RowHeader", Resources.RowHeader);

        public static readonly ControlType ScrollBar = GetControlType("ScrollBar", Resources.ScrollBar);

        public static readonly ControlType SemanticZoom = GetControlType("SemanticZoom", Resources.SemanticZoom);

        public static readonly ControlType Separator = GetControlType("Separator", Resources.Separator);

        public static readonly ControlType Slider = GetControlType("Slider", Resources.Slider);

        public static readonly ControlType Spinner = GetControlType("Spinner", Resources.Spinner);

        public static readonly ControlType SplitButton = GetControlType("SplitButton", Resources.SplitButton);

        public static readonly ControlType StatusBar = GetControlType("StatusBar", Resources.StatusBar);

        public static readonly ControlType Table = GetControlType("Table", Resources.Table);

        public static readonly ControlType TabList = GetControlType("TabList", Resources.TabList);

        public static readonly ControlType TabPage = GetControlType("TabPage", Resources.TabPage);

        public static readonly ControlType Text = GetControlType("Text", Resources.Label);

        public static readonly ControlType TitleBar = GetControlType("TitleBar", Resources.TitleBar);

        public static readonly ControlType ToggleButton = GetControlType("ToggleButton", Resources.ToggleButton);

        public static readonly ControlType ToggleSwitch = GetControlType("ToggleSwitch", Resources.ToggleSwitch);

        public static readonly ControlType ToolBar = GetControlType("ToolBar", Resources.ToolBar);

        public static readonly ControlType ToolTip = GetControlType("ToolTip", Resources.ToolTip);

        public static readonly ControlType Tree = GetControlType("Tree", Resources.Tree);

        public static readonly ControlType TreeItem = GetControlType("TreeItem", Resources.TreeItem);

        public static readonly ControlType Video = GetControlType("Video", Resources.Video);

        public static readonly ControlType Window = GetControlType("Window", Resources.Window);

        private ControlType(string name, string friendlyName)
        {
            this.name = name;
            this.friendlyName = friendlyName;
        }

        public override bool Equals(object other)
        {
            ControlType type = other as ControlType;
            return type != null && NameComparer.Equals(Name, type.Name);
        }

        public static ControlType GetControlType(string name) =>
            GetControlType(name, name);

        public static ControlType GetControlType(string name, string friendlyName)
        {
            if (ControlTypes.ContainsKey(name))
            {
                return ControlTypes[name];
            }
            ControlType type = new ControlType(name, friendlyName);
            ControlTypes.Add(name, type);
            return type;
        }

        public override int GetHashCode() =>
            Name.ToUpperInvariant().GetHashCode();

        public bool NameEquals(string controlName) =>
            NameComparer.Equals(Name, controlName);

        public static bool operator ==(ControlType left, ControlType right)
        {
            if (!ReferenceEquals(left, null))
            {
                return left.Equals(right);
            }
            return ReferenceEquals(right, null);
        }

        public static implicit operator ControlType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Empty;
            }
            return GetControlType(name);
        }

        public static bool operator !=(ControlType left, ControlType right) =>
            !(left == right);

        public override string ToString() =>
            Name;

        public string FriendlyName =>
            friendlyName;

        public string Name =>
            name;

        public static StringComparer NameComparer =>
            StringComparer.OrdinalIgnoreCase;
    }

}
