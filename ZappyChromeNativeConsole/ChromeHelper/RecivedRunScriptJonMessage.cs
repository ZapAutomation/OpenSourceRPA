using System.Collections.Generic;

namespace ZappyChromeNativeConsole.ChromeHelper
{
    public class RecivedRunScriptJonMessage : ReceivedJsonMessage
    {
        public new List<List<List<string>>> target { get; set; }
    }
}