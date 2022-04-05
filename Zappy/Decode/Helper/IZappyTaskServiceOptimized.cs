using Zappy.ActionMap.TaskTechnology;

namespace Zappy.Decode.Helper
{
    internal interface IZappyTaskServiceOptimized : IZappyTaskService
    {
        TaskActivityElement GetFirstChildFast(TaskActivityElement element);
        TaskActivityElement GetNextSiblingFast(TaskActivityElement element);
        TaskActivityElement GetParentFast(TaskActivityElement element);
        TaskActivityElement GetPreviousSiblingFast(TaskActivityElement element);
    }
}