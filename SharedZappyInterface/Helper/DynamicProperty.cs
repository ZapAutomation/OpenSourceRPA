using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Zappy.SharedInterface.Helper
{

    [TypeConverter(typeof(TypeConverterDynamicProperty))]
    public class DynamicProperty<T> : IDynamicProperty
    {
        static TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));

        public DynamicProperty()
        {
            _DymanicKey = string.Empty;
        }


        T _Value;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public T Value { get { return _Value; } set { _Value = value; ValueSpecified = true; } }


        [XmlIgnore]
        public bool RuntimeScriptSpecified { get; set; }


        string _RuntimeScript;
        public string RuntimeScript { get { return _RuntimeScript; } set { _RuntimeScript = value; RuntimeScriptSpecified = true; } }


        [XmlIgnore]
        public bool ValueSpecified { get; set; }


        string _DymanicKey;

        public string DymanicKey { get { return _DymanicKey; } set { _DymanicKey = value; DymanicKeySpecified = true; } }

        [DefaultValue(false)]
        public bool EvaluateOnFirstUse { get; set; }

        [XmlIgnore]
        public bool EvaluateOnFirstUseSpecified { get { return EvaluateOnFirstUse; } }


        [XmlIgnore]
        public bool DymanicKeySpecified { get; set; }
        [XmlIgnore]
        public Type ElementType => typeof(T);

        [XmlIgnore]
        public object ObjectValue
        {
            get { return Value; }
            set
            {
                try
                {
                    Value = (T)value;
                }
                catch
                {
                    if (typeof(T) == typeof(string))
                    {
                        Value = (T)tc.ConvertFrom(value.ToString());
                    }
                    if (tc.CanConvertFrom(value.GetType()))
                    {
                        Value = (T)tc.ConvertFrom(value);
                    }
                    else
                        throw new InvalidCastException(string.Format("can not convert from {0} to {1}", value.GetType().Name, typeof(T).Name));
                }
            }
        }


        public virtual void ResetFlags()
        {
            DymanicKeySpecified = ValueSpecified = RuntimeScriptSpecified = false;
        }

                                

        public override string ToString()
        {
            if (ValueSpecified)
            {
                if (Value != null)
                {
                    string __Value = Value.ToString();
                                                                                                    return __Value;
                }
                else
                    return string.Empty;
            }
            else if (DymanicKeySpecified)
            {                
                return _DymanicKey;
            }
            else if (RuntimeScriptSpecified)
                return _RuntimeScript;
            else
                return string.Empty;
        }


        public static implicit operator T(DynamicProperty<T> d)
        {
            if (ReferenceEquals(d, null))
            {
                                return new DynamicProperty<T>();
                            }

            return d.ValueSpecified ? d.Value : default(T);
        }

        public static implicit operator DynamicProperty<T>(T d)
        {
            return new DynamicProperty<T>() { Value = d };
        }

    }
}
