using System;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Triggers.Helpers;
using Timer = System.Threading.Timer;

namespace Zappy.ZappyActions.Triggers.Trigger
{
    public class TimerTriggerManager
    {
        internal IDisposable ConfigureTimerTrigger(IZappyTimerTrigger _Trigger)
        {
                        if (_Trigger != null)
            {
                if (_Trigger.DueTimeInSeconds <= 0)
                {
                    throw new Exception("Due Time in the past or equal to 0");
                                    }
                Timer _tmr = new System.Threading.Timer(OnTimerTrigger, _Trigger, _Trigger.DueTimeInSeconds * 1000, Timeout.Infinite);
                return _tmr;
            }
            return null;
        }

        internal void OnTimerTrigger(object context)
        {
            if (context != null && context is IZappyTimerTrigger)
            {
                IZappyTimerTrigger _Trigger = context as IZappyTimerTrigger;
                IZappyAction _ZappyTrigger = _Trigger as IZappyAction;
                ZappyTask _Task = null;
                bool ExecuteTrigger = true;
                if (_Trigger.AskBeforeExecuting)
                {
                    DialogResult result = MessageBox.Show("Excetue Timer Trigger Task: " + _ZappyTrigger.DisplayName,
                        "Execute Trigger", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        ExecuteTrigger = false;
                }

                if (ExecuteTrigger)
                    ZappyTriggerManager.ValidateAndRaiseTrigger(_Trigger as IZappyAction, null);

                Timer _tmr = null;

                IDisposable _TimerObject;

                if (ZappyTriggerManager._Triggers.TryGetValue(context as IZappyAction, out _TimerObject))
                    _tmr = _TimerObject as Timer;

                if (_tmr != null)
                {
                    if(_Trigger is FireOnceTimerTrigger)
                    {
                                                ZappyTriggerManager.UnregisterTriggerHelper(_Trigger as IZappyAction, _Task);
                    }
                    else                     {
                        _tmr.Change(_Trigger.DueTimeInSeconds * 1000, Timeout.Infinite);
                                            }
                }
            }
        }
    }
}
