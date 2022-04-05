using System;
using System.Globalization;
using System.Xml.Serialization;
using Crapy.ActionMap.ScreenMaps;
using Crapy.ActionMap.TaskTechnology;
using Crapy.ActionMap.ZappyTaskUtil;
using Crapy.Decode.Aggregator;
using Crapy.ExecuteTask.Execute;
using Crapy.ExecuteTask.Helpers;
using Crapy.ExecuteTask.TaskExecutor;
using Crapy.Properties;
using Crapy.ZappyTaskEditor;

namespace Crapy.ActionMap.TaskAction
{
    [Serializable]
    public class BrowserAction : AggregatedAction
    {
        private BrowserActionType actionType;

        public BrowserAction()
        {
        }

        public BrowserAction(TaskActivityElement uiElement, BrowserActionType actionType) : base(uiElement)
        {
            ActionType = actionType;
        }

        public override void Invoke(ZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker, ScreenIdentifier map)
        {
            //CrapyLogger.log.InfoFormat("UIActionInterpreter.Invoke(): {0}", args);

            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, map);
            BrowserWindow uiControl = null;
            if (uITaskControl != null)
            {
                if (uITaskControl is BrowserWindow)
                {
                    uiControl = uITaskControl as BrowserWindow;
                }
                else
                {
                    uiControl = new BrowserWindow(uITaskControl);
                }
            }
            Execute.PlaybackContext = new InterpreterPlaybackContext(map, this, uiControl);
            switch (this.ActionType)
            {
                case BrowserActionType.Back:
                    uiControl.Back();
                    return;

                case BrowserActionType.Forward:
                    uiControl.Forward();
                    return;

                case BrowserActionType.Refresh:
                    uiControl.Refresh();
                    return;

                case BrowserActionType.Stop:
                    uiControl.StopPageLoad();
                    return;

                case BrowserActionType.Close:
                    uiControl.Close();
                    return;
            }
            object[] objArray2 = { this.ActionType.ToString() };
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.ActionNotSupported, objArray2));
        }

        [XmlIgnore]
        public override string ActionName =>
            actionType.ToString();

        public BrowserActionType ActionType
        {
            get => 
                actionType;
            set
            {
                actionType = value;
                NotifyPropertyChanged("ActionType");
            }
        }
    }
}

