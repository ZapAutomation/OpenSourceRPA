using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.Decode.Mssa
{
    public class FilterCondition : QueryCondition
    {
        public static readonly string ConditionDelimiter = ",";
        private static readonly Regex ConditionDelimiterRegex;
        private static readonly Regex OrderedMatchFormat = new Regex(@"\s*FilterCondition\s*\(\s*(?<OrderedQueryIdString>.+)\s*\)\s*\z", ZappyTaskUtilities.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        static FilterCondition()
        {
            object[] args = { ConditionDelimiter };
            ConditionDelimiterRegex = new Regex(string.Format(CultureInfo.InvariantCulture, "({0})", args), ZappyTaskUtilities.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public FilterCondition()
        {
            Conditions = null;
        }

        public FilterCondition(params IQueryCondition[] conditions) : base(conditions)
        {
        }

        public override bool Match(ITaskActivityElement element) =>
            true;

        public static FilterCondition Parse(string queryElement)
        {
            if (string.IsNullOrEmpty(queryElement))
            {
                throw new ArgumentNullException("queryElement", Resources.InvalidQueryString);
            }
            Match match = OrderedMatchFormat.Match(queryElement);
            if (!match.Success)
            {
                throw new ArgumentException(Resources.InvalidQueryString, "queryElement");
            }
            return new FilterCondition(PropertyCondition.ParseList(match.Groups["OrderedQueryIdString"].Value, ConditionDelimiter, ConditionDelimiterRegex).ToArray());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (Conditions != null && Conditions.Length != 0)
            {
                builder.Append("FilterCondition(");
                builder.Append(Conditions[0]);
                for (int i = 1; i < Conditions.Length; i++)
                {
                    builder.Append(ConditionDelimiter);
                    builder.Append(" ");
                    builder.Append(Conditions[i]);
                }
                builder.Append(")");
            }
            return builder.ToString();
        }

        public static bool TryParse(string queryElement, out FilterCondition condition)
        {
            try
            {
                condition = Parse(queryElement);
            }
            catch (ArgumentException)
            {
                condition = null;
                return false;
            }
            return true;
        }
    }
}