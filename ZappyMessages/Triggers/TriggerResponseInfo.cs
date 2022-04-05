namespace ZappyMessages.Triggers
{
    public class TriggerResponseInfo : TriggerMessageInfo
    {
        public TriggerResponseInfo() : base(null)
        { }

        public TriggerResponseInfo(TriggerMessageInfo info) : base(info)
        { }

        public string TriggerName { get; set; }

        public object TriggerAction { get; set; }
    }
}
