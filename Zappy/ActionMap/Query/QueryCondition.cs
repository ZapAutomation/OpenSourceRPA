using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.Query
{
    [Serializable, XmlInclude(typeof(AndCondition)), XmlInclude(typeof(FilterCondition)), XmlInclude(typeof(PropertyCondition))]
    public abstract class QueryCondition : IQueryCondition
    {
        private string name;
        private IQueryCondition[] queryConditions;

        protected QueryCondition()
        {
        }

        protected QueryCondition(params IQueryCondition[] conditions)
        {
            queryConditions = conditions;
        }

        public virtual void BindParameters(ValueMap valueMap)
        {
            if (Conditions != null)
            {
                foreach (IQueryCondition condition in Conditions)
                {
                    if (condition != null)
                    {
                        condition.BindParameters(valueMap);
                    }
                }
            }
        }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            QueryCondition condition = other as QueryCondition;
            if (condition != null && string.Equals(name, condition.name, StringComparison.Ordinal))
            {
                if (queryConditions == null && condition.queryConditions == null)
                {
                    return true;
                }
                if (queryConditions != null && condition.queryConditions != null && queryConditions.Length == condition.queryConditions.Length)
                {
                    for (int i = 0; i < queryConditions.Length; i++)
                    {
                        if (!Equals(queryConditions[i], condition.queryConditions[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int num = 0;
            if (name != null)
            {
                num ^= name.GetHashCode();
            }
            if (queryConditions != null)
            {
                foreach (IQueryCondition condition in queryConditions)
                {
                    if (condition != null)
                    {
                        num ^= condition.GetHashCode();
                    }
                }
            }
            return num;
        }

        public virtual object GetPropertyValue(string nameOfProperty)
        {
            object propertyValue = null;
            if (Conditions != null)
            {
                foreach (IQueryCondition condition in Conditions)
                {
                    if (condition != null)
                    {
                        propertyValue = condition.GetPropertyValue(nameOfProperty);
                        if (propertyValue != null)
                        {
                            return propertyValue;
                        }
                    }
                }
            }
            return propertyValue;
        }

        public abstract bool Match(ITaskActivityElement element);
        public virtual bool ParameterizeProperty(string nameOfProperty, string nameOfParameter)
        {
            if (Conditions != null)
            {
                foreach (IQueryCondition condition in Conditions)
                {
                    if (condition != null && !string.IsNullOrEmpty(nameOfParameter) && condition.ParameterizeProperty(nameOfProperty, nameOfParameter))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public abstract override string ToString();

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public IQueryCondition[] Conditions
        {
            get =>
                queryConditions;
            set
            {
                queryConditions = value;
            }
        }

        [XmlElement(ElementName = "Conditions")]
        [Browsable(false)]
        public QueryCondition[] ConditionsWrapper
        {
            get
            {
                QueryCondition[] conditionArray = null;
                if (queryConditions != null)
                {
                    conditionArray = new QueryCondition[queryConditions.Length];
                    for (int i = 0; i < queryConditions.Length; i++)
                    {
                        conditionArray[i] = queryConditions[i] as QueryCondition;
                    }
                }
                return conditionArray;
            }
            set
            {
                queryConditions = value;
            }
        }

        [XmlAttribute(AttributeName = "Id")]
        public string Name
        {
            get =>
                name;
            set
            {
                name = value;
            }
        }
    }
}
