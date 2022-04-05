using System;
using System.ComponentModel;
using System.Globalization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.ExecuteTask.Helpers
{
    public class PropertyExpression : ICloneable, INotifyPropertyChanged
    {
        private string propertyName;
        private PropertyExpressionOperator propertyOperator;
        private string propertyValue;

                public event PropertyChangedEventHandler PropertyChanged;

        public PropertyExpression()
        {
            PropertyOperator = PropertyExpressionOperator.EqualTo;
        }

        internal PropertyExpression(PropertyCondition propertyCondition)
        {
            propertyName = propertyCondition.PropertyName;
            propertyValue = propertyCondition.Value != null ? propertyCondition.Value.ToString() : string.Empty;
            propertyOperator = (PropertyExpressionOperator)Enum.Parse(typeof(PropertyExpressionOperator),
                propertyCondition.PropertyOperator.ToString());
        }

        public PropertyExpression(string propertyName, string propertyValue) : this(propertyName, propertyValue,
            PropertyExpressionOperator.EqualTo)
        {
        }

        public PropertyExpression(string propertyName, string propertyValue,
            PropertyExpressionOperator propertyOperator)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
            this.propertyOperator = propertyOperator;
        }

        public object Clone() =>
            new PropertyExpression(PropertyName, PropertyValue, PropertyOperator);

        public override bool Equals(object other)
        {
            PropertyExpression expression = other as PropertyExpression;
            if (expression == null)
            {
                return false;
            }
            return PropertyOperator == expression.PropertyOperator && string.Equals(PropertyName,
                       expression.PropertyName, StringComparison.OrdinalIgnoreCase) &&
                   Equals(PropertyValue, expression.PropertyValue);
        }

        private void FirePropertyChange(string changedPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedPropertyName));
        }

        public override int GetHashCode()
        {
            int hashCode = PropertyOperator.GetHashCode();
            if (PropertyName != null)
            {
                hashCode ^= PropertyName.GetHashCode();
            }
            if (PropertyValue != null)
            {
                hashCode ^= PropertyValue.GetHashCode();
            }
            return hashCode;
        }

        internal PropertyCondition ToPropertyCondition() =>
            new PropertyCondition(PropertyName, PropertyValue,
                (PropertyConditionOperator)Enum.Parse(typeof(PropertyConditionOperator),
                    PropertyOperator.ToString()));

        public override string ToString()
        {
            object[] args = { PropertyName, PropertyOperator, PropertyValue };
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} '{2}'", args);
        }

        public string PropertyName
        {
            get =>
                propertyName;
            set
            {
                propertyName = value;
                FirePropertyChange("PropertyName");
            }
        }

        public PropertyExpressionOperator PropertyOperator
        {
            get =>
                propertyOperator;
            set
            {
                propertyOperator = value;
                FirePropertyChange("PropertyOperator");
            }
        }

        public string PropertyValue
        {
            get =>
                propertyValue;
            set
            {
                propertyValue = value;
                FirePropertyChange("PropertyValue");
            }
        }
    }
}