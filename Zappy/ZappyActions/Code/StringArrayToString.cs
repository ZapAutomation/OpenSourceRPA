using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Gets String From StringArray")]
    public class StringArrayToString : TemplateAction
    {
                                [Category("Input")]
        [Description("StringArray")]
        public DynamicProperty<string[]> InputStringArray { get; set; }

                                [Category("Output")]
        [Description("Gets String from StringArray")]
        public string String { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            String = string.Join(Environment.NewLine, InputStringArray);
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input String:" + this.InputStringArray + " Output String:" + this.String;
        }
    }
}