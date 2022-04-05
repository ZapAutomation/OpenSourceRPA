using System;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Aggregator
{
    internal interface IAggregatorFilter
    {
        event EventHandler<RecordedEventArgs> OnFilteredAction;

        event EventHandler<EventArgs> Started;

        void CompleteAggregation();
        void ReliableStop();
        void Start();
        void Stop();

        ZappyTaskActionStack FilteredActionList { get; set; }

        ZappyTaskActionStack RawActionList { get; set; }

        RecorderOptions RecorderOptions { get; set; }
    }
}

