using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                [Description("Gets StringArray From String")]
    public class StringToStringArray : TemplateAction
    {
        public StringToStringArray()
        {
            Seperator = new[] { Environment.NewLine, "\n" };
        }

                                [Category("Input")]
        [Description("String Text")]
        public DynamicProperty<string> Text { get; set; }

                                [Category("Input"), DefaultValue("\r\n")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("String Seperator")]
        public string[] Seperator { get; set; }

                                [Category("Output")]
        [Description("Gets StringArray from String")]
        public string[] StringArray { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            StringArray = Text.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input String:" + this.Text + " Output String Array:" + this.StringArray;
        }
    }
}