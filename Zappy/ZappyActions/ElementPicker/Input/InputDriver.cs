





using System.Collections.Generic;

namespace Zappy.ZappyActions.ElementPicker.Input
{
    public sealed partial class EditorPageInputDriver
    {
        public event InputEventHandler OnKeyUp = delegate { };

        public event InputEventHandler OnKeyDown = delegate { };

        public event InputEventHandler OnMouseUp = delegate { };

        public event InputEventHandler OnMouseDown = delegate { };

        public event InputEventHandler OnMouseMove = delegate { };

        public event InputEventHandler OnInput = delegate { };

        public void KeyUp(KeyboardKey key) => SetInputState(new InputEventArgs() { Type = InputEventType.KeyUp, Key = key });

        public void KeyDown(KeyboardKey key) => SetInputState(new InputEventArgs() { Type = InputEventType.KeyDown, Key = key });

        public void MouseUp(MouseButton button) => SetInputState(new InputEventArgs() { Type = InputEventType.MouseUp, Button = button });

        public void MouseDown(MouseButton button) => SetInputState(new InputEventArgs() { Type = InputEventType.MouseDown, Button = button });

        public void MouseMove(int x, int y) => SetInputState(new InputEventArgs() { Type = InputEventType.MouseMove, X = x, Y = y });

        public void Click(MouseButton button)
        {
            this.MouseDown(button);
            this.MouseUp(button);
        }

        public void Press(params KeyboardKey[] keys)
        {
            var modKeys = new Dictionary<KeyboardKey, bool>
            {
                { KeyboardKey.LeftAlt, false },
                { KeyboardKey.LeftCtrl, false },
                { KeyboardKey.LeftShift, false },
                { KeyboardKey.LeftWin, false },
                { KeyboardKey.RightAlt, false },
                { KeyboardKey.RightCtrl, false },
                { KeyboardKey.RightShift, false },
                { KeyboardKey.RightWin, false }
            };
            foreach (KeyboardKey key in keys)
            {
                this.KeyDown(key);
                if (modKeys.ContainsKey(key))
                {
                    modKeys[key] = true;
                }
                else
                {
                    this.KeyUp(key);
                }
            }
            foreach (KeyboardKey key in modKeys.Keys)
            {
                if (modKeys[key] == true)
                {
                    this.KeyUp(key);
                }
            }
        }

        public void Write(string text)
        {
            foreach (char c in text ?? string.Empty)
            {
                this.KeyDown((KeyboardKey)(-c));
                this.KeyUp((KeyboardKey)(-c));
            }
        }
    }
}
