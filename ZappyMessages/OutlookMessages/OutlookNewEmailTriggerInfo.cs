using System;

namespace ZappyMessages.OutlookMessages
{
    public class OutlookNewEmailTriggerInfo
    {
        public Guid ParentTaskId { get; set; }
        public Guid TriggerId { get; set; }
        public string SaveDirPath { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string Body { get; set; }

        public bool RemoveTrigger { get; set; }
    }
}