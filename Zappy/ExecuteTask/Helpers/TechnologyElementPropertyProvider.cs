using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Extension.WinControls;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.ExecuteTask.Helpers
{
    internal class TechnologyElementPropertyProvider : PropertyProviderBase
    {
        private static PropertyProviderBase instance;

        public TechnologyElementPropertyProvider()
        {
            commonProperties = InitializeCommonProperties();
        }

        internal static string[] CreateQueryIdForSelectedIndices(string technologyName, int[] selectedIndices)
        {
            int num = 0;
            string[] strArray = new string[selectedIndices.Length];
            foreach (int num3 in selectedIndices)
            {
                object[] args = { technologyName, num3 + 1 };
                strArray[num++] = string.Format(CultureInfo.InvariantCulture, ";[{0}]ControlType='ListItem' && Instance='{1}'", args);
            }
            return strArray;
        }

        public override int GetControlSupportLevel(ZappyTaskControl uiControl) =>
            1;

        public override ZappyTaskPropertyDescriptor GetPropertyDescriptor(ZappyTaskControl uiControl, string propertyName)
        {
            if (commonProperties.ContainsKey(propertyName))
            {
                return commonProperties[propertyName];
            }
            return null;
        }

        protected override string GetPropertyForAction(string controlType, ZappyTaskAction action)
        {
            if (action is SetValueAction)
            {
                return ZappyTaskControl.PropertyNames.Value;
            }
            return null;
        }

        protected override string[] GetPropertyForControlState(string controlType, ControlStates uiState, out bool[] stateValues)
        {
            stateValues = new bool[0];
            return null;
        }

        public override ICollection<string> GetPropertyNames(ZappyTaskControl uiControl) =>
            commonProperties.Keys;

        public override object GetPropertyValue(ZappyTaskControl uiControl, string propertyName)
        {
            if (string.Equals(ZappyTaskControl.PropertyNames.ClassName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyElement.ClassName;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Enabled, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.IsWindowEnabled(uiControl);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.FriendlyName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyElement.FriendlyName;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.NativeElement, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyElement.NativeElement;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.QueryId, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ScreenMapUtil.GetCompleteQueryId(uiControl.TechnologyElement);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.ControlType, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ControlType.GetControlType(uiControl.TechnologyElement.ControlTypeName);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates none = ControlStates.None;
                if (FrameworkUtilities.IsTopLevelElement(uiControl.TechnologyElement))
                {
                    return ScreenElement.GetWindowState(uiControl.TechnologyElement.WindowHandle);
                }
                return none | ALUtility.ConvertState(uiControl.TechnologyElement.GetRequestedState(~AccessibleStates.None));
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.WindowHandle, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyElement.WindowHandle;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.IsTopParent, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return FrameworkUtilities.IsTopLevelElement(uiControl.TechnologyElement);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.TechnologyName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyName;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.TopParent, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TaskActivityElement objA = FrameworkUtilities.TopLevelElement(uiControl.TechnologyElement);
                if (objA == null)
                {
                    return null;
                }
                if (Equals(objA, uiControl.TechnologyElement))
                {
                    return uiControl;
                }
                return ZappyTaskControl.FromTechnologyElement(objA);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.HasFocus, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return TaskActivityElement.IsState(uiControl.TechnologyElement, AccessibleStates.Focused) || Equals(ZappyTaskService.Instance.GetFocusedElement(), uiControl.TechnologyElement);
                }
                catch (ZappyTaskException)
                {
                    return false;
                }
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.BoundingRectangle, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                int num;
                int num2;
                int num3;
                int num4;
                try
                {
                    uiControl.TechnologyElement.GetBoundingRectangle(out num, out num2, out num3, out num4);
                }
                catch (InvalidCastException)
                {
                    object[] args = { uiControl.ToString() };
                    CrapyLogger.log.ErrorFormat("Invalid bounding rectangle was returned for {0}", args);
                    if (Execute.ExecutionHandler.Settings.AutoRefetchEnabled)
                    {
                        object[] objArray2 = { propertyName, uiControl.TechnologyElement.ToString() };
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.GetPropertyFailed, objArray2));
                    }
                    return Rectangle.Empty;
                }
                return new Rectangle(num, num2, num3, num4);
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Height, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.BoundingRectangle.Height;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Width, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.BoundingRectangle.Width;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Top, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.BoundingRectangle.Top;
            }
            if (string.Equals(ZappyTaskControl.PropertyNames.Left, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.BoundingRectangle.Left;
            }
            return uiControl.TechnologyElement.GetPropertyValue(propertyName);
        }

        protected override object GetPropertyValueInternal(ZappyTaskControl uiControl, string propertyName) =>
            null;

        private static Dictionary<string, ZappyTaskPropertyDescriptor> InitializeCommonProperties() =>
            new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.ClassName,
                    new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.FriendlyName,
                    new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.NativeElement,
                    new ZappyTaskPropertyDescriptor(typeof(object), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.QueryId,
                    new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.ControlType,
                    new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.State,
                    new ZappyTaskPropertyDescriptor(typeof(ControlStates), ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.WindowHandle,
                    new ZappyTaskPropertyDescriptor(typeof(IntPtr), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.IsTopParent,
                    new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.TechnologyName,
                    new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.TopParent,
                    new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.HasFocus,
                    new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.BoundingRectangle,
                    new ZappyTaskPropertyDescriptor(typeof(Rectangle), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.Height,
                    new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.Width,
                    new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.Top,
                    new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                },
                {
                    ZappyTaskControl.PropertyNames.Left,
                    new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                }
            };

        internal static void SelectUsingInstanceAndName(ZappyTaskControl uiControl, string name, int instance)
        {
            string controlName = uiControl.ControlType.Name;
            string selectedIndex = string.Empty;
            if (ControlType.ComboBox.NameEquals(controlName))
            {
                selectedIndex = WinComboBox.PropertyNames.SelectedIndex;
            }
            else if (ControlType.List.NameEquals(controlName))
            {
                selectedIndex = WinList.PropertyNames.SelectedIndices;
            }
            string technologyName = uiControl.TechnologyName;
            object[] args = { technologyName, name, instance };
            string str4 = string.Format(CultureInfo.InvariantCulture, ";[{0}]ControlType='ListItem' && Name='{1}' && Instance='{2}'", args);
            if (ControlType.ComboBox.NameEquals(controlName))
            {
                uiControl.ScreenElement.SetValueAsComboBoxUsingQueryId(str4);
            }
            else if (ControlType.List.NameEquals(controlName))
            {
                string[] values = { str4 };
                uiControl.ScreenElement.SetValueAsListBox(values, true);
            }
        }

        internal static void SetCheckBoxState(ZappyTaskControl uiControl, ControlStates state)
        {
            if ((ControlStates.Checked & state) != ControlStates.None)
            {
                uiControl.ScreenElement.Check();
            }
            else if ((ControlStates.None | ControlStates.Normal) == state)
            {
                uiControl.ScreenElement.Uncheck();
            }
            else if (ControlStates.Indeterminate == state)
            {
                uiControl.ScreenElement.CheckIndeterminate();
            }
            else
            {
                object[] args = { state, uiControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetStateNotSupportedForControlTypeMessage, args));
            }
        }

        internal static void SetListItemState(ZappyTaskControl uiControl, ControlStates state)
        {
            if (uiControl.ControlType == ControlType.ListItem)
            {
                if (state == (ControlStates.None | ControlStates.Selected))
                {
                    uiControl.ScreenElement.Select();
                }
                else
                {
                    object[] args = { state, uiControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetStateNotSupportedForControlTypeMessage, args));
                }
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiControl, string propertyName, object value)
        {
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SetValue(uiControl, value, false, false);
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SetState(uiControl, (ControlStates)Enum.Parse(typeof(ControlStates), (string)value));
            }
            else
            {
                object[] args = { propertyName, uiControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyNotSupportedMessage, args));
            }
        }

        internal static void SetRadioButtonState(ZappyTaskControl uiControl, ControlStates state)
        {
            if ((ControlStates.Checked & state) != ControlStates.None)
            {
                uiControl.ScreenElement.Check();
            }
            else
            {
                object[] args = { state, uiControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetStateNotSupportedForControlTypeMessage, args));
            }
        }

        internal static void SetState(ZappyTaskControl uiControl, ControlStates state)
        {
            if (uiControl.ControlType == ControlType.RadioButton)
            {
                SetRadioButtonState(uiControl, state);
            }
            else if (uiControl.ControlType == ControlType.CheckBox)
            {
                SetCheckBoxState(uiControl, state);
            }
            else if (uiControl.ControlType == ControlType.ListItem)
            {
                SetListItemState(uiControl, state);
            }
            else
            {
                if (uiControl.ControlType == ControlType.Window)
                {
                    object property = uiControl.GetProperty(ZappyTaskControl.PropertyNames.IsTopParent);
                    if (property is bool && (bool)property)
                    {
                        SetWindowState(uiControl, state);
                        return;
                    }
                    object[] objArray1 = { state, uiControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyFailed, objArray1));
                }
                object[] args = { state, uiControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetStateNotSupportedForControlTypeMessage, args));
            }
        }

        internal static void SetValue(ZappyTaskControl uiControl, object value, bool isEncoded, bool preferEdit)
        {
            if (uiControl.ControlType == ControlType.ComboBox)
            {
                SetValueAsComboBox(uiControl, value as string, preferEdit);
            }
            else if (uiControl.ControlType == ControlType.Edit)
            {
                SetValueAsEditBox(uiControl, value as string, isEncoded, false);
            }
            else if (uiControl.ControlType == ControlType.List)
            {
                string[] values = value as string[];
                if (value != null && values == null)
                {
                    values = CommaListBuilder.GetCommaSeparatedValues(value.ToString()).ToArray();
                }
                SetValueAsListBox(uiControl, values);
            }
            else
            {
                object[] args = { uiControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetValueNotSupportedMessage, args));
            }
        }

        internal static void SetValueAsComboBox(ZappyTaskControl uiControl, string value, bool preferEdit)
        {
            uiControl.ScreenElement.SetValueAsComboBox(value, preferEdit);
        }

        internal static void SetValueAsEditBox(ZappyTaskControl uiControl, string value, bool isEncoded, bool useCopyPaste)
        {
            ThrowExceptionIfReadOnly(uiControl);
            if (useCopyPaste)
            {
                uiControl.ScreenElement.SetValueAsEditBox(value, 0x800);
            }
            else
            {
                uiControl.ScreenElement.SetValueAsEditBox(value, isEncoded);
            }
        }

        internal static void SetValueAsListBox(ZappyTaskControl uiControl, string value)
        {
            uiControl.ScreenElement.SetValueAsListBox(value);
        }

        internal static void SetValueAsListBox(ZappyTaskControl uiControl, string[] values)
        {
            uiControl.ScreenElement.SetValueAsListBox(values);
        }

        internal static void SetValueUsingQueryId(ZappyTaskControl uiControl, int[] selectedIndices, string technologyName, int maxCount)
        {
            string name = uiControl.ControlType.Name;
            string selectedIndex = string.Empty;
            if (ControlType.ComboBox.NameEquals(name))
            {
                selectedIndex = WinComboBox.PropertyNames.SelectedIndex;
            }
            else if (ControlType.List.NameEquals(name))
            {
                selectedIndex = WinList.PropertyNames.SelectedIndices;
            }
            foreach (int num2 in selectedIndices)
            {
                if (num2 >= maxCount || num2 < 0)
                {
                    object[] args = { num2, uiControl.ControlType.Name, selectedIndex };
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                }
            }
            string[] values = CreateQueryIdForSelectedIndices(technologyName, selectedIndices);
            if (ControlType.ComboBox.NameEquals(name))
            {
                uiControl.ScreenElement.SetValueAsComboBoxUsingQueryId(values[0]);
            }
            else if (ControlType.List.NameEquals(name))
            {
                uiControl.ScreenElement.SetValueAsListBox(values, true);
            }
        }

        internal static void SetWindowState(ZappyTaskControl uiControl, ControlStates state)
        {
            uiControl.ScreenElement.SetWindowState(state);
        }

        private static void ThrowExceptionIfReadOnly(ZappyTaskControl uiControl)
        {
            bool property = false;
            if (uiControl != null)
            {
                try
                {
                    property = (bool)uiControl.GetProperty("ReadOnly");
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
                throw new Exception("ActionNotSupportedOnDisabledControlException");
            }
        }

        internal static PropertyProviderBase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TechnologyElementPropertyProvider();
                }
                return instance;
            }
        }
    }
}