using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal interface ISqmUpdater
    {
        void UpdateSqmForElement(ITaskActivityElement element);
    }
}