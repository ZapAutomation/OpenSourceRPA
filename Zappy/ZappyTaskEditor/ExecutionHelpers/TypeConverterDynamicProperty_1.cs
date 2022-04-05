using System;
using System.ComponentModel;
using System.Globalization;
using Zappy.InputData;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public class TypeConverterDynamicProperty_1 : TypeConverter
    {
        public readonly TypeConverter InnerTypeConverter;
        public TypeConverterDynamicProperty_1(Type type)
        {
            InnerTypeConverter = TypeDescriptor.GetConverter(typeof(string));
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
                if (value is DynamicTextProperty dynamicTextProperty)
                {
                    if (dynamicTextProperty.DymanicKeySpecified)
                    {
                        return TypeConverterDynamicProperty.GetDynamicProeprtyString(context, dynamicTextProperty.DymanicKey);
                    }
                    DynamicTextProperty _Dp = (value as DynamicTextProperty);
                    string str = _Dp;
                    return str;
                }
                else if (value == null)
                    return string.Empty;
                else
                    return
                        value.ToString();
            }

                        return base.ConvertTo(context, culture, value, destinationType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            DynamicTextProperty dp = context.PropertyDescriptor.GetValue(context.Instance) as DynamicTextProperty;
            if (dp == null)
                dp = new DynamicTextProperty();
            SetValue(dp, value.ToString());
            return dp;
        }

        public void SetValue(DynamicTextProperty dp, string __Value)
        {
            dp.ResetFlags();
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
                                
                dp.Value = __Value;
            }
        }
    }
}