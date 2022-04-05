using System;
using Crapy.ActionMap.Browser;
using Crapy.ActionMap.ScreenMaps;
using Crapy.ActionMap.ZappyTaskUtil;
using Crapy.Decode.Aggregator;
using Crapy.ExecuteTask.Execute;
using Crapy.ExecuteTask.Helpers;
using Crapy.ZappyTaskEditor;

namespace Crapy.ActionMap.TaskAction
{
    [Serializable]
    public class WebDialogAction : AggregatedAction
    {
        public WebDialogAction()
        {
            PromptText = string.Empty;
        }

        public override void Invoke(ZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker, ScreenIdentifier map)
        {
            //actionInvoker.Invoke(this, map);
            BrowserWindow uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, map) as BrowserWindow;
            if (uITaskControl == null)
            {
                throw new Exception();
            }
            uITaskControl.PerformDialogAction(this.ActionType, this.PromptText);
        }

        public BrowserDialogAction ActionType { get; set; }

        public string PromptText { get; set; }
    }
}

