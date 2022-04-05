using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
                    [Description("Split Text")]
    public class SplitText : TemplateAction
    {
        public SplitText()
        {
            Separater = new string[] { Environment.NewLine };
            Index = 0;
        }

                        
                                [Category("Input")]
        [Description("Set the any text")]
        public DynamicProperty<string> Textvalue { get; set; }

                                [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Input")]
        [Description("set one or more String Separater eg: '.,;' if You Want To set comma delimeter '' ")]
        public string[] Separater { get; set; }

                                [Category("Input")]
        [Description("Enter index number whatever you want from the string ")]
        public DynamicProperty<int> Index { get; set; }

                                [Category("Output")]
        [Description("Gets substring from Text")]
        public string PropertyValue { get; set; }

        [Category("Output")]
        [Description("Gets substring from Text")]
        public string[] SplitStringArray { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            string text = Textvalue.ToString();
            string[] lines = text.Split(Separater, StringSplitOptions.None);
            PropertyValue = lines[Index];
            SplitStringArray = lines;
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " TextValue: " + this.Textvalue + " Separater: " + this.Separater + " Index: " + this.Index;
        }
    }
}
