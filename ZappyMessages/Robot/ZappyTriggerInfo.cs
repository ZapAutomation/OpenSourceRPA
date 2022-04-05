using ZappyMessages.RecordPlayback;

namespace ZappyMessages.Robot
{
    public struct ZappyTriggerInfo
    {
        public string Guid { get; set; }

        public string Description { get; set; }

        public ZappyTaskExecutionState State { get; set; }
    }
}