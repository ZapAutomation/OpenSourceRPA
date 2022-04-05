using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Code
{
    [Description("Set the variable value")]
                public class SetVariableValue : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter variable name like ${test}")]
        public string VariableName { get; set; }

                                        [Category("Input")]
        [Description("Enter a value to set in the variable - only string values allowed")]
        public DynamicProperty<string> VariableValue { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                    ZappyExecutionContext _context = context as ZappyExecutionContext;
            if (_context.Variables.ContainsKey(VariableName))
                _context.Variables[VariableName] = _context.ExpandVariable(VariableValue);
            else
                throw new Exception("Variable " + VariableName + " not declared");
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Variable Name:" + this.VariableName + " Variable Value:" + this.VariableName;
        }
    }
}
