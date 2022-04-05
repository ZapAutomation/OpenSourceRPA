using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Graph;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.InputData
{
    [TypeConverter(typeof(TypeConverterDynamicProperty_1))]
    public class DynamicTextProperty : DynamicProperty<string>, IEquatable<string>, IEquatable<DynamicTextProperty>
    {
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public bool TextIndexSpecified { get; set; }

        int _TextIndex;
        public int TextIndex { get { return _TextIndex; } set { _TextIndex = value; TextIndexSpecified = true; } }

        public void BeforeMLSerialization()
        {
            if (ValueSpecified)
            {

                if (TextIndex == 0)
                {
                    Cleanup();

                    if (Value.Length <= 1)
                        return;
                    TextIndex = UserTextDataManager.GetStringIndex(Value);
                    ValueSpecified = false;
                }
            }
        }

        public void AfterMLSerialization()
        {
            TextIndexSpecified = false;
            ValueSpecified = true;
        }


        public void Cleanup()
        {
            if (ValueSpecified)
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    foreach (string item in UserTextDataManager.removeEndSpecialChar)
                    {
                        if (Value.EndsWith(item))                        {
                            Value = Value.Substring(0, Value.Length - item.Length);
                            break;
                        }
                    }
                }
            }
        }


        public override void ResetFlags()
        {
            base.ResetFlags();
            TextIndexSpecified = false;
        }
        public static implicit operator string(DynamicTextProperty d)
        {
            if (d == null)
                return string.Empty;

            if (d.ValueSpecified)
            {

                if (d.Value != null)
                {
                    string __Value = d.Value.ToString();
                                                            return __Value;
                }
                else
                    return string.Empty;
            }
            else if (d.TextIndexSpecified)
                return "{TextIndex:" + d.TextIndex.ToString() + "}";
            else if (d.DymanicKeySpecified)
                return d.DymanicKey;
            else if (d.RuntimeScriptSpecified)
                return d.RuntimeScript;
            else
                return string.Empty;
        }

        public static implicit operator DynamicTextProperty(string d)
        {
            return new DynamicTextProperty() { Value = d };
        }


        public bool Equals(string other)
        {
            if (ValueSpecified)
            {
                return StringComparer.Ordinal.Equals(Value, other);
            }

            return false;
        }

        public bool Equals(DynamicTextProperty other)
        {
            throw new NotImplementedException();
        }

    }
}