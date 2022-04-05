using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Properties;

namespace Zappy.ActionMap.ElementManager
{
    internal class ElementManager : IElementManager
    {
        private UIElementDictionary elementDictionary;
        private RecorderOptions recorderOptions;

        TaskActivityElement IElementManager.AddElement(TaskActivityElement uiElement)
        {
            if (uiElement == null)
            {
                throw new ZappyTaskControlNotAvailableException(Resources.FailedToGetUIElement);
            }
                        {
                return elementDictionary.Add(uiElement);
            }
        }

        RecorderOptions IElementManager.RecorderOptions
        {
            get =>
                recorderOptions;
            set
            {
                recorderOptions = value;
            }
        }

        UIElementDictionary IElementManager.UIElementList
        {
            get =>
                elementDictionary;
            set
            {
                elementDictionary = value;
            }
        }
    }
}