using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfCellPropertyProvider : WpfControlPropertyProvider
    {
        public WpfCellPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfCell.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action) =>
            WpfCell.PropertyNames.Value;

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & ControlStates.Checked) != ControlStates.None)
            {
                list.Add(WpfCell.PropertyNames.Checked);
                stateValues.Add(true);
                return list;
            }
            if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
            {
                list.Add(WpfCell.PropertyNames.Checked);
                stateValues.Add(false);
                return list;
            }
            if ((uiState & ControlStates.Indeterminate) != ControlStates.None)
            {
                list.Add(WpfCell.PropertyNames.Indeterminate);
                stateValues.Add(true);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WpfCell.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.Value;
            }
            if (string.Equals(WpfCell.PropertyNames.ReadOnly, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(uiTaskControl.TechnologyElement, AccessibleStates.ReadOnly);
            }
            if (WpfCell.PropertyNames.Selected.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SelectionItemPattern pattern = GetAutomationPattern<SelectionItemPattern>(uiTaskControl, SelectionItemPattern.Pattern, AutomationElement.IsSelectionItemPatternAvailableProperty);
                return pattern?.Current.IsSelected;
            }
            if (WpfCell.PropertyNames.Checked.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.IsTableCellCheckBox(uiTaskControl))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                ZappyTaskControl firstChild = uiTaskControl.FirstChild;
                if (firstChild == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                return TaskActivityElement.IsState(firstChild.TechnologyElement, AccessibleStates.Checked);
            }
            if (WpfCell.PropertyNames.Indeterminate.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.IsTableCellCheckBox(uiTaskControl))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                ZappyTaskControl control2 = uiTaskControl.FirstChild;
                if (control2 == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                return TaskActivityElement.IsState(control2.TechnologyElement, AccessibleStates.Indeterminate);
            }
            if (WpfCell.PropertyNames.ContentControlType.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControl control3 = uiTaskControl.FirstChild;
                if (control3 == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                if (control3.NextSibling != null)
                {
                    return ControlType.Custom;
                }
                return control3.ControlType;
            }
            if (WpfCell.PropertyNames.ColumnIndex.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TableItemPattern pattern2 = GetAutomationPattern<TableItemPattern>(uiTaskControl, TableItemPattern.Pattern, AutomationElement.IsTableItemPatternAvailableProperty);
                if (pattern2 != null)
                {
                    return pattern2.Current.Column;
                }
                ALUtility.ThrowNotSupportedException(true);
                return 0;
            }
            if (WpfCell.PropertyNames.RowIndex.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TableItemPattern pattern3 = GetAutomationPattern<TableItemPattern>(uiTaskControl, TableItemPattern.Pattern, AutomationElement.IsTableItemPatternAvailableProperty);
                if (pattern3 != null)
                {
                    return pattern3.Current.Row;
                }
                ALUtility.ThrowNotSupportedException(true);
                return 0;
            }
            if (WpfCell.PropertyNames.ColumnHeader.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfCell.PropertyNames.ColumnHeader);
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfCell);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.Value, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.Checked, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.ReadOnly, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.Selected, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.Indeterminate, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.ContentControlType, new ZappyTaskPropertyDescriptor(typeof(ControlType)));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.RowIndex, new ZappyTaskPropertyDescriptor(typeof(int)));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.ColumnIndex, new ZappyTaskPropertyDescriptor(typeof(int)));
            m_ControlSpecificProperties.Add(WpfCell.PropertyNames.ColumnHeader, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName)
        {
            if (!string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.IsCommonReadableProperty(propertyName);
            }
            return true;
        }

        public override bool IsCommonWritableProperty(string propertyName)
        {
            if (!string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.IsCommonWritableProperty(propertyName);
            }
            return true;
        }

        private bool IsTableCellCheckBox(ZappyTaskControl UIControl)
        {
            if (UIControl.FirstChild == null)
            {
                return false;
            }
            AutomationElement nativeElement = UIControl.FirstChild.TechnologyElement.NativeElement as AutomationElement;
            AutomationElement element2 = UIControl.TechnologyElement.NativeElement as AutomationElement;
            if (((nativeElement == null) || (element2 == null)) || ((nativeElement.Current.ControlType != ControlType.CheckBox) || !string.Equals(element2.Current.ClassName, "DataGridCell", StringComparison.Ordinal)))
            {
                return false;
            }
            for (int i = 1; nativeElement != null; i++)
            {
                if (i > 1)
                {
                    return false;
                }
                nativeElement = TreeWalker.ControlViewWalker.GetNextSibling(nativeElement);
            }
            return true;
        }

        private void MakeCellEditableByClicking(ZappyTaskControl cell, ref TaskActivityElement firstChild)
        {
            int num = 0;
            while (((firstChild != null) && (firstChild.ControlTypeName != ActionMap.HelperClasses.ControlType.Edit)) && (num++ < 2))
            {
                Mouse.Click(cell);
                firstChild = ZappyTaskService.Instance.GetFirstChild(cell.TechnologyElement);
            }
        }

        private void MakeCellVisibleByClicking(ZappyTaskControl cell, ZappyTaskControl firstChild)
        {
            int num = 0;
            while (((firstChild.StateValue & (ControlStates.None | ControlStates.Offscreen)) == ControlStates.None) && (num++ < 2))
            {
                Mouse.Click(firstChild);
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (WpfCell.PropertyNames.Checked.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.ThrowExceptionIfControlDisabled(uiTaskControl);
                object[] args = new object[] { value };
                                ControlStates state = ZappyTaskUtilities.ConvertToType<bool>(value) ? ControlStates.Checked : (ControlStates.None | ControlStates.Normal);
                this.SetTableCellState(uiTaskControl, state, propertyName);
            }
            else if (WpfCell.PropertyNames.Indeterminate.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.ThrowExceptionIfControlDisabled(uiTaskControl);
                object[] objArray2 = new object[] { value };
                                ControlStates states2 = ZappyTaskUtilities.ConvertToType<bool>(value) ? ControlStates.Indeterminate : (ControlStates.None | ControlStates.Normal);
                this.SetTableCellState(uiTaskControl, states2, propertyName);
            }
            else if (WpfCell.PropertyNames.Value.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                object[] objArray3 = new object[] { value };
                                this.ThrowExceptionIfReadOnly(uiTaskControl);
                this.ThrowExceptionIfControlDisabled(uiTaskControl);
                ZappyTaskControl cell = uiTaskControl;
                TaskActivityElement firstChild = ZappyTaskService.Instance.GetFirstChild(cell.TechnologyElement);
                if (((firstChild == null) || (((firstChild.ControlTypeName != ActionMap.HelperClasses.ControlType.Text) && (firstChild.ControlTypeName != ActionMap.HelperClasses.ControlType.Edit)) && (firstChild.ControlTypeName != ActionMap.HelperClasses.ControlType.ComboBox))) || (ZappyTaskService.Instance.GetNextSibling(firstChild) != null))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                if ((firstChild.ControlTypeName == ActionMap.HelperClasses.ControlType.Text) || (firstChild.ControlTypeName == ActionMap.HelperClasses.ControlType.Edit))
                {
                    this.MakeCellEditableByClicking(cell, ref firstChild);
                    if ((firstChild == null) || (firstChild.ControlTypeName != ActionMap.HelperClasses.ControlType.Edit))
                    {
                        object[] objArray4 = new object[] { uiTaskControl.ControlType.Name };
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetValueNotSupportedMessage, objArray4));
                    }
                    if (!string.IsNullOrEmpty(firstChild.Value))
                    {
                        firstChild.Value = string.Empty;
                    }
                    firstChild.SetFocus();
                    Keyboard.SendKeys(cell, Keyboard.HandleSpecialCharacters(value as string));
                }
                else
                {
                    ZappyTaskControl control2 = ZappyTaskControl.FromTechnologyElement(firstChild, cell);
                    this.MakeCellVisibleByClicking(cell, control2);
                    control2 = cell.FirstChild;
                    if (control2 != null)
                    {
                        if ((control2.ControlType == ActionMap.HelperClasses.ControlType.ComboBox) && ((control2.StateValue & ControlStates.Expanded) != ControlStates.None))
                        {
                            Mouse.Click(control2);
                        }
                        control2.SetProperty(ZappyTaskControl.PropertyNames.Value, value as string);
                    }
                }
            }
            else
            {
                object[] objArray5 = new object[] { uiTaskControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetValueNotSupportedMessage, objArray5));
            }
        }

        private void SetTableCellState(ZappyTaskControl UIControl, ControlStates state, string PropertyName)
        {
            if (!this.IsTableCellCheckBox(UIControl))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            if ((((ControlStates.Checked & state) == ControlStates.None) && ((ControlStates.Indeterminate & state) == ControlStates.None)) && (((ControlStates.None | ControlStates.Normal) & state) == ControlStates.None))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            int num = 0;
            ZappyTaskControl firstChild = UIControl.FirstChild;
            if (!this.VerifyTriStateForCheckBox(firstChild, state))
            {
                while (num < 5)
                {
                    Mouse.Click(firstChild);
                    num++;
                    try
                    {
                        if (this.VerifyTriStateForCheckBox(firstChild, state))
                        {
                            return;
                        }
                        continue;
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                        object[] objArray1 = new object[] { UIControl.TechnologyElement };
                                                return;
                    }
                }
                object[] args = new object[] { PropertyName, UIControl.ControlType.Name };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetPropertyFailed, args));
            }
        }

        private void ThrowExceptionIfControlDisabled(ZappyTaskControl UIControl)
        {
            if (((UIControl != null) && !UIControl.Enabled) && !UIControl.WaitForControlEnabled(500))
            {
                            }
        }

        private void ThrowExceptionIfReadOnly(ZappyTaskControl UIControl)
        {
            bool property = false;
            if (UIControl != null)
            {
                try
                {
                    property = (bool)UIControl.GetProperty("ReadOnly");
                }
                catch (NotSupportedException)
                {
                }
                catch (InvalidCastException)
                {
                }
            }
            if (property)
            {
                throw new Exception();
            }
        }

        private bool VerifyTriStateForCheckBox(ZappyTaskControl element, ControlStates requiredState)
        {
            if ((element.StateValue & (ControlStates.None | ControlStates.Unavailable)) != ControlStates.None)
            {
                            }
            if ((((ControlStates.Checked & requiredState) == ControlStates.None) || !TaskActivityElement.IsState(element.TechnologyElement, AccessibleStates.Checked)) && (((ControlStates.Indeterminate & requiredState) == ControlStates.None) || !TaskActivityElement.IsState(element.TechnologyElement, AccessibleStates.Indeterminate)))
            {
                if ((ControlStates.None | ControlStates.Normal) == requiredState)
                {
                    AccessibleStates[] states = new AccessibleStates[] { AccessibleStates.Checked, AccessibleStates.Indeterminate };
                    return !TaskActivityElement.IsAnyState(element.TechnologyElement, states);
                }
                return false;
            }
            return true;
        }

        public override ActionMap.HelperClasses.ControlType SupportedControlType =>
            ActionMap.HelperClasses.ControlType.Cell;
    }
}

