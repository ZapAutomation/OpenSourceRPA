using ZappyMessages.RecordPlayback;

namespace ZappyMessages.Robot
{
    public class ZappyTaskInfo
    {
        public string Guid { get; set; }

        public string FileName { get; set; }

        public byte[] FileData { get; set; }

        public ZappyTaskExecutionState State { get; set; }

        public string StatusMessage { get; set; }

        public ZappyTaskInfo()
        {

        }

        public ZappyTaskInfo(string fname, byte[] data)
        {
            FileName = fname;
            FileData = data;
            State = ZappyTaskExecutionState.Idle;
        }
    }
}
