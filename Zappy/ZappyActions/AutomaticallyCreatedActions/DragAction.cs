using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Identify Drag By mouseButton Or PointLocation")]
    public class DragAction : InputAction
    {
        private MouseButtons mouseButton;
        private Point moveBy;
        private Point startLocation;

        public DragAction()
        {
        }

        public DragAction(TaskActivityElement uiElement, MouseButtons mouseButton) : base(uiElement)
        {
            MouseButton = mouseButton;
        }

        internal override string GetParameterString()
        {
            StringBuilder builder = new StringBuilder();
            if (ModifierKeys != ModifierKeys.None)
            {
                builder.Append(ModifierKeys);
                builder.Append(" + ");
            }
            builder.Append(StartLocation);
            builder.Append(" ");
            builder.Append(moveBy);
            return builder.ToString();
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                    
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, this, uITaskControl);
            if (this.MouseButton == MouseButtons.None)
            {
                object[] objArray2 = { this.MouseButton };
                
                this.MouseButton = MouseButtons.Left;
            }
            Mouse.StartDragging(uITaskControl, new Point(this.StartLocation.X, this.StartLocation.Y), this.MouseButton, this.ModifierKeys);
            Mouse.StopDragging(uITaskControl, this.MoveBy.X, this.MoveBy.Y);

        }

        internal override void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            base.ShallowCopy(source, isSeparateAction);
            if (isSeparateAction)
            {
                DragAction action = source as DragAction;
                if (action != null)
                {
                    StartLocation = action.StartLocation;
                    MoveBy = action.MoveBy;
                    MouseButton = action.MouseButton;
                }
            }
        }

        public MouseButtons MouseButton
        {
            get =>
                mouseButton;
            set
            {
                mouseButton = value;
                NotifyPropertyChanged("MouseButton");
            }
        }

        public Point MoveBy
        {
            get =>
                moveBy;
            set
            {
                moveBy = value;
                NotifyPropertyChanged("MoveBy");
            }
        }

        public Point StartLocation
        {
            get =>
                startLocation;
            set
            {
                startLocation = value;
                NotifyPropertyChanged("StartLocation");
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " StartLocation:" + this.StartLocation + " MoveBy:" + this.MoveBy;
        }
    }
}