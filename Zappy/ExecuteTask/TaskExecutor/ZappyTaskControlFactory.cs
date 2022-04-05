using System;
using System.Drawing;

namespace Zappy.ExecuteTask.TaskExecutor
{
    public static class ZappyTaskControlFactory
    {
        public static ZappyTaskControl FromNativeElement(object nativeElement, string technologyName) =>
            ZappyTaskControl.FromNativeElement(nativeElement, technologyName);

        public static ZappyTaskControl FromPoint(Point absoluteCoordinates) =>
            ZappyTaskControl.FromPoint(absoluteCoordinates);

        public static ZappyTaskControl FromWindowHandle(IntPtr windowHandle) =>
            ZappyTaskControl.FromWindowHandle(windowHandle);
    }
}