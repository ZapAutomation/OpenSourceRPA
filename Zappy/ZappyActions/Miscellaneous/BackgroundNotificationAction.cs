using System;
using Zappy.Graph;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class BackgroundNotificationAction : TemplateAction
    {
        public DynamicProperty<string> NotificationText { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            PubSubClient clnt = context.ContextData[CrapyConstants.ExternalActionPublisher] as PubSubClient;

            clnt.Publish(PubSubTopicRegister.Notification, NotificationText);
        }
    }

}
