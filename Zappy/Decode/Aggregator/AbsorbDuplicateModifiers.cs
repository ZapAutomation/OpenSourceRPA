using System;
using System.Windows.Forms;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbDuplicateModifiers : ActionFilter
    {
        private long lastKeyUpActionId;

        public AbsorbDuplicateModifiers() : base("AbsorbDuplicateModifiers", ZappyTaskActionFilterType.Binary, false, "SystemAggregators")
        {
        }

        private bool IsLast2ModifierKeyDownSubsetOfLast(InputAction lastAction, SendKeysAction last2Action) =>
            last2Action != null && lastAction != null && lastKeyUpActionId < last2Action.Id && IsModifierKey(last2Action.Text) && (lastAction.ModifierKeys | last2Action.ModifierKeys) == lastAction.ModifierKeys;

        private bool IsLastRepeatOfLast2ModifierKeyDown(KeyboardAction lastAction, InputAction last2Action) =>
            lastAction != null && last2Action != null && lastAction.ModifierKeys == last2Action.ModifierKeys && lastKeyUpActionId < last2Action.Id && IsModifierKey(lastAction.Key);

        protected override bool IsMatch(ZappyTaskActionStack actions) =>
            actions.Count > 1 && actions.Peek() is InputAction;

        private static bool IsModifierKey(string keyValue)
        {
            if (keyValue.Length > 2 && keyValue[0] == '{' && keyValue[keyValue.Length - 1] == '}')
            {
                try
                {
                    Keys key = (Keys)Enum.Parse(typeof(Keys), keyValue.Substring(1, keyValue.Length - 2), true);
                    return IsModifierKey(key);
                }
                catch (ArgumentException)
                {
                }
            }
            return false;
        }

        private static bool IsModifierKey(Keys key)
        {
            switch (key)
            {
                case Keys.Menu:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                case Keys.LMenu:
                case Keys.RMenu:
                    return true;

                                

                
                default:
                    if (key - 0x5b <= Keys.LButton)
                    {
                        return true;
                    }

                    return false;
            }

        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
                        return false;
            KeyboardAction lastAction = actions.Peek() as KeyboardAction;
            if (lastAction != null)
            {
                if (lastAction.ActionType == KeyActionType.KeyUp)
                {
                    lastKeyUpActionId = lastAction.Id;
                    return false;
                }
                if (lastAction.ActionType == KeyActionType.KeyDown && IsLastRepeatOfLast2ModifierKeyDown(lastAction, actions.Peek(1) as InputAction))
                {
                    actions.Pop();
                    return true;
                }
            }
            InputAction action2 = actions.Peek() as InputAction;
            SendKeysAction action3 = actions.Peek(1) as SendKeysAction;
            if (IsLast2ModifierKeyDownSubsetOfLast(action2, action3))
            {
                actions.Pop(1);
                return false;
            }
            return false;
        }
    }
}

