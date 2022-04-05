using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappyMessages.Robot
{
    public class RobotToHubLogMessage
    {
        public RobotInfo robotInfo { get; set; }
        public string message { get; set; }
    }
}
