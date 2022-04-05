





using System;

namespace Zappy.ZappyActions.ElementPicker.Input
{
    public sealed class InputEventArgs : EventArgs
    {
        public InputEventType Type { get; internal set; }

        public int X { get; internal set; }

        public int Y { get; internal set; }

        public MouseButton Button { get; internal set; }

        public KeyboardKey Key { get; internal set; }

        public bool AltKey { get; internal set; }

        public bool CtrlKey { get; internal set; }

        public bool ShiftKey { get; internal set; }

        public bool WinKey { get; internal set; }

        public override string ToString()
        {
            return string.Format(
                "[{0} X={1}, Y={2}, Button={3}, Key={4}, AltKey={5}, CtrlKey={6}, ShiftKey={7}, WinKey={8}]",
                this.Type, this.X, this.Y, this.Button, this.Key, this.AltKey, this.CtrlKey, this.ShiftKey, this.WinKey);
        }
    }
}
