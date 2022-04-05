using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.HelperClasses
{
    public class AndCondition : QueryCondition
    {
        public static readonly string ConditionDelimiter = "&&";
        private static readonly Regex ConditionDelimiterRegex;

        static AndCondition()
        {
            object[] args = { ConditionDelimiter };
            ConditionDelimiterRegex = new Regex(string.Format(CultureInfo.InvariantCulture, "({0})", args), ZappyTaskUtilities.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public AndCondition()
        {
        }

        public AndCondition(params IQueryCondition[] conditions) : base(conditions)
        {
        }

        public override bool Match(ITaskActivityElement element)
        {
            if (Conditions != null)
            {
                foreach (IQueryCondition condition in Conditions)
                {
                    if (condition != null && !condition.Match(element))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static AndCondition Parse(string queryElement) =>
            new AndCondition(PropertyCondition.ParseList(queryElement, ConditionDelimiter, ConditionDelimiterRegex).ToArray());

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (Conditions != null && Conditions.Length != 0)
            {
                builder.Append(Conditions[0]);
                for (int i = 1; i < Conditions.Length; i++)
                {
                    object[] args = { ConditionDelimiter };
                    builder.AppendFormat(CultureInfo.InvariantCulture, " {0} ", args);
                    builder.Append(Conditions[i]);
                }
            }
            return builder.ToString();
        }
    }
}