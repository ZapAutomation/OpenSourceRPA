using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Description("Invoke")]
    public class InvokeAction : ZappyTaskAction
    {
        public InvokeAction()
        {
        }

        public InvokeAction(TaskActivityElement uiElement, object value) : base(uiElement)
        {
            ZappyTaskUtilities.CheckForNull(uiElement, "uiElement");
            if (value is ZappyTaskActionLogEntry)
            {
                            }
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                    }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public TaskActivityElement SourceElement { get; set; }
    }
}