using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Drag And Drop From StartLocation To EndLocation")]
    public class DragDropAction : InputAction
    {
        private MouseButtons mouseButton;
        private Point startLocation;
        private Point stopLocation;
        [NonSerialized]
        private TaskActivityElement targetElement;
        private string targetElementName;

        public DragDropAction()
        {
        }

        public DragDropAction(TaskActivityElement sourceUIElement, TaskActivityElement targetUIElement, MouseButtons mouseButton) : base(sourceUIElement)
        {
            ZappyTaskUtilities.CheckForNull(sourceUIElement, "sourceUIElement");
            ZappyTaskUtilities.CheckForNull(targetUIElement, "targetUIElement");
            if (mouseButton != MouseButtons.Left && mouseButton != MouseButtons.Right && mouseButton != MouseButtons.Middle)
            {
                throw new ArgumentOutOfRangeException("mouseButton");
            }
            TargetElement = targetUIElement;
            MouseButton = mouseButton;
        }

                                                        
                
        internal override string GetParameterString()
        {
            object[] args = { TargetElementName, StartLocation, StopLocation };
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}-{2}", args);
        }

        internal override ICollection<TaskActivityElement> GetUIElementCollection()
        {
            List<TaskActivityElement> list = new List<TaskActivityElement>(base.GetUIElementCollection());
            if (TargetElement != null)
            {
                list.Add(TargetElement);
            }
            return list.ToArray();
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            ZappyTaskControl control = UIActionInterpreter.GetZappyTaskControl(this.TargetElementName, WindowIdentifier);
            ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, this, uITaskControl);
            control.EnsureClickable(new Point(this.StopLocation.X, this.StopLocation.Y));
            if (this.MouseButton == MouseButtons.None)
            {
                object[] objArray2 = { this.MouseButton };

                this.MouseButton = MouseButtons.Left;
            }
            Mouse.StartDragging(uITaskControl, new Point(this.StartLocation.X, this.StartLocation.Y), this.MouseButton, this.ModifierKeys);
            Mouse.StopDragging(control, new Point(this.StopLocation.X, this.StopLocation.Y));

        }

        internal override void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            base.ShallowCopy(source, isSeparateAction);
            if (isSeparateAction)
            {
                DragDropAction action = source as DragDropAction;
                if (action != null)
                {
                    StartLocation = action.StartLocation;
                    StopLocation = action.StopLocation;
                    SourceElement = action.SourceElement;
                    TargetElement = action.TargetElement;
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

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public TaskActivityElement SourceElement
        {
            get =>
                ActivityElement;
            set
            {
                ActivityElement = value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public string SourceElementName
        {
            get =>
                TaskActivityIdentifier;
            set
            {
                TaskActivityIdentifier = value;
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

        public Point StopLocation
        {
            get =>
                stopLocation;
            set
            {
                stopLocation = value;
                NotifyPropertyChanged("StopLocation");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public TaskActivityElement TargetElement
        {
            get =>
                targetElement;
            set
            {
                targetElement = value;
                NotifyPropertyChanged("TargetElement");
            }
        }

        public string TargetElementName
        {
            get =>
                targetElementName;
            set
            {
                targetElementName = value;
                NotifyPropertyChanged("TargetElementName");
            }
        }
    }
}

