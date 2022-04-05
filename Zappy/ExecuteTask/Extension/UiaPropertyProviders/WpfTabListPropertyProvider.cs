using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTabListPropertyProvider : WpfControlPropertyProvider
    {
        public WpfTabListPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        private ZappyTaskControl GetChildByControlType(ZappyTaskControl UIControl, ControlType type, int instance)
        {
            ZappyTaskControl control = new ZappyTaskControl(UIControl)
            {
                TechnologyName = UIControl.TechnologyName,
                SearchProperties = {
                    {
                        ZappyTaskControl.PropertyNames.ControlType,
                        type.Name
                    },
                    {
                        ZappyTaskControl.PropertyNames.Instance,
                        instance.ToString(CultureInfo.InvariantCulture)
                    },
                    {
                        ZappyTaskControl.PropertyNames.MaxDepth,
                        "1"
                    }
                }
            };
            try
            {
                control.Find();
            }
            catch (ZappyTaskException)
            {
            }
            return control;
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfTabList.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfTabList.PropertyNames.Tabs, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.GetDescendantsByControlType(UIControl, UIControl.TechnologyName, ControlType.TabPage, 1);
            }
            if (!string.Equals(propertyName, WpfTabList.PropertyNames.SelectedIndex, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(UIControl, propertyName);
            }
            int[] selectedIndices = this.GetSelectedIndices(UIControl, ControlType.TabPage);
            if ((selectedIndices != null) && (selectedIndices.Length == 1))
            {
                return selectedIndices[0];
            }
            return -1;
        }

        private int[] GetSelectedIndices(ZappyTaskControl uiControl, ControlType expectedChildControlType)
        {
            AutomationElement nativeElement = uiControl.TechnologyElement.NativeElement as AutomationElement;
            AutomationElement[] selection = GetAutomationPattern<SelectionPattern>(uiControl, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty).Current.GetSelection();
            ZappyTaskControlCollection children = uiControl.GetChildren();
            children.RemoveAll(element => !element.ControlType.NameEquals(expectedChildControlType.Name));
            List<int> list = new List<int>(selection.Length);
            int item = 0;
            int index = 0;
            while ((item < children.Count) && (index < selection.Length))
            {
                if (selection[index] == ((AutomationElement)children[item].TechnologyElement.NativeElement))
                {
                    list.Add(item);
                    index++;
                }
                item++;
            }
            return list.ToArray();
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfTabList);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfTabList.PropertyNames.Tabs, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTabList.PropertyNames.SelectedIndex, new ZappyTaskPropertyDescriptor(typeof(int), s_ReadWritePermissions));
            return m_ControlSpecificProperties;
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (string.Equals(propertyName, WpfTabList.PropertyNames.SelectedIndex, StringComparison.OrdinalIgnoreCase))
            {
                int num = ZappyTaskUtilities.ConvertToType<int>(value);
                if (num < 0)
                {
                    object[] args = new object[] { num, uiTaskControl.ControlType.Name, propertyName };
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, args));
                }
                ZappyTaskControl control = this.GetChildByControlType(uiTaskControl, ControlType.TabPage, num + 1);
                if ((control == null) || !control.Exists)
                {
                    object[] objArray2 = new object[] { num, uiTaskControl.ControlType.Name, propertyName };
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, objArray2));
                }
                Mouse.Click(control);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.TabList;
    }
}

