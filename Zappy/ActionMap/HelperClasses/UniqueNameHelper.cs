using System.Collections.Concurrent;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.HelperClasses
{
    internal class UniqueNameHelper
    {
        internal ConcurrentDictionary<ITaskActivityElement, string> UniqueNameDictionary = new ConcurrentDictionary<ITaskActivityElement, string>(new ElementQueryComparer());

        internal bool TryAdd(ITaskActivityElement element, string objectName) =>
            UniqueNameDictionary.TryAdd(element, objectName);
    }
}