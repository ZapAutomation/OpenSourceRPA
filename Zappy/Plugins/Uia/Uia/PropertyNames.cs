using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class PropertyNames
    {
        internal const string AcceleratorKey = "AcceleratorKey";
        internal const string AccessKey = "AccessKey";
        private static Dictionary<string, ControlType> automationControlTypeMap;
        internal const string AutomationId = "AutomationId";
        internal const string CanToggle = "CanToggle";
        internal const string Checked = "Checked";
        internal const string ClassName = "ClassName";
        internal const string ColumnHeader = "ColumnHeader";
        internal const string ColumnIndex = "ColumnIndex";
        internal const string ControlType = "ControlType";
        internal const string CopyPastedText = "CopyPastedText";
        internal const string Date = "Date";
        internal const string DisplayText = "DisplayText";
        internal const string Enabled = "Enabled";
        internal const string Expanded = "Expanded";
        internal const string Font = "Font";
        internal const string FrameworkId = "FrameworkId";
        internal const string HelpText = "HelpText";
        internal const string Indeterminate = "Indeterminate";
        internal const string Instance = "Instance";
        internal const string IsGroupedTable = "IsGroupedTable";
        internal const string IsHeaderInteractable = "IsHeaderInteractable";
        internal const string IsMultipleSelection = "IsMultipleSelection";
        internal const string ItemStatus = "ItemStatus";
        internal const string LabeledBy = "LabeledBy";
        internal const string Name = "Name";
        internal static Dictionary<string, AutomationProperty> nameToAutomationProperty = InitializeNameToAutomationProperty();
        internal const string NativeControlType = "NativeControlType";
        internal const string Position = "Position";
        private static Dictionary<string, string> propertyNames = InitializeProperties();
        internal const string QueryPropertyOrderOfInvoke = "OrderOfInvocation";
        internal const string ReadOnly = "ReadOnly";
        internal const string RichEditBoxClassName = "RichEditBox";
        internal const string Selected = "Selected";
        internal const string SelectedDates = "SelectedDates";
        internal const string SelectedIndex = "SelectedIndex";
        internal const string SelectedIndices = "SelectedIndices";
        internal const string SelectedItem = "SelectedItem";
        internal const string SelectedItems = "SelectedItems";
        internal const string SelectionText = "SelectionText";
        internal const string Shortcut = "Shortcut";
        internal static readonly string[] SingleQueryConditonProperties = { "Name", "LabeledBy", "HelpText", "AccessKey", "AcceleratorKey" };
        internal const string Value = "Value";

        static PropertyNames()
        {
            Dictionary<string, ControlType> dictionary1 = new Dictionary<string, ControlType> {
                {
                    ActionMap.HelperClasses.ControlType.Button.Name,
                    System.Windows.Automation.ControlType.Button
                },
                {
                    ActionMap.HelperClasses.ControlType.Calendar.Name,
                    System.Windows.Automation.ControlType.Calendar
                },
                {
                    ActionMap.HelperClasses.ControlType.CheckBox.Name,
                    System.Windows.Automation.ControlType.CheckBox
                },
                {
                    ActionMap.HelperClasses.ControlType.ColumnHeader.Name,
                    System.Windows.Automation.ControlType.HeaderItem
                },
                {
                    ActionMap.HelperClasses.ControlType.ComboBox.Name,
                    System.Windows.Automation.ControlType.ComboBox
                },
                {
                    "DataItem",
                    System.Windows.Automation.ControlType.DataItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Document.Name,
                    System.Windows.Automation.ControlType.Document
                },
                {
                    ActionMap.HelperClasses.ControlType.Edit.Name,
                    System.Windows.Automation.ControlType.Edit
                },
                {
                    ActionMap.HelperClasses.ControlType.FlipView.Name,
                    System.Windows.Automation.ControlType.List
                },
                {
                    ActionMap.HelperClasses.ControlType.FlipViewItem.Name,
                    System.Windows.Automation.ControlType.ListItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Group.Name,
                    System.Windows.Automation.ControlType.Group
                },
                {
                    "Header",
                    System.Windows.Automation.ControlType.Header
                },
                {
                    "HeaderItem",
                    System.Windows.Automation.ControlType.HeaderItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Hyperlink.Name,
                    System.Windows.Automation.ControlType.Hyperlink
                },
                {
                    ActionMap.HelperClasses.ControlType.Image.Name,
                    System.Windows.Automation.ControlType.Image
                },
                {
                    ActionMap.HelperClasses.ControlType.Indicator.Name,
                    System.Windows.Automation.ControlType.Thumb
                },
                {
                    ActionMap.HelperClasses.ControlType.List.Name,
                    System.Windows.Automation.ControlType.List
                },
                {
                    ActionMap.HelperClasses.ControlType.ListItem.Name,
                    System.Windows.Automation.ControlType.ListItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Menu.Name,
                    System.Windows.Automation.ControlType.Menu
                },
                {
                    ActionMap.HelperClasses.ControlType.MenuBar.Name,
                    System.Windows.Automation.ControlType.MenuBar
                },
                {
                    ActionMap.HelperClasses.ControlType.MenuItem.Name,
                    System.Windows.Automation.ControlType.MenuItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Pane.Name,
                    System.Windows.Automation.ControlType.Pane
                },
                {
                    ActionMap.HelperClasses.ControlType.ProgressBar.Name,
                    System.Windows.Automation.ControlType.ProgressBar
                },
                {
                    ActionMap.HelperClasses.ControlType.RowHeader.Name,
                    System.Windows.Automation.ControlType.HeaderItem
                },
                {
                    ActionMap.HelperClasses.ControlType.RadioButton.Name,
                    System.Windows.Automation.ControlType.RadioButton
                },
                {
                    ActionMap.HelperClasses.ControlType.Row.Name,
                    System.Windows.Automation.ControlType.DataItem
                },
                {
                    ActionMap.HelperClasses.ControlType.ScrollBar.Name,
                    System.Windows.Automation.ControlType.ScrollBar
                },
                {
                    ActionMap.HelperClasses.ControlType.Separator.Name,
                    System.Windows.Automation.ControlType.Separator
                },
                {
                    ActionMap.HelperClasses.ControlType.Slider.Name,
                    System.Windows.Automation.ControlType.Slider
                },
                {
                    ActionMap.HelperClasses.ControlType.Spinner.Name,
                    System.Windows.Automation.ControlType.Spinner
                },
                {
                    ActionMap.HelperClasses.ControlType.SplitButton.Name,
                    System.Windows.Automation.ControlType.SplitButton
                },
                {
                    ActionMap.HelperClasses.ControlType.StatusBar.Name,
                    System.Windows.Automation.ControlType.StatusBar
                },
                {
                    ActionMap.HelperClasses.ControlType.Text.Name,
                    System.Windows.Automation.ControlType.Text
                },
                {
                    ActionMap.HelperClasses.ControlType.Table.Name,
                    System.Windows.Automation.ControlType.Table
                },
                {
                    ActionMap.HelperClasses.ControlType.TabList.Name,
                    System.Windows.Automation.ControlType.TabItem
                },
                {
                    ActionMap.HelperClasses.ControlType.TabPage.Name,
                    System.Windows.Automation.ControlType.Tab
                },
                {
                    ActionMap.HelperClasses.ControlType.TitleBar.Name,
                    System.Windows.Automation.ControlType.TitleBar
                },
                {
                    ActionMap.HelperClasses.ControlType.ToolBar.Name,
                    System.Windows.Automation.ControlType.ToolBar
                },
                {
                    ActionMap.HelperClasses.ControlType.ToolTip.Name,
                    System.Windows.Automation.ControlType.ToolTip
                },
                {
                    ActionMap.HelperClasses.ControlType.Tree.Name,
                    System.Windows.Automation.ControlType.Tree
                },
                {
                    ActionMap.HelperClasses.ControlType.TreeItem.Name,
                    System.Windows.Automation.ControlType.TreeItem
                },
                {
                    ActionMap.HelperClasses.ControlType.Window.Name,
                    System.Windows.Automation.ControlType.Window
                }
            };
            automationControlTypeMap = dictionary1;
        }

        internal static ControlType GetAutomationControlTypeFromName(string controlTypeName)
        {
            ControlType custom;
            if (!automationControlTypeMap.TryGetValue(controlTypeName, out custom))
            {
                custom = System.Windows.Automation.ControlType.Custom;
            }
            return custom;
        }

        internal static string GetPropertyNameInCorrectCase(string propertyName)
        {
            if (propertyNames.ContainsKey(propertyName))
            {
                return propertyNames[propertyName];
            }
            return propertyName;
        }

        private static Dictionary<string, AutomationProperty> InitializeNameToAutomationProperty() =>
            new Dictionary<string, AutomationProperty>(StringComparer.OrdinalIgnoreCase) {
                {
                    "AutomationId",
                    AutomationElement.AutomationIdProperty
                },
                {
                    "ClassName",
                    AutomationElement.ClassNameProperty
                },
                {
                    "ControlType",
                    AutomationElement.ControlTypeProperty
                },
                {
                    "Name",
                    AutomationElement.NameProperty
                },
                {
                    "AccessKey",
                    AutomationElement.AccessKeyProperty
                },
                {
                    "AcceleratorKey",
                    AutomationElement.AcceleratorKeyProperty
                },
                {
                    "HelpText",
                    AutomationElement.HelpTextProperty
                },
                {
                    "LabeledBy",
                    AutomationElement.LabeledByProperty
                }
            };

        private static Dictionary<string, string> InitializeProperties() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {
                    "AutomationId",
                    "AutomationId"
                },
                {
                    "ClassName",
                    "ClassName"
                },
                {
                    "ControlType",
                    "ControlType"
                },
                {
                    "NativeControlType",
                    "NativeControlType"
                },
                {
                    "Name",
                    "Name"
                },
                {
                    "AccessKey",
                    "AccessKey"
                },
                {
                    "AcceleratorKey",
                    "AcceleratorKey"
                },
                {
                    "HelpText",
                    "HelpText"
                },
                {
                    "LabeledBy",
                    "LabeledBy"
                },
                {
                    "Instance",
                    "Instance"
                },
                {
                    "ColumnHeader",
                    "ColumnHeader"
                },
                {
                    "ColumnIndex",
                    "ColumnIndex"
                },
                {
                    "Value",
                    "Value"
                },
                {
                    "FrameworkId",
                    "FrameworkId"
                }
            };

        internal static void ThrowIfInvalid(string propertyName)
        {
            if (!propertyNames.ContainsKey(propertyName))
            {
                throw new ArgumentException(string.Empty, "propertyName");
            }
        }
    }
}

