using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                [Description("Gets The Value From Clipboard")]
    public class ClipboardToString : TemplateAction
    {
                                [Category("Output")]
        [Description("Return the clipboard value")]
        public string Text { get; set; }

                                                        
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string __Text;
            if (!CommonProgram.GetTextFromClipboard(out __Text))
                throw new Exception("Unable to \"Set\" text to clipboard!");
                        Text = __Text;
                                }

        public override string AuditInfo()
        {
            return "Clipboard Data: " + this.Text;
        }
    }
}