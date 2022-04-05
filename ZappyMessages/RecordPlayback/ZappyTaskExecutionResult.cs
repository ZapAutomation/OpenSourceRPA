using System;

namespace ZappyMessages.RecordPlayback
{
    public class ZappyTaskExecutionResult
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string StatusMessage { get; set; }

        public ZappyTaskExecutionState State { get; set; }

        public ZappyTaskExecutionResult()
        {

        }

        public ZappyTaskExecutionResult(Guid id, string fileName, ZappyTaskExecutionState state, string statusMessage)
        {
            Id = id;
            FileName = fileName;
            State = state;
            StatusMessage = statusMessage;
        }
    }
}
