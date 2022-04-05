using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.ActionMap.HelperClasses
{
    public class PropertyCondition : QueryCondition
    {
        private static byte[] category;
        private const string ContainsOperatorString = "=>";
        private const string EqualToOperatorString = "=";
        internal const int MaxPropertyValueLength = 40;
        private string parameterName;
        private const char PropertyMetaCharacterEscapeChar = '\\';
        private string propertyName;
        private PropertyConditionOperator propertyOperator;
        private static readonly Regex PropertyRegex = new Regex(@"\b\s*(?<name>\w+)\s*(?<operator>=\S?)\s*'(?<value>.*)'\s*\z", ZappyTaskUtilities.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        private object propertyValue;
        private const char PropertyValueStartEndMarker = '\'';
        private const string XmlTextEscapeString = @"\";

        public PropertyCondition()
        {
            propertyOperator = PropertyConditionOperator.EqualTo;
        }

        public PropertyCondition(string name, object value) : this(name, value, PropertyConditionOperator.EqualTo)
        {
        }

        public PropertyCondition(string name, object value, PropertyConditionOperator op)
        {
            propertyName = name;
            propertyValue = value;
            propertyOperator = op;
        }

        public override void BindParameters(ValueMap valueMap)
        {
            object obj2;
            if (!string.IsNullOrEmpty(ParameterName) && valueMap.TryGetParameterValue(ParameterName, out obj2))
            {
                Value = obj2;
            }
        }

        public override bool Equals(object other)
        {
            PropertyCondition condition = other as PropertyCondition;
            if (condition == null)
            {
                return false;
            }
            return propertyOperator == condition.propertyOperator && string.Equals(propertyName, condition.propertyName, StringComparison.Ordinal) && Equals(propertyValue, condition.propertyValue);
        }

        public static string Escape(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!IsMetachar(input[i]))
                {
                    continue;
                }
                StringBuilder builder = new StringBuilder();
                char ch = input[i];
                builder.Append(input, 0, i);
                do
                {
                    builder.Append('\\');
                    builder.Append(ch);
                    i++;
                    int startIndex = i;
                    while (i < input.Length)
                    {
                        ch = input[i];
                        if (IsMetachar(ch))
                        {
                            break;
                        }
                        i++;
                    }
                    builder.Append(input, startIndex, i - startIndex);
                }
                while (i < input.Length);
                return builder.ToString();
            }
            return input;
        }

        public override int GetHashCode()
        {
            int hashCode = propertyOperator.GetHashCode();
            if (propertyName != null)
            {
                hashCode ^= propertyName.GetHashCode();
            }
            if (propertyValue != null)
            {
                hashCode ^= propertyValue.GetHashCode();
            }
            return hashCode;
        }

        public override object GetPropertyValue(string nameOfProperty)
        {
            object propertyValue = null;
            if (string.Equals(propertyName, nameOfProperty, StringComparison.Ordinal))
            {
                propertyValue = this.propertyValue;
            }
            return propertyValue;
        }

        private static bool IsMetachar(char ch) =>
            ch <= '|' && Category[ch] >= 1;

        public override bool Match(ITaskActivityElement element)
        {
            object propertyValue = null;
            try
            {
                propertyValue = element.GetPropertyValue(PropertyName);
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (NotImplementedException)
            {
                return false;
            }
            string a = propertyValue == null ? string.Empty : propertyValue.ToString();
            string str2 = Value == null ? string.Empty : Value.ToString();
            a = a == null ? string.Empty : a;
            str2 = str2 == null ? string.Empty : str2;
            if (a.Length > 40 && str2.Length == 40 && propertyOperator != PropertyConditionOperator.Contains)
            {
                return a.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
            }
            if (propertyOperator == PropertyConditionOperator.Contains)
            {
                return a.IndexOf(str2, StringComparison.OrdinalIgnoreCase) != -1;
            }
            return string.Equals(a, str2, StringComparison.OrdinalIgnoreCase);
        }

        public override bool ParameterizeProperty(string nameOfProperty, string nameOfParameter)
        {
            if (string.Equals(PropertyName, nameOfProperty, StringComparison.OrdinalIgnoreCase))
            {
                ParameterName = nameOfParameter;
                return true;
            }
            return false;
        }

        public static PropertyCondition Parse(string queryElement)
        {
            PropertyConditionOperator equalTo;
            if (string.IsNullOrEmpty(queryElement))
            {
                throw new ArgumentNullException("queryElement", Resources.InvalidQueryString);
            }
            Match match = PropertyRegex.Match(queryElement);
            if (!match.Success)
            {
                throw new ArgumentException(Resources.InvalidQueryString, "queryElement");
            }
            string a = match.Groups["operator"].Value;
            if (string.Equals(a, "=", StringComparison.Ordinal))
            {
                equalTo = PropertyConditionOperator.EqualTo;
            }
            else
            {
                if (!string.Equals(a, "=>", StringComparison.Ordinal))
                {
                    throw new ArgumentException(Resources.InvalidQueryString, "queryElement");
                }
                equalTo = PropertyConditionOperator.Contains;
            }
            string name = match.Groups["name"].Value;
            return new PropertyCondition(name, Unescape(match.Groups["value"].Value), equalTo);
        }

        internal static List<IQueryCondition> ParseList(string queryElement, string conditionDelimiter, Regex conditionDelimiterRegex)
        {
            if (string.IsNullOrEmpty(queryElement))
            {
                throw new ArgumentNullException("queryElement", Resources.InvalidQueryString);
            }
            bool flag = false;
            int startIndex = 0;
            List<IQueryCondition> list = new List<IQueryCondition>();
            int num2 = queryElement.Length - conditionDelimiter.Length;
            for (int i = 0; i <= queryElement.Length; i++)
            {
                if (!flag && (i == queryElement.Length || i < num2 && string.Equals(queryElement.Substring(i, conditionDelimiter.Length), conditionDelimiter)))
                {
                    string str = queryElement.Substring(startIndex, i - startIndex).Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        PropertyCondition item = Parse(str);
                        list.Add(item);
                    }
                    startIndex = i + conditionDelimiter.Length;
                }
                else if (Equals(queryElement[i], '\\'))
                {
                    i++;
                }
                else if (Equals(queryElement[i], '\''))
                {
                    flag = !flag;
                }
            }
            if (flag)
            {
                                throw new ArgumentException(Resources.InvalidQueryString, "queryElement");
            }
            return list;
        }

        public override string ToString()
        {
            string str = string.Empty;
            if (propertyValue != null)
            {
                str = Escape(propertyValue.ToString());
            }
            string str2 = "=";
            if (propertyOperator == PropertyConditionOperator.Contains)
            {
                str2 = "=>";
            }
            object[] args = { propertyName, str2, '\'', str };
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{2}", args);
        }


        public static string Unescape(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(input, 0, i);
                    do
                    {
                        i++;
                        if (input[i] == '\\')
                        {
                            builder.Append('\\');
                            i++;
                        }
                        int startIndex = i;
                        while (i < input.Length && input[i] != '\\')
                        {
                            i++;
                        }
                        builder.Append(input, startIndex, i - startIndex);
                    }
                    while (i < input.Length);
                    return builder.ToString();
                }
            }
            return input;
        }

        private static byte[] Category
        {
            get
            {
                if (category == null)
                {
                    category = new byte[0x80];
                    category[0x27] = 1;
                    category[0x5c] = 1;
                }
                return category;
            }
        }

        [XmlAttribute(AttributeName = "ParameterName")]
        public string ParameterName
        {
            get =>
                parameterName;
            set
            {
                parameterName = value;
            }
        }

        [XmlAttribute(AttributeName = "Name")]
        public string PropertyName
        {
            get =>
                propertyName;
            set
            {
                propertyName = value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public PropertyConditionOperator PropertyOperator
        {
            get =>
                propertyOperator;
            set
            {
                propertyOperator = value;
            }
        }

        [XmlAttribute(AttributeName = "Operator")]
        public string PropertyOperatorWrapper
        {
            get
            {
                if (propertyOperator == PropertyConditionOperator.EqualTo)
                {
                    return null;
                }
                return propertyOperator.ToString();
            }
            set
            {
                propertyOperator = PropertyConditionOperator.EqualTo;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        propertyOperator = (PropertyConditionOperator)Enum.Parse(typeof(PropertyConditionOperator), value, true);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public object Value
        {
            get =>
                propertyValue;
            set
            {
                propertyValue = value;
            }
        }

        [XmlText]
        public string ValueWrapper
        {
            get
            {
                string str = Value != null ? Value.ToString() : string.Empty;
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
