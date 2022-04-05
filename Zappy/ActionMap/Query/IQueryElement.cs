using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.Query
{
    public interface IQueryElement
    {

        string[] SearchConfigurations { get; set; }
        ITaskActivityElement Ancestor { get; set; }
        IQueryCondition Condition { get; set; }
    }
}