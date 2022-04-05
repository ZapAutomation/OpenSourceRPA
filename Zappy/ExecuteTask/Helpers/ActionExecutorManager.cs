using System.Collections.Generic;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal class ActionExecutorManager
    {
        private static IList<ZappyTaskActionExecutor> s_actionExecutorList;
        private static readonly ActionExecutorManager s_instance = new ActionExecutorManager();
        private readonly object syncLock = new object();

        private ActionExecutorManager()
        {
        }

        public ZappyTaskActionExecutor GetActionExecutor(ZappyTaskControl uiControl)
        {
            ZappyTaskActionExecutor executor3;
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                                {
                    ZappyTaskActionExecutor executor = null;
                    int num = 0;
                    TaskActivityElement techElementFromZappyTaskControl = ALUtility.GetTechElementFromZappyTaskControl(uiControl);
                    foreach (ZappyTaskActionExecutor executor2 in s_actionExecutorList)
                    {
                        int controlSupportLevel = executor2.GetControlSupportLevel(techElementFromZappyTaskControl);
                        if (controlSupportLevel > num)
                        {
                            executor = executor2;
                            num = controlSupportLevel;
                        }
                    }
                    if (executor == null && uiControl != null)
                    {
                        object[] args = { uiControl.TechnologyName };
                        
                    }
                    executor3 = executor;
                }
            }
            return executor3;
        }

        public ZappyTaskActionExecutorCore GetDefaultExecutor() =>
            ZappyTaskActionExecutorCore.Instance;

        public IList<ZappyTaskActionExecutor> ActionExecutors =>
            s_actionExecutorList;

        public static ActionExecutorManager Instance
        {
            get
            {
                if (s_actionExecutorList == null)
                {
                    bool flag = false;
                    if (!Execute.ExecutionHandler.IsInitialized)
                    {
                        ZappyTaskService.Instance.Initialize();
                        flag = true;
                    }
                    s_actionExecutorList = ZappyTaskService.Instance.GetExtensions<ZappyTaskActionExecutor>();
                    if (flag)
                    {
                        ZappyTaskService.Instance.Cleanup();
                    }
                }
                return s_instance;
            }
        }
    }
}