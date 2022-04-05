using System;
using System.Collections.Generic;

namespace Zappy.ActionMap.HelperClasses
{
    internal class PropertyBag<TProperty> : Dictionary<TProperty, object>
    {
        private string propertyBagName;

        public PropertyBag(string propertBagName)
        {
            propertyBagName = propertBagName;
        }

        public void AddProperty<TValue>(TProperty property, object propertyValue, bool throwIfValueIsNull)
        {
            TValue local = ConvertToType<TValue>(propertyValue, throwIfValueIsNull);
            if (ContainsKey(property))
            {
                Remove(property);
            }
            Add(property, local);
        }

        private T ConvertToType<T>(object value, bool throwIfValueIsNull)
        {
            if (throwIfValueIsNull && value == null)
            {
                throw new ArgumentNullException(propertyBagName);
            }
            if (value != null)
            {
                try
                {
                    return (T)value;
                }
                catch (InvalidCastException)
                {
                    object[] args = { value, propertyBagName, typeof(T), value.GetType() };
                                    }
            }
            return default(T);
        }

        public TValue GetProperty<TValue>(TProperty property)
        {
            if (ContainsKey(property))
            {
                return ConvertToType<TValue>(base[property], false);
            }
            return default(TValue);
        }
    }
}