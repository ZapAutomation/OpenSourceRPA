using System.Collections.Generic;

namespace ZappyChromeNativeConsole.ChromeHelper
{
    public class ReceivedJsonMessage
    {
        public string command { get; set; }
        public List<List<string>> target { get; set; }
        public string value { get; set; }
        public string frameLocation { get; set; }
        public int windowId { get; set; }
        public int tabId { get; set; }
        public string url { get; set; }

        public List<string> targetModified { get; set; }
        public string title { get; set; }

    }
}