using Zappy.ActionMap.Enums;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.HelperClasses
{
    public interface IZappyTaskEventNotify
    {
        void Notify(ITaskActivityElement source, ITaskActivityElement target, ZappyTaskEventType eventType, object eventArgs);

        void NotifyMultiSource(ITaskActivityElement[] sources, ITaskActivityElement target, ZappyTaskEventType eventType, object eventArgs, ElementForThumbnailCapture elementForThumbnailImage);
    }
}