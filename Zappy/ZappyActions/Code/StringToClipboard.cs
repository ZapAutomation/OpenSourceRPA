using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Sets string to the clipboard")]
    public class StringToClipboard : TemplateAction
    {
        public StringToClipboard()
        {
            this.PauseTimeAfterAction = 500;
        }
                                [Category("Input")]
        [Description("Enter the string")]
        public DynamicTextProperty Text { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string __Text = Text;
            if (!CommonProgram.SetTextInClipboard(__Text))
                throw new Exception("Unable to \"Set\" text to clipboard!");
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input Text:" + this.Text;
        }
    }
}