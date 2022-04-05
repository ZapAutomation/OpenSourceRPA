using System;

namespace ZappyMessages.Robot
{
    public class RobotTraceBucket
    {
        const int _TraceDepth = 7;

        public int RobotTraceUpdateChannel { get; set; }

        public int TraceCounter { get; set; }

        public Trace[] TraceData { get; set; }

        public RobotTraceBucket()
        {
            TraceData = new Trace[_TraceDepth + 1];
            for (int i = 0; i < TraceData.Length; i++)
                TraceData[i] = new Trace();
        }

        public Trace AppendTrace(int Channel, string Trace)
        {
            int _Index = TraceCounter++ & _TraceDepth;
            Trace _Trc = TraceData[_Index];
            _Trc.TimeStamp = DateTime.Now;
            _Trc.Channel = Channel;
            _Trc.TraceData = Trace;
            return _Trc;
        }
    }

}
