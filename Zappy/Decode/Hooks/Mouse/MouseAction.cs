
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Invoker;
using Zappy.Properties;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.Mouse
{
    public class MouseAction : InputAction
    {
        private Point absLocation;
        private Point absoluteMouseDownLocation;
        private MouseActionType actionType;
        private bool implicitHover;
        private Point location;
        private MouseButtons mouseButton;
        private int mouseDownTimeStamp;
        private int wheelDirection;

        public MouseAction()
        {
            absoluteMouseDownLocation = new Point(-100, -100);
        }

        public MouseAction(MouseButtons button, MouseActionType actionType)
        {
            absoluteMouseDownLocation = new Point(-100, -100);
            ActionType = actionType;
            MouseButton = button;
            Location = Point.Empty;
        }


        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public Point AbsoluteLocation
        {
            get =>
                absLocation;
            set
            {
                absLocation = value;
                NotifyPropertyChanged("AbsoluteLocation");
            }
        }
        [Category("Input")]

        internal Point AbsoluteMouseDownLocation
        {
            get =>
                absoluteMouseDownLocation;
            set
            {
                absoluteMouseDownLocation = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public override string ActionName
        {
            get
            {
                MouseActionType wheelRotate;
                if (ActionType == MouseActionType.WheelRotate)
                {
                    wheelRotate = MouseActionType.WheelRotate;
                    return wheelRotate.ToString();
                }
                if (ActionType == MouseActionType.Hover)
                {
                    wheelRotate = MouseActionType.Hover;
                    return wheelRotate.ToString();
                }
                StringBuilder builder = new StringBuilder();
                builder.Append(this.MouseButton);
                builder.Append(' ');
                builder.Append(this.ActionType);
                return builder.ToString();
            }
            set
            {
                int num;
                ZappyTaskUtilities.CheckForNull(value, "value");
                if (int.TryParse(value, out num))
                {
                    ActionType = MouseActionType.WheelRotate;
                    WheelDirection = num;
                }
                else
                {
                    char[] separator = { ' ' };
                    string[] strArray = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length == 1)
                    {
                        ActionType = (MouseActionType)Enum.Parse(typeof(MouseActionType), strArray[0]);
                    }
                    else
                    {
                        if (strArray.Length != 2)
                        {
                            throw new ArgumentOutOfRangeException("value");
                        }
                        MouseButton = (MouseButtons)Enum.Parse(typeof(MouseButtons), strArray[0]);
                        ActionType = (MouseActionType)Enum.Parse(typeof(MouseActionType), strArray[1]);
                    }
                }
                NotifyPropertyChanged("ActionName");
            }
        }
        [Category("Input")]

        public MouseActionType ActionType
        {
            get =>
                actionType;
            set
            {
                actionType = value;
                NotifyPropertyChanged("ActionType");
            }
        }
        [Category("Input")]

        internal bool ImplicitHover
        {
            get =>
                implicitHover;
            set
            {
                implicitHover = value;
            }
        }
        [Category("Input")]

        public Point Location
        {
            get =>
                location;
            set
            {
                location = value;
                NotifyPropertyChanged("Location");
            }
        }
        [Category("Input")]

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
        [Browsable(false)]
        internal int MouseDownTimeStamp
        {
            get =>
                mouseDownTimeStamp;
            set
            {
                mouseDownTimeStamp = value;
            }
        }
        [Category("Input")]

        public int WheelDirection
        {
            get =>
                wheelDirection;
            set
            {
                wheelDirection = value;
                NotifyPropertyChanged("WheelDirection");
            }
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            updateWindowAndTaskActivityIdentifier();
            ZappyTaskControl uITaskControl =
                UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
                                                            
            List<string> windowTitles = new List<string>();
            int size = 0;
            if (!ApplicationSettingProperties.Instance.EnableComPlayback)
            {
                if (WindowIdentifier.TopLevelWindows[0].Condition.GetPropertyValue("FrameworkId") as string ==
                    "WPF")
                {
                    IntPtr WindowHandle =
                        FocusElement.GetMainWindowHandle(WindowIdentifier.TopLevelWindows[0].WindowTitles[0]);
                                                            TaskActivityObject uIObjectFromUIObjectId = WindowIdentifier.TopLevelWindows[0];
                    if (uIObjectFromUIObjectId.Descendants != null && uIObjectFromUIObjectId.Descendants.Count > 0)
                    {
                        int left = 0, top = 0, width = 0, height = 0;
                        ITaskActivityElement focusElement =
                            FocusElement.GetFocusELement(WindowIdentifier, WindowHandle);
                                                FocusElement.ShowIfMinimized(WindowHandle);
                        NativeMethods.SetForegroundWindow(WindowHandle);
                                                focusElement.GetBoundingRectangle(out left, out top, out width, out height);
                        Cursor.Position = new Point(this.Location.X + left, this.Location.Y + top);
                        Cursor.Show();
                                            }
                }
                else
                {
                    windowTitles = FocusElement.GetWindowTitles(WindowIdentifier);
                    size = windowTitles.Count;
                }
            }
            int result = 0;
            switch (this.ActionType)
            {
                case MouseActionType.Click:
                                                                                                    if (ApplicationSettingProperties.Instance.EnableComPlayback)
                        ExecuteTask.Helpers.Mouse.Click(uITaskControl, this.MouseButton, this.ModifierKeys,
                            new Point(this.Location.X, this.Location.Y));
                    else
                        result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.mouseSingleClick(this.Location.X,
                        this.Location.Y,
                        Convert.ToInt32(this.MouseButton), Convert.ToInt32(this.ModifierKeys),
                        windowTitles.ToArray(), size);
                    if (result != 0)
                        throw new Exception("Could not find " + windowTitles[result - 1]);
                    return;

                case MouseActionType.DoubleClick:
                    if (ApplicationSettingProperties.Instance.EnableComPlayback)

                        ExecuteTask.Helpers.Mouse.DoubleClick(uITaskControl, this.MouseButton, this.ModifierKeys,
                            new Point(this.Location.X, this.Location.Y));
                    else
                        result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.mouseDoubleClick(this.Location.X,
                        this.Location.Y,
                        Convert.ToInt32(this.MouseButton), Convert.ToInt32(this.ModifierKeys),
                        windowTitles.ToArray(), size);
                    if (result != 0)
                        throw new Exception("Could not find " + windowTitles[result - 1]);
                    return;

                case MouseActionType.WheelRotate:
                    ExecuteTask.Helpers.Mouse.MoveScrollWheel(uITaskControl, this.WheelDirection,
                        this.ModifierKeys);
                    return;

                case MouseActionType.Hover:
                    ExecuteTask.Helpers.Mouse.Hover(uITaskControl,
                        new Point(this.Location.X, this.Location.Y));
                    return;
            }

            object[] objArray2 = { this.ActionType.ToString() };
            CrapyLogger.log.ErrorFormat(CultureInfo.CurrentCulture, Resources.ActionNotSupported, objArray2);
            throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture,
                Resources.ActionNotSupported, objArray2));

        }

        public MouseAction(TaskActivityElement uiElement, MouseButtons button, MouseActionType actionType) : base(uiElement)
        {
            absoluteMouseDownLocation = new Point(-100, -100);
            ActionType = actionType;
            MouseButton = button;
            Location = Point.Empty;
        }

        public MouseAction(MouseButtons button, MouseActionType actionType, Point location)
        {
            absoluteMouseDownLocation = new Point(-100, -100);
            MouseButton = button;
            ActionType = actionType;
            Location = location;
        }

        internal override string GetParameterString()
        {
            if (ActionType == MouseActionType.WheelRotate)
            {
                return WheelDirection.ToString(CultureInfo.InvariantCulture);
            }
            if (ImplicitHover)
            {
                object[] objArray1 = { Location };
                return string.Format(CultureInfo.InvariantCulture, "Implicit Hover : Location = {0}", objArray1);
            }
            object[] args = { ModifierKeys, Location };
            return string.Format(CultureInfo.InvariantCulture, "Modifier={0}, Location={1}", args);
        }



        internal static bool IsExplicitHover(IZappyAction recordedAction)
        {
            MouseAction action = recordedAction as MouseAction;
            return action != null && action.actionType == MouseActionType.Hover && !action.ImplicitHover;
        }

        internal static bool IsImplicitHover(IZappyAction recordedAction)
        {
            MouseAction action = recordedAction as MouseAction;
            return action != null && action.ImplicitHover;
        }

        internal override void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            base.ShallowCopy(source, isSeparateAction);
            if (isSeparateAction)
            {
                MouseAction action = source as MouseAction;
                if (action != null)
                {
                    ActionType = action.ActionType;
                    MouseButton = action.MouseButton;
                    Location = action.Location;
                }
            }
        }

    }
}
