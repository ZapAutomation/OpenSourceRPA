using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfControlPropertyProvider : IUiaControlsPropertyProvider
    {
        protected Dictionary<string, ZappyTaskPropertyDescriptor> m_CommonPropertyNames = new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase);
        protected Dictionary<string, ZappyTaskPropertyDescriptor> m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase);
        protected Dictionary<string, ZappyTaskPropertyDescriptor> m_DescriptorDictionary;
        protected static ZappyTaskPropertyAttributes s_ReadNonAssertPermissions = (ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable);
        protected static ZappyTaskPropertyAttributes s_ReadWritePermissions = (ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable);
        private static readonly Regex s_WpfClassNameRegex = new Regex(@"^HwndWrapper\[.*;.*;.*\]", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public WpfControlPropertyProvider()
        {
            this.m_DescriptorDictionary = this.InitilizeDescriptorDictionary();
        }

        protected T GetAutomationPattern<T>(ZappyTaskControl uiControl, AutomationPattern pattern, AutomationProperty isPatternAvailableProperty)
        {
            AutomationElement nativeElement = uiControl.TechnologyElement.NativeElement as AutomationElement;
            return this.GetAutomationPattern<T>(nativeElement, pattern, isPatternAvailableProperty);
        }

        protected T GetAutomationPattern<T>(AutomationElement automationElement, AutomationPattern pattern, AutomationProperty isPatternAvailableProperty)
        {
            try
            {
                if (automationElement != null)
                {
                    object obj3;
                    object currentPropertyValue = automationElement.GetCurrentPropertyValue(isPatternAvailableProperty);
                    if ((((currentPropertyValue != null) && (currentPropertyValue != AutomationElement.NotSupported)) && ((currentPropertyValue is bool) && ((bool)currentPropertyValue))) && automationElement.TryGetCurrentPattern(pattern, out obj3))
                    {
                        return (T)obj3;
                    }
                }
            }
            catch (ElementNotAvailableException)
            {
            }
            return default(T);
        }

        protected int GetChildListItemCount(ZappyTaskControl control)
        {
            AutomationElement nativeElement = control.NativeElement as AutomationElement;
            int count = 0;
            if ((AutomationElement.IsItemContainerPatternAvailableProperty != null) && ((bool)nativeElement.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty)))
            {
                object obj2;
                if (nativeElement.TryGetCurrentPattern(ItemContainerPattern.Pattern, out obj2))
                {
                    ItemContainerPattern pattern = obj2 as ItemContainerPattern;
                    for (AutomationElement element2 = pattern.FindItemByProperty(null, AutomationElement.ControlTypeProperty, ControlType.ListItem); element2 != null; element2 = pattern.FindItemByProperty(element2, AutomationElement.ControlTypeProperty, ControlType.ListItem))
                    {
                        count++;
                    }
                }
                return count;
            }
            AutomationElementCollection elements = nativeElement.FindAll(TreeScope.Children, new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            if (elements != null)
            {
                count = elements.Count;
            }
            return count;
        }

        public virtual int GetControlSupportLevel(ZappyTaskControl control)
        {
            if (control.SearchProperties.Contains(WpfControl.PropertyNames.FrameworkId))
            {
                string b = control.SearchProperties[WpfControl.PropertyNames.FrameworkId];
                if (string.Equals(WpfControl.s_FrameworkId, b, StringComparison.OrdinalIgnoreCase))
                {
                    return 100;
                }
            }
            else if (control.TechnologyElement != null)
            {
                object propertyValue = control.TechnologyElement.GetPropertyValue(WpfControl.PropertyNames.FrameworkId);
                if ((propertyValue != null) && string.Equals(WpfControl.s_FrameworkId, propertyValue.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return 100;
                }
            }
            else if (!control.SearchProperties.Contains(WpfControl.PropertyNames.FrameworkId))
            {
                control.SearchProperties.Add(WpfControl.PropertyNames.FrameworkId, WpfControl.s_FrameworkId);
                return 100;
            }
            return 0;
        }

        public virtual Dictionary<string, ZappyTaskPropertyDescriptor> GetPropertiesMap() =>
            this.m_DescriptorDictionary;

        public virtual Type GetPropertyClassName() =>
            typeof(WpfControl.PropertyNames);

        public virtual string GetPropertyForAction(ZappyTaskAction action) =>
            null;

        public virtual List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues) =>
            new List<string>();

        public virtual object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName) =>
            ALUtility.ThrowNotSupportedException(true);

        public virtual Type GetSpecializedClass() =>
            typeof(WpfControl);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitilizeDescriptorDictionary()
        {
            this.m_CommonPropertyNames.Add(ZappyTaskControl.PropertyNames.ControlType, new ZappyTaskPropertyDescriptor(typeof(ControlType), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.HelpText, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.Font, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.AcceleratorKey, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.AccessKey, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.AutomationId, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.LabeledBy, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.ItemStatus, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable));
            this.m_CommonPropertyNames.Add(WpfControl.PropertyNames.FrameworkId, new ZappyTaskPropertyDescriptor(typeof(string), (ZappyTaskPropertyAttributes.CommonToTechnology | s_ReadNonAssertPermissions) | ZappyTaskPropertyAttributes.Searchable));
            return this.m_CommonPropertyNames;
        }

        public virtual bool IsCommonReadableProperty(string propertyName) =>
            (this.m_DescriptorDictionary.ContainsKey(propertyName) && this.m_DescriptorDictionary[propertyName].Attributes.HasFlag(ZappyTaskPropertyAttributes.Readable));

        public virtual bool IsCommonWritableProperty(string propertyName) =>
            (this.m_DescriptorDictionary.ContainsKey(propertyName) && this.m_DescriptorDictionary[propertyName].Attributes.HasFlag(ZappyTaskPropertyAttributes.Writable));

        public virtual void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if ((uiTaskControl != null) && uiTaskControl.IsBound)
            {
                uiTaskControl.TechnologyElement.SetPropertyValue(propertyName, value);
            }
        }

        public virtual ActionMap.HelperClasses.ControlType SupportedControlType =>
            null;
    }
}

