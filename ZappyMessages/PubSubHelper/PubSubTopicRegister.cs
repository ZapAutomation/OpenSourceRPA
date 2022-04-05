namespace ZappyMessages.PubSubHelper
{
    public static class PubSubTopicRegister
    {

        public const int ControlSignals = 0;
        public const int Notification = 1;
        public const int StartPlaybackFromFile = 2;
        public const int StartPlaybackFromActions = 3;
        public const int AutoRunRequest = 4;
        public const int ExecutionTaskQueueUpdate = 5;

        public const int ZappyPlaybackHelper2ChromeRequest = 6;
        public const int Chrome2ZappyPlaybackHelperResponse = 7;

        public const int Chrome2ZappyActionPlayback = 8;

        public const int TaskEditorFileOpenRequest = 9;
        public const int ZappyGlobalUpdate = 10;

        //NOTE: response chanel is always one number added to request channel for Excel
        public const int ZappyExcelRequest = 11;
        public const int ZappyExcelResponse = 12;

        public const int ZappyPlaybackHelper2ExcelRequest = 13;
        public const int Excel2ZappyPlaybackHelperResponse = 14;

        //response = +1 of request channel
        public const int ZappyOutlookRequest = 15;
        public const int ZappyOutlookResponse = 16;

        public const int ZappyPlaybackHelper2OutlookRequest = 17;
        public const int Outlook2ZappyPlaybackHelperResponse = 18;

        //for Java recording channel
        public const int JavaBridge2ZappyRequest = 19;

        public const int DebugTask = 20;
        public const int DebugNextStep = 21;

        public const int ActiveTriggersRequest = 22;
        public const int ActiveTriggersReponse = 23;


        public const int DebugStepProgress = 40;
        public const int AuditLogsChannel = 41;


        //TempforCom
        public const int UpdatedEnableComAppSettingsChannel = 42;

        public const int HubToZappyChannel = 51;

        public const int HubRemoteControlClientRegistrationChannel = 52;

        public const int HubRemoteControlClientRequestChannel = 53;
        public const int HubRemoteControlClientResponseChannel = 54;

        public const int ZappyPlayback2Zappy = 55;
        public const int Zappy2ZappyPlayback = 56;

        public const int Zappy2ZappyPlaybackRequest = 57;

        //262144
        public const int ZappyRobotTraceSeedChannel = (1 << 18);
        //2147483647
        public const int ZappyTraceChannel = int.MaxValue;

        //Hub and robot channels
        public const int HubToRobotCommandChannel = 1000;
        public const int RobotToHubProfileChannel = 1001;
        public const int RobotToHubStateChannel = 1002;

        public const int HubToRobotRequestChannel = 1003;
        public const int RobotToHubResponseChannel = 1004;

        //Not required for now
        //public const int RobotTraceLogChannel = 1005;
        //public const int RobotAuditLogChannel = 1006;
    }
}