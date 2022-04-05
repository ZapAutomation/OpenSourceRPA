using System;
using System.Drawing;
using Zappy.ActionMap.ElementManager;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Hooks.LowLevelHookEvent
{
    internal interface IEventCapture
    {
        event EventHandler<EventArgs> Started;


        bool SetTrackingElement(TaskActivityElement element, Point interactionPoint, bool alwaysTakeSnapshot);

        IElementManager ElementManager { get; set; }

        IZappyActionStack RawActionList { get; set; }

        RecorderOptions RecorderOptions { get; set; }

    }
}