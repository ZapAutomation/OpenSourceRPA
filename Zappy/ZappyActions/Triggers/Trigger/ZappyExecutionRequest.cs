using System;
using System.Collections.Generic;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Triggers.Trigger
{
    public class ZappyExecutionRequest
    {
        public Guid RequestID { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime ChangeTime { get; set; }

        public bool IsActive { get; set; }

        public ZappyTask Task { get; set; }

        public string FilePath { get; set; }

        public Dictionary<string, object> ContextData { get; set; }

        public string Status { get; set; }

        public ZappyExecutionRequest()
        {
            RequestID = Guid.NewGuid();
            ChangeTime = CreationTime = WallClock.Now;
            IsActive = true;
            Status = "Request Created";
        }

    }


}
