using System.Collections.Generic;

namespace ZappyMessages.Robot
{
    public class ZappyRobotProfile
    {
        public string InstID { get; set; }
        public string GroupNames { get; set; }
        public string RobotName { get; set; }
        public string ReportingHubs { get; set; }
        //public string HostConfig { get; set; }
        public string NT_UserAccount { get; set; }
        public string UserId { get; set; }
        public int ProcessID { get; set; }
        public List<System.Uri> ListeningEndpoints { get; set; }

        public string RobotLocalIPAddress { get; set; }
        //public string ExtraInfo { get; set; }
    }
}