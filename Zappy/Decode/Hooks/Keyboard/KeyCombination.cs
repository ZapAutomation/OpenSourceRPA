using System;
using System.Windows.Forms;

namespace Zappy.Decode.Hooks.Keyboard
{
    [Serializable]
    public class KeyCombination
    {
        private Keys key;
        private ModifierKeys modifierKeys;

        public KeyCombination(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public KeyCombination(string modifierValue, string keyValue)
        {
            Modifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), modifierValue, true);
            bool ignoreCase = Modifier > ModifierKeys.None;
            Key = (Keys)Enum.Parse(typeof(Keys), keyValue, ignoreCase);
        }

        public Keys Key
        {
            get =>
                key;
            set
            {
                key = value;
            }
        }

        public ModifierKeys Modifier
        {
            get =>
                modifierKeys;
            set
            {
                modifierKeys = value;
            }
        }
    }
}