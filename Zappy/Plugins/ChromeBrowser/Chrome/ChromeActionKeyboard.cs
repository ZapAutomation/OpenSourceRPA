using System;
using System.ComponentModel;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    [Serializable]
    public class ChromeActionKeyboard : ChromeAction, IUserTextAction
    {
        [Category("Output")]
        public string ClipboardData { get; set; }

        public ChromeActionKeyboard(string ParamCommandName) : base(ParamCommandName)
        {

        }

        public ChromeActionKeyboard() : base("sendKeys")
        {

        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            base.Invoke(context, actionInvoker);
            ClipboardData = this.PlaybackResponseObject.ActionUrlTab;
        }

                        

                                        
                                                                        
                                                
                                                                                                
        public void Cleanup()
        {
        }
    }
}
