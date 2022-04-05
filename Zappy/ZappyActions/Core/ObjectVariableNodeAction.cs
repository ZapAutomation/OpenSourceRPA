using System.ComponentModel;
using System.Windows;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core.Helper;

namespace Zappy.ZappyActions.Core
{
    public sealed class ObjectVariableNodeAction : TemplateAction, IVariableAction
    {
                public ObjectVariableNodeAction()
        {
            EvaluationExpression = "";
        }
                                [Description("Variable name is same as Display Name - cannot use copy and paste as variable name")]
        [Category("Input")]
        public string VariableName
        {
            get => DisplayName;
            set
            {
                DisplayName = value;
                                if (DisplayName.ToUpper().Equals("COPY") || DisplayName.ToUpper().Equals("PASTE"))
                {
                    DisplayName = "Undefined";
                    MessageBox.Show("Invalid Variable Name");
                }
            }
        }

                [Description("Variable value - return type can be any C# object")]
        [Category("Output/Input")]
        public DynamicProperty<object> EvaluationExpression { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " EvaluationExpression:" + this.EvaluationExpression + " VariableName:" + this.VariableName;
        }
    }

}
