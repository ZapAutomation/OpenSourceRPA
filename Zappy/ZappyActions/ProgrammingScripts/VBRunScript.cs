using System.Collections.Generic;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    [Description("Execute VB Script")]
    public class VBRunScript : TemplateAction
    {

        [Category("Input")]
        [Description("Write script in VB language")]
        public DynamicProperty<string> Script { get; set; }

        [Category("Input")]
        [Description("Dont use ${ inside strings when this is set to true")]
        public DynamicProperty<bool> ExpandVariable { get; set; }

        [Category("Input")]
        [Description("True, If you want to execute multiple VB statements")]
        public DynamicProperty<bool> ExecuteMultipleStatements { get; set; }

        [Category("Output")]
        [Description("Return Script Result")]
        public object ExecutionResult { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            VBHiddenForm frm = new VBHiddenForm();
            string NewScriptWithVariableReplaced = Script;
            if (ExpandVariable)
            {
                //int _Index = Script.Value.IndexOf(SharedConstants.VariableNameBegin);
                ZappyExecutionContext _context = context as ZappyExecutionContext;
                //_context.Variables[VariableName] = _context.ExpandVariable(VariableValue);
                List<string> GetVariableList = ExtractFromBody(NewScriptWithVariableReplaced, SharedConstants.VariableNameBegin,
                    CrapyConstants.VariableNameEnd);
                foreach (string variable in GetVariableList)
                {
                    string variableValue = _context.GetValueFromKey(variable).ToString();
                    //var abc = _context.Variables[variables].Item2.ToString();
                    NewScriptWithVariableReplaced = NewScriptWithVariableReplaced.Replace(variable, variableValue);
                }

            }

            if (ExecuteMultipleStatements)
            {
                frm.Execute(NewScriptWithVariableReplaced);
            }
            else
            {
                ExecutionResult = frm.Evaluate(NewScriptWithVariableReplaced);
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Script:" + this.Script + " ExecuteMultipleStatements:" + this.ExecuteMultipleStatements + " ExecutionResult:" + this.ExecutionResult;
        }

        //TODO - List of string
        private List<string> ExtractFromBody(string body, string start, string end)
        {
            List<string> matched = new List<string>();

            int indexStart = 0;
            int indexEnd = 0;

            bool exit = false;
            while (!exit)
            {
                indexStart = body.IndexOf(start);

                if (indexStart != -1)
                {
                    indexEnd = indexStart + body.Substring(indexStart).IndexOf(end);

                    matched.Add(body.Substring(indexStart, indexEnd - indexStart + end.Length)); // + start.Length - indexStart - start.Length

                    body = body.Substring(indexEnd + end.Length);
                }
                else
                {
                    exit = true;
                }
            }

            return matched;
        }
    }
}
