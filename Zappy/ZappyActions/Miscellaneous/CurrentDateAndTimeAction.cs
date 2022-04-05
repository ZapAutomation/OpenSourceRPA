using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class CurrentDateAndTimeAction : TemplateAction
    {
        public CurrentDateAndTimeAction()
        {
            DateFormatted = "yyyyddM H:mm:ss";
        }

        [Category("Input")]
        [Description("Set the date format like dddd, dd MMMM yyyy  ")]
        public DynamicProperty<string> DateFormatted { get; set; }

        [Category("Optional")]
        [Description("Get the UTC date time instead of local time")]
        public DynamicProperty<bool> GetUtcDateTime { get; set; }

                        
        [Category("Output")]
        [Description("Current date of system")]
        public string Date { get; set; }

        [Category("Output")]
        [Description("Current time of system")]
        public string Time { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        if (GetUtcDateTime)
            {
                Date = DateTime.UtcNow.ToString(DateFormatted);
                Time = string.Format("{0:HH:mm:ss tt}", DateTime.UtcNow);
            }
            else
            {
                Date = DateTime.Now.ToString(DateFormatted);
                Time = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            }
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " Date format: " + this.DateFormatted + " Date: " + this.Date + " Time: " + this.Time;
        }
    }
}
