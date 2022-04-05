using System;
using System.ComponentModel;
using Zappy.ExecuteTask;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using ZappyMessages.PubSubHelper;

namespace Zappy.ZappyActions.Triggers
{
    public class WindowLaunchTrigger : TemplateAction, IZappyTrigger
    {
        [Category("Optional")]
        public DynamicProperty<string> WindowTitle { get; set; }

        [Category("Input")]
        public DynamicProperty<string> ApplicationExeName { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            
        }
        WindowLaunchTriggerHelper windowLaunchTriggerHelper;
        public IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            windowLaunchTriggerHelper = new WindowLaunchTriggerHelper();
            windowLaunchTriggerHelper.ApplicationExeName = ApplicationExeName;
            windowLaunchTriggerHelper.WindowTitle = WindowTitle;
            windowLaunchTriggerHelper.SelfGuid = SelfGuid;

                        ZappyPlaybackCommunicator.Instance.RegisterWindowLaunchTrigger
                (PubSubTopicRegister.ZappyPlayback2Zappy, PubSubTopicRegister.Zappy2ZappyPlayback, windowLaunchTriggerHelper);
                        return null;
        }

        public void UnRegisterTrigger()
        {
            ZappyPlaybackCommunicator.Instance.UnRegisterWindowLaunchTrigger
                        (PubSubTopicRegister.ZappyPlayback2Zappy, PubSubTopicRegister.Zappy2ZappyPlayback, windowLaunchTriggerHelper);
        }
    }
}
