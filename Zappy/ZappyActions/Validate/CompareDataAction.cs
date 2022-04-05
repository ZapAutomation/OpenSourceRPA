using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Validate
{
    public class CompareDataAction : TemplateAction
    {
        [Category("Input")]
        public DynamicProperty<string> FirstObject { get; set; }

        [Category("Input")]
        public DynamicProperty<string> SecondObject { get; set; }

        [Category("Input")]
        public CompareDataHelper ComparisionType { get; set; }

        [Category("Output")]
        public bool Result { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        Result = false;
            if (ComparisionType == CompareDataHelper.Equal && FirstObject.Value.Equals(SecondObject.Value))
                Result = true;
            else if (ComparisionType == CompareDataHelper.GreaterThan)
            {
                if (Double.TryParse(FirstObject.Value, out double firstValue))
                    if (Double.TryParse(SecondObject.Value, out double secondValue))
                    {
                        if (firstValue > secondValue)
                            Result = true;
                    }
            }
            else if (ComparisionType == CompareDataHelper.LessThan)
            {
                if (Double.TryParse(FirstObject.Value, out double firstValue))
                    if (Double.TryParse(SecondObject.Value, out double secondValue))
                    {
                        if (firstValue < secondValue)
                            Result = true;
                    }
            }
            else
                Result = false;
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FirstObject:" + this.FirstObject + " SecondObject:" + this.SecondObject + " Result:" + this.Result;
        }
    }
}
