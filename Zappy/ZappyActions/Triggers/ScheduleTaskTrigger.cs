using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;

namespace Zappy.ZappyActions.Triggers
{
                        
    [Description("Schedules Zappy Task")]
    public class ScheduleTaskTrigger : TimerTriggerHelper
    {
        public ScheduleTaskTrigger() : base()
        {
            TriggerFireTime = DateTime.Now.ToString("");
            RepeatDays = ZappyDayOfWeek.Weekdays;
        }
       
        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The days Of week this event will be fired.")]
        [Category("Optional")]
        public ZappyDayOfWeek RepeatDays { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(UITypeEditor))]
        [Description("Time to set the trigger")]
        [Category("Input")]
        public string TriggerFireTime { get; set; }     

        [Description("Time difference between the set time for fired the trigger and system current time in second")]
        [Category("Output")]
        [XmlIgnore]
        public override DynamicProperty<int> DueTimeInSeconds
        {
            get
            {
                DateTime _Now = DateTime.Now, _TriggerFireTime_Parsed;
                TriggerFireTime = TriggerFireTime.Trim();
                if (DateTime.TryParseExact(TriggerFireTime, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _TriggerFireTime_Parsed))
                {
                    DateTime _TriggerFireTime = _Now.Date.AddHours(_TriggerFireTime_Parsed.Hour).AddMinutes(_TriggerFireTime_Parsed.Minute).AddSeconds(_TriggerFireTime_Parsed.Second);

                    if (_Now >= _TriggerFireTime)
                        _TriggerFireTime = _TriggerFireTime.AddDays(1);

                    ZappyDayOfWeek _DayOfWeek = ZappyDayOfWeek.Sunday;
                    Enum.TryParse(_TriggerFireTime.DayOfWeek.ToString(), out _DayOfWeek);
                    while ((RepeatDays & _DayOfWeek) != _DayOfWeek)
                    {
                        _TriggerFireTime = _TriggerFireTime.AddDays(1);
                        Enum.TryParse(_TriggerFireTime.DayOfWeek.ToString(), out _DayOfWeek);
                    }

                    return (int)Math.Ceiling((_TriggerFireTime - _Now).TotalSeconds);
                }
                return -1;
            }
        }      
    }
}