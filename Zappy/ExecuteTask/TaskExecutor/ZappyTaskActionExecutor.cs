using System;
using System.Drawing;
using System.Windows.Forms;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.ExecuteTask.TaskExecutor
{
    [CLSCompliant(true)]
    public abstract class ZappyTaskActionExecutor
    {
        public abstract void Click(TaskActivityElement control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate);
        public abstract void DoubleClick(TaskActivityElement control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate);
        public abstract int GetControlSupportLevel(TaskActivityElement control);
        public abstract void Hover(TaskActivityElement control, Point relativeCoordinate, int millisecondsDuration);
        public abstract void MouseMove(TaskActivityElement control, Point relativeCoordinate);
        public abstract void MoveScrollWheel(TaskActivityElement control, int wheelMoveCount, ModifierKeys modifierKeys);
        public abstract void PressModifierKeys(TaskActivityElement control, ModifierKeys keys);
        public abstract void ReleaseKeyboard();
        public abstract void ReleaseModifierKeys(TaskActivityElement control, ModifierKeys keys);
        public abstract void ReleaseMouse();
        public abstract void SendKeys(TaskActivityElement control, string text, ModifierKeys modifierKeys, bool isUnicode);
        public abstract void StartDragging(TaskActivityElement control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate);
        public abstract void StopDragging(TaskActivityElement control, Point coordinate, bool isDisplacement);

    }
}