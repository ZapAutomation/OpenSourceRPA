using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Plugins.ChromeBrowser.Chrome.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    public class ChromeMouseAction : ChromeAction
    {
                public ChromeMouseAction()
        {
            base.CommandName = ChromeMouseOptions.clickAt.ToString();
        }

        public ChromeMouseAction(ChromeMouseOptions chromeActionOption)
        {
            this.ChromeActionOption = chromeActionOption;
        }
        [Category("Input")]
        [XmlIgnore, JsonIgnore]
        public ChromeMouseOptions ChromeActionOption
        {
            get
            {
                Enum.TryParse(base.CommandName, out ChromeMouseOptions myStatus);
                return myStatus;
            }
            set
            {
                base.CommandName = value.ToString();
            }
        }
    }
}