using System;
using System.ComponentModel;
using System.Windows.Forms;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
                [Serializable]
    [Description("Generate Warning")]
    public class WarningAction : TemplateAction
    {
                
        public WarningAction()
        {
            Message = "Alert";
        }

        public DynamicProperty<string> Message { get; set; }

                                
                                        
                                        
                
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            MessageBox.Show(Message, "Error Title", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

                                                                                

    }
}
