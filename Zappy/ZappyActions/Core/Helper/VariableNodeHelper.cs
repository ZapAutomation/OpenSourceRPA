using Zappy.Helpers;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Core.Helper
{
    static class VariableNodeHelper
    {
        public static void UpdateContextWithVariables(IVariableAction iVariableAction, ZappyExecutionContext _context, bool allowMultipleVariableWithSameName = false)
        {
            if (iVariableAction is VariableNodeAction _VariableNodeAction)
            {
                string _VariableName = VariableNameCheck(_VariableNodeAction.VariableName, _context, allowMultipleVariableWithSameName);
                _context.Variables[_VariableName] = _context.ExpandVariable(_VariableNodeAction.EvaluationExpression);            }
            else if (iVariableAction is ObjectVariableNodeAction _ObjectVariableNodeAction)
            {
                                string _VariableName = VariableNameCheck(_ObjectVariableNodeAction.VariableName, _context, allowMultipleVariableWithSameName);
                _context.Variables[_VariableName] = _context.ExpandVariableObjectValue(_ObjectVariableNodeAction.EvaluationExpression);            }
            else if (iVariableAction is MutipleVariablesAction _multiVariableNodeAction)
            {
                                foreach (string variableName in _multiVariableNodeAction.Variables)
                {
                    string _VariableName = VariableNameCheck(variableName, _context, allowMultipleVariableWithSameName);
                    _context.Variables[_VariableName] = _context.ExpandVariable("");                }
            }
        }

        public static string VariableNameCheck(string variableName, ZappyExecutionContext _context, bool allowMultipleVariableWithSameName)
        {
            string _VariableName = SharedConstants.VariableNameBegin + variableName +
                        CrapyConstants.VariableNameEnd;
            if (!allowMultipleVariableWithSameName && _context.Variables.ContainsKey(_VariableName))
                throw new System.Exception(" Multiple variables of same name declared");
            return _VariableName;
        }
    }
}