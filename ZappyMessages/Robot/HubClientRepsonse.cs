﻿namespace ZappyMessages.Robot
{
    public class HubClientRepsonse
    {
        public string ClientRegistrationID { get; set; }
        public int RequestID { get; set; }

        public string RobotInstID { get; set; }

        public string Command { get; set; }

        public int RobotChannel { get; set; }

        public string ResponseData { get; set; }
    }
}