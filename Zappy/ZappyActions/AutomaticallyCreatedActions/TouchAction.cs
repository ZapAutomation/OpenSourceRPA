using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Zappy.Decode.Helper;
using Zappy.Decode.Helper.Enums;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Touch Action")]
    public class TouchAction : MouseAction
    {
        private string actionName;
        private GestureType gestureType;
        private bool isGestureComplete;
        private ManipulationType[] manipulations = new ManipulationType[0];

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            MouseAction mouseAction = (MouseAction)this;
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(mouseAction.TaskActivityIdentifier, WindowIdentifier);
            ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, mouseAction, uITaskControl);
            switch (this.ActionType)
            {
                case MouseActionType.Click:
                    Mouse.Click(uITaskControl, mouseAction.MouseButton, mouseAction.ModifierKeys, new Point(mouseAction.Location.X, mouseAction.Location.Y));
                    return;

                case MouseActionType.DoubleClick:
                    Mouse.DoubleClick(uITaskControl, mouseAction.MouseButton, mouseAction.ModifierKeys, new Point(mouseAction.Location.X, mouseAction.Location.Y));
                    return;

                case MouseActionType.WheelRotate:
                    Mouse.MoveScrollWheel(uITaskControl, mouseAction.WheelDirection, mouseAction.ModifierKeys);
                    return;

                case MouseActionType.Hover:
                    Mouse.Hover(uITaskControl, new Point(mouseAction.Location.X, mouseAction.Location.Y));
                    return;
            }
            object[] objArray2 = { this.ActionType.ToString() };
            CrapyLogger.log.ErrorFormat(CultureInfo.CurrentCulture, Resources.ActionNotSupported, objArray2);
            throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.ActionNotSupported, objArray2));
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override string ActionName
        {
            get
            {
                if (string.IsNullOrEmpty(actionName))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(IsGestureComplete);
                    builder.Append(' ');
                    builder.Append(GestureType);
                    foreach (ManipulationType type in manipulations)
                    {
                        builder.Append(' ');
                        builder.Append(type);
                    }
                    builder.Append(' ');
                    builder.Append(base.ActionName);
                    actionName = builder.ToString();
                }
                return actionName;
            }
            set
            {
                actionName = value;
            }
        }

        public GestureType GestureType
        {
            get =>
                gestureType;
            set
            {
                gestureType = value;
            }
        }

        public bool IsGestureComplete
        {
            get =>
                isGestureComplete;
            set
            {
                isGestureComplete = value;
            }
        }


        public ManipulationType[] Manipulations
        {
            get =>
                manipulations;
            set
            {
                manipulations = value;
            }
        }
    }
}