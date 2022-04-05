using System;
using System.Globalization;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Helper
{
    public class Entry
    {
        public Entry() : this(string.Empty, true)
        {
        }

        public Entry(string value) : this(value, true)
        {
        }

        public Entry(string value, bool isConstant)
        {
            EntryType = typeof(string);
            ValueString = isConstant ? string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { value }) : value;
        }

        public Entry(Type type, string value)
        {
            EntryType = type;
            ValueString = value;
        }

        public override string ToString()
        {
            if (EntryType == null)
            {
                return string.Empty;
            }
            if (EntryType != typeof(string))
            {
                try
                {
                    return Convert.ChangeType(ValueString, EntryType, CultureInfo.CurrentCulture).ToString();
                }
                catch (InvalidCastException exception)
                {
                    CrapyLogger.log.Error(exception.Message);
                    return string.Empty;
                }
            }
            return ZappyTaskActionLogEntry.GetLocalizedString(ValueString, false);
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public Type EntryType { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string TypeName
        {
            get =>
                EntryType?.FullName;
            set
            {
                string typeName = value;
                EntryType = Type.GetType(typeName);
                if (EntryType == null)
                {
                    EntryType = typeof(string);
                }
            }
        }

        public string ValueString { get; set; }
    }
}