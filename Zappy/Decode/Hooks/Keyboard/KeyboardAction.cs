using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.Keyboard
{

    public class KeyboardAction : InputAction
    {
        private KeyActionType actionType;
        private Keys key;
        private string keyValue;

        public KeyboardAction()
        {
        }

        public KeyboardAction(KeyActionType actionType, Keys key, string keyValue)
        {
            InitializeInternal(actionType, key, keyValue);
        }

        public KeyboardAction(TaskActivityElement uiElement, KeyActionType actionType, Keys key, string keyValue) : base(uiElement)
        {
            InitializeInternal(actionType, key, keyValue);
        }

        internal override string GetParameterString()
        {
            string str = string.Empty;
            if (IsControl())
            {
                if (ModifierKeys != ModifierKeys.None)
                {
                    str = ModifierKeys + " + ";
                }
                return str + KeysToString(Key);
            }
            return KeyValue;
        }

        private void InitializeInternal(KeyActionType keyActionType, Keys keys, string value)
        {
            ZappyTaskUtilities.CheckForNull(value, "value");
            ActionType = keyActionType;
            Key = keys;
            KeyValue = value;
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                    }

        internal bool IsControl()
        {
            if (!string.IsNullOrEmpty(KeyValue) && ModifierKeys != ModifierKeys.Windows && ModifierKeys != ModifierKeys.Alt && ModifierKeys != (ModifierKeys.Shift | ModifierKeys.Alt))
            {
                return char.IsControl(KeyValue, 0);
            }
            return true;
        }

        private static string KeysToString(Keys key)
        {
            if (key == Keys.Next)
            {
                return "PageDown";
            }
            if (key == Keys.Capital)
            {
                return "CapsLock";
            }
            return key.ToString();
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override string ActionName
        {
            get =>
                ActionType.ToString();
            set
            {
                ActionType = (KeyActionType)Enum.Parse(typeof(KeyActionType), value);
                NotifyPropertyChanged("ActionName");
            }
        }

        public KeyActionType ActionType
        {
            get =>
                actionType;
            set
            {
                actionType = value;
                NotifyPropertyChanged("ActionType");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override string AdditionalInfo
        {
            get
            {
                if (!IsGlobalHotkey && !IsControl())
                {
                    return "Printable";
                }
                return "NonPrintable";
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public Keys Key
        {
            get =>
                key;
            set
            {
                key = value;
                NotifyPropertyChanged("Key");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public string KeyValue
        {
            get =>
                keyValue;
            set
            {
                keyValue = value;
                NotifyPropertyChanged("KeyValue");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override string ValueAsString
        {
            get
            {
                if (IsControl())
                {
                    return Key.ToString();
                }
                return KeyValue;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }


}
