using System;
using System.Collections.Generic;
using System.IO;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Triggers.Helpers;
using ZappyMessages;
using ZappyMessages.Helpers;

namespace Zappy.ZappyActions.Triggers.Trigger
{

    public static class ZappyTriggerManager
    {

        public static event Action<IZappyAction, ZappyTask, object> TriggerFired;

        public static Dictionary<IZappyAction, ZappyTask> _TriggerRegisteredTasks = new Dictionary<IZappyAction, ZappyTask>();

                public static Dictionary<Guid, GlobalUpdateTrigger> _globalUpdateTriggers = new Dictionary<Guid, GlobalUpdateTrigger>();
        public static Dictionary<IZappyAction, IDisposable> _Triggers = new Dictionary<IZappyAction, IDisposable>();
        public static List<IZappyAction> _RegisteredTriggers;




        public static void CheckAndFireTrigger(IZappyTrigger _Trigger, object Data)
        {
            ZappyTask _Task = null;
            if (!_Trigger.IsDisabled && _TriggerRegisteredTasks.TryGetValue(_Trigger as IZappyAction, out _Task))
                TriggerFired?.Invoke(_Trigger as IZappyAction, _Task, Data);
        }



                                
        
                        
                                
        

        public static void ValidateAndRaiseTrigger(IZappyAction action, object msgInfo)
        {
            CheckAndFireTrigger((IZappyTrigger)action, msgInfo);
        }

        public static void UnregisterTriggerHelper(IZappyAction TriggerAction, ZappyTask Task)
        {
                        string triggerTaskPath = Path.Combine(ZappyMessagingConstants.TriggerFolder, Task.Id.ToString() + ".zappy");
            if (File.Exists(triggerTaskPath))
                File.Delete(triggerTaskPath);

            lock (ZappyTriggerManager._TriggerRegisteredTasks)
            {
                IZappyAction removal = null;
                ZappyTask removalTask = null;

                foreach (KeyValuePair<IZappyAction, ZappyTask> item in ZappyTriggerManager._TriggerRegisteredTasks)
                {
                    if (item.Key.SelfGuid == TriggerAction.SelfGuid)
                    {
                        removal = item.Key;
                        removalTask = item.Value;
                        break;
                    }
                }

                if (removal != null)
                {
                    (removal as IZappyTrigger).IsDisabled = true;

                    
                                                                                                    (removal as IZappyTrigger).UnRegisterTrigger();
                    _Triggers.Remove(removal);
                    ZappyTriggerManager._TriggerRegisteredTasks.Remove(removal);

                    
                                                                                                                        
                                    }
            }

                                                            
        }

        

    }
}
