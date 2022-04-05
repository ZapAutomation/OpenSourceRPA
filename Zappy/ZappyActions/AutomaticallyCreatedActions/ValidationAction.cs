using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    public abstract class ValidationAction : ZappyTaskAction
    {
        private string expectedValue;
        private string messageOnValidationFailure;
        private string propertyName;

        protected ValidationAction()
        {
        }

        protected ValidationAction(TaskActivityElement uiElement, string propertyName, string expectedValue) : base(uiElement)
        {
            this.propertyName = propertyName;
            this.expectedValue = expectedValue;
        }

        public override void BindParameter(ValueMap valueMap, ControlType controltType)
        {
            if (IsParameterized)
            {
                object parameterValue = null;
                if (valueMap.TryGetParameterValue(ParameterName, out parameterValue))
                {
                    ExpectedValue = parameterValue.ToString();
                }
            }
            BindWithCurrentValues();
        }

        public virtual string ExpectedValue
        {
            get =>
                expectedValue;
            set
            {
                expectedValue = value;
                NotifyPropertyChanged("ExpectedValue");
            }
        }

        [XmlIgnore]
        public override bool IsParameterizable =>
            true;

        public virtual string MessageOnValidationFailure
        {
            get =>
                messageOnValidationFailure;
            set
            {
                messageOnValidationFailure = value;
                NotifyPropertyChanged("MessageOnValidationFailure");
            }
        }

        public string PropertyName
        {
            get =>
                propertyName;
            set
            {
                propertyName = value;
                NotifyPropertyChanged("PropertyName");
            }
        }
    }
}