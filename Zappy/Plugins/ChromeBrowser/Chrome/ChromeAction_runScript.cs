using Zappy.SharedInterface.Helper;
using System.ComponentModel;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    public class ChromeAction_runScript : ChromeAction
    {
                        
        public ChromeAction_runScript() : base("runScript")
        {
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            base.Invoke(context, actionInvoker);

                                }
    }
}