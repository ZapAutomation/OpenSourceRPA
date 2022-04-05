using System;
using System.Collections.Generic;
using System.Globalization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal abstract class PropertyProviderBase : ZappyTaskPropertyProvider
    {
        protected Dictionary<string, ZappyTaskPropertyDescriptor> commonProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase);
        private string controlString = "Control";
        protected Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>> controlTypeToPropertiesMap;
        protected Dictionary<ControlType, Type> controlTypeToPropertyNamesClassMap;
        protected string specializedClassesNamespace;
        protected string specializedClassNamePrefix;
        protected string technologyName;

        public override int GetControlSupportLevel(ZappyTaskControl uiControl)
        {
            int num = 0;
            if (string.Equals(technologyName, uiControl.TechnologyName, StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }
            if (string.Equals(ZappyTaskService.Instance.GetCoreTechnologyName(uiControl.TechnologyName), technologyName, StringComparison.OrdinalIgnoreCase))
            {
                num = 1;
            }
            return num;
        }

        public override string[] GetPredefinedSearchProperties(Type specializedClass)
        {
            string[] strArray = new string[0];
            if (!string.Equals(specializedClassNamePrefix + controlString, specializedClass.Name, StringComparison.Ordinal))
            {
                strArray = new[] { ZappyTaskControl.PropertyNames.ControlType };
            }
            return strArray;
        }

        public override ZappyTaskPropertyDescriptor GetPropertyDescriptor(ZappyTaskControl uiControl, string propertyName)
        {
            ZappyTaskPropertyDescriptor descriptor = null;
            if (uiControl != null && !string.IsNullOrEmpty(propertyName))
            {
                Dictionary<string, ZappyTaskPropertyDescriptor> dictionary = null;
                if (controlTypeToPropertiesMap != null && controlTypeToPropertiesMap.TryGetValue(uiControl.ControlType, out dictionary))
                {
                    dictionary.TryGetValue(propertyName, out descriptor);
                    return descriptor;
                }
                commonProperties.TryGetValue(propertyName, out descriptor);
            }
            return descriptor;
        }

        public override string GetPropertyForAction(ZappyTaskControl uiControl, ZappyTaskAction action)
        {
            PropertyExpression expression = uiControl.SearchProperties.Find(ZappyTaskControl.PropertyNames.ControlType);
            if (expression != null && !string.IsNullOrEmpty(expression.PropertyValue))
            {
                return GetPropertyForAction(expression.PropertyValue, action);
            }
            return null;
        }

        protected abstract string GetPropertyForAction(string controlType, ZappyTaskAction action);
        internal override string[] GetPropertyForControlState(ZappyTaskControl uiControl, ControlStates uiState, out bool[] stateValues)
        {
            PropertyExpression expression = uiControl.SearchProperties.Find(ZappyTaskControl.PropertyNames.ControlType);
            if (expression != null && !string.IsNullOrEmpty(expression.PropertyValue))
            {
                return GetPropertyForControlState(expression.PropertyValue, uiState, out stateValues);
            }
            stateValues = new bool[0];
            return new string[0];
        }

        protected abstract string[] GetPropertyForControlState(string controlType, ControlStates uiState, out bool[] stateValues);
        public override ICollection<string> GetPropertyNames(ZappyTaskControl uiControl)
        {
            Dictionary<string, ZappyTaskPropertyDescriptor> dictionary = null;
            PropertyExpression expression = uiControl.SearchProperties.Find(ZappyTaskControl.PropertyNames.ControlType);
            if (expression != null && !string.IsNullOrEmpty(expression.PropertyValue) && controlTypeToPropertiesMap != null && controlTypeToPropertiesMap.TryGetValue(ControlType.GetControlType(expression.PropertyValue), out dictionary))
            {
                return dictionary.Keys;
            }
            return commonProperties.Keys;
        }

        public override Type GetPropertyNamesClassType(ZappyTaskControl uiControl)
        {
            Type type;
            PropertyExpression expression = uiControl.SearchProperties.Find(ZappyTaskControl.PropertyNames.ControlType);
            if (expression != null && !string.IsNullOrEmpty(expression.PropertyValue) && controlTypeToPropertyNamesClassMap != null && controlTypeToPropertyNamesClassMap.TryGetValue(expression.PropertyValue, out type))
            {
                return type;
            }
            return null;
        }

        public override object GetPropertyValue(ZappyTaskControl uiControl, string propertyName)
        {
            object obj2;
            if (!string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) && TryGetPropertyFromTechnologyElement(uiControl, propertyName, out obj2))
            {
                return obj2;
            }
            return GetPropertyValueInternal(uiControl, propertyName);
        }

        protected abstract object GetPropertyValueInternal(ZappyTaskControl uiControl, string propertyName);
        public override Type GetSpecializedClass(ZappyTaskControl uiControl)
        {
            if (this is TechnologyElementPropertyProvider)
            {
                throw new NotSupportedException();
            }
            ControlType controlType = uiControl.ControlType;
            object[] args = { specializedClassesNamespace, specializedClassNamePrefix + controlType.Name };
            Type type = Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", args));
            if (type == null)
            {
                object[] objArray2 = { specializedClassesNamespace, specializedClassNamePrefix + controlString };
                type = Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", objArray2));
            }
            return type;
        }

        public override void SetPropertyValue(ZappyTaskControl uiControl, string propertyName, object value)
        {
        }

        internal void ThrowExceptionIfControlDisabled()
        {
            if (UIControl != null && !UIControl.Enabled && !UIControl.WaitForControlEnabled(500))
            {
                            }
        }

        internal void ThrowExceptionIfReadOnly()
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
                            }
        }
    }
}