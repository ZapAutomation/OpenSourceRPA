using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class CombineStrings : TemplateAction
    {
        [Category("Input")]
        [Description("Set First String which you want combine with second string")]
        public DynamicProperty<string> FirstString { get; set; }

        [Category("Input")]
        [Description("Set the any text")]
        public DynamicProperty<string> SecondString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> ThirdString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> FourthString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> FifthString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> SixthString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> SeventhString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> EightString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> NinethString { get; set; }

        [Category("Optional")]
        [Description("Set the any text")]
        public DynamicProperty<string> TenthString { get; set; }

        [Category("Output")]
        [Description("Return Combine multiple string in one string line")]
        public string CombineString { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            CombineString = string.Concat(FirstString, SecondString, ThirdString, FourthString, FifthString, SixthString, SeventhString, EightString, NinethString, TenthString);
        }
        public override string AuditInfo()
        {
            return " First String"+this.FirstString+"Second String:"+this.SecondString+"Third String:"+this.ThirdString+ "Fourth String:" + this.FourthString+"Fifth String:"+this.FifthString+ "SixthString:"+this.SixthString+ "SeventhString:"+this.SeventhString+ "EightString:" + this.EightString+ "NinethString:" + this.NinethString+ "TenthString:" + this.TenthString +"Combine String"+this.CombineString;
        }
    }
}
