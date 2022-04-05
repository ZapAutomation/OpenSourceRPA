using System;
using System.Collections.Generic;
using System.Globalization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Helpers;
using Zappy.Properties;

namespace Zappy.ExecuteTask.TaskExecutor
{
    [CLSCompliant(true)]
    public abstract class ZappyTaskPropertyProvider
    {
        public abstract int GetControlSupportLevel(ZappyTaskControl uiTaskControl);
        public abstract string[] GetPredefinedSearchProperties(Type specializedClass);
        public abstract ZappyTaskPropertyDescriptor GetPropertyDescriptor(ZappyTaskControl uiTaskControl, string propertyName);
        public abstract string GetPropertyForAction(ZappyTaskControl uiTaskControl, ZappyTaskAction action);
        internal abstract string[] GetPropertyForControlState(ZappyTaskControl uiTaskControl, ControlStates uiState, out bool[] stateValues);
        public abstract ICollection<string> GetPropertyNames(ZappyTaskControl uiTaskControl);
        public abstract Type GetPropertyNamesClassType(ZappyTaskControl uiTaskControl);
        public abstract object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName);
        internal object GetPropertyValueWrapper(ZappyTaskControl uiControl, string propertyName)
        {
            object propertyValue;
            if (string.Equals(ZappyTaskControl.PropertyNames.UITechnologyElement, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiControl.TechnologyElement;
            }
            ZappyTaskControl uIControl = UIControl;
            string str = PropertyName;
            bool isGetProperty = IsGetProperty;
            bool restoreIfMinimized = uiControl.RestoreIfMinimized;
            bool useCachedControl = uiControl.UseCachedControl;
            uiControl.RestoreIfMinimized = false;
            uiControl.UseCachedControl = true;
            object[] args = { propertyName, uiControl.TechnologyElement.ControlTypeName };
            
            try
            {
                UIControl = uiControl;
                PropertyName = propertyName;
                IsGetProperty = true;
                try
                {
                    propertyValue = GetPropertyValue(uiControl, propertyName);
                }
                catch (NotSupportedException)
                {
                    object obj3;
                    if (this is TechnologyElementPropertyProvider || !TryGetPropertyFromTechnologyElement(uiControl, propertyName, out obj3))
                    {
                        throw;
                    }
                    propertyValue = obj3;
                }
                catch (RethrowException exception)
                {
                    if (!MapAndThrowException(exception))
                    {
                        throw;
                    }
                    propertyValue = null;
                }
            }
            finally
            {
                UIControl = uIControl;
                IsGetProperty = isGetProperty;
                PropertyName = str;
                uiControl.RestoreIfMinimized = restoreIfMinimized;
                uiControl.UseCachedControl = useCachedControl;
            }
            return propertyValue;
        }

        public abstract Type GetSpecializedClass(ZappyTaskControl uiTaskControl);
        internal bool MapAndThrowException(Exception exception)
        {
            RethrowException exception2 = exception as RethrowException;
            if (exception2 != null)
            {
                Exception internalException = exception2.InternalException;
                if (internalException is ArgumentNullException)
                {
                    object[] args = { PropertyName, UIControl.ControlType.Name };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidNullValue, args));
                }
                if (internalException is InvalidCastException)
                {
                    object[] objArray2 = { exception2.Value, UIControl.ControlType.Name, PropertyName, exception2.DataType, exception2.Value.GetType() };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValueOfDataType, objArray2));
                }
                if (internalException is NotSupportedException)
                {
                    string getPropertyNotSupportedMessage;
                    if (exception2.IsNotSupported)
                    {
                        if (IsGetProperty)
                        {
                            getPropertyNotSupportedMessage = Resources.GetPropertyNotSupportedMessage;
                        }
                        else
                        {
                            getPropertyNotSupportedMessage = Resources.SetPropertyNotSupportedMessage;
                        }
                    }
                    else if (IsGetProperty)
                    {
                        getPropertyNotSupportedMessage = Resources.GetPropertyFailed;
                    }
                    else
                    {
                        getPropertyNotSupportedMessage = Resources.SetPropertyFailed;
                    }
                    object[] objArray3 = { PropertyName, UIControl.ControlType };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, getPropertyNotSupportedMessage, objArray3));
                }
                if (internalException != null)
                {
                    throw internalException;
                }
            }
            return false;
        }

        public abstract void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value);
        internal void SetPropertyValueWrapper(ZappyTaskControl uiControl, string propertyName, object value)
        {
            bool useCachedControl = uiControl.UseCachedControl;
            uiControl.UseCachedControl = true;
            object[] args = { propertyName, uiControl.TechnologyElement.ControlTypeName };
            
            UIControl = uiControl;
            PropertyName = propertyName;
            IsGetProperty = false;
            try
            {
                SetPropertyValue(uiControl, propertyName, value);
            }
            catch (Exception exception)
            {
                if (!MapAndThrowException(exception))
                {
                    throw;
                }
            }
            finally
            {
                uiControl.UseCachedControl = useCachedControl;
                Execute.ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        internal static bool TryGetPropertyFromTechnologyElement(ZappyTaskControl uiControl, string propertyName, out object value)
        {
            try
            {
                value = TechnologyElementPropertyProvider.Instance.GetPropertyValue(uiControl, propertyName);
                return true;
            }
            catch (NotSupportedException)
            {
                value = null;
                return false;
            }
        }

        protected internal bool IsGetProperty { get; set; }

        protected internal string PropertyName { get; set; }

        protected internal ZappyTaskControl UIControl { get; set; }
    }
}