using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Extension.WinControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension
{
    internal class WinPropertyProvider : PropertyProviderBase
    {
        private const string _isNumericUpDownControl = "_isNumericUpDownControl";
        private const string _isWin32ForSure = "_isWin32ForSure";
        private const string richTextClassName = "RICHTEXT";
        private WindowsControl win;
        private const string WinformsDataGridCellCheckBoxHelpText = "DataGridViewCheckBoxCell(DataGridViewCell)";

        public WinPropertyProvider()
        {
            commonProperties = InitializeCommonProperties();
            controlTypeToPropertiesMap = this.InitializePropertiesMap();
            controlTypeToPropertyNamesClassMap = this.InitializePropertyNameToClassMap();
            technologyName = "MSAA";
            specializedClassNamePrefix = "Win";
            specializedClassesNamespace = "WinControls";
        }

        private static void CheckItems(ZappyTaskControlCollection itemsToCheck, ZappyTaskControlCollection allItems)
        {
            foreach (ZappyTaskControl control in allItems)
            {
                control.StateValue = ControlStates.None | ControlStates.Normal;
            }
            foreach (ZappyTaskControl control2 in itemsToCheck)
            {
                control2.StateValue = ControlStates.Checked;
            }
        }

        private object GetAcceleratorKeyForMenuItem()
        {
            AutomationElement element = AutomationElement.FromHandle(UIControl.TechnologyElement.WindowHandle);
            string name = UIControl.Name;
            for (AutomationElement element2 = TreeWalker.ControlViewWalker.GetFirstChild(element);
                element2 != null;
                element2 = TreeWalker.ControlViewWalker.GetNextSibling(element2))
            {
                string a = name;
                if (a.Contains(element2.Current.Name))
                {
                    if (!string.Equals(a, element2.Current.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        a = a.Remove(a.IndexOf(element2.Current.Name, StringComparison.Ordinal),
                            element2.Current.Name.Length);
                    }
                    bool flag = string.IsNullOrEmpty(element2.Current.AcceleratorKey);
                    if (flag || a.Contains(element2.Current.AcceleratorKey))
                    {
                        string acceleratorKey = null;
                        if (!flag)
                        {
                            if (string.IsNullOrEmpty(a
                                .Remove(a.IndexOf(element2.Current.AcceleratorKey, StringComparison.Ordinal),
                                    element2.Current.AcceleratorKey.Length).Trim()))
                            {
                                acceleratorKey = element2.Current.AcceleratorKey;
                            }
                        }
                        else if (a.Contains("\t"))
                        {
                            acceleratorKey = a.Remove(0, a.LastIndexOf("\t") + 1);
                        }
                        if (acceleratorKey != null)
                        {
                            int x = element2.Current.BoundingRectangle.X;
                            int y = element2.Current.BoundingRectangle.Y;
                            int width = element2.Current.BoundingRectangle.Width;
                            int height = element2.Current.BoundingRectangle.Height;
                            Rectangle objA = new Rectangle(x, y, width, height);
                            if (Equals(objA, UIControl.BoundingRectangle))
                            {
                                return acceleratorKey;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private object GetButtonProperty(string propertyName)
        {
            if (string.Equals(WinButton.PropertyNames.DisplayText, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetText();
            }
            if (string.Equals(WinButton.PropertyNames.Shortcut, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue(WinControl.PropertyNames.AccessKey);
            }
            if (UIControl.GetParent().ControlType == ControlType.ToolBar)
            {
                return this.GetToolBarItemProperty(propertyName);
            }
            return null;
        }

        private object GetCheckBoxProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinCheckBox.PropertyNames.Checked, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Checked == (UIControl.StateValue & ControlStates.Checked));
            }
            if (string.Equals(WinCheckBox.PropertyNames.Indeterminate, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Indeterminate == (UIControl.StateValue & ControlStates.Indeterminate));
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            return null;
        }

        private object GetCheckBoxTreeNodeProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinCheckBoxTreeItem.PropertyNames.Checked, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Checked == (UIControl.StateValue & ControlStates.Checked));
            }
            if (string.Equals(WinTreeItem.PropertyNames.Expanded, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Expanded == (UIControl.StateValue & ControlStates.Expanded));
            }
            if (string.Equals(WinTreeItem.PropertyNames.ParentNode, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.GetParentNodeForTreeNode();
            }
            if (string.Equals(WinTreeItem.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((ControlStates.None | ControlStates.Selected) ==
                        (UIControl.StateValue & (ControlStates.None | ControlStates.Selected)));
            }
            if (string.Equals(WinCheckBoxTreeItem.PropertyNames.Indeterminate, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Indeterminate == (UIControl.StateValue & ControlStates.Indeterminate));
            }
            if (string.Equals(WinTreeItem.PropertyNames.Nodes, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.GetTreeItemNodes();
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (string.Equals(WinTreeItem.PropertyNames.HasChildNodes, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return !this.IsLeafTreeItem(UIControl);
            }
            return null;
        }

        private object GetComboBoxProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if ((bool)UIControl.TechnologyElement.GetPropertyValue("_isNumericUpDownControl"))
            {
                ZappyTaskControl control =
                    ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Edit, -1)[0];
                return this.GetNumericUpDownProperty(propertyName, control.TechnologyElement);
            }
            if (string.Equals(WinComboBox.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ThrowExceptionIfControlDisabled();
                ZappyTaskControlCollection children = new ZappyTaskControlCollection();
                ZappyTaskControl control2 = new ZappyTaskControl(UIControl)
                {
                    TechnologyName = technologyName,
                    SearchProperties =
                    {
                        {
                            ZappyTaskControl.PropertyNames.ControlType,
                            "List"
                        }
                    }
                };
                control2.Find();
                if (control2 != null)
                {
                    children = control2.GetChildren();
                    children.RemoveAll(element =>
                        !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                        !element.ControlType.NameEquals(ControlType.CheckBox.Name));
                }
                return children;
            }
            if (string.Equals(propertyName, WinComboBox.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                return (this.win.GetInt(0x157) == 1);
            }
            if (string.Equals(propertyName, WinComboBox.PropertyNames.IsEditable, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls2 =
                    ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Edit, 2);
                return ((controls2 == null) ? ((object)0) : ((object)(controls2.Count > 0)));
            }
            if (string.Equals(WinComboBox.PropertyNames.SelectedIndex, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x147);
            }
            if (!string.Equals(WinComboBox.PropertyNames.SelectedItem, propertyName,
                    StringComparison.OrdinalIgnoreCase) && !string.Equals(ZappyTaskControl.PropertyNames.Value,
                    propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(WinComboBox.PropertyNames.VerticalScrollBar, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return this.GetWinScrollBar(UIControl, 1);
                }
                if (string.Equals(WinComboBox.PropertyNames.HorizontalScrollBar, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return this.GetWinScrollBar(UIControl, 0);
                }
                if (!string.Equals(WinComboBox.PropertyNames.EditableItem, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (!((bool)this.GetComboBoxProperty(WinComboBox.PropertyNames.IsEditable)))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
            }
            return UIControl.TechnologyElement.Value;
        }

        private object GetCommonProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinControl.PropertyNames.ControlName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue(propertyName);
            }
            if (string.Equals(WinControl.PropertyNames.HelpText, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl parent = UIControl.GetParent();
                if (parent != null)
                {
                    IntPtr windowHandle = UIControl.TechnologyElement.WindowHandle;
                    IntPtr ptr2 = parent.TechnologyElement.WindowHandle;
                    if ((windowHandle != IntPtr.Zero) &&
                        ((ptr2 != windowHandle) ||
                         (ControlType.Window.NameEquals(parent.ControlType.Name) && (windowHandle == ptr2))))
                    {
                        try
                        {
                            return AutomationElement.FromHandle(windowHandle).Current.HelpText;
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is InvalidOperationException) &&
                                !(exception is ElementNotAvailableException))
                            {
                                throw;
                            }
                            if (UIControl.TechnologyElement != null)
                            {
                                                                                            }
                            throw new ZappyTaskControlNotAvailableException(exception);
                        }
                    }
                }
                return null;
            }
            if (string.Equals(WinControl.PropertyNames.ControlId, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (int)UIControl.TechnologyElement.GetPropertyValue(propertyName);
            }
            if (string.Equals(WinControl.PropertyNames.AccessibleDescription, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue(propertyName);
            }
            object[] objArray2 = new object[] { propertyName, UIControl.ControlType.Name };
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                Resources.GetPropertyNotSupportedMessage, objArray2));
        }

        private object GetDateTimePickerProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        IntPtr zero = IntPtr.Zero;
            if (string.Equals(WinDateTimePicker.PropertyNames.Checked, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (this.win.HasStyle(2))
                {
                    NativeMethods.SYSTEMTIME systemtime = new NativeMethods.SYSTEMTIME();
                    return (this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1001, ref systemtime) == 0);
                }
                ALUtility.ThrowNotSupportedException(false);
            }
            else if (string.Equals(WinDateTimePicker.PropertyNames.DateTime, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                NativeMethods.SYSTEMTIME systemtime2 = new NativeMethods.SYSTEMTIME();
                if (this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1001, ref systemtime2) == 0)
                {
                    return systemtime2.ToDateTime();
                }
                ALUtility.ThrowNotSupportedException(false);
            }
            else if (
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinDateTimePicker.PropertyNames.DateTimeAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                NativeMethods.SYSTEMTIME systemtime3 = new NativeMethods.SYSTEMTIME();
                if (this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1001, ref systemtime3) == 0)
                {
                    return systemtime3.ToDateTime().ToString(CultureInfo.InvariantCulture);
                }
                ALUtility.ThrowNotSupportedException(false);
            }
            else
            {
                if (string.Equals(WinDateTimePicker.PropertyNames.Calendar, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    zero = this.win.GetHandle(0x1008);
                    if (!zero.Equals(IntPtr.Zero))
                    {
                        return ZappyTaskControlFactory.FromWindowHandle(zero);
                    }
                    ALUtility.ThrowNotSupportedException(false);
                    return null;
                }
                if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    if ((bool)this.GetDateTimePickerProperty(WinDateTimePicker.PropertyNames.Checked))
                    {
                        return ControlStates.Checked;
                    }
                    return (ControlStates.None | ControlStates.Normal);
                }
                if (string.Equals(WinDateTimePicker.PropertyNames.ShowCalendar, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return !this.win.GetHandle(0x1008).Equals(IntPtr.Zero);
                }
                if (string.Equals(WinDateTimePicker.PropertyNames.HasCheckBox, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return this.win.HasStyle(2);
                }
                if (string.Equals(WinDateTimePicker.PropertyNames.HasDropDownButton, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return !this.win.HasStyle(1);
                }
                if (string.Equals(WinDateTimePicker.PropertyNames.HasSpinner, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return this.win.HasStyle(1);
                }
                if (!string.Equals(WinDateTimePicker.PropertyNames.Format, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (this.win.HasStyle(4))
                {
                    return DateTimePickerFormat.Long;
                }
                if (this.win.HasStyle(0))
                {
                    return DateTimePickerFormat.Short;
                }
                if (this.win.HasStyle(8))
                {
                    return DateTimePickerFormat.Time;
                }
                return DateTimePickerFormat.Custom;
            }
            return null;
        }

        private object GetEditProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (UIControl.TechnologyElement.IsPassword && !string.Equals(WinEdit.PropertyNames.IsPassword,
                    propertyName, StringComparison.OrdinalIgnoreCase))
            {
                object[] objArray2 = new object[] { technologyName };
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
                    Resources.GetPropertyFailedOnPassword, objArray2));
            }
            if (string.Equals(WinEdit.PropertyNames.LineCount, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0xba);
            }
            if (string.Equals(WinEdit.PropertyNames.MaxLength, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0xd5);
            }
            if (string.Equals(WinEdit.PropertyNames.ReadOnly, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.ReadOnly);
            }
            if (string.Equals(WinEdit.PropertyNames.InsertionIndexAbsolute, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (this.win.GetInt(0xb0, 0, 0) & 0xffff);
            }
            if (string.Equals(WinEdit.PropertyNames.InsertionIndexLineRelative, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                int num2 = this.win.GetInt(0xb0, 0, 0) & 0xffff;
                int num3 = this.win.GetInt(0xbb, -1, 0);
                return (num2 - num3);
            }
            if (string.Equals(WinEdit.PropertyNames.SelectionStart, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetSelectionRange(0x434, 0xb0).cpMin;
            }
            if (string.Equals(WinEdit.PropertyNames.SelectionEnd, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetSelectionRange(0x434, 0xb0).cpMax;
            }
            if (string.Equals(WinEdit.PropertyNames.SelectionText, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (UIControl.TechnologyElement.Value == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                    return null;
                }
                WinNativeMethods.CHARRANGE selectionRange = this.win.GetSelectionRange(0x434, 0xb0);
                return UIControl.TechnologyElement.Value.Substring(selectionRange.cpMin,
                    selectionRange.cpMax - selectionRange.cpMin);
            }
            if (string.Equals(WinEdit.PropertyNames.CurrentLine, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0xc9, -1, 0);
            }
            if (string.Equals(WinEdit.PropertyNames.IsPassword, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.IsPassword;
            }
            if (string.Equals(WinEdit.PropertyNames.Text, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!UIControl.TechnologyElement.IsPassword)
                {
                    return UIControl.TechnologyElement.Value;
                }
                ALUtility.ThrowNotSupportedException(true);
            }
            return null;
        }

        private object GetHyperLinkProperty(string propertyName)
        {
            if (string.Equals(propertyName, WinHyperlink.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetText();
            }
            return null;
        }

        private object GetListBoxProperty(string propertyName)
        {
            List<string> list;
            object[] args = new object[] { propertyName };
                        if ((string.Equals(WinList.PropertyNames.IsListView, propertyName, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(WinList.PropertyNames.IsIconView, propertyName, StringComparison.OrdinalIgnoreCase)) ||
                (string.Equals(WinList.PropertyNames.IsSmallIconView, propertyName,
                     StringComparison.OrdinalIgnoreCase) || string.Equals(WinList.PropertyNames.IsReportView,
                     propertyName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            if (string.Equals(WinList.PropertyNames.IsMultipleSelection, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (this.win.HasStyle(8) ? ((object)1) : ((object)this.win.HasStyle(0x800)));
            }
            if (string.Equals(WinList.PropertyNames.IsCheckedList, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.IsCheckedList();
            }
            if (string.Equals(WinList.PropertyNames.CheckedItems, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(true);
                    return null;
                }
                list = new List<string>();
                foreach (ZappyTaskControl control in UIControl.GetChildren())
                {
                    if ((control.StateValue & ControlStates.Checked) == ControlStates.Checked)
                    {
                        list.Add(control.Name);
                    }
                }
                return list.ToArray();
            }
            if (string.Equals(WinList.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection children = UIControl.GetChildren();
                children.RemoveAll(element =>
                    !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                    !element.ControlType.NameEquals(ControlType.CheckBox.Name));
                return children;
            }
            if (string.Equals(WinList.PropertyNames.SelectedItems, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                list = new List<string>();
                int wParam = 0;
                foreach (ZappyTaskControl control2 in UIControl.GetChildren())
                {
                    if (this.win.GetInt(0x187, wParam, 0) > 0)
                    {
                        list.Add(control2.Name);
                    }
                    wParam++;
                }
                return list.ToArray();
            }
            if (string.Equals(WinTree.PropertyNames.HorizontalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl parent = UIControl.GetParent();
                return this.GetWinScrollBar(parent, 0);
            }
            if (string.Equals(WinTree.PropertyNames.VerticalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl uiControl = UIControl.GetParent();
                return this.GetWinScrollBar(uiControl, 1);
            }
            if (string.Equals(WinList.PropertyNames.CheckedIndices, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(true);
                    return null;
                }
                List<int> list2 = new List<int>();
                ZappyTaskControlCollection controls2 = UIControl.GetChildren();
                controls2.RemoveAll(element =>
                    !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                    !element.ControlType.NameEquals(ControlType.CheckBox.Name));
                int item = 0;
                foreach (ZappyTaskControl control5 in controls2)
                {
                    if ((control5.StateValue & ControlStates.Checked) == ControlStates.Checked)
                    {
                        list2.Add(item);
                    }
                    item++;
                }
                return list2.ToArray();
            }
            if (string.Equals(WinList.PropertyNames.SelectedIndices, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.GetSelectedIndicesForList();
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinList.PropertyNames.SelectedItemsAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                string[] stringCollection = null;
                if (this.win.IsCheckedList())
                {
                    stringCollection = this.GetListBoxProperty(WinList.PropertyNames.CheckedItems) as string[];
                }
                else
                {
                    stringCollection = this.GetListBoxProperty(WinList.PropertyNames.SelectedItems) as string[];
                }
                CommaListBuilder builder = new CommaListBuilder();
                if (stringCollection != null)
                {
                    builder.AddRange(stringCollection);
                }
                return builder.ToString();
            }
            ALUtility.ThrowNotSupportedException(false);
            return null;
        }

        private object GetListViewItemProperty(string propertyName)
        {
            if (string.Equals(propertyName, WinListItem.PropertyNames.DisplayText,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName,
                    ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.GetProperty(ZappyTaskControl.PropertyNames.Name);
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (string.Equals(propertyName, WinListItem.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Selected);
            }
            return null;
        }

        private object GetListViewProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinList.PropertyNames.IsMultipleSelection, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return !this.win.HasStyle(4);
            }
            if (string.Equals(WinList.PropertyNames.IsIconView, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((this.win.GetStyle() & 3) == 0);
            }
            if (string.Equals(WinList.PropertyNames.IsSmallIconView, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((this.win.GetStyle() & 3) == 2);
            }
            if (string.Equals(WinList.PropertyNames.IsListView, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((this.win.GetStyle() & 3) == 3);
            }
            if (string.Equals(WinList.PropertyNames.IsReportView, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((this.win.GetStyle() & 3) == 1);
            }
            if (string.Equals(WinList.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls = UIControl.GetChildren();
                controls.RemoveAll(element =>
                    !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                    !element.ControlType.NameEquals(ControlType.CheckBox.Name));
                return controls;
            }
            if (string.Equals(WinList.PropertyNames.SelectedItems, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                List<string> list = new List<string>();
                int wParam = 0;
                foreach (ZappyTaskControl control in UIControl.GetChildren())
                {
                    if (this.win.GetInt(0x102c, wParam, 2) == 2)
                    {
                        list.Add(control.Name);
                    }
                    wParam++;
                }
                return list.ToArray();
            }
            if (string.Equals(WinList.PropertyNames.SelectedItemsAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(ZappyTaskControl.PropertyNames.Value,
                    propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string[] listViewProperty = this.GetListViewProperty(WinList.PropertyNames.SelectedItems) as string[];
                CommaListBuilder builder = new CommaListBuilder();
                if (listViewProperty != null)
                {
                    builder.AddRange(listViewProperty);
                }
                return builder.ToString();
            }
            if (string.Equals(WinTree.PropertyNames.HorizontalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl parent = UIControl.GetParent();
                return this.GetWinScrollBar(parent, 0);
            }
            if (string.Equals(WinTree.PropertyNames.VerticalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl control3 = UIControl.GetParent();
                return this.GetWinScrollBar(control3, 1);
            }
            if (!string.Equals(WinList.PropertyNames.Columns, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(propertyName, WinList.PropertyNames.SelectedIndices,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return this.GetSelectedIndicesForList();
                }
                ALUtility.ThrowNotSupportedException(false);
                return null;
            }
            IntPtr @int = (IntPtr)this.win.GetInt(0x101f);
            ZappyTaskControl uiControl = ZappyTaskControlFactory.FromWindowHandle(@int);
            int num2 = new WindowsControl(uiControl).GetInt(0x1200);
            ZappyTaskControlCollection children = uiControl.GetChildren();
            ZappyTaskControl control6 = null;
            foreach (ZappyTaskControl control7 in children)
            {
                if (control7.ControlType == ControlType.List)
                {
                    control6 = control7;
                    break;
                }
            }
            if (control6 != null)
            {
                return control6.GetChildren();
            }
            return new ZappyTaskControlCollection();
        }

        private object GetMenuBarProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (!string.Equals(WinMenu.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            ZappyTaskControlCollection children = UIControl.GetChildren();
            ZappyTaskControlCollection controls2 = new ZappyTaskControlCollection();
            foreach (ZappyTaskControl control in children)
            {
                if (((control.ControlType == ControlType.MenuItem) || (control.ControlType == ControlType.ComboBox)) ||
                    (control.ControlType == ControlType.Edit))
                {
                    controls2.Add(control);
                }
            }
            return controls2;
        }

        private object GetMenuItemProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinMenuItem.PropertyNames.Checked, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Checked == (UIControl.StateValue & ControlStates.Checked));
            }
            if (string.Equals(WinMenuItem.PropertyNames.HasChildNodes, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return (UIControl.GetChildren().Count > 0);
            }
            if (string.Equals(WinMenuItem.PropertyNames.IsTopLevelMenu, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return ((UIControl.ControlType == ControlType.Menu) ||
                        (UIControl.GetParent().ControlType != ControlType.MenuItem));
            }
            if (string.Equals(WinMenuItem.PropertyNames.Shortcut, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue(WinControl.PropertyNames.AccessKey);
            }
            if (string.Equals(WinMenuItem.PropertyNames.AcceleratorKey, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.GetAcceleratorKeyForMenuItem();
            }
            if (string.Equals(WinMenuItem.PropertyNames.DisplayText, propertyName,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(ZappyTaskControl.PropertyNames.Value,
                    propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.Name;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (!string.Equals(WinMenuItem.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            ZappyTaskControlCollection children = UIControl.GetChildren();
            if (((children.Count == 1) && ControlType.Window.NameEquals(children[0].ControlType.Name)) &&
                string.Equals(children[0].ClassNameValue, "#32768", StringComparison.Ordinal))
            {
                children = children[0].GetChildren();
            }
            if ((children.Count == 1) && ControlType.Menu.NameEquals(children[0].ControlType.Name))
            {
                children = children[0].GetChildren();
            }
            children.RemoveAll(element => !element.ControlType.NameEquals(ControlType.MenuItem.Name));
            return children;
        }

        private bool GetMinAndMaxDateRange(out DateTime min, out DateTime max, int flag)
        {
            NativeMethods.SYSTEMTIMEARRAY systemtimearray = new NativeMethods.SYSTEMTIMEARRAY();
            if (this.win.GetGeneric<NativeMethods.SYSTEMTIMEARRAY>(flag, ref systemtimearray) != 0)
            {
                systemtimearray.ToDateTimeRange(out min, out max);
                return true;
            }
            min = new DateTime();
            max = new DateTime();
            return false;
        }

        private object GetMonthCalenderProperty(string propertyName)
        {
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinCalendar.PropertyNames.SelectionRangeAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                DateTime time;
                DateTime time2;
                if (this.GetMinAndMaxDateRange(out time, out time2, 0x1005))
                {
                    return ZappyTaskUtilities.GetShortDateRangeString(time, time2);
                }
            }
            else
            {
                DateTime time3;
                DateTime time4;
                if (string.Equals(WinCalendar.PropertyNames.SelectionRange, propertyName,
                        StringComparison.OrdinalIgnoreCase) && this.GetMinAndMaxDateRange(out time3, out time4, 0x1005))
                {
                    return new SelectionRange(time3, time4);
                }
            }
            return null;
        }

        private object GetNumericUpDownProperty(string propertyName, TaskActivityElement textBox)
        {
            if ((!string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(propertyName, WinComboBox.PropertyNames.EditableItem,
                     StringComparison.OrdinalIgnoreCase)) && !string.Equals(propertyName,
                    WinComboBox.PropertyNames.SelectedItem, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return textBox.Value;
        }

        private object GetParentNodeForTreeNode()
        {
            QueryElement queryId = UIControl.TechnologyElement.QueryId as QueryElement;
            if ((queryId == null) || (queryId.Ancestor == null))
            {
                return null;
            }
            ZappyTaskControl control = ZappyTaskControl.FromTechnologyElement(queryId.Ancestor);
            if ((ControlType.TreeItem != control.ControlType) && (ControlType.CheckBoxTreeItem != control.ControlType))
            {
                return null;
            }
            return control;
        }

        private object GetProgressBarProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinProgressBar.PropertyNames.MinimumValue, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x407, 1, 0);
            }
            if (string.Equals(WinProgressBar.PropertyNames.MaximumValue, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x407);
            }
            if (string.Equals(WinProgressBar.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x408);
            }
            return null;
        }

        protected override string GetPropertyForAction(string controlType, ZappyTaskAction action)
        {
            SetValueAction action2 = action as SetValueAction;
            if (action2 == null)
            {
                throw new NotSupportedException();
            }
            if (ControlType.Edit.NameEquals(controlType))
            {
                if (action2.IsActionOnProtectedElement())
                {
                    return WinEdit.PropertyNames.Password;
                }
                return WinEdit.PropertyNames.Text;
            }
            if (ControlType.ComboBox.NameEquals(controlType))
            {
                if (action2.PreferEdit)
                {
                    return WinComboBox.PropertyNames.EditableItem;
                }
                return WinComboBox.PropertyNames.SelectedItem;
            }
            if (ControlType.Cell.NameEquals(controlType))
            {
                return WinCell.PropertyNames.Value;
            }
            if (ControlType.List.NameEquals(controlType))
            {
                return WinList.PropertyNames.SelectedItemsAsString;
            }
            if (ControlType.Slider.NameEquals(controlType))
            {
                return WinSlider.PropertyNames.PositionAsString;
            }
            if (ControlType.ScrollBar.NameEquals(controlType))
            {
                return WinScrollBar.PropertyNames.Position;
            }
            if (ControlType.DateTimePicker.NameEquals(controlType))
            {
                return WinDateTimePicker.PropertyNames.DateTimeAsString;
            }
            if (ControlType.Calendar.NameEquals(controlType))
            {
                return WinCalendar.PropertyNames.SelectionRangeAsString;
            }
            return null;
        }

        protected override string[] GetPropertyForControlState(string controlType, ControlStates uiState,
            out bool[] stateValues)
        {
            List<string> list = new List<string>();
            List<bool> list2 = new List<bool>();
            if (ControlType.CheckBoxTreeItem.NameEquals(controlType))
            {
                if ((uiState & (ControlStates.None | ControlStates.Selected)) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Selected);
                    list2.Add(true);
                }
                if ((uiState & ControlStates.Checked) != ControlStates.None)
                {
                    list.Add(WinCheckBoxTreeItem.PropertyNames.Checked);
                    list2.Add(true);
                }
                if ((uiState & ControlStates.Expanded) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Expanded);
                    list2.Add(true);
                }
                if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
                {
                    list.Add(WinCheckBoxTreeItem.PropertyNames.Checked);
                    list2.Add(false);
                }
                if ((uiState & ControlStates.Collapsed) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Expanded);
                    list2.Add(false);
                }
            }
            else if (ControlType.CheckBox.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Checked) != ControlStates.None)
                {
                    list.Add(WinCheckBox.PropertyNames.Checked);
                    list2.Add(true);
                }
                else if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
                {
                    list.Add(WinCheckBox.PropertyNames.Checked);
                    list2.Add(false);
                }
                else if ((uiState & ControlStates.Indeterminate) != ControlStates.None)
                {
                    list.Add(WinCheckBox.PropertyNames.Indeterminate);
                    list2.Add(true);
                }
            }
            else if (ControlType.TreeItem.NameEquals(controlType))
            {
                if ((uiState & (ControlStates.None | ControlStates.Selected)) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Selected);
                    list2.Add(true);
                }
                if ((uiState & ControlStates.Expanded) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Expanded);
                    list2.Add(true);
                }
                if ((uiState & ControlStates.Collapsed) != ControlStates.None)
                {
                    list.Add(WinTreeItem.PropertyNames.Expanded);
                    list2.Add(false);
                }
            }
            else if (ControlType.RadioButton.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Checked) != ControlStates.None)
                {
                    list.Add(WinRadioButton.PropertyNames.Selected);
                    list2.Add(true);
                }
            }
            else if (ControlType.Cell.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Checked) != ControlStates.None)
                {
                    list.Add(WinCell.PropertyNames.Checked);
                    list2.Add(true);
                }
                else if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
                {
                    list.Add(WinCell.PropertyNames.Checked);
                    list2.Add(false);
                }
                else if ((uiState & ControlStates.Indeterminate) != ControlStates.None)
                {
                    list.Add(WinCell.PropertyNames.Indeterminate);
                    list2.Add(true);
                }
            }
            else if (ControlType.Window.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Maximized) != ControlStates.None)
                {
                    list.Add(WinWindow.PropertyNames.Maximized);
                    list2.Add(true);
                }
                else if ((uiState & ControlStates.Minimized) != ControlStates.None)
                {
                    list.Add(WinWindow.PropertyNames.Minimized);
                    list2.Add(true);
                }
                else if ((uiState & (ControlStates.None | ControlStates.Restored)) != ControlStates.None)
                {
                    list.Add(WinWindow.PropertyNames.Restored);
                    list2.Add(true);
                }
            }
            else if (ControlType.MenuItem.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Checked) != ControlStates.None)
                {
                    list.Add(WinMenuItem.PropertyNames.Checked);
                    list2.Add(true);
                }
                else if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
                {
                    list.Add(WinMenuItem.PropertyNames.Checked);
                    list2.Add(false);
                }
            }
            stateValues = list2.ToArray();
            return list.ToArray();
        }

        public override object GetPropertyValue(ZappyTaskControl uiControl, string propertyName)
        {
            if (!ZappyTaskControl.PropertyNames.Enabled.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiControl, propertyName);
            }
            if ((bool)uiControl.TechnologyElement.GetPropertyValue("_isWin32ForSure"))
            {
                return NativeMethods.IsWindowEnabled(uiControl.TechnologyElement.WindowHandle);
            }
            return !TaskActivityElement.IsState(uiControl.TechnologyElement, AccessibleStates.Unavailable);
        }

        protected override object GetPropertyValueInternal(ZappyTaskControl uiControl, string propertyName)
        {
            this.win = new WindowsControl(uiControl);
            UIControl = uiControl;
            if ((WinControl.PropertyNames.HelpText.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ||
                 WinControl.PropertyNames.ControlName.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) ||
                (WinControl.PropertyNames.ControlId.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ||
                 WinControl.PropertyNames.AccessibleDescription.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                ))
            {
                return this.GetCommonProperty(propertyName);
            }
            ZappyTaskPropertyDescriptor propertyDescriptor = this.GetPropertyDescriptor(uiControl, propertyName);
            if (!this.IsCommonReadableProperty(uiControl.ControlType, propertyName) &&
                ((propertyDescriptor == null) || ((propertyDescriptor.Attributes & ZappyTaskPropertyAttributes.Readable) ==
                                                  ZappyTaskPropertyAttributes.None)))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            ControlType controlType = uiControl.ControlType;
            if ((ControlType.Button == controlType) || (ControlType.SplitButton == controlType))
            {
                return this.GetButtonProperty(propertyName);
            }
            if (ControlType.Hyperlink == controlType)
            {
                return this.GetHyperLinkProperty(propertyName);
            }
            if (ControlType.List == controlType)
            {
                if (this.win.IsListView())
                {
                    return this.GetListViewProperty(propertyName);
                }
                return this.GetListBoxProperty(propertyName);
            }
            if (ControlType.ListItem == controlType)
            {
                return this.GetListViewItemProperty(propertyName);
            }
            if (ControlType.Spinner == controlType)
            {
                return this.GetSpinnerProperty(propertyName);
            }
            if (ControlType.ProgressBar == controlType)
            {
                return this.GetProgressBarProperty(propertyName);
            }
            if (ControlType.Slider == controlType)
            {
                return this.GetSliderProperty(propertyName);
            }
            if (ControlType.StatusBar == controlType)
            {
                return this.GetStatusBarProperty(propertyName);
            }
            if (ControlType.Edit == controlType)
            {
                return this.GetEditProperty(propertyName);
            }
            if (ControlType.DateTimePicker == controlType)
            {
                return this.GetDateTimePickerProperty(propertyName);
            }
            if (ControlType.Calendar == controlType)
            {
                return this.GetMonthCalenderProperty(propertyName);
            }
            if (ControlType.Tree == controlType)
            {
                return this.GetTreeViewProperty(propertyName);
            }
            if (ControlType.Window == controlType)
            {
                return this.GetWindowProperty(propertyName);
            }
            if (ControlType.TreeItem == controlType)
            {
                return this.GetTreeNodeProperty(propertyName);
            }
            if (ControlType.CheckBoxTreeItem == controlType)
            {
                return this.GetCheckBoxTreeNodeProperty(propertyName);
            }
            if (ControlType.CheckBox == controlType)
            {
                return this.GetCheckBoxProperty(propertyName);
            }
            if (ControlType.ComboBox == controlType)
            {
                return this.GetComboBoxProperty(propertyName);
            }
            if (ControlType.RadioButton == controlType)
            {
                return this.GetRadioButtonProperty(propertyName);
            }
            if (ControlType.TabList == controlType)
            {
                return this.GetTabListProperty(propertyName);
            }
            if (ControlType.TabPage == controlType)
            {
                return this.GetTabPageProperty(propertyName);
            }
            if (ControlType.Table == controlType)
            {
                return this.GetTableProperty(propertyName);
            }
            if (ControlType.Row == controlType)
            {
                return this.GetTableRowProperty(propertyName);
            }
            if (ControlType.RowHeader == controlType)
            {
                return this.GetRowHeaderProperty(propertyName);
            }
            if (ControlType.Cell == controlType)
            {
                return this.GetTableCellProperty(propertyName);
            }
            if (ControlType.MenuItem == controlType)
            {
                return this.GetMenuItemProperty(propertyName);
            }
            if ((ControlType.MenuBar == controlType) || (ControlType.Menu == controlType))
            {
                return this.GetMenuBarProperty(propertyName);
            }
            if (ControlType.ScrollBar == controlType)
            {
                return this.GetScrollBarProperty(propertyName);
            }
            if (ControlType.ToolBar == controlType)
            {
                return this.GetToolBarProperty(propertyName);
            }
            if (ControlType.Text == controlType)
            {
                return this.GetTextProperty(propertyName);
            }
            if (ControlType.TitleBar == controlType)
            {
                return this.GetTitleBarProperty(propertyName);
            }
            ALUtility.ThrowNotSupportedException(true);
            return null;
        }

        private object GetRadioButtonProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinRadioButton.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Checked == (UIControl.StateValue & ControlStates.Checked));
            }
            if (string.Equals(WinRadioButton.PropertyNames.Group, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl parent = UIControl.GetParent();
                if (ControlType.Group.NameEquals(parent.ControlType.Name))
                {
                    return parent;
                }
                if (ControlType.Window.NameEquals(parent.ControlType.Name))
                {
                    parent = parent.GetParent();
                    if ((parent != null) && ControlType.Group.NameEquals(parent.ControlType.Name))
                    {
                        return parent;
                    }
                }
                return null;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            return null;
        }

        private int GetRelativeCellClickablePoint(ZappyTaskControl cellControl, ZappyTaskControl rowControl,
            Rectangle cellBounds)
        {
            int num = -1;
            TaskActivityElement technologyElement = rowControl.TechnologyElement;
            TaskActivityElement firstChild = ZappyTaskService.Instance.GetFirstChild(technologyElement);
            if ((firstChild != null) && ControlType.RowHeader.NameEquals(firstChild.ControlTypeName))
            {
                int num2;
                int num3;
                int num4;
                int num5;
                firstChild.GetBoundingRectangle(out num2, out num3, out num4, out num5);
                if ((((num2 != -1) && (num3 != -1)) && ((num4 != -1) && (num5 != -1))) &&
                    (((num2 + num4) > cellBounds.Left) && ((num2 + num4) < (cellBounds.Left + cellBounds.Width))))
                {
                    num = cellBounds.Width - 5;
                }
            }
            else
            {
                int num6;
                int num7;
                int num8;
                int num9;
                technologyElement.GetBoundingRectangle(out num6, out num7, out num8, out num9);
                Rectangle rectangle = new Rectangle(num6, num7, num8, num9);
                if ((((num6 != -1) && (num7 != -1)) && ((num8 != -1) && (num9 != -1))) &&
                    !rectangle.Contains(cellBounds))
                {
                    num = cellBounds.Width - 5;
                }
            }
            if (num <= 0)
            {
                num = cellBounds.Width / 2;
            }
            return num;
        }

        private object GetRowHeaderProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinRowHeader.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Selected);
            }
            return null;
        }

        private object GetScrollBarProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        WinNativeMethods.SCROLLINFO structure = new WinNativeMethods.SCROLLINFO
            {
                fMask = 0x17
            };
            structure.cbSize = (uint)Marshal.SizeOf(structure);
            int fnBar = this.IsScrollBarVertical() ? 1 : 0;
            if (!this.win.GetScrollInfo(fnBar, ref structure) && !this.win.GetScrollInfo(2, ref structure))
            {
                ALUtility.ThrowNotSupportedException(false);
            }
            if (string.Equals(propertyName, WinScrollBar.PropertyNames.MaximumPosition,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(UIControl.ClassNameValue) &&
                    UIControl.ClassNameValue.ToUpperInvariant().Contains("RICHTEXT"))
                {
                    return (structure.nMax - structure.nPage);
                }
                return ((structure.nMax - ((int)structure.nPage)) + ((structure.nPage > 0) ? 1 : 0));
            }
            if (string.Equals(propertyName, WinScrollBar.PropertyNames.MinimumPosition,
                StringComparison.OrdinalIgnoreCase))
            {
                return structure.nMin;
            }
            if (!string.Equals(propertyName, WinScrollBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return structure.nPos;
        }

        private int[] GetSelectedIndicesForList()
        {
            List<int> list = new List<int>();
            ZappyTaskControlCollection children = UIControl.GetChildren();
            children.RemoveAll(element =>
                !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                !element.ControlType.NameEquals(ControlType.CheckBox.Name));
            int wParam = 0;
            if (this.win.IsListView())
            {
                foreach (ZappyTaskControl control in children)
                {
                    if (this.win.GetInt(0x102c, wParam, 2) == 2)
                    {
                        list.Add(wParam);
                    }
                    wParam++;
                }
            }
            else
            {
                foreach (ZappyTaskControl control2 in children)
                {
                    if (this.win.GetInt(0x187, wParam, 0) > 0)
                    {
                        list.Add(wParam);
                    }
                    wParam++;
                }
            }
            return list.ToArray();
        }

        private object GetSliderProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(propertyName, WinSlider.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                double num;
                if (this.win.GetInt(0x402) != this.win.GetInt(0x401))
                {
                    return (double)this.win.GetInt(0x400);
                }
                if (double.TryParse(UIControl.TechnologyElement.Value, out num))
                {
                    return num;
                }
                return null;
            }
            if (string.Equals(propertyName, WinSlider.PropertyNames.PositionAsString,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName,
                    ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                double num2;
                if (this.win.GetInt(0x402) != this.win.GetInt(0x401))
                {
                    return this.win.GetInt(0x400).ToString(CultureInfo.InvariantCulture);
                }
                if (double.TryParse(UIControl.TechnologyElement.Value, out num2))
                {
                    return num2.ToString(CultureInfo.InvariantCulture);
                }
                return null;
            }
            if (string.Equals(WinSlider.PropertyNames.MinimumPosition, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x401);
            }
            if (string.Equals(WinSlider.PropertyNames.MaximumPosition, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                int @int = this.win.GetInt(0x402);
                if (@int == this.win.GetInt(0x401))
                {
                    return 100;
                }
                return @int;
            }
            if (string.Equals(WinSlider.PropertyNames.TickValue, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x403);
            }
            if (string.Equals(WinSlider.PropertyNames.TickCount, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x410);
            }
            if (string.Equals(WinSlider.PropertyNames.TickPosition, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x40f);
            }
            if (string.Equals(WinSlider.PropertyNames.PageSize, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x416);
            }
            if (string.Equals(WinSlider.PropertyNames.LineSize, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x418);
            }
            return null;
        }

        private object GetSpinnerProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinSpinner.PropertyNames.MinimumValue, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (this.win.GetInt(0x466, 0, 0) & 0x7fff);
            }
            if (string.Equals(WinSpinner.PropertyNames.MaximumValue, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                int num = this.win.GetInt(0x466, 0, 0);
                return ((num - (num & 0x7fff)) / 0x10000);
            }
            return null;
        }

        private object GetStatusBarProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (!string.Equals(WinStatusBar.PropertyNames.Panels, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            foreach (ZappyTaskControl control in UIControl.GetChildren())
            {
                if (!this.IsInvisible(control))
                {
                    controls.Add(control);
                }
            }
            return controls;
        }

        private string[] GetStringArrayForSelectedIndices(int[] selectedIndices, string propertyName)
        {
            List<string> list = new List<string>();
            ZappyTaskControlCollection children = UIControl.GetChildren();
            children.RemoveAll(element =>
                !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                !element.ControlType.NameEquals(ControlType.CheckBox.Name));
            foreach (int num2 in selectedIndices)
            {
                if (num2 < children.Count)
                {
                    list.Add(children[num2].Name);
                }
                else
                {
                    object[] args = new object[] { num2, UIControl.ControlType.Name, propertyName };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValue, args));
                }
            }
            return list.ToArray();
        }

        private object GetTableCellProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinCell.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.Value;
            }
            if (string.Equals(WinCell.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Selected);
            }
            if (WinCell.PropertyNames.Checked.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(UIControl.GetProperty("HelpText") as string,
                    "DataGridViewCheckBoxCell(DataGridViewCell)", StringComparison.OrdinalIgnoreCase))
                {
                    object[] objArray2 = new object[] { propertyName, UIControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                        Resources.GetPropertyNotSupportedMessage, objArray2));
                }
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked);
            }
            if (WinCell.PropertyNames.Indeterminate.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(UIControl.GetProperty("HelpText") as string,
                    "DataGridViewCheckBoxCell(DataGridViewCell)", StringComparison.OrdinalIgnoreCase))
                {
                    object[] objArray3 = new object[] { propertyName, UIControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                        Resources.GetPropertyNotSupportedMessage, objArray3));
                }
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Indeterminate);
            }
            if (WinCell.PropertyNames.ColumnIndex.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl control = UIControl.GetParent();
                if ((control == null) || (control.ControlType != ControlType.Row))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                return UIControl.TechnologyElement.GetPropertyValue("_LightWeightInstance");
            }
            if (!WinCell.PropertyNames.RowIndex.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            ZappyTaskControl parent = UIControl.GetParent();
            if ((parent == null) || (parent.ControlType != ControlType.Row))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            return parent.TechnologyElement.GetPropertyValue("_LightWeightInstance");
        }

        private object GetTableProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinTable.PropertyNames.Rows, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls2 = new ZappyTaskControlCollection();
                foreach (ZappyTaskControl control in ALUtility.GetDescendantsByControlType(UIControl,
                    technologyName, ControlType.Row, 1))
                {
                    if ((ControlStates.None | ControlStates.Selectable) ==
                        (((ControlStates)control.GetProperty(ZappyTaskControl.PropertyNames.State)) &
                         (ControlStates.None | ControlStates.Selectable)))
                    {
                        controls2.Add(control);
                    }
                }
                return controls2;
            }
            if (string.Equals(WinTable.PropertyNames.RowHeaders, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls3 = new ZappyTaskControlCollection();
                foreach (ZappyTaskControl control2 in ALUtility.GetDescendantsByControlType(UIControl,
                    technologyName, ControlType.Row, 1))
                {
                    WinRowHeader item = new WinRowHeader(control2)
                    {
                        SearchProperties =
                        {
                            {
                                ZappyTaskControl.PropertyNames.MaxDepth,
                                "1"
                            }
                        }
                    };
                    try
                    {
                        item.Find();
                        controls3.Add(item);
                    }
                    catch (Exception)
                    {
                    }
                }
                return controls3;
            }
            if (string.Equals(WinTable.PropertyNames.ColumnHeaders, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl uiControl = new ZappyTaskControl(UIControl)
                {
                    SearchProperties =
                    {
                        {
                            ZappyTaskControl.PropertyNames.ControlType,
                            ControlType.Row.Name
                        },
                        {
                            ZappyTaskControl.PropertyNames.MaxDepth,
                            "1"
                        }
                    },
                    TechnologyName = technologyName
                };
                try
                {
                    uiControl.Find();
                    return ALUtility.GetDescendantsByControlType(uiControl, technologyName,
                        ControlType.ColumnHeader, 1);
                }
                catch (Exception)
                {
                }
                return new ZappyTaskControlCollection();
            }
            if (string.Equals(WinTable.PropertyNames.Cells, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Cell, 2);
            }
            if (string.Equals(WinTable.PropertyNames.HorizontalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.GetWinScrollBar(UIControl, 0);
            }
            if (string.Equals(WinTable.PropertyNames.VerticalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return this.GetWinScrollBar(UIControl, 1);
            }
            return null;
        }

        private object GetTableRowProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinRow.PropertyNames.Cells, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Cell, -1);
            }
            if (string.Equals(WinRow.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.Value;
            }
            if (string.Equals(WinRow.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Selected);
            }
            if (string.Equals(WinRow.PropertyNames.RowIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue("_LightWeightInstance");
            }
            return null;
        }

        private object GetTabListProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinTabList.PropertyNames.SelectedIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetInt(0x130b);
            }
            if (string.Equals(WinTabList.PropertyNames.Tabs, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls =
                    ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.TabPage, 1);
                foreach (ZappyTaskControl control in controls)
                {
                    if (this.IsInvisible(control))
                    {
                        controls.Remove(control);
                    }
                }
                return controls;
            }
            if (string.Equals(WinTabList.PropertyNames.TabSpinner, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls2 =
                    ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Spinner, 2);
                if (controls2.Count == 1)
                {
                    return controls2[0];
                }
            }
            return null;
        }

        private object GetTabPageProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(propertyName, WinTabPage.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.Name;
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            return null;
        }

        private object GetTextProperty(string propertyName)
        {
            if (string.Equals(WinText.PropertyNames.DisplayText, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.GetText();
            }
            return null;
        }

        private object GetTitleBarProperty(string propertyName)
        {
            if (string.Equals(propertyName, WinTitleBar.PropertyNames.DisplayText))
            {
                return this.win.GetText();
            }
            return null;
        }

        private object GetToolBarItemProperty(string propertyName)
        {
            if (string.Equals("Shortcut", propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.GetProperty(WinControl.PropertyNames.AccessKey);
            }
            if (string.Equals("Pressed", propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((UIControl.StateValue & (ControlStates.None | ControlStates.Pressed)) ==
                        (ControlStates.None | ControlStates.Pressed));
            }
            return null;
        }

        private object GetToolBarProperty(string propertyName)
        {
            if (!string.Equals(WinToolBar.PropertyNames.Items, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            foreach (ZappyTaskControl control in UIControl.GetChildren())
            {
                if ((!ControlType.Grip.NameEquals(control.ControlType.Name) &&
                     !ControlType.Separator.NameEquals(control.ControlType.Name)) && !this.IsInvisible(control))
                {
                    controls.Add(control);
                }
            }
            return controls;
        }

        private ZappyTaskControlCollection GetTreeItemNodes()
        {
            int num;
            int num2;
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            if (!int.TryParse(UIControl.TechnologyElement.Value, out num))
            {
                return controls;
            }
            ZappyTaskControl nextSibling = UIControl.NextSibling;
            bool flag = ((nextSibling != null) && int.TryParse(nextSibling.TechnologyElement.Value, out num2)) &&
                        (num != num2);
            num2 = num;
            if (flag)
            {
                for (ZappyTaskControl control2 = nextSibling; control2 != null; control2 = control2.NextSibling)
                {
                    if (!int.TryParse(control2.TechnologyElement.Value, out num))
                    {
                        return controls;
                    }
                    if (num == (num2 + 1))
                    {
                        if (controls.Contains(control2))
                        {
                            return controls;
                        }
                        controls.Add(control2);
                    }
                    else if (num2 == num)
                    {
                        return controls;
                    }
                }
                return controls;
            }
            return UIControl.GetChildren();
        }

        private object GetTreeNodeProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinTreeItem.PropertyNames.Expanded, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (ControlStates.Expanded == (UIControl.StateValue & ControlStates.Expanded));
            }
            if (string.Equals(WinTreeItem.PropertyNames.ParentNode, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.GetParentNodeForTreeNode();
            }
            if (string.Equals(WinTreeItem.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ((ControlStates.None | ControlStates.Selected) ==
                        (UIControl.StateValue & (ControlStates.None | ControlStates.Selected)));
            }
            if (string.Equals(WinTreeItem.PropertyNames.Nodes, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.GetTreeItemNodes();
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (string.Equals(WinTreeItem.PropertyNames.HasChildNodes, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return !this.IsLeafTreeItem(UIControl);
            }
            return null;
        }

        private object GetTreeViewProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinTree.PropertyNames.Nodes, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
                foreach (ZappyTaskControl control in UIControl.GetChildren())
                {
                    if ((ControlType.TreeItem.NameEquals(control.ControlType.Name) ||
                         ControlType.CheckBoxTreeItem.NameEquals(control.ControlType.Name)) &&
                        string.Equals(control.TechnologyElement.Value, "0", StringComparison.Ordinal))
                    {
                        controls.Add(control);
                    }
                }
                return controls;
            }
            if (string.Equals(WinTree.PropertyNames.HorizontalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl parent = UIControl.GetParent();
                return this.GetWinScrollBar(parent, 0);
            }
            if (string.Equals(WinTree.PropertyNames.VerticalScrollBar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl uiControl = UIControl.GetParent();
                return this.GetWinScrollBar(uiControl, 1);
            }
            return null;
        }

        private object GetWindowProperty(string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinWindow.PropertyNames.Resizable, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(0x40000);
            }
            if (string.Equals(WinWindow.PropertyNames.HasTitleBar, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(0xc00000);
            }
            if (string.Equals(WinWindow.PropertyNames.Popup, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(-2147483648);
            }
            if (string.Equals(WinWindow.PropertyNames.TabStop, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(0x10000);
            }
            if (string.Equals(WinWindow.PropertyNames.Transparent, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasExtendedStyle(0x20);
            }
            if (string.Equals(WinWindow.PropertyNames.Maximized, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(0x1000000);
            }
            if (string.Equals(WinWindow.PropertyNames.Minimized, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasStyle(0x20000000);
            }
            if (string.Equals(WinWindow.PropertyNames.Restored, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (this.win.HasStyle(0x1000000) ? ((object)0) : ((object)!this.win.HasStyle(0x20000000)));
            }
            if (string.Equals(WinWindow.PropertyNames.AlwaysOnTop, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.win.HasExtendedStyle(8);
            }
            if (string.Equals(WinWindow.PropertyNames.ShowInTaskbar, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return (!this.win.HasStyle(0x10000000)
                    ? ((object)0)
                    : (!this.win.HasOwnerWindow() ? ((object)1) : ((object)this.win.HasExtendedStyle(0x40000))));
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ScreenElement.GetWindowState(UIControl.TechnologyElement.WindowHandle);
            }
            if (string.Equals(WinWindow.PropertyNames.OrderOfInvocation, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (UIControl.FilterProperties.Contains(propertyName))
                {
                    return int.Parse(UIControl.FilterProperties[propertyName], CultureInfo.InvariantCulture);
                }
                return 0;
            }
            if (string.Equals(WinWindow.PropertyNames.AccessibleName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetPropertyValue(WinWindow.PropertyNames.AccessibleName);
            }
            return null;
        }

        private ZappyTaskControl GetWinScrollBar(ZappyTaskControl uiControl, int style)
        {
            ZappyTaskControl uIControl = UIControl;
            WindowsControl win = this.win;
            try
            {
                foreach (ZappyTaskControl control3 in ALUtility.GetDescendantsByControlType(uiControl, technologyName,
                    ControlType.ScrollBar, -1))
                {
                    UIControl = control3;
                    this.win = new WindowsControl(control3);
                    if (!TaskActivityElement.IsState(control3.TechnologyElement, AccessibleStates.Invisible))
                    {
                        bool flag = this.IsScrollBarVertical();
                        if (flag && (style == 1))
                        {
                            return control3;
                        }
                        if (!flag && (style == 0))
                        {
                            return control3;
                        }
                    }
                }
            }
            catch (NotSupportedException)
            {
                return null;
            }
            finally
            {
                UIControl = uIControl;
                this.win = win;
            }
            return null;
        }

        private static Dictionary<string, ZappyTaskPropertyDescriptor> InitializeCommonProperties() =>
            new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    WinControl.PropertyNames.HelpText,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    WinControl.PropertyNames.AccessKey,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    WinControl.PropertyNames.ControlName,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable |
                        ZappyTaskPropertyAttributes.Readable)
                },
                {
                    WinControl.PropertyNames.ControlId,
                    new ZappyTaskPropertyDescriptor(typeof(int),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable |
                        ZappyTaskPropertyAttributes.Readable)
                },
                {
                    WinControl.PropertyNames.AccessibleDescription,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable)
                }
            };

        private Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>> InitializePropertiesMap()
        {
            Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>> dictionary =
                new Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>>();
            ZappyTaskPropertyAttributes attributes = ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable;
            ZappyTaskPropertyAttributes attributes2 =
                ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable;
            Dictionary<string, ZappyTaskPropertyDescriptor> dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinButton.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinButton.PropertyNames.Shortcut,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.Button, dictionary2);
            dictionary.Add(ControlType.SplitButton, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinDateTimePicker.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinDateTimePicker.PropertyNames.HasDropDownButton,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinDateTimePicker.PropertyNames.HasCheckBox,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinDateTimePicker.PropertyNames.HasSpinner,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinDateTimePicker.PropertyNames.DateTime,
                        new ZappyTaskPropertyDescriptor(typeof(DateTime), attributes)
                    },
                    {
                        WinDateTimePicker.PropertyNames.DateTimeAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinDateTimePicker.PropertyNames.Calendar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinDateTimePicker.PropertyNames.ShowCalendar,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinDateTimePicker.PropertyNames.Format,
                        new ZappyTaskPropertyDescriptor(typeof(DateTimePickerFormat))
                    }
                };
            dictionary.Add(ControlType.DateTimePicker, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinEdit.PropertyNames.LineCount,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinEdit.PropertyNames.MaxLength,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinEdit.PropertyNames.InsertionIndexAbsolute,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinEdit.PropertyNames.InsertionIndexLineRelative,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinEdit.PropertyNames.SelectionStart,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinEdit.PropertyNames.SelectionEnd,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinEdit.PropertyNames.SelectionText,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinEdit.PropertyNames.IsPassword,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinEdit.PropertyNames.ReadOnly,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinEdit.PropertyNames.CurrentLine,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinEdit.PropertyNames.Text,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinEdit.PropertyNames.CopyPastedText,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinEdit.PropertyNames.Password,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Writable)
                    }
                };
            dictionary.Add(ControlType.Edit, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinList.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinList.PropertyNames.SelectedItems,
                        new ZappyTaskPropertyDescriptor(typeof(string[]), attributes | attributes2)
                    },
                    {
                        WinList.PropertyNames.SelectedItemsAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinList.PropertyNames.HorizontalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinList.PropertyNames.VerticalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinList.PropertyNames.CheckedItems,
                        new ZappyTaskPropertyDescriptor(typeof(string[]), attributes | attributes2)
                    },
                    {
                        WinList.PropertyNames.CheckedIndices,
                        new ZappyTaskPropertyDescriptor(typeof(int[]), attributes | attributes2)
                    },
                    {
                        WinList.PropertyNames.SelectedIndices,
                        new ZappyTaskPropertyDescriptor(typeof(int[]), attributes | attributes2)
                    },
                    {
                        WinList.PropertyNames.IsMultipleSelection,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinList.PropertyNames.IsCheckedList,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinList.PropertyNames.IsIconView,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinList.PropertyNames.Columns,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinList.PropertyNames.IsSmallIconView,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinList.PropertyNames.IsListView,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinList.PropertyNames.IsReportView,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    }
                };
            dictionary.Add(ControlType.List, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinHyperlink.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.Hyperlink, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinListItem.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinListItem.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    }
                };
            dictionary.Add(ControlType.ListItem, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinProgressBar.PropertyNames.MinimumValue,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinProgressBar.PropertyNames.MaximumValue,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinProgressBar.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    }
                };
            dictionary.Add(ControlType.ProgressBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinSlider.PropertyNames.Position,
                        new ZappyTaskPropertyDescriptor(typeof(double), attributes)
                    },
                    {
                        WinSlider.PropertyNames.PositionAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinSlider.PropertyNames.MaximumPosition,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.MinimumPosition,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.TickValue,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.TickPosition,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.TickCount,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.PageSize,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSlider.PropertyNames.LineSize,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    }
                };
            dictionary.Add(ControlType.Slider, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinStatusBar.PropertyNames.Panels,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    }
                };
            dictionary.Add(ControlType.StatusBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinWindow.PropertyNames.Resizable,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.HasTitleBar,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.Popup,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.TabStop,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.Transparent,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.AlwaysOnTop,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.Maximized,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinWindow.PropertyNames.Minimized,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinWindow.PropertyNames.ShowInTaskbar,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinWindow.PropertyNames.Restored,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinWindow.PropertyNames.OrderOfInvocation,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes2)
                    },
                    {
                        WinWindow.PropertyNames.AccessibleName,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.Window, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTree.PropertyNames.Nodes,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTree.PropertyNames.VerticalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinTree.PropertyNames.HorizontalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    }
                };
            dictionary.Add(ControlType.Tree, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTreeItem.PropertyNames.Expanded,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinTreeItem.PropertyNames.ParentNode,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinTreeItem.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinTreeItem.PropertyNames.Nodes,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTreeItem.PropertyNames.HasChildNodes,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    }
                };
            dictionary.Add(ControlType.TreeItem, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(dictionary2, StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinCheckBoxTreeItem.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinCheckBoxTreeItem.PropertyNames.Indeterminate,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    }
                };
            dictionary.Add(ControlType.CheckBoxTreeItem, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinCheckBox.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinCheckBox.PropertyNames.Indeterminate,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    }
                };
            dictionary.Add(ControlType.CheckBox, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinRadioButton.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinRadioButton.PropertyNames.Group,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    }
                };
            dictionary.Add(ControlType.RadioButton, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinComboBox.PropertyNames.Expanded,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinComboBox.PropertyNames.IsEditable,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinComboBox.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinComboBox.PropertyNames.SelectedIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinComboBox.PropertyNames.EditableItem,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinComboBox.PropertyNames.SelectedItem,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinComboBox.PropertyNames.VerticalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinComboBox.PropertyNames.HorizontalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    }
                };
            dictionary.Add(ControlType.ComboBox, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTabList.PropertyNames.SelectedIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinTabList.PropertyNames.Tabs,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTabList.PropertyNames.TabSpinner,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    }
                };
            dictionary.Add(ControlType.TabList, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTabPage.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.TabPage, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinMenu.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    }
                };
            dictionary.Add(ControlType.MenuBar, dictionary2);
            dictionary.Add(ControlType.Menu, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinMenuItem.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinMenuItem.PropertyNames.HasChildNodes,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinMenuItem.PropertyNames.IsTopLevelMenu,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinMenuItem.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinMenuItem.PropertyNames.Shortcut,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinMenuItem.PropertyNames.AcceleratorKey,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinMenuItem.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    }
                };
            dictionary.Add(ControlType.MenuItem, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTable.PropertyNames.Rows,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTable.PropertyNames.RowHeaders,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTable.PropertyNames.ColumnHeaders,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTable.PropertyNames.Cells,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinTable.PropertyNames.HorizontalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    },
                    {
                        WinTable.PropertyNames.VerticalScrollBar,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), attributes2)
                    }
                };
            dictionary.Add(ControlType.Table, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinRowHeader.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    }
                };
            dictionary.Add(ControlType.RowHeader, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinRow.PropertyNames.Cells,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    },
                    {
                        WinRow.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    },
                    {
                        WinRow.PropertyNames.RowIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinRow.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    }
                };
            dictionary.Add(ControlType.Row, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinCell.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        WinCell.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinCell.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool))
                    },
                    {
                        WinCell.PropertyNames.Indeterminate,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        WinCell.PropertyNames.RowIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinCell.PropertyNames.ColumnIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    }
                };
            dictionary.Add(ControlType.Cell, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinScrollBar.PropertyNames.Position,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        WinScrollBar.PropertyNames.MinimumPosition,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinScrollBar.PropertyNames.MaximumPosition,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    }
                };
            dictionary.Add(ControlType.ScrollBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinToolBar.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), attributes2)
                    }
                };
            dictionary.Add(ControlType.ToolBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinCalendar.PropertyNames.SelectionRange,
                        new ZappyTaskPropertyDescriptor(typeof(SelectionRange), attributes | attributes2)
                    },
                    {
                        WinCalendar.PropertyNames.SelectionRangeAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.Calendar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinTitleBar.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.TitleBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinText.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string))
                    }
                };
            dictionary.Add(ControlType.Text, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        WinSpinner.PropertyNames.MinimumValue,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    },
                    {
                        WinSpinner.PropertyNames.MaximumValue,
                        new ZappyTaskPropertyDescriptor(typeof(int))
                    }
                };
            dictionary.Add(ControlType.Spinner, dictionary2);
            dictionary.Add(ControlType.Client, commonProperties);
            dictionary.Add(ControlType.ColumnHeader, commonProperties);
            dictionary.Add(ControlType.Group, commonProperties);
            dictionary.Add(ControlType.Separator, commonProperties);
            dictionary.Add(ControlType.ToolTip, commonProperties);
            dictionary.Add(ControlType.Pane, commonProperties);
            dictionary.Add(ControlType.Label, commonProperties);
            return dictionary;
        }

        private Dictionary<ControlType, System.Type> InitializePropertyNameToClassMap() =>
            new Dictionary<ControlType, System.Type>
            {
                {
                    ControlType.Button,
                    typeof(WinButton.PropertyNames)
                },
                {
                    ControlType.DateTimePicker,
                    typeof(WinDateTimePicker.PropertyNames)
                },
                {
                    ControlType.Edit,
                    typeof(WinEdit.PropertyNames)
                },
                {
                    ControlType.List,
                    typeof(WinList.PropertyNames)
                },
                {
                    ControlType.ProgressBar,
                    typeof(WinProgressBar.PropertyNames)
                },
                {
                    ControlType.Slider,
                    typeof(WinSlider.PropertyNames)
                },
                {
                    ControlType.StatusBar,
                    typeof(WinStatusBar.PropertyNames)
                },
                {
                    ControlType.Window,
                    typeof(WinWindow.PropertyNames)
                },
                {
                    ControlType.Tree,
                    typeof(WinTree.PropertyNames)
                },
                {
                    ControlType.TreeItem,
                    typeof(WinTreeItem.PropertyNames)
                },
                {
                    ControlType.CheckBoxTreeItem,
                    typeof(WinCheckBoxTreeItem.PropertyNames)
                },
                {
                    ControlType.CheckBox,
                    typeof(WinCheckBox.PropertyNames)
                },
                {
                    ControlType.RadioButton,
                    typeof(WinRadioButton.PropertyNames)
                },
                {
                    ControlType.ComboBox,
                    typeof(WinComboBox.PropertyNames)
                },
                {
                    ControlType.TabList,
                    typeof(WinTabList.PropertyNames)
                },
                {
                    ControlType.TabPage,
                    typeof(WinTabPage.PropertyNames)
                },
                {
                    ControlType.MenuBar,
                    typeof(WinMenu.PropertyNames)
                },
                {
                    ControlType.MenuItem,
                    typeof(WinMenuItem.PropertyNames)
                },
                {
                    ControlType.Table,
                    typeof(WinTable.PropertyNames)
                },
                {
                    ControlType.Row,
                    typeof(WinRow.PropertyNames)
                },
                {
                    ControlType.Cell,
                    typeof(WinCell.PropertyNames)
                },
                {
                    ControlType.ScrollBar,
                    typeof(WinScrollBar.PropertyNames)
                },
                {
                    ControlType.ToolBar,
                    typeof(WinToolBar.PropertyNames)
                },
                {
                    ControlType.ListItem,
                    typeof(WinListItem.PropertyNames)
                },
                {
                    ControlType.Calendar,
                    typeof(WinCalendar.PropertyNames)
                },
                {
                    ControlType.Text,
                    typeof(WinText.PropertyNames)
                },
                {
                    ControlType.Menu,
                    typeof(WinMenu.PropertyNames)
                },
                {
                    ControlType.Hyperlink,
                    typeof(WinHyperlink.PropertyNames)
                },
                {
                    ControlType.TitleBar,
                    typeof(WinTitleBar.PropertyNames)
                },
                {
                    ControlType.Spinner,
                    typeof(WinSpinner.PropertyNames)
                },
                {
                    ControlType.Client,
                    typeof(WinControl.PropertyNames)
                },
                {
                    ControlType.ColumnHeader,
                    typeof(WinControl.PropertyNames)
                },
                {
                    ControlType.RowHeader,
                    typeof(WinRowHeader.PropertyNames)
                },
                {
                    ControlType.Group,
                    typeof(WinControl.PropertyNames)
                },
                {
                    ControlType.Separator,
                    typeof(WinControl.PropertyNames)
                },
                {
                    ControlType.SplitButton,
                    typeof(WinButton.PropertyNames)
                },
                {
                    ControlType.ToolTip,
                    typeof(WinControl.PropertyNames)
                },
                {
                    ControlType.Pane,
                    typeof(WinControl.PropertyNames)
                }
            };

        private bool IsCommonReadableProperty(ControlType controlType, string propertyName)
        {
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                if ((((!(controlType == ControlType.CheckBox) && !(controlType == ControlType.ListItem)) &&
                      (!(controlType == ControlType.MenuItem) && !(controlType == ControlType.RadioButton))) &&
                     ((!(controlType == ControlType.TreeItem) && !(controlType == ControlType.CheckBoxTreeItem)) &&
                      (!(controlType == ControlType.DateTimePicker) && !(controlType == ControlType.TabPage)))) &&
                    !(controlType == ControlType.Window))
                {
                    return (controlType == ControlType.Cell);
                }
                return true;
            }
            if (!string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if ((((!(controlType == ControlType.Edit) && !(controlType == ControlType.Button)) &&
                  (!(controlType == ControlType.SplitButton) && !(controlType == ControlType.ListItem))) &&
                 ((!(controlType == ControlType.MenuItem) && !(controlType == ControlType.ProgressBar)) &&
                  (!(controlType == ControlType.Cell) && !(controlType == ControlType.List)))) &&
                (((!(controlType == ControlType.ComboBox) && !(controlType == ControlType.DateTimePicker)) &&
                  (!(controlType == ControlType.Calendar) && !(controlType == ControlType.ScrollBar))) &&
                 !(controlType == ControlType.TabPage)))
            {
                return (controlType == ControlType.Slider);
            }
            return true;
        }

        private bool IsCommonWritableProperty(ControlType controlType, string propertyName)
        {
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                if ((((!(controlType == ControlType.CheckBox) && !(controlType == ControlType.CheckBoxTreeItem)) &&
                      (!(controlType == ControlType.ListItem) && !(controlType == ControlType.MenuItem))) &&
                     ((!(controlType == ControlType.RadioButton) && !(controlType == ControlType.DateTimePicker)) &&
                      (!(controlType == ControlType.TreeItem) && !(controlType == ControlType.Tree)))) &&
                    !(controlType == ControlType.Window))
                {
                    return (controlType == ControlType.Cell);
                }
                return true;
            }
            if (!string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (((!(controlType == ControlType.Edit) && !(controlType == ControlType.FileInput)) &&
                 (!(controlType == ControlType.List) && !(controlType == ControlType.ComboBox))) &&
                ((!(controlType == ControlType.DateTimePicker) && !(controlType == ControlType.Calendar)) &&
                 (!(controlType == ControlType.ScrollBar) && !(controlType == ControlType.Slider))))
            {
                return (controlType == ControlType.Cell);
            }
            return true;
        }

        private bool IsDatagridElement(ZappyTaskControl uiControl)
        {
            if (string.Equals(uiControl.TechnologyElement.Name,
                    LocalizedSystemStrings.Instance.WinFormsEditableControlNameENU, StringComparison.Ordinal) ||
                string.Equals(uiControl.TechnologyElement.Name,
                    LocalizedSystemStrings.Instance.WinFormsEditableControlNameLocalized, StringComparison.Ordinal))
            {
                return true;
            }
            TaskActivityElement parent = ZappyTaskService.Instance.GetParent(uiControl.TechnologyElement);
            for (int i = 0; (i < 4) && (parent != null); i++)
            {
                if (ControlType.Table.NameEquals(parent.ControlTypeName))
                {
                    return true;
                }
                parent = ZappyTaskService.Instance.GetParent(parent);
            }
            return false;
        }

        private bool IsInvisible(ZappyTaskControl child)
        {
            ControlStates stateValue = child.StateValue;
            return (((stateValue & ControlStates.Invisible) == ControlStates.Invisible) &&
                    ((stateValue & (ControlStates.None | ControlStates.Offscreen)) !=
                     (ControlStates.None | ControlStates.Offscreen)));
        }

        private bool IsLeafTreeItem(ZappyTaskControl uiControl)
        {
            bool flag = false;
            ZappyTaskControlCollection treeNodeProperty =
                (ZappyTaskControlCollection)this.GetTreeNodeProperty(WinTreeItem.PropertyNames.Nodes);
            if ((treeNodeProperty != null) && (treeNodeProperty.Count != 0))
            {
                return flag;
            }
            return true;
        }

        private bool IsScrollBarVertical()
        {
            ZappyTaskControlCollection controls =
                ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Button, 1);
            if (controls.Count != 4)
            {
                ALUtility.ThrowNotSupportedException(false);
            }
            Rectangle boundingRectangle = controls[0].BoundingRectangle;
            Rectangle rectangle2 = controls[3].BoundingRectangle;
            if (boundingRectangle.IsEmpty || rectangle2.IsEmpty)
            {
                return this.win.HasStyle(1);
            }
            if (boundingRectangle.Y == rectangle2.Y)
            {
                return false;
            }
            return true;
        }

        private void SetCheckBoxProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinCheckBox.PropertyNames.Checked, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    UIControl.StateValue = ControlStates.Checked;
                }
                else
                {
                    UIControl.StateValue = ControlStates.None | ControlStates.Normal;
                }
            }
            else if (string.Equals(WinCheckBox.PropertyNames.Indeterminate, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    UIControl.StateValue = ControlStates.None | ControlStates.Normal;
                }
                else
                {
                    UIControl.StateValue = ControlStates.Indeterminate;
                    if (!((bool)UIControl.GetProperty(WinCheckBox.PropertyNames.Indeterminate)))
                    {
                        ALUtility.ThrowNotSupportedException(false);
                    }
                }
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TechnologyElementPropertyProvider.SetCheckBoxState(UIControl, (ControlStates)value);
            }
        }

        private void SetCheckBoxTreeNodeProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        ControlStates state = ControlStates.None | ControlStates.Normal;
            if (string.Equals(WinCheckBoxTreeItem.PropertyNames.Checked, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.Checked;
                }
                else
                {
                    state = ControlStates.None | ControlStates.Normal;
                }
            }
            else if (string.Equals(WinTreeItem.PropertyNames.Selected, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.None | ControlStates.Selected;
                }
                else
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
            }
            else if (string.Equals(WinCheckBoxTreeItem.PropertyNames.Indeterminate, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.Indeterminate;
                }
                else
                {
                    state = ControlStates.None | ControlStates.Normal;
                }
            }
            else if (string.Equals(WinTreeItem.PropertyNames.Expanded, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.Expanded;
                }
                else
                {
                    state = ControlStates.Collapsed;
                }
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                state = (ControlStates)value;
            }
            else
            {
                object[] objArray2 = new object[] { state, UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetStateNotSupportedForControlTypeMessage, objArray2));
            }
            this.SetCheckBoxTreeNodeState(state);
        }

        private void SetCheckBoxTreeNodeState(ControlStates state)
        {
            ControlStates states = ControlStates.None | ControlStates.Normal;
            bool flag = false;
            if (((ControlStates.Checked & state) == ControlStates.None) &&
                (((ControlStates.None | ControlStates.Normal) & state) == ControlStates.None))
            {
                if ((bool)this.GetCheckBoxTreeNodeProperty(WinCheckBoxTreeItem.PropertyNames.Checked))
                {
                    states = ControlStates.Checked;
                }
                flag = true;
            }
            if (((ControlStates.None | ControlStates.Selected) & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Select();
            }
            if ((ControlStates.Expanded & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Expand(null);
            }
            else if ((ControlStates.Collapsed & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Collapse(null);
            }
            if (flag)
            {
                state = states;
            }
            if ((ControlStates.Checked & state) != ControlStates.None)
            {
                if ((ControlStates.Checked & UIControl.StateValue) == ControlStates.None)
                {
                    UIControl.ScreenElement.CheckTreeItem();
                }
            }
            else if ((((ControlStates.None | ControlStates.Normal) & state) != ControlStates.None) &&
                     (((ControlStates.Checked & UIControl.StateValue) != ControlStates.None) ||
                      ((ControlStates.Indeterminate & UIControl.StateValue) != ControlStates.None)))
            {
                UIControl.ScreenElement.UncheckTreeItem();
            }
        }

        private void SetComboBoxProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        ZappyTaskControl control = null;
            ZappyTaskControl control2 = null;
            if ((bool)UIControl.TechnologyElement.GetPropertyValue("_isNumericUpDownControl"))
            {
                control = ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Edit,
                    -1)[0];
                control2 = ALUtility.GetDescendantsByControlType(UIControl, technologyName,
                    ControlType.Spinner, -1)[0];
                if ((string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName,
                         StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(WinComboBox.PropertyNames.EditableItem, propertyName,
                         StringComparison.OrdinalIgnoreCase)) || string.Equals(WinComboBox.PropertyNames.SelectedItem,
                        propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    string str = ZappyTaskUtilities.ConvertToType<string>(value, false);
                    SetValueAsNumericControl(control.ScreenElement, str, control2.ScreenElement);
                    return;
                }
                ALUtility.ThrowNotSupportedException(false);
            }
            bool property = (bool)UIControl.GetProperty("IsSimpleComboBoxType");
            if (string.Equals(WinComboBox.PropertyNames.SelectedIndex, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                int num = ZappyTaskUtilities.ConvertToType<int>(value);
                int @int = this.win.GetInt(0x146);
                if (!property)
                {
                    int[] selectedIndices = new int[] { num };
                    TechnologyElementPropertyProvider.SetValueUsingQueryId(UIControl, selectedIndices,
                        technologyName, @int);
                }
                else
                {
                    ZappyTaskControlCollection controls = ALUtility.GetDescendantsByControlType(UIControl,
                        technologyName, ControlType.ListItem, -1);
                    TechnologyElementPropertyProvider.SetValueAsComboBox(UIControl, controls[num].Name, true);
                }
            }
            else if (string.Equals(WinComboBox.PropertyNames.Expanded, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                bool flag2 = ZappyTaskUtilities.ConvertToType<bool>(value);
                bool flag3 = false;
                bool comboBoxProperty = (bool)this.GetComboBoxProperty(WinComboBox.PropertyNames.Expanded);
                int num3 = 0;
                while (((num3 < 2) && !flag3) && ((flag2 && !comboBoxProperty) || (!flag2 & comboBoxProperty)))
                {
                    flag3 = !UIControl.ScreenElement.ExpandCollapseComboBox(";[MSAA]ControlType='Button'");
                    comboBoxProperty = (bool)this.GetComboBoxProperty(WinComboBox.PropertyNames.Expanded);
                    num3++;
                }
                if (flag3 || (num3 == 2))
                {
                    this.win.SetInt(0x14f, flag2 ? 1 : 0, 0);
                }
            }
            else if (string.Equals(WinComboBox.PropertyNames.EditableItem, propertyName,
                         StringComparison.OrdinalIgnoreCase) || (string.Equals(ZappyTaskControl.PropertyNames.Value,
                                                                     propertyName, StringComparison.OrdinalIgnoreCase) &
                                                                 property))
            {
                string str2 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                if (!((bool)this.GetComboBoxProperty(WinComboBox.PropertyNames.IsEditable)))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                TechnologyElementPropertyProvider.SetValueAsComboBox(UIControl, str2, true);
            }
            else
            {
                if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    string str3 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                    try
                    {
                        TechnologyElementPropertyProvider.SetValueAsComboBox(UIControl, str3, false);
                        return;
                    }
                    catch (COMException)
                    {
                        if (!((bool)this.GetComboBoxProperty(WinComboBox.PropertyNames.IsEditable)))
                        {
                            throw;
                        }
                                                TechnologyElementPropertyProvider.SetValueAsComboBox(UIControl, str3, true);
                        return;
                    }
                }
                if (string.Equals(WinComboBox.PropertyNames.SelectedItem, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    string str4 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                    TechnologyElementPropertyProvider.SetValueAsComboBox(UIControl, str4, false);
                }
            }
        }

        private void SetDateTimePickerProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinDateTimePicker.PropertyNames.Checked, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!this.win.HasStyle(2))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                else
                {
                    NativeMethods.SYSTEMTIME systemtime = new NativeMethods.SYSTEMTIME();
                    bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                    bool flag2 = this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1001, ref systemtime) == 0;
                    int x = 8;
                    Rectangle boundingRectangle = UIControl.BoundingRectangle;
                    if ((flag2 && !flag) || (!flag2 & flag))
                    {
                        if (UIControl.TechnologyElement.GetRightToLeftProperty(RightToLeftKind.Layout) ||
                            UIControl.TechnologyElement.GetRightToLeftProperty(RightToLeftKind.Text))
                        {
                            x = boundingRectangle.Width - 8;
                        }
                        Mouse.Click(UIControl, new Point(x, boundingRectangle.Height / 2));
                    }
                }
            }
            else if (string.Equals(WinDateTimePicker.PropertyNames.DateTime, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                DateTime time;
                DateTime time2;
                DateTime time3 = ZappyTaskUtilities.ConvertToType<DateTime>(value);
                if (this.GetMinAndMaxDateRange(out time, out time2, 0x1003) &&
                    ((DateTime.Compare(time2, time3) < 0) || (DateTime.Compare(time3, time) < 0)))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                NativeMethods.SYSTEMTIME systemtime2 = NativeMethods.SYSTEMTIME.FromDateTime(time3);
                if (this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1002, ref systemtime2) == 0)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                Mouse.Click(UIControl);
                Keyboard.SendKeys(UIControl, "{UP}");
                Keyboard.SendKeys(UIControl, "{DOWN}");
                DateTime dateTimePickerProperty =
                    (DateTime)this.GetDateTimePickerProperty(WinDateTimePicker.PropertyNames.DateTime);
                if (!DateTime.Equals(time3, dateTimePickerProperty) &&
                    (this.win.GetGeneric<NativeMethods.SYSTEMTIME>(0x1002, ref systemtime2) == 0))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
            }
            else if (string.Equals(WinDateTimePicker.PropertyNames.ShowCalendar, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                bool flag3 = (bool)value;
                IntPtr handle = this.win.GetHandle(0x1008);
                if ((flag3 && handle.Equals(IntPtr.Zero)) || (!flag3 && !handle.Equals(IntPtr.Zero)))
                {
                    Rectangle rectangle2 = UIControl.BoundingRectangle;
                    Mouse.Click(UIControl, new Point(rectangle2.Width - 8, rectangle2.Height / 2));
                    handle = this.win.GetHandle(0x1008);
                }
            }
            else if (
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinDateTimePicker.PropertyNames.DateTimeAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                string str = ZappyTaskUtilities.ConvertToType<string>(value, false);
                this.SetValueAsDateTimePicker(str);
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates states = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                switch (states)
                {
                    case ControlStates.Checked:
                        this.SetDateTimePickerProperty(WinDateTimePicker.PropertyNames.Checked, true);
                        return;

                    case (ControlStates.None | ControlStates.Normal):
                        this.SetDateTimePickerProperty(WinDateTimePicker.PropertyNames.Checked, false);
                        return;
                }
                object[] objArray2 = new object[] { states, UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetStateNotSupportedForControlTypeMessage, objArray2));
            }
        }

        private void SetEditProperty(string propertyName, object value)
        {
            int cpMax;
            int cpMin;
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinEdit.PropertyNames.SelectionStart, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                cpMin = (int)value;
                if (cpMin < 0)
                {
                    this.win.GetInt(0xb1, -1, -1);
                }
                else
                {
                    cpMax = this.win.GetSelectionRange(0x434, 0xb0).cpMax;
                    if ((cpMax < 0) || (cpMax < cpMin))
                    {
                        cpMax = cpMin;
                    }
                    this.win.GetInt(0xb1, cpMin, cpMax);
                }
            }
            else if (string.Equals(WinEdit.PropertyNames.SelectionEnd, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                cpMax = (int)value;
                if (cpMax < 0)
                {
                    this.win.GetInt(0xb1, -1, -1);
                }
                else
                {
                    cpMin = this.win.GetSelectionRange(0x434, 0xb0).cpMin;
                    if ((cpMin < 0) || (cpMax < cpMin))
                    {
                        cpMin = cpMax;
                    }
                    this.win.GetInt(0xb1, cpMin, cpMax);
                }
            }
            else if (string.Equals(WinEdit.PropertyNames.SelectionText, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                string str = (string)value;
                if (string.IsNullOrEmpty(str))
                {
                    this.win.GetInt(0xb1, -1, -1);
                }
                else
                {
                    cpMin = UIControl.TechnologyElement.Value.IndexOf(str, StringComparison.Ordinal);
                    if (cpMin < 0)
                    {
                        throw new ArgumentException(WinEdit.PropertyNames.SelectionText, str);
                    }
                    this.win.GetInt(0xb1, cpMin, cpMin + str.Length);
                }
            }
            else if (string.Equals(WinEdit.PropertyNames.InsertionIndexAbsolute, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                cpMin = (int)value;
                this.win.GetInt(0xb1, cpMin, cpMin);
            }
            else
            {
                if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName,
                        StringComparison.OrdinalIgnoreCase) || string.Equals(WinEdit.PropertyNames.Text, propertyName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    if (UIControl.TechnologyElement.IsPassword && string.Equals(ZappyTaskControl.PropertyNames.Value,
                            propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        this.SetEditProperty(WinEdit.PropertyNames.Password, value);
                        return;
                    }
                    string propertyValue = string.Empty;
                    if (value != null)
                    {
                        propertyValue = ZappyTaskUtilities.ConvertToType<string>(value);
                    }
                    string name = UIControl.TechnologyElement.Name;
                    if (this.IsDatagridElement(UIControl))
                    {
                        ThrowExceptionIfReadOnly();
                        this.SetValueInEditableControl(UIControl, propertyValue);
                        return;
                    }
                    try
                    {
                        TechnologyElementPropertyProvider.SetValueAsEditBox(UIControl, propertyValue, false,
                            false);
                        return;
                    }
                    catch (COMException)
                    {
                        bool flag = false;
                        if (((ScreenElement.ImeLanguageList.Count > 0) && (UIControl.TechnologyElement != null)) &&
                            (UIControl.TechnologyElement.WindowHandle != IntPtr.Zero))
                        {
                            int lcidFromWindowHandle =
                                ZappyTaskUtilities.GetLcidFromWindowHandle(UIControl.TechnologyElement.WindowHandle);
                            flag = ScreenElement.ImeLanguageList.Contains(lcidFromWindowHandle);
                        }
                        if (flag)
                        {
                            int property = (int)UIControl.GetProperty(WinEdit.PropertyNames.MaxLength);
                            if ((property < 0) || (property >= propertyValue.Length))
                            {
                                                                UIControl.ScreenElement.SetValueAsEditBox(propertyValue, 0x200);
                                return;
                            }
                        }
                        throw;
                    }
                }
                if (string.Equals(WinEdit.PropertyNames.Password, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!UIControl.TechnologyElement.IsPassword)
                    {
                        ALUtility.ThrowNotSupportedException(false);
                    }
                    string str4 = string.Empty;
                    if (value != null)
                    {
                        str4 = ZappyTaskUtilities.ConvertToType<string>(value);
                    }
                    TechnologyElementPropertyProvider.SetValueAsEditBox(UIControl, str4, true, false);
                }
                else if (string.Equals(WinEdit.PropertyNames.CopyPastedText, propertyName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    string str5 = string.Empty;
                    if (value != null)
                    {
                        str5 = ZappyTaskUtilities.ConvertToType<string>(value);
                    }
                    TechnologyElementPropertyProvider.SetValueAsEditBox(UIControl, str5, false, true);
                }
            }
        }

        private void SetListBoxProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinList.PropertyNames.CheckedItems, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                string[] strArray = ZappyTaskUtilities.ConvertToType<string[]>(value);
                ZappyTaskControlCollection itemsToCheck = new ZappyTaskControlCollection();
                ZappyTaskControlCollection children = UIControl.GetChildren();
                bool flag = false;
                foreach (string str in strArray)
                {
                    flag = false;
                    foreach (ZappyTaskControl control in children)
                    {
                        if (string.Equals(control.Name, str, StringComparison.Ordinal))
                        {
                            flag = true;
                            itemsToCheck.Add(control);
                            break;
                        }
                    }
                    if (!flag)
                    {
                        object[] objArray2 = new object[] { str, UIControl.ControlType.Name, propertyName };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Resources.InvalidParameterValue, objArray2));
                    }
                }
                CheckItems(itemsToCheck, children);
            }
            else if (string.Equals(WinList.PropertyNames.CheckedIndices, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                int[] numArray = ZappyTaskUtilities.ConvertToType<int[]>(value);
                ZappyTaskControlCollection controls3 = new ZappyTaskControlCollection();
                ZappyTaskControlCollection allItems = UIControl.GetChildren();
                allItems.RemoveAll(element =>
                    !element.ControlType.NameEquals(ControlType.ListItem.Name) &&
                    !element.ControlType.NameEquals(ControlType.CheckBox.Name));
                foreach (int num3 in numArray)
                {
                    if (num3 < allItems.Count)
                    {
                        controls3.Add(allItems[num3]);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { num3, UIControl.ControlType.Name, propertyName };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Resources.InvalidParameterValue, objArray3));
                    }
                }
                CheckItems(controls3, allItems);
            }
            else if (string.Equals(WinList.PropertyNames.SelectedItems, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                string[] values = ZappyTaskUtilities.ConvertToType<string[]>(value, false);
                TechnologyElementPropertyProvider.SetValueAsListBox(UIControl, values);
            }
            else if (string.Equals(WinList.PropertyNames.SelectedIndices, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (this.win.IsCheckedList())
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                int[] selectedIndices = ZappyTaskUtilities.ConvertToType<int[]>(value);
                int @int = this.win.GetInt(0x18b);
                TechnologyElementPropertyProvider.SetValueUsingQueryId(UIControl, selectedIndices,
                    technologyName, @int);
            }
            else if (
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinList.PropertyNames.SelectedItemsAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                string[] strArray4 = CommaListBuilder
                    .GetCommaSeparatedValues(ZappyTaskUtilities.ConvertToType<string>(value, false)).ToArray();
                if (this.win.IsCheckedList())
                {
                    this.SetListBoxProperty(WinList.PropertyNames.CheckedItems, strArray4);
                }
                else
                {
                    this.SetListBoxProperty(WinList.PropertyNames.SelectedItems, strArray4);
                }
            }
            else
            {
                ALUtility.ThrowNotSupportedException(false);
            }
        }

        private void SetListViewProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinList.PropertyNames.SelectedItems, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string[] values = ZappyTaskUtilities.ConvertToType<string[]>(value);
                TechnologyElementPropertyProvider.SetValueAsListBox(UIControl, values);
            }
            else if (string.Equals(WinList.PropertyNames.SelectedItemsAsString, propertyName,
                         StringComparison.OrdinalIgnoreCase) || string.Equals(ZappyTaskControl.PropertyNames.Value,
                         propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string[] strArray2 = CommaListBuilder
                    .GetCommaSeparatedValues(ZappyTaskUtilities.ConvertToType<string>(value, false)).ToArray();
                TechnologyElementPropertyProvider.SetValueAsListBox(UIControl, strArray2);
            }
            else if (string.Equals(WinList.PropertyNames.SelectedIndices, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                int[] selectedIndices = ZappyTaskUtilities.ConvertToType<int[]>(value);
                string[] stringArrayForSelectedIndices =
                    this.GetStringArrayForSelectedIndices(selectedIndices, propertyName);
                TechnologyElementPropertyProvider.SetValueAsListBox(UIControl, stringArrayForSelectedIndices);
            }
            else
            {
                ALUtility.ThrowNotSupportedException(false);
            }
        }

        private void SetMenuItemProperty(string propertyName, object value)
        {
            if (string.Equals(WinMenuItem.PropertyNames.Checked, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetMenuItemState(flag ? ControlStates.Checked : (ControlStates.None | ControlStates.Normal));
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                this.SetMenuItemState(state);
            }
        }

        private void SetMenuItemState(ControlStates state)
        {
            if (((ControlStates.Checked & state) == ControlStates.None) &&
                ((ControlStates.None | ControlStates.Normal) != state))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            if ((((ControlStates.Checked & state) != ControlStates.None) &&
                 !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked)) ||
                (((ControlStates.None | ControlStates.Normal) == state) &&
                 TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked)))
            {
                Mouse.Click(UIControl);
            }
        }

        private void SetMonthCalendarProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinCalendar.PropertyNames.SelectionRangeAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                string str2;
                string str3;
                DateTime time;
                string str = ZappyTaskUtilities.ConvertToType<string>(value);
                ZappyTaskUtilities.TryGetDateTimeRangeString(str, out str2, out str3);
                if (string.IsNullOrEmpty(str2))
                {
                    object[] objArray2 = new object[]
                        {str, UIControl.ControlType.Name, propertyName, "\"Date1\"-\"Date2\"", "Date"};
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValueFormat, objArray2));
                }
                if (ZappyTaskUtilities.TryGetShortDate(str2, out time))
                {
                    DateTime time2;
                    if (string.IsNullOrEmpty(str3))
                    {
                        time2 = time;
                    }
                    else
                    {
                        ZappyTaskUtilities.TryGetShortDate(str3, out time2);
                    }
                    Mouse.Click(UIControl);
                    this.SetMonthCalendarProperty(WinCalendar.PropertyNames.SelectionRange,
                        new SelectionRange(time, time2));
                    return;
                }
                object[] objArray3 = new object[]
                {
                    str2, ZappyTaskUtilities.DateFormat, WallClock.Now.ToShortDateString(), ZappyTaskUtilities.DateFormat,
                    ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, false)
                };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidDateFormat,
                    objArray3));
            }
            if (string.Equals(WinCalendar.PropertyNames.SelectionRange, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                DateTime time6;
                DateTime time7;
                SelectionRange range = ZappyTaskUtilities.ConvertToType<SelectionRange>(value);
                DateTime start = range.Start;
                DateTime end = range.End;
                if (DateTime.Compare(end, start) < 0)
                {
                    object[] objArray4 = new object[] { value, propertyName };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValue, objArray4));
                }
                if (this.GetMinAndMaxDateRange(out time7, out time6, 0x1011) &&
                    ((DateTime.Compare(time7, start) > 0) || (DateTime.Compare(end, time6) > 0)))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                NativeMethods.SYSTEMTIMEARRAY systemtimearray =
                    NativeMethods.SYSTEMTIMEARRAY.FromDateTimeRange(start, end);
                if (this.win.GetGeneric<NativeMethods.SYSTEMTIMEARRAY>(0x1006, ref systemtimearray) == 0)
                {
                    object[] objArray5 = new object[] { propertyName, UIControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                        Resources.SetPropertyFailed, objArray5));
                }
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiControl, string propertyName, object value)
        {
            this.win = new WindowsControl(uiControl);
            ControlType controlType = uiControl.ControlType;
            ZappyTaskPropertyDescriptor propertyDescriptor = this.GetPropertyDescriptor(uiControl, propertyName);
            if (!this.IsCommonWritableProperty(controlType, propertyName) &&
                ((propertyDescriptor == null) || ((propertyDescriptor.Attributes & ZappyTaskPropertyAttributes.Writable) ==
                                                  ZappyTaskPropertyAttributes.None)))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            if (ControlType.List == controlType)
            {
                if (this.win.IsListView())
                {
                    this.SetListViewProperty(propertyName, value);
                }
                else
                {
                    this.SetListBoxProperty(propertyName, value);
                }
            }
            else if (ControlType.Spinner == controlType)
            {
                this.SetSpinnerProperty(propertyName, value);
            }
            else if (ControlType.ScrollBar == controlType)
            {
                this.SetScrollBarProperty(propertyName, value);
            }
            else if (ControlType.Slider == controlType)
            {
                this.SetSliderProperty(propertyName, value);
            }
            else if (ControlType.Edit == controlType)
            {
                this.SetEditProperty(propertyName, value);
            }
            else if (ControlType.DateTimePicker == controlType)
            {
                this.SetDateTimePickerProperty(propertyName, value);
            }
            else if (ControlType.TreeItem == controlType)
            {
                this.SetTreeNodeProperty(propertyName, value);
            }
            else if (ControlType.CheckBoxTreeItem == controlType)
            {
                this.SetCheckBoxTreeNodeProperty(propertyName, value);
            }
            else if (ControlType.CheckBox == controlType)
            {
                this.SetCheckBoxProperty(propertyName, value);
            }
            else if (ControlType.RadioButton == controlType)
            {
                this.SetRadioButtonProperty(propertyName, value);
            }
            else if (ControlType.Calendar == controlType)
            {
                this.SetMonthCalendarProperty(propertyName, value);
            }
            else if (ControlType.TabList == controlType)
            {
                this.SetTabListProperty(propertyName, value);
            }
            else if (ControlType.ComboBox == controlType)
            {
                this.SetComboBoxProperty(propertyName, value);
            }
            else if (ControlType.Cell == controlType)
            {
                this.SetTableCellProperty(propertyName, value);
            }
            else if (ControlType.Window == controlType)
            {
                this.SetWindowProperty(propertyName, value);
            }
            else if (ControlType.MenuItem == controlType)
            {
                this.SetMenuItemProperty(propertyName, value);
            }
            else if (ControlType.ToolBar == uiControl.GetParent().ControlType)
            {
                this.SetToolBarItemProperty(propertyName, value);
            }
            else
            {
                ALUtility.ThrowNotSupportedException(false);
            }
        }

        private void SetRadioButtonProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinRadioButton.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                if (flag)
                {
                    UIControl.StateValue = ControlStates.Checked;
                    return;
                }
                object[] objArray2 = new object[] { flag, UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetStateNotSupportedForControlTypeMessage, objArray2));
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) &&
                ((((ControlStates)value) == (ControlStates.None | ControlStates.Selected)) ||
                 (((ControlStates)value) == ControlStates.Checked)))
            {
                TechnologyElementPropertyProvider.SetRadioButtonState(UIControl, ControlStates.Checked);
            }
        }

        private void SetScrollBarProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        int result = -1;
            if (string.Equals(propertyName, WinScrollBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                result = ZappyTaskUtilities.ConvertToType<int>(value, true);
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                string s = ZappyTaskUtilities.ConvertToType<string>(value);
                if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) &&
                    !int.TryParse(s, out result))
                {
                    object[] objArray2 = new object[] { value, propertyName, UIControl.ControlType };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValue, objArray2));
                }
            }
            this.SetValueAsScrollBar(result);
        }

        private void SetSliderProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(propertyName, WinSlider.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                double absoluteValue = ZappyTaskUtilities.ConvertToType<double>(value);
                this.SetSliderValue(propertyName, absoluteValue);
            }
            else if (
                string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(WinSlider.PropertyNames.PositionAsString, propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                double doubleValue = -1.0;
                if (!ALUtility.ConvertStringToDouble(ZappyTaskUtilities.ConvertToType<string>(value), out doubleValue))
                {
                    object[] objArray2 = new object[] { value, propertyName, UIControl.ControlType };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValue, objArray2));
                }
                this.SetSliderValue(propertyName, doubleValue);
            }
        }

        private void SetSliderValue(string propertyName, double absoluteValue)
        {
            int sliderProperty = (int)this.GetSliderProperty(WinSlider.PropertyNames.MinimumPosition);
            int num2 = (int)this.GetSliderProperty(WinSlider.PropertyNames.MaximumPosition);
            double num3 = (num2 == sliderProperty)
                ? absoluteValue
                : (((absoluteValue - sliderProperty) * 100.0) / ((double)(num2 - sliderProperty)));
            if ((num3 > 100.0) || (num3 < 0.0))
            {
                object[] args = new object[] { absoluteValue, UIControl.ControlType.Name, propertyName };
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture,
                    Resources.InvalidParameterValue, args));
            }
            UIControl.ScreenElement.SetValueAsSlider(num3.ToString(CultureInfo.CurrentCulture));
        }

        private void SetSpinnerProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        ALUtility.ThrowNotSupportedException(true);
        }

        private void SetTableCellProperty(string propertyName, object value)
        {
            bool flag = false;
            if (WinCell.PropertyNames.Value.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                flag = false;
                object[] args = new object[] { value };
                            }
            else if (WinEdit.PropertyNames.Password.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ThrowExceptionIfReadOnly();
                flag = true;
                            }
            else if (WinEdit.PropertyNames.Text.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ThrowExceptionIfReadOnly();
                flag = false;
                object[] objArray2 = new object[] { value };
                            }
            else
            {
                if (WinCell.PropertyNames.Checked.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    ThrowExceptionIfControlDisabled();
                    object[] objArray3 = new object[] { value };
                                        ControlStates state = ZappyTaskUtilities.ConvertToType<bool>(value)
                        ? ControlStates.Checked
                        : (ControlStates.None | ControlStates.Normal);
                    this.SetTableCellState(state);
                    return;
                }
                if (WinCell.PropertyNames.Indeterminate.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    ThrowExceptionIfControlDisabled();
                    object[] objArray4 = new object[] { value };
                                        ControlStates states2 = ZappyTaskUtilities.ConvertToType<bool>(value)
                        ? ControlStates.Indeterminate
                        : (ControlStates.None | ControlStates.Normal);
                    this.SetTableCellState(states2);
                    return;
                }
                object[] objArray5 = new object[] { UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetValueNotSupportedMessage, objArray5));
            }
            string str = ZappyTaskUtilities.ConvertToType<string>(value, false);
            ZappyTaskControl parent = UIControl.GetParent();
            ZappyTaskControl table = null;
            bool flag2 = false;
            if (parent != null)
            {
                table = parent.GetParent();
                if (((parent.ControlType == ControlType.Row) && (table != null)) &&
                    (table.ControlType == ControlType.Table))
                {
                    Rectangle boundingRectangle = UIControl.BoundingRectangle;
                    Point absoluteCoordinates = new Point(boundingRectangle.X + (boundingRectangle.Width / 2),
                        boundingRectangle.Y + (boundingRectangle.Height / 2));
                    ZappyTaskControl uiControl = null;
                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {
                            uiControl = ZappyTaskControlFactory.FromPoint(absoluteCoordinates);
                        }
                        catch (ZappyTaskControlNotAvailableException)
                        {
                            UIControl.SetFocus();
                        }
                    }
                    if (((uiControl == null) ||
                         (!ControlType.Edit.NameEquals(uiControl.TechnologyElement.ControlTypeName) &&
                          !ControlType.ComboBox.NameEquals(uiControl.TechnologyElement.ControlTypeName))) ||
                        !this.IsDatagridElement(uiControl))
                    {
                        Mouse.Hover(UIControl,
                            new Point(boundingRectangle.Width / 2, boundingRectangle.Height / 2));
                    }
#if COMENABLED
                    bool playbackProperty =
                        (bool) ScreenElement.GetPlaybackProperty(ExecuteParameter.UISYNCHRONIZATION_ENABLED);
                    ScreenElement.SetPlaybackProperty(ExecuteParameter.UISYNCHRONIZATION_ENABLED, false);
#endif
                    try
                    {
                        Rectangle cellBounds = UIControl.BoundingRectangle;
                        int x = this.GetRelativeCellClickablePoint(UIControl, parent, cellBounds);
                        Point point2 = new Point(cellBounds.X + x, cellBounds.Y + (cellBounds.Height / 2));
                        ZappyTaskControl control = ZappyTaskControlFactory.FromPoint(point2);
                        if (control.ControlType == ControlType.Cell)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                Mouse.Click(UIControl, new Point(x, cellBounds.Height / 2));
                                control = ZappyTaskControlFactory.FromPoint(point2);
                                if (control.ControlType != ControlType.Cell)
                                {
                                    break;
                                }
                            }
                        }
                        if ((control.ControlType == ControlType.ComboBox) &&
                            ((control.StateValue & ControlStates.Expanded) != ControlStates.None))
                        {
                            Mouse.Click(control);
                        }
                        if (control.ControlType == ControlType.Text)
                        {
                            control = control.GetParent();
                        }
                        if (control.ControlType == ControlType.Cell)
                        {
                            TaskActivityElement focusedElement = ZappyTaskService.Instance.GetFocusedElement();
                            if ((focusedElement != null) &&
                                !ControlType.Cell.NameEquals(focusedElement.ControlTypeName))
                            {
                                this.SetTableCellValueOnFocussedElement(control, table, focusedElement, str);
                            }
                            else
                            {
                                Keyboard.SendKeys(Keyboard.HandleSpecialCharacters(str.ToString()));
                            }
                        }
                        else if (control.ControlType == ControlType.Edit)
                        {
                            this.SetValueInEditableControl(control, str.ToString());
                        }
                        else if (flag)
                        {
                            control.SetProperty(WinEdit.PropertyNames.Password, str);
                        }
                        else
                        {
                            control.SetProperty(ZappyTaskControl.PropertyNames.Value, str);
                        }
                        goto Label_0424;
                    }
                    finally
                    {
#if COMENABLED
                        ScreenElement.SetPlaybackProperty(ExecuteParameter.UISYNCHRONIZATION_ENABLED,
                            playbackProperty);
#endif
                    }
                }
                flag2 = true;
            }
            else
            {
                flag2 = true;
            }
        Label_0424:
            if (flag2)
            {
                object[] objArray6 = new object[] { UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetValueNotSupportedMessage, objArray6));
            }
        }

        private void SetTableCellState(ControlStates state)
        {
            if (!string.Equals(UIControl.GetProperty("HelpText") as string,
                "DataGridViewCheckBoxCell(DataGridViewCell)", StringComparison.OrdinalIgnoreCase))
            {
                object[] args = new object[] { PropertyName, UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetPropertyNotSupportedMessage, args));
            }
            if ((((ControlStates.Checked & state) == ControlStates.None) &&
                 ((ControlStates.Indeterminate & state) == ControlStates.None)) &&
                (((ControlStates.None | ControlStates.Normal) & state) == ControlStates.None))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            if (!this.VerifyTriState(state))
            {
                int num = 0;
                while (num < 5)
                {
                    Mouse.Click(UIControl);
                    num++;
                    try
                    {
                        if (this.VerifyTriState(state))
                        {
                            return;
                        }
                        continue;
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                        object[] objArray2 = new object[] { UIControl.TechnologyElement };
                                                return;
                    }
                }
                object[] objArray3 = new object[] { PropertyName, UIControl.ControlType.Name };
                throw new Exception(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetPropertyFailed, objArray3));
            }
        }

        private void SetTableCellValueOnFocussedElement(ZappyTaskControl cell, ZappyTaskControl table,
            TaskActivityElement elementInFocus, string value)
        {
            int num = 0;
            bool flag = false;
            TaskActivityElement parent = elementInFocus;
            while ((num++ < 4) && (parent != null))
            {
                if (ControlType.ComboBox.NameEquals(parent.ControlTypeName))
                {
                    flag = true;
                    break;
                }
                parent = ZappyTaskService.Instance.GetParent(parent);
            }
            if (flag)
            {
                bool flag2 = false;
                if (ControlType.Edit.NameEquals(elementInFocus.ControlTypeName))
                {
                    flag2 = true;
                }
                ZappyTaskControl control = ZappyTaskControl.FromTechnologyElement(parent);
                if (flag2)
                {
                    control.TechnologyElement.Value = string.Empty;
                    Keyboard.SendKeys(ZappyTaskControl.FromTechnologyElement(elementInFocus),
                        Keyboard.HandleSpecialCharacters(value));
                }
                else
                {
                    string text = value.Substring(0, 1);
                    string a = cell.TechnologyElement.Value;
                    if (!string.Equals(a, value, StringComparison.Ordinal))
                    {
                        string str3;
                        if ((control.StateValue & ControlStates.Expanded) != ControlStates.None)
                        {
                            Mouse.Click(cell);
                        }
                        do
                        {
                            Keyboard.SendKeys(control, text);
                            Mouse.Click(table);
                            Mouse.Click(cell);
                            str3 = cell.TechnologyElement.Value;
                            if (string.Equals(str3, value, StringComparison.Ordinal))
                            {
                                return;
                            }
                        } while (!string.Equals(str3, a, StringComparison.Ordinal));
                    }
                }
            }
            else
            {
                ZappyTaskControl control3 = ZappyTaskControl.FromTechnologyElement(elementInFocus);
                Keyboard.SendKeys(control3, "{SPACE}");
                Keyboard.SendKeys(control3, "{HOME}");
                Keyboard.SendKeys(control3, "+{END}");
                Keyboard.SendKeys(control3, "{DELETE}");
                Keyboard.SendKeys(control3, Keyboard.HandleSpecialCharacters(value));
            }
        }

        private void SetTabListProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WinTabList.PropertyNames.SelectedIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                int num = ZappyTaskUtilities.ConvertToType<int>(value);
                ZappyTaskControlCollection children = UIControl.GetChildren();
                children.RemoveAll(element => !element.ControlType.NameEquals(ControlType.TabPage.Name));
                if ((num >= children.Count) || (num < 0))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                Mouse.Click(children[num]);
            }
        }

        private void SetToolBarItemProperty(string propertyName, object value)
        {
            if (string.Equals("Pressed", propertyName, StringComparison.OrdinalIgnoreCase))
            {
                Mouse.Click(UIControl);
            }
        }

        private void SetTreeNodeProperty(string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        ControlStates state = ControlStates.None | ControlStates.Normal;
            if (string.Equals(WinTreeItem.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.None | ControlStates.Selected;
                }
                else
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
            }
            else if (string.Equals(WinTreeItem.PropertyNames.Expanded, propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    state = ControlStates.Expanded;
                }
                else
                {
                    state = ControlStates.Collapsed;
                }
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                state = (ControlStates)value;
            }
            else
            {
                object[] objArray2 = new object[] { state, UIControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SetStateNotSupportedForControlTypeMessage, objArray2));
            }
            this.SetTreeNodeState(state);
        }

        private void SetTreeNodeState(ControlStates state)
        {
            if (((ControlStates.None | ControlStates.Selected) & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Select();
            }
            if ((ControlStates.Expanded & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Expand(null);
            }
            else if ((ControlStates.Collapsed & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Collapse(null);
            }
        }

        private void SetValueAsDateTimePicker(string value)
        {
            bool includeTime =
                (NativeMethods.GetWindowLong(UIControl.WindowHandle, NativeMethods.GWLParameter.GWL_STYLE) & 8) ==
                8;
            string str = includeTime ? ZappyTaskUtilities.DateTimeFormat : ZappyTaskUtilities.DateFormat;
            DateTime dateTimeObject = new DateTime();
            if (!ZappyTaskUtilities.TryParseDateTimeString(value, out dateTimeObject))
            {
                object[] args = new object[]
                {
                    value, str,
                    includeTime ? WallClock.Now.ToString(CultureInfo.CurrentCulture) : WallClock.Now.ToShortDateString(),
                    str, ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, includeTime)
                };
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InvalidDateFormat, args));
            }
            this.SetDateTimePickerProperty(WinDateTimePicker.PropertyNames.DateTime, dateTimeObject);
        }

        private static void SetValueAsNumericControl(ScreenElement editBox, string value, ScreenElement spinBox)
        {
            if (TaskActivityElement.IsState(editBox.TechnologyElement, AccessibleStates.ReadOnly))
            {
                editBox.SetValueAsEditBox(value, 0x400);
                ScreenElement element = null;
                ScreenElement element2 = null;
                try
                {
                    if (((spinBox != null) && (editBox.Parent != null)) && (editBox.Parent.Parent != null))
                    {
                        ScreenElement[] elementArray =
                            spinBox.FindAllScreenElement(";[MSAA, VisibleOnly]ControlType='Button'", 1, true, true);
                        if ((elementArray != null) && (elementArray.Length == 2))
                        {
                            element = elementArray[0];
                            element2 = elementArray[1];
                        }
                    }
                }
                catch (COMException exception)
                {
                    object[] args = new object[] { exception };
                                    }
                if ((element != null) && (element2 != null))
                {
                    element.MouseButtonClick(MouseButtons.Left);
                    if (string.Equals(value, editBox.TechnologyElement.Value, StringComparison.Ordinal))
                    {
                        element2.MouseButtonClick(MouseButtons.Left);
                        element.MouseButtonClick(MouseButtons.Left);
                    }
                    else
                    {
                        element2.MouseButtonClick(MouseButtons.Left);
                    }
                }
                else
                {
                    editBox.SendKeys("{UP}");
                    if (string.Equals(value, editBox.TechnologyElement.Value, StringComparison.Ordinal))
                    {
                        editBox.SendKeys("{DOWN}");
                        editBox.SendKeys("{UP}");
                    }
                    else
                    {
                        editBox.SendKeys("{DOWN}");
                    }
                }
            }
            else
            {
                editBox.SetValueAsEditBox(value);
            }
        }

        private void SetValueAsScrollBar(int position)
        {
            int num;
            int num2;
            int num3;
            ZappyTaskControl topParent = UIControl.TopParent;
            if ((topParent != null) && (topParent.TechnologyElement != null))
            {
                NativeMethods.SetForegroundWindow(topParent.TechnologyElement.WindowHandle);
            }
            if (this.IsScrollBarVertical())
            {
                num = 1;
                num2 = 0x115;
            }
            else
            {
                num = 0;
                num2 = 0x114;
            }
            WinNativeMethods.SCROLLINFO structure = new WinNativeMethods.SCROLLINFO
            {
                fMask = 0x17
            };
            structure.cbSize = (uint)Marshal.SizeOf(structure);
            if (!this.win.GetScrollInfo(num, ref structure))
            {
                if (!this.win.GetScrollInfo(2, ref structure))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                num = 2;
            }
            ZappyTaskControlCollection controls =
                ALUtility.GetDescendantsByControlType(UIControl, technologyName, ControlType.Indicator, -1);
            if (controls.Count != 1)
            {
                ALUtility.ThrowNotSupportedException(false);
            }
            int nPos = structure.nPos;
            if (!string.IsNullOrEmpty(UIControl.ClassNameValue) &&
                UIControl.ClassNameValue.ToUpperInvariant().Contains("RICHTEXT"))
            {
                num3 = structure.nMax - ((int)structure.nPage);
            }
            else
            {
                num3 = (structure.nMax - ((int)structure.nPage)) + ((structure.nPage > 0) ? 1 : 0);
            }
            if ((position > num3) || (position < structure.nMin))
            {
                object[] args = new object[] { position, UIControl.ControlType.Name, PropertyName };
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture,
                    Resources.InvalidParameterValue, args));
            }
            Mouse.Click(controls[0]);
            structure.nPos = position;
            this.win.SetScrollInfo(num, ref structure, num2, true);
            Mouse.Click(controls[0]);
        }

        private void SetValueInEditableControl(ZappyTaskControl editControl, string propertyValue)
        {
            if (!string.IsNullOrEmpty(editControl.TechnologyElement.Value))
            {
                editControl.TechnologyElement.Value = string.Empty;
            }
            editControl.ScreenElement.SetValueAsEditBox(propertyValue, false, 0x1000);
        }

        private void SetWindowProperty(string propertyName, object value)
        {
            if ((string.Equals(WinWindow.PropertyNames.Maximized, propertyName, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(WinWindow.PropertyNames.Minimized, propertyName, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(WinWindow.PropertyNames.Restored, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.SetWindowStateProperty(propertyName, value);
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                TechnologyElementPropertyProvider.SetState(UIControl, state);
            }
        }

        private void SetWindowStateProperty(string propertyName, object value)
        {
            bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
            ControlStates state = ControlStates.None | ControlStates.Restored;
            if (string.Equals(WinWindow.PropertyNames.Maximized, propertyName, StringComparison.OrdinalIgnoreCase) &
                flag)
            {
                state = ControlStates.Maximized;
            }
            else if (string.Equals(WinWindow.PropertyNames.Restored, propertyName,
                         StringComparison.OrdinalIgnoreCase) && !flag)
            {
                state = ControlStates.Maximized;
            }
            else if (string.Equals(WinWindow.PropertyNames.Minimized, propertyName,
                         StringComparison.OrdinalIgnoreCase) & flag)
            {
                state = ControlStates.Minimized;
            }
            TechnologyElementPropertyProvider.SetState(UIControl, state);
        }

        private bool VerifyTriState(ControlStates requiredState)
        {
            if ((UIControl.StateValue & (ControlStates.None | ControlStates.Unavailable)) ==
                (ControlStates.None | ControlStates.Unavailable))
            {
                            }
            if ((((ControlStates.Checked & requiredState) == ControlStates.None) ||
                 !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked)) &&
                (((ControlStates.Indeterminate & requiredState) == ControlStates.None) ||
                 !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Indeterminate)))
            {
                if ((ControlStates.None | ControlStates.Normal) == requiredState)
                {
                    AccessibleStates[] states = new AccessibleStates[]
                        {AccessibleStates.Checked, AccessibleStates.Indeterminate};
                    return !TaskActivityElement.IsAnyState(UIControl.TechnologyElement, states);
                }
                return false;
            }
            return true;
        }

                                                                                                        
                
                
                
                
                
                
                
                
                        
    }
}