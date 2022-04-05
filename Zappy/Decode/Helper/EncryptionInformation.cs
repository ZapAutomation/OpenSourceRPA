using System;
using System.Xml.Serialization;

namespace Zappy.Decode.Helper
{
    [Serializable]
    public class EncryptionInformation
    {
        private bool isEncoded;
        private string textValue;
        private const string XmlTextEscapeString = @"\";

        public EncryptionInformation()
        {
        }

        public EncryptionInformation(string value, bool encode)
        {
            Value = value;
            isEncoded = encode;
        }

        [XmlAttribute]
        public bool Encoded
        {
            get =>
                isEncoded;
            set
            {
                isEncoded = value;
            }
        }

        [XmlIgnore]
        public string Value
        {
            get =>
                textValue;
            set
            {
                textValue = value;
            }
        }

        [XmlText]
        public string ValueWrapper
        {
            get
            {
                string str = Value != null ? Value : string.Empty;
                if (string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(str.Trim()) && !str.StartsWith(@"\", StringComparison.Ordinal))
                {
                    return str;
                }
                return str.Insert(0, @"\");
            }
            set
            {
                string str = value;
                if (!string.IsNullOrEmpty(str) && str.StartsWith(@"\", StringComparison.Ordinal))
                {
                    str = str.Remove(0, @"\".Length);
                }
                Value = str;
            }
        }
    }
}