using ZappyMessages.RecordPlayback;

namespace ZappyMessages.Robot
{
    public struct ZappyRobotTaskInfo
    {
        public string Guid { get; set; }

        public string Description { get; set; }

        public string RobotGuid { get; set; }

        public string RobotName { get; set; }

        public string FileGuid { get; set; }

        public string FileName { get; set; }

        public ZappyTaskExecutionState State { get; set; }
    }
}