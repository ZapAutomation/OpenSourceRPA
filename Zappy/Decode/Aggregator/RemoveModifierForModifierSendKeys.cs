using System;
using System.Windows.Forms;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.Decode.Aggregator
{
    internal class RemoveModifierForModifierSendKeys : ActionFilter
    {
        public RemoveModifierForModifierSendKeys() : base("RemoveModifierForModifierSendKeys", ZappyTaskActionFilterType.Binary, false, "SystemAggregators")
        {
        }

        private static ModifierKeys GetModifierForKey(string keyValue)
        {
            if (keyValue.Length > 2 && keyValue[0] == '{' && keyValue[keyValue.Length - 1] == '}')
            {
                try
                {
                    Keys key = (Keys)Enum.Parse(typeof(Keys), keyValue.Substring(1, keyValue.Length - 2), true);
                    return GetModifierForKey(key);
                }
                catch (ArgumentException)
                {
                }
            }
            return ModifierKeys.None;
        }

        private static ModifierKeys GetModifierForKey(Keys key)
        {
            switch (key)
            {
                case Keys.ShiftKey:
                    goto Label_0046;

                case Keys.ControlKey:
                    break;

                case Keys.Menu:
                    goto Label_0044;

                default:
                    if (key - 0x5b <= Keys.LButton)
                    {
                        return ModifierKeys.Windows;
                    }
                    switch (key)
                    {
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            goto Label_0046;

                        case Keys.LMenu:
                        case Keys.RMenu:
                            goto Label_0044;
                    }
                    return ModifierKeys.None;
            }
            return ModifierKeys.Control;
        Label_0044:
            return ModifierKeys.Alt;
        Label_0046:
            return ModifierKeys.Shift;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions) =>
            actions.Count > 1 && actions.Peek(1) is SendKeysAction;

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SendKeysAction action = actions.Peek(1) as SendKeysAction;
            ModifierKeys modifierForKey = GetModifierForKey(action.Text);
            if (modifierForKey != ModifierKeys.None && (action.ModifierKeys | modifierForKey) == action.ModifierKeys)
            {
                action.ModifierKeys ^= modifierForKey;
                action.AdditionalInfo = "ModifierKey";
            }
            return false;
        }
    }
}

