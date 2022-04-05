using System.ComponentModel;
using Zappy.SharedInterface.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    [Description("Gets HTML content of active tab in chrome - overrides command target to //*")]
    public class ChromeGetHTMLContent : ChromeActionKeyboard
    {
        public ChromeGetHTMLContent() : base("sendKeys")
        {
            //this.CommandTarget.Contains("//*");
            this.CommandValue = "${Copy}";
            this.ActionUrlTab = "";
        }

        [Category("Output")]
        public string HTMLContent { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            this.CommandTarget = new System.Collections.Generic.List<string>(){
                "//*"
            };
            base.Invoke(context, actionInvoker);

            HTMLContent = this.PlaybackResponseObject.ActionUrlTab;
        }
    }
}