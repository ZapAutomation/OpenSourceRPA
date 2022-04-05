using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class ExtractNumber : TemplateAction
    {
        [Category("Input")]
        [Description("Set the string from which user want an extract numbers")]
        public DynamicProperty<string> StringValue { get; set; }

        [Category("Optional")]
        [Description("set index number which user want to get number serially")]
        public DynamicProperty<int> IndexNumber{ get; set; }

        [Category("Output")]
        [Description("Return particular number from given string")]
        public string IntergerValue { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            int i = 0;
            var doubleArray = Regex.Split(StringValue, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "");
            foreach (string s in doubleArray)
            {
                if (i == IndexNumber - 1)
                {
                    IntergerValue = s;
                    break;
                }
                i++;
            }
        }
        public override string AuditInfo()
        {
            return "String:"+this.StringValue+"Index Number:"+this.IndexNumber+"Numeric Value:"+this.IntergerValue;
        }
    }
}
