using System;
using System.ComponentModel;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Picture
{
    public class FocusWindowType : TemplateAction
    {
        [Category("Input")]
        [Description("Text to send on focused window")]
        public DynamicTextProperty Text { get; set; }

        [Category("Input")]
        [Description("Select one of the modifier key")]
        public ModifierKeys ModifierKeys { get; set; }
        [Category("Optional")] public int DelayBetweenKeyboardCharacters { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                                                                                
            int result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.sendKeys(Text, Convert.ToInt32(this.ModifierKeys), null, 0, DelayBetweenKeyboardCharacters);

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Text " + this.Text;
        }
    }
}
