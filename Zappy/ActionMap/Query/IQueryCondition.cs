using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.Query
{
    public interface IQueryCondition
    {
        string Name { get; set; }

        IQueryCondition[] Conditions { get; set; }
        bool Match(ITaskActivityElement element);
        object GetPropertyValue(string nameOfProperty);
        void BindParameters(ValueMap valueMap);
        bool ParameterizeProperty(string nameOfProperty, string nameOfParameter);
    }
}
