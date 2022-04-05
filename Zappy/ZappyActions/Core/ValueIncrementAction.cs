using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("To increment the values")]
    public class ValueIncrementAction : TemplateAction
    {
        [Category("Input")]
        [Description("By how much user want to increment")]
        public DynamicProperty<int> StepValue { get; set; }


        [Category("Input")]
        [Description("Value")]
        public DynamicProperty<int> IntValue { get; set; }


        [Category("Output")]
        [Description("Return incremented value")]
        public int IncrementedIntValue { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            IncrementedIntValue = IntValue + StepValue;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Step Value:" + this.StepValue + " Integer Value:" + this.IntValue + " Incremented Value:" + this.IncrementedIntValue;
        }
    }
}
