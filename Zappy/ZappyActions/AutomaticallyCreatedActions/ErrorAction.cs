using System.ComponentModel;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Description("Display Error With It's Id And Message")]
    public class ErrorAction : ZappyTaskAction
    {
        private string idTag;
        private string message;

        public ErrorAction()
        {
        }

        public ErrorAction(string message)
        {
            Message = message;
        }

        public ErrorAction(string message, TaskActivityElement uiElement)
        {
            Message = message;
            ActivityElement = uiElement;
        }

        public ErrorAction(string message, string idTag)
        {
            this.message = message;
            this.idTag = idTag;
        }

        internal override string GetParameterString() =>
            message;

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                    }

        public string IdTag
        {
            get =>
                idTag;
            set
            {
                idTag = value;
                NotifyPropertyChanged("IdTag");
            }
        }

        public string Message
        {
            get =>
                message;
            set
            {
                message = value;
                NotifyPropertyChanged("Message");
            }
        }
    }



}
