using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Checks Input Actions")]
    public abstract class InputAction : ZappyTaskAction
    {
        private bool isGlobalHotKey;
        private ModifierKeys modifierKey;

        protected InputAction()
        {
        }

        protected InputAction(TaskActivityElement uiElement) : base(uiElement)
        {
        }

        internal override void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            base.ShallowCopy(source, isSeparateAction);
            InputAction action = source as InputAction;
            if (action != null)
            {
                ModifierKeys = action.ModifierKeys;
                isGlobalHotKey = action.IsGlobalHotkey;
                            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsGlobalHotkey
        {
            get =>
                isGlobalHotKey;
            set
            {
                isGlobalHotKey = value;
                NotifyPropertyChanged("IsGlobalHotkey");
            }
        }

        public ModifierKeys ModifierKeys
        {
            get =>
                modifierKey;
            set
            {
                modifierKey = value;
                NotifyPropertyChanged("ModifierKeys");
            }
        }
    }

}
