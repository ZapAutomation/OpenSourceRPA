using System;

namespace ZappyMessages.Triggers
{
    public abstract class TriggerMessageInfo
    {
        public TriggerMessageInfo(TriggerMessageInfo info)
        {
            if (info != null)
            {
                TriggerKind = info.TriggerKind;
                ParentTaskId = info.ParentTaskId;
                TriggerId = info.TriggerId;
                Data = info.Data;
            }
        }

        public ZappyTriggerKind TriggerKind { get; set; }

        public Guid ParentTaskId { get; set; }

        public Guid TriggerId { get; set; }

        public Type DataType { get; set; }

        public object Data { get; set; }
    }
}
