using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Zappy.Plugins.ChromeBrowser
{
    public class CsvConverter : TypeConverter
    {
                public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            List<String> v = value as List<String>;
            if (destinationType == typeof(string))
            {
                return String.Join(",", v.ToArray());
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}