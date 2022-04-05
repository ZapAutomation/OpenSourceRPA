using System;
using System.ComponentModel;
using Zappy.Plugins.ChromeBrowser.Chrome.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    public class ChromeActionMiscellaneous : ChromeAction
    {
                [Category("Input")]
        public ChromeActionMiscellaneousOptions ChromeActionOption
        {
            get
            {
                Enum.TryParse(base.CommandName, out ChromeActionMiscellaneousOptions myStatus);
                return myStatus;
            }
            set
            {
                base.CommandName = value.ToString();
            }
        }
    }
}