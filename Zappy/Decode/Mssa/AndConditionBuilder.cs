using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;

namespace Zappy.Decode.Mssa
{
    internal class AndConditionBuilder
    {
        private List<IQueryCondition> conditions;

        public AndConditionBuilder()
        {
            conditions = new List<IQueryCondition>();
        }

        public AndConditionBuilder(IQueryCondition condition)
        {
            conditions = new List<IQueryCondition>();
            Append(condition);
        }

        public void Append(IQueryCondition condition)
        {
            conditions.Add(condition);
        }

        public void Append(string propertyName, object propertyValue)
        {
            conditions.Add(new PropertyCondition(propertyName, propertyValue));
        }

        public AndCondition Build()
        {
            List<IQueryCondition> list = new List<IQueryCondition>();
            List<IQueryCondition> list2 = new List<IQueryCondition>();
            foreach (IQueryCondition condition2 in conditions)
            {
                if (condition2 is PropertyCondition)
                {
                    list.Add(condition2);
                }
                else
                {
                    list2.Add(condition2);
                }
            }
            if (list.Count == 0 || list2.Count == 0)
            {
                return new AndCondition(conditions.ToArray());
            }
            AndCondition item = new AndCondition(list.ToArray());
            list2.Insert(0, item);
            return new AndCondition(list2.ToArray());
        }

        public int Count =>
            conditions.Count;
    }
}