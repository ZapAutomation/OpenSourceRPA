using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Zappy.SharedInterface.Helper
{
    public class TypeConverterDynamicProperty : TypeConverter
    {
        Type _genericInstanceType, _innerType;
        public readonly TypeConverter InnerTypeConverter;
        PropertyInfo _Value;
        public TypeConverterDynamicProperty(Type type)
        {
            if (type.IsGenericType
         && type.GetGenericTypeDefinition() == typeof(DynamicProperty<>)
        && type.GetGenericArguments().Length == 1)
            {
                _genericInstanceType = type;
                _innerType = type.GetGenericArguments()[0];
                InnerTypeConverter = TypeDescriptor.GetConverter(_innerType);
                _Value = type.GetProperty("Value");
            }
            else
            {
                throw new ArgumentException("Incompatible type", "type");
            }
        }
                                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {

            if (destinationType == typeof(string))
            {
                return true;
            }

                        return InnerTypeConverter.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }

            return InnerTypeConverter.CanConvertFrom(context, sourceType);
        }

                public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is IDynamicProperty)
                {
                    IDynamicProperty _Dp = (value as IDynamicProperty);
                    if (_Dp.ValueSpecified)
                        return value.ToString();
                    else if (_Dp.DymanicKeySpecified)
                    {
                        return GetDynamicProeprtyString(context, _Dp.DymanicKey);
                    }
                    else if (_Dp.RuntimeScriptSpecified)
                        return _Dp.RuntimeScript;
                    else
                        return string.Empty;
                }
                else if (value == null)
                    return string.Empty;
                else
                    return
                        value.ToString();
            }

                        return base.ConvertTo(context, culture, value, destinationType);
        }

        public static string GetDynamicProeprtyString(ITypeDescriptorContext context, string dynamicProperty)
        {
            try
            {
                dynamic _Page = (context.GetType().GetProperty("OwnerGrid").GetValue(context) as PropertyGrid).Tag;
                if (!_Page.GetType().GetProperty("showDynamicPropertyCodeBool").GetValue(_Page))
                {
                    string[] splitedValues = dynamicProperty.Split(':');
                    string guid = splitedValues[0].Replace(SharedConstants.VariableNameBegin, "");
                    splitedValues[1] = splitedValues[1].Replace("}", "");
                    foreach (dynamic pageNode in _Page.GetType().GetProperty("Nodes").GetValue(_Page))
                    {
                        if (pageNode.Activity.SelfGuid.ToString() == guid)
                        {
                            
                            splitedValues[0] = string.IsNullOrEmpty(pageNode.Activity.DisplayName)
                                ? pageNode.Activity.GetType().Name
                                : pageNode.Activity.DisplayName;
                            break;
                        }
                    }
                    return splitedValues[0] + " -> " + splitedValues[1];
                }
                else
                {
                    return dynamicProperty;
                }
            }
            catch
            {
                return dynamicProperty;
            }
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            IDynamicProperty dp = null;
                        if (context != null)
                dp = context.PropertyDescriptor.GetValue(context.Instance) as IDynamicProperty;
            if (dp == null)
                dp = _genericInstanceType.GetConstructor(new Type[0] { }).Invoke(null) as IDynamicProperty;
            SetValue(dp, value);
            return dp;
        }

        public void SetValue(IDynamicProperty dp, object value)
        {
            if (value is string)
            {
                string __Value = value.ToString();
                if (__Value.StartsWith(SharedConstants.VariableNameBegin))
                {
                    dp.DymanicKey = __Value;
                    dp.ValueSpecified = dp.RuntimeScriptSpecified = false;
                }
                else if (__Value.Contains(SharedConstants.ScriptCriteria))
                {
                    dp.RuntimeScript = __Value;
                    dp.ValueSpecified = dp.DymanicKeySpecified = false;
                }
                else
                {
                    dp.RuntimeScriptSpecified = dp.DymanicKeySpecified = false;
                                        
                    if (_innerType.IsInstanceOfType(value))
                        _Value.SetValue(dp, __Value);

                    else if (InnerTypeConverter.CanConvertFrom(typeof(string)))
                    {
                        try
                        {
                            _Value.SetValue(dp, InnerTypeConverter.ConvertFromString(__Value));
                        }
                        catch
                        {
                            string _ErrorText =
                        "Variable value must be 1) Dynamic variable ${VAR_NAME}, 2) Object Value, 3) some string value which can be converted to " + _innerType.FullName;
                            throw new Exception(_ErrorText);
                        }
                    }
                }
            }
            else
            {
                if (InnerTypeConverter.CanConvertFrom(value.GetType()))
                {
                    try
                    {
                        _Value.SetValue(dp, InnerTypeConverter.ConvertFrom(value));
                    }
                    catch
                    {
                        string _ErrorText =
                    "Variable value must be 1) Dynamic variable ${VAR_NAME}, 2) Object Value, 3) some string value which can be converted to " + _innerType.FullName;
                        throw new Exception(_ErrorText);
                    }
                }
            }
        }
    }
}
