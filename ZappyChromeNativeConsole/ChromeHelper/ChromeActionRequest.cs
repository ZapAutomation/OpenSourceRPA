using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using ZappyMessages.Chrome;

namespace ZappyChromeNativeConsole.ChromeHelper
{
    internal class ChromeActionRequest
    {
        public BaseChromeAction _BaseAction;
        public ChromeActionRequest(BaseChromeAction BaseAction)
        {
            _BaseAction = BaseAction;
        }
        public ChromeActionRequest(DateTime Now, int windowId, int tabId,
            string url,
            string command, List<string> targetModified, string commandvalue)
        {
            _BaseAction = new BaseChromeAction()
            {
                Timestamp = Now,
                WindowID = windowId,
                TabID = tabId,
                ActionUrlTab = url,
                CommandTarget = targetModified,
                CommandName = command,
                CommandValue = commandvalue,
                SelfGuid = Guid.NewGuid()
            };

        }

        [JsonIgnore]
        public DateTime TimeStamp { get { return _BaseAction.Timestamp; } }
        [JsonIgnore]
        public int WindowID { get { return _BaseAction.WindowID; } }
        [JsonIgnore]
        public int TabID { get { return _BaseAction.TabID; } }
        [JsonIgnore]
        public string ActionUrlTab { get { return _BaseAction.ActionUrlTab; } }
        public string CommandName { get { return _BaseAction.CommandName; } set { _BaseAction.CommandName = value; } }
        public List<string> CommandTarget { get { return _BaseAction.CommandTarget; } }
        public string CommandValue { get { return _BaseAction.CommandValue; } set { _BaseAction.CommandValue = value; } }

        public string CommandResult { get; set; }

        [JsonIgnore]
        public AutoResetEvent TimeOutEvent { get; set; }
        public bool IsOpenURLCommand()
        {
            return !string.IsNullOrEmpty(CommandName) && CommandName.StartsWith("open");
        }

        public string DisplayString()
        {
            return _BaseAction.SelfGuid + "^^" + CommandName + "^^" + CommandTarget + "^^" + CommandValue;
        }
    }
}