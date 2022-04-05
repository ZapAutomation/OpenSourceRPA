namespace ZappyMessages.Triggers
{
    public class TriggerRequestInfo : TriggerMessageInfo
    {
        public TriggerRequestInfo() : base(null)
        { }

        public TriggerRequestInfo(TriggerMessageInfo info) : base(info)
        { }

        public TriggerRequest RequestType { get; set; }

        public string RequestBody { get; set; }
    }
}
