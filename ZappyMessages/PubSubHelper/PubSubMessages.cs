namespace ZappyMessages.PubSubHelper
{
    public static class PubSubMessages
    {
        public static readonly string CancelZappyExecutionMessage = "AbandonZappyExecution45345345634";
        public static readonly string StartZappyExecutionMessage = "StartZappyRecording";
        //public static readonly string StartChromeRecordingMessage = "StartRecordingChromeForZappy";
        public static readonly string StopZappyExecutionMessage = "StopZappyRecording";
        //public static readonly string SendXPathTargetsTrue = "SendXPathTargetsTrue";
        //public static readonly string SendXPathTargetsFalse = "SendXPathTargetsFalse";
        public static readonly string TriggerRequestMessage = "GetActiveTriggers";
        public static readonly string TriggerAddMessage = "AddOrUpdateTrigger";
        public static readonly string TriggerDeleteMessage = "DeleteTrigger";

        public static readonly string ReloadAssembliesFromDllXMLMessage = "ReloadAssembliesFromDllXMLMessage";

        public static readonly string RequestStateFromZappy = "RequestState";

        public static readonly string ExitZappyMessage = "ExitZappyMessageFromRobot";
        
        //public static readonly string GetLastActivityTime = "GetLastActivitiesTimeZappy";

    }
}
