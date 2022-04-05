using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;
using DateTime = System.DateTime;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Set The Value")]
    public class SetValueAction : SetBaseAction
    {
        private bool canDecodeStringBeCalled;
        private EncryptionInformation encryptedInfo;
        private object objectValue;
        private bool preferEdit;

        public SetValueAction()
        {
            canDecodeStringBeCalled = true;
        }

        public SetValueAction(TaskActivityElement uiElement, object value) : base(uiElement)
        {
            canDecodeStringBeCalled = true;
            ZappyTaskUtilities.CheckForNull(uiElement, "uiElement");
            if (value is ZappyTaskActionLogEntry)
            {
                ZappyTaskActionLogEntry entry = value as ZappyTaskActionLogEntry;
                if (entry.Value != null)
                {
                    Value = entry.Value.ToString();
                }
                            }
            else
            {
                Value = value;
            }
        }

        public override void BindParameter(ValueMap valueMap, ControlType controlType)
        {
            if (IsParameterized)
            {
                object parameterValue = null;
                if (valueMap.TryGetParameterValue(ParameterName, out parameterValue))
                {
                    if (IsActionOnProtectedElement() && parameterValue != null)
                    {
                        parameterValue = EncodeDecode.EncodeString(parameterValue.ToString());
                    }
                    parameterValue = GetValueInRequiredVSTTFormat(controlType, parameterValue.ToString());
                    ObjectValue = parameterValue;
                }
            }
            BindWithCurrentValues();
        }

        internal override string GetParameterString()
        {
            if (!IsActionOnProtectedElement())
            {
                return ValueAsString;
            }
            string str = string.Empty;
            if (ObjectValue != null)
            {
                str = ObjectValue.ToString();
            }
            return new string('*', str.Length);
        }

        internal string GetValueAsCurrentCultureString(ControlType controlType)
        {
            string shortDateRangeStringInCurrentCulture = string.Empty;
            if (ObjectValue == null)
            {
                return shortDateRangeStringInCurrentCulture;
            }
            if (IsEncoded)
            {
                return EncodeDecode.DecodeString(ObjectValue.ToString());
            }
            if (controlType == ControlType.DateTimePicker)
            {
                DateTime time;
                if (!ZappyTaskUtilities.TryParseDateTimeString(ObjectValue.ToString(), out time))
                {
                    return shortDateRangeStringInCurrentCulture;
                }
                if (ZappyTaskUtilities.HasObjectTime(ObjectValue.ToString()))
                {
                    return time.ToString(CultureInfo.CurrentCulture);
                }
                return time.ToShortDateString();
            }
            if (controlType == ControlType.Calendar)
            {
                string str2;
                string str3;
                DateTime time2;
                DateTime time3;
                ZappyTaskUtilities.TryGetDateTimeRangeString(ObjectValue.ToString(), out str2, out str3);
                if (ZappyTaskUtilities.TryGetShortDate(str2, out time2) && ZappyTaskUtilities.TryGetShortDate(str3, out time3))
                {
                    shortDateRangeStringInCurrentCulture = ZappyTaskUtilities.GetShortDateRangeStringInCurrentCulture(time2, time3);
                }
                return shortDateRangeStringInCurrentCulture;
            }
            return ObjectValue.ToString();
        }

        private string GetValueInRequiredVSTTFormat(ControlType controlType, string paramValue)
        {
            if (controlType == ControlType.DateTimePicker)
            {
                DateTime time;
                if (ZappyTaskUtilities.TryParseDateTimeString(paramValue, out time))
                {
                    return ZappyTaskUtilities.GetDateTimeToString(time, true);
                }
                return paramValue;
            }
            if (controlType == ControlType.Calendar)
            {
                string str;
                string str2;
                ZappyTaskUtilities.TryGetDateTimeRangeString(paramValue, out str, out str2);
                if (!string.IsNullOrEmpty(str))
                {
                    DateTime time2;
                    DateTime time3;
                    if (!ZappyTaskUtilities.TryParseDateTimeString(str, out time2))
                    {
                        return paramValue;
                    }
                    if (string.IsNullOrEmpty(str2) || !ZappyTaskUtilities.TryParseDateTimeString(str2, out time3))
                    {
                        time3 = time2;
                    }
                    return ZappyTaskUtilities.GetShortDateRangeString(time2, time3);
                }
                CrapyLogger.log.ErrorFormat("SetValueAction: Format Specified was wrong");
            }
            return paramValue;
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, this, uITaskControl);
            Type propertyDataType = null;
            try
            {
                string propertyName = ExecuteTaskUtility.GetControlValueProperty(uITaskControl, this, out propertyDataType);
                uITaskControl.SetProperty(propertyName, ExecuteTaskUtility.GetSetValueActionDataForType(this, propertyDataType));
            }
            catch (NotSupportedException exception)
            {
                string str2 = string.Empty;
                if (this.IsParameterized && this.IsParameterBound)
                {
                    object[] objArray2 = { this.ParameterName, this.ValueAsString };
                    str2 = string.Format(CultureInfo.CurrentCulture, Resources.ParameterInformation, objArray2);
                }
                object[] objArray3 = { exception.Message, str2 };
                throw new Exception();            }
        }

        public bool IsActionOnProtectedElement()
        {
            if ((ActivityElement == null || !ActivityElement.IsPassword) && !IsEncoded)
            {
                return false;
            }
            return true;
        }

        private bool MatchControlSpecificParameters(string specifiedString)
        {
            if (ActivityElement != null && ControlType.DateTimePicker.NameEquals(ActivityElement.ControlTypeName))
            {
                DateTime time;
                DateTime time2;
                if (ZappyTaskUtilities.TryParseDateTimeString(specifiedString, out time) && ZappyTaskUtilities.TryParseDateTimeString(ValueAsString, out time2))
                {
                    return DateTime.Equals(time, time2);
                }
            }
            else if (ActivityElement != null && ControlType.Calendar.NameEquals(ActivityElement.ControlTypeName))
            {
                DateTime time3;
                DateTime time4;
                string minDate = string.Empty;
                string maxDate = string.Empty;
                ZappyTaskUtilities.TryGetDateTimeRangeString(specifiedString, out minDate, out maxDate);
                if (string.IsNullOrEmpty(minDate))
                {
                    return false;
                }
                string str3 = string.Empty;
                string str4 = string.Empty;
                ZappyTaskUtilities.TryGetDateTimeRangeString(ValueAsString, out str3, out str4);
                if (string.IsNullOrEmpty(str3))
                {
                    return false;
                }
                if (string.Equals(str3, str4, StringComparison.Ordinal) && string.IsNullOrEmpty(maxDate))
                {
                    str4 = string.Empty;
                }
                if (ZappyTaskUtilities.TryParseDateTimeString(minDate, out time3) && ZappyTaskUtilities.TryParseDateTimeString(str3, out time4) && DateTime.Equals(time3, time4))
                {
                    DateTime time5;
                    DateTime time6;
                    if (!string.IsNullOrEmpty(str4) && !string.IsNullOrEmpty(maxDate) && ZappyTaskUtilities.TryParseDateTimeString(maxDate, out time5) && ZappyTaskUtilities.TryParseDateTimeString(str4, out time6))
                    {
                        return DateTime.Equals(time6, time5);
                    }
                    return string.IsNullOrEmpty(str4) && string.IsNullOrEmpty(maxDate);
                }
            }
            return false;
        }

        internal override bool MatchParameter(string specifiedParameter)
        {
            if (string.IsNullOrEmpty(specifiedParameter))
            {
                return false;
            }
            if (ActivityElement == null || !ControlType.DateTimePicker.NameEquals(ActivityElement.ControlTypeName) && !ControlType.Calendar.NameEquals(ActivityElement.ControlTypeName))
            {
                return base.MatchParameter(specifiedParameter);
            }
            return MatchControlSpecificParameters(specifiedParameter);
        }

        private bool TryValueAsString(out string valueAsString)
        {
            try
            {
                valueAsString = ValueAsString;
                return true;
            }
            catch (Exception)
            {
                valueAsString = ObjectValue.ToString();
            }
                                                            return false;
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public bool IsEncoded
        {
            get =>
                encryptedInfo != null && encryptedInfo.Encoded;
            set
            {
                if (encryptedInfo != null)
                {
                    encryptedInfo.Encoded = value;
                    NotifyPropertyChanged("IsEncoded");
                }
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override bool IsParameterizable =>
            true;

        private object ObjectValue
        {
            get
            {
                if (IsParameterized && !IsParameterBound)
                {
                    return null;
                }
                return objectValue;
            }
            set
            {
                objectValue = value;
                NotifyPropertyChanged("ObjectValue");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public bool PreferEdit
        {
            get =>
                preferEdit;
            set
            {
                preferEdit = value;
                NotifyPropertyChanged("PreferEdit");
            }
        }

        [XmlElement(ElementName = "PreferEdit")]
        public string PreferEditWrapper
        {
            get
            {
                if (preferEdit)
                {
                    return preferEdit.ToString();
                }
                return null;
            }
            set
            {
                bool flag;
                if (bool.TryParse(value, out flag) & flag)
                {
                    preferEdit = true;
                }
                else
                {
                    preferEdit = false;
                }
                NotifyPropertyChanged("PreferEditWrapper");
            }
        }

        [XmlElement(ElementName = "Value")]
        public EncryptionInformation TextValue
        {
            get
            {
                string str;
                bool flag = TryValueAsString(out str);
                if (!IsActionOnProtectedElement() || string.IsNullOrEmpty(str))
                {
                    return new EncryptionInformation(str, false);
                }
                if (flag)
                {
                    return new EncryptionInformation(EncodeDecode.EncodeString(str), true);
                }
                return new EncryptionInformation(str, true);
            }
            set
            {
                encryptedInfo = value;
                ValueAsString = encryptedInfo.Value;
                NotifyPropertyChanged("TextValue");
            }
        }


        public string Type
        {
            get
            {
                string name = null;
                if (ObjectValue != null)
                {
                    name = ObjectValue.GetType().Name;
                }
                return name;
            }
            set
            {
                if (typeof(AccessibleStates).Name.Equals(value, StringComparison.Ordinal))
                {
                    if (ObjectValue != null)
                    {
                        ObjectValue = Enum.Parse(typeof(AccessibleStates), ObjectValue.ToString());
                    }
                }
                else if (objectValue == null && value.Equals("String", StringComparison.OrdinalIgnoreCase))
                {
                    objectValue = string.Empty;
                }
                NotifyPropertyChanged("Type");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public object Value
        {
            get
            {
                if (IsEncoded && canDecodeStringBeCalled)
                {
                    try
                    {
                        return EncodeDecode.DecodeString(ObjectValue.ToString());
                    }
                    catch (Exception)
                    {
                        canDecodeStringBeCalled = false;
                        throw;
                    }
                                                                                                                    }
                return ObjectValue;
            }
            set
            {
                ObjectValue = value;
                NotifyPropertyChanged("Value");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public override string ValueAsString
        {
            get
            {
                string str = string.Empty;
                if (ObjectValue == null)
                {
                    return str;
                }
                if (IsEncoded)
                {
                    return EncodeDecode.DecodeString(ObjectValue.ToString());
                }
                return ObjectValue.ToString();
            }
            set
            {
                ObjectValue = value;
                NotifyPropertyChanged("ValueAsString");
            }
        }
    }
}