using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    [Serializable]

    public class Setting
    {
        private const int DefaultWarningLevel = 1;
        private string name;
        private string settingValue;
        private int warningLevel;

        public Setting()
        {
        }

        public Setting(string name, object value) : this(name, value, 1)
        {
        }

        public Setting(string name, object value, int warningLevel)
        {
            Name = name;
            if (value != null)
            {
                settingValue = value.ToString();
            }
            this.warningLevel = warningLevel;
        }

        public bool Equals(Setting target) =>
            target != null && string.Equals(Name, target.Name, StringComparison.Ordinal) && string.Equals(Value, target.Value, StringComparison.Ordinal);

        public override bool Equals(object obj)
        {
            string b = obj as string;
            if (b != null)
            {
                return string.Equals(Name, b, StringComparison.Ordinal);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() =>
            base.GetHashCode();

        public override string ToString()
        {
            object[] args = { Name, Value };
            return string.Format(CultureInfo.InvariantCulture, "Name: {0}, Value: {1}. ", args);
        }

        [XmlAttribute]
        public string Name
        {
            get =>
                name;
            set
            {
                name = value;
            }
        }

        [XmlAttribute]
        public string Value
        {
            get =>
                settingValue;
            set
            {
                settingValue = value;
            }
        }

        [XmlAttribute]
        public int WarningLevel
        {
            get =>
                warningLevel;
            set
            {
                warningLevel = value;
            }
        }
    }
}