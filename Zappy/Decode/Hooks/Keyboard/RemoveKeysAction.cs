using System;
using System.Windows.Forms;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.Keyboard
{
    [Serializable]
    public class RemoveKeysAction : InputAction
    {
        private Keys key;

        public RemoveKeysAction()
        {
        }

        public RemoveKeysAction(ModifierKeys modifierKeys, Keys key)
        {
            ModifierKeys = modifierKeys;
            Key = key;
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
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
    }
}