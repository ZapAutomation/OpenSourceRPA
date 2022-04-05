using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Core
{

                
    [Description("Execute C# Script (Lambda Expression)")]
    public sealed class RunScriptActivity : TemplateAction
    {
        [RunScriptFunctionType(typeof(Func<ZappyExecutionContext, IZappyAction, object>))]
        [Category("Input")]
        [Description("Write script in c# Language (Lambda Expression")]
        public DynamicProperty<string> Script { get; set; }

        public RunScriptActivity()
        {
            Script = new DynamicProperty<string>();
            Script.RuntimeScript = "(context , currentactivity) => { DateTime dt = DateTime.Now; return dt; }";
        }

                
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]

        [Category("Output")]
        [Description("Return script result")]
        public object ExecutionResult { get; set; }

        Func<IZappyExecutionContext, IZappyAction, object> _VariableExpression;

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (Script.RuntimeScriptSpecified)
            {
                try
                {
                    if (_VariableExpression == null)
                        _VariableExpression = ZappyExecutionContext.ExpandDynamicExpression<IZappyExecutionContext, IZappyAction, object>(Script.RuntimeScript);
                    ExecutionResult = _VariableExpression(context, this);
                }
                catch (Exception ex)
                {
                    ExecutionResult = ex;
                    throw;
                }
            }
            else
            {
                ExecutionResult = new Exception("Runtime script can only be a script, Variable name / values are not permitted!");
                throw ExecutionResult as Exception;
            }
                        GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Script:" + this.Script + " ExecutionResult:" + this.ExecutionResult;
        }
    }
}

