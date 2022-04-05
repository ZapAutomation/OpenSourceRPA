using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;

namespace Zappy.ActionMap.ElementManager
{
    internal interface IElementManager
    {
        TaskActivityElement AddElement(TaskActivityElement uiElement);

        RecorderOptions RecorderOptions { get; set; }

        UIElementDictionary UIElementList { get; set; }
    }
}