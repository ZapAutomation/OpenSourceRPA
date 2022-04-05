using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Description("Sets State")]
    public class SetStateAction : SetBaseAction
    {
        private object objectState;

        public SetStateAction()
        {
        }

        public SetStateAction(TaskActivityElement uiElement, object value) : base(uiElement)
        {
            ZappyTaskUtilities.CheckForNull(uiElement, "uiElement");
            if (value is ZappyTaskActionLogEntry)
            {
                ZappyTaskActionLogEntry entry = value as ZappyTaskActionLogEntry;
                if (entry.Value != null)
                {
                    this.State = entry.Value.ToString();
                }
                            }
            else
            {
                State = value;
            }
        }

        internal override string GetParameterString() =>
            StateAsString;

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                    
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, this, uITaskControl);
            try
            {
                bool[] propertyValues = null;
                string[] strArray = ExecuteTaskUtility.GetControlStateProperty(uITaskControl, this.States, out propertyValues);
                if (strArray != null && propertyValues != null && strArray.Length != 0 && strArray.Length == propertyValues.Length)
                {
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        uITaskControl.SetProperty(strArray[i], propertyValues[i]);
                    }
                }
                else
                {
                    uITaskControl.SetProperty(ZappyTaskControl.PropertyNames.State, this.States);
                }
            }
            catch (NotSupportedException exception)
            {
                string str = string.Empty;
                if (this.IsParameterized && this.IsParameterBound)
                {
                    object[] objArray2 = { this.ParameterName, this.ValueAsString };
                    str = string.Format(CultureInfo.CurrentCulture, Resources.ParameterInformation, objArray2);
                }
                object[] objArray3 = { exception.Message, str };
                throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.ControlStateNotSupported, objArray3));
            }
        }

        private object ObjectState
        {
            get
            {
                if (IsParameterized && !IsParameterBound)
                {
                    return null;
                }
                return objectState;
            }
            set
            {
                objectState = value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public object State
        {
            get =>
                States;
            set
            {
                if (value != null)
                {
                    string str = value.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        try
                        {
                            States = (ControlStates)Enum.Parse(typeof(ControlStates), str, true);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                    NotifyPropertyChanged("State");
                }
            }
        }

        [XmlElement(ElementName = "State")]
        public string StateAsString
        {
            get
            {
                string str = string.Empty;
                if (ObjectState != null)
                {
                    str = ObjectState.ToString();
                }
                return str;
            }
            set
            {
                ObjectState = value;
                NotifyPropertyChanged("StateAsString");
            }
        }

        [XmlIgnore, JsonIgnore]
        internal ControlStates States
        {
            get
            {
                ControlStates none = ControlStates.None;
                try
                {
                    none = (ControlStates)Enum.Parse(typeof(ControlStates), ValueAsString, true);
                }
                catch (ArgumentException)
                {
                }
                return none;
            }
            set
            {
                StateAsString = value.ToString();
                NotifyPropertyChanged("States");
            }
        }
    }
}