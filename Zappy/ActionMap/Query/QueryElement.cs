using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.Query
{
    public class QueryElement : IQueryElement
    {
        private ITaskActivityElement ancestorNode;
        private QueryCondition condition;
        private string[] searchConfigurations;

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            QueryElement element = other as QueryElement;
            if (element == null)
            {
                return false;
            }
            return SearchConfiguration.AreEqual(searchConfigurations, element.searchConfigurations) && Equals(condition, element.condition) && Equals(ancestorNode, element.ancestorNode);
        }

        public override int GetHashCode()
        {
            int num = 0;
            if (ancestorNode != null)
            {
                num ^= ancestorNode.GetHashCode();
            }
            if (condition != null)
            {
                num ^= condition.GetHashCode();
            }
            return num;
        }

        public ITaskActivityElement Ancestor
        {
            get =>
                ancestorNode;
            set
            {
                ancestorNode = value;
            }
        }

        public IQueryCondition Condition
        {
            get =>
                condition;
            set
            {
                condition = value as QueryCondition;
            }
        }


        public string[] SearchConfigurations
        {
            get =>
                searchConfigurations;
            set
            {
                searchConfigurations = value;
            }
        }
    }
}
