using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Mssa;

namespace Zappy.ExecuteTask.Helpers
{
    internal class SingleQueryId
    {
        private const char attributeSeparator = ',';
        private static readonly Regex attributesRegEx = new Regex(@"(\[(?<attributes>[^\]]*)\])?(?<propertyList>.*)", ZappyTaskUtilities.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        private List<PropertyCondition> filterProperties;
        public const string FindAllAttribute = "FindAll";
        private static readonly Regex nameValueRegex = new Regex(@"(?<secondaryProperty>FilterCondition\(.*\))|((?<name>[^=\&\(]+)\s*(?<operator>(=|=>))\s*'(?<value>(\\\\|\\'|[^'])*)')", ZappyTaskUtilities.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        private List<PropertyCondition> properties;
        private const string propertySeparater = "&&";
        private string secondaryPropertyString;
        private List<string> technologyAttributes;

        public SingleQueryId(string queryString)
        {
            object[] args = { queryString };
            
            ZappyTaskUtilities.CheckForNull(queryString, "queryString");
            Match match = attributesRegEx.Match(queryString);
            string str = match.Groups["attributes"].ToString();
            string parameter = match.Groups["propertyList"].ToString();
            technologyAttributes = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                char[] separator = { ',' };
                foreach (string str3 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    technologyAttributes.Add(str3.Trim());
                }
            }
            properties = new List<PropertyCondition>();
            filterProperties = new List<PropertyCondition>();
            ZappyTaskUtilities.CheckForNull(parameter, "propValuePairString");
            for (match = nameValueRegex.Match(parameter); match.Success; match = match.NextMatch())
            {
                string str4 = match.Groups["secondaryProperty"].ToString();
                if (!string.IsNullOrEmpty(str4))
                {
                    secondaryPropertyString = str4;
                    foreach (IQueryCondition condition in FilterCondition.Parse(str4).Conditions)
                    {
                        filterProperties.Add((PropertyCondition)condition);
                    }
                }
                else
                {
                    PropertyCondition item = new PropertyCondition
                    {
                        PropertyName = match.Groups["name"].ToString().Trim(),
                        Value = PropertyCondition.Unescape(match.Groups["value"].ToString())
                    };
                    if (string.Equals(match.Groups["operator"].ToString().Trim(), "=>", StringComparison.Ordinal))
                    {
                        PropertyConditionOperator contains = PropertyConditionOperator.Contains;
                        item.PropertyOperatorWrapper = contains.ToString();
                    }
                    else
                    {
                        item.PropertyOperatorWrapper = PropertyConditionOperator.EqualTo.ToString();
                    }
                    properties.Add(item);
                }
            }
        }

        public void AddTechnologyAttribute(string attribute)
        {
            foreach (string str in technologyAttributes)
            {
                if (string.Equals(str, attribute, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            technologyAttributes.Add(attribute);
        }

        public int GetQueryPropertyIndex(string propertyName)
        {
            int num = 0;
            foreach (PropertyCondition condition in SearchProperties)
            {
                if (string.Equals(propertyName, condition.PropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public string GetQueryPropertyValue(string propertyName)
        {
            int queryPropertyIndex = GetQueryPropertyIndex(propertyName);
            if (queryPropertyIndex != -1)
            {
                return SearchProperties[queryPropertyIndex].ValueWrapper;
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(";");
            if (TechnologyAttributes.Count > 0)
            {
                builder.Append("[");
                builder.Append(TechnologyAttributes[0]);
                for (int i = 1; i < TechnologyAttributes.Count; i++)
                {
                    builder.Append(',');
                    builder.Append(TechnologyAttributes[i]);
                }
                builder.Append("]");
            }
            bool flag = true;
            foreach (PropertyCondition condition in properties)
            {
                if (!flag)
                {
                    builder.Append(" ");
                    builder.Append("&&");
                    builder.Append(" ");
                }
                flag = false;
                builder.Append(condition);
            }
            if (!string.IsNullOrEmpty(secondaryPropertyString))
            {
                builder.Append(" ");
                builder.Append("&&");
                builder.Append(" ");
                builder.Append(secondaryPropertyString);
            }
            return builder.ToString();
        }

        public bool UpdatePropertyValue(string propertyName, object propertyValue)
        {
            int queryPropertyIndex = GetQueryPropertyIndex(propertyName);
            if (queryPropertyIndex != -1)
            {
                SearchProperties[queryPropertyIndex].Value = propertyValue;
                return true;
            }
            return false;
        }

        public List<PropertyCondition> FilterProperties =>
            filterProperties;

        public bool IsExpandable
        {
            get
            {
                foreach (PropertyCondition condition in SearchProperties)
                {
                    if (condition.PropertyName.Equals("Role"))
                    {
                        string valueWrapper = condition.ValueWrapper;
                        if (string.Equals(valueWrapper, "menu item", StringComparison.OrdinalIgnoreCase) || string.Equals(valueWrapper, "outline item", StringComparison.OrdinalIgnoreCase) || string.Equals(valueWrapper, "check box", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    else if (condition.PropertyName.Equals("ControlType"))
                    {
                        string controlName = condition.ValueWrapper;
                        if (ControlType.TreeItem.NameEquals(controlName) || ControlType.MenuItem.NameEquals(controlName) || ControlType.CheckBoxTreeItem.NameEquals(controlName))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public List<PropertyCondition> SearchProperties =>
            properties;

        public IList<string> TechnologyAttributes =>
            technologyAttributes;
    }
}