using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class DateFormat : TemplateAction
    {
        [Category("Input")]
        [Description("Set the date you want to change the format")]
        public DynamicProperty<string> Date { get; set; }

        [Category("Input")]
        [Description("Enter the valid date format you want")]
        public DynamicProperty<string> ChangeDateFormat { get; set; }

        [Category("Output")]
        [Description("return the new formatted date")]
        public string NewFormatDate { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Date = Regex.Replace(Date, "[A-Za-z ]", "");
                                    DateTime date = DateTime.Parse(Date);
            NewFormatDate = date.ToString(ChangeDateFormat, CultureInfo.InvariantCulture);
        }
        public override string AuditInfo()
        {
            return "Date:" + this.Date + " Date Format" + this.ChangeDateFormat + "New Formatted:" + this.NewFormatDate;
        }
    }
}
