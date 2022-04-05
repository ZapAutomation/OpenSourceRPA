using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ZappyMessages.Chrome
{
    public class BaseChromeAction
    {
        public DateTime Timestamp { get; set; }

        public int WindowID { get; set; }

        public int TabID { get; set; }

        public string CommandName { get; set; }

        public List<string> CommandTarget { get; set; }

        public string CommandValue { get; set; }

        public string ActionUrlTab { get; set; }

        [Category("Internals")] public Guid SelfGuid { get; set; }

        public string ActionWindowtitle { get; set; }

    }

}
