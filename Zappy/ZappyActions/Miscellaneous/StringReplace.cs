using System.Activities;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Replace String With Using Regular Expression")]
    public sealed class StringReplace : TemplateAction
    {
                public StringReplace()
        {
                    }

        [Category("Input"), RequiredArgument]
        [Description("")]
        public DynamicProperty<string> InputString { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("Old string")]
        public DynamicProperty<string> OldValue { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("New string which user want to replace")]
        public DynamicProperty<string> NewValue { get; set; }


        [Category("Output"), RequiredArgument]
        [Description("Input string with replace new string")]
        public string OutputString { get; set; }


                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutputString = InputString.Value.Replace(OldValue, NewValue);
        }



        public override string AuditInfo()
        {
            return base.AuditInfo() + " InputString: " + this.InputString + " OldValue: " + this.OldValue + " NewValue: " + this.NewValue + " OutputString: " + this.OutputString;
        }
    }
}

