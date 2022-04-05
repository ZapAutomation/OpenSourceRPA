using System;
using System.ComponentModel;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Core
{
                    
    [Description("Based on the result, it either follows the true or the false path")]
    public class DecisionNodeAction : IZappyAction
    {
                                public DecisionNodeAction()
        {
            SelfGuid = Guid.NewGuid(); Id = ActionIDRegister.GetUniqueId(); Timestamp = WallClock.UtcNow;
        }

                                        [System.Xml.Serialization.XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public long Id { get; set; }
        [Category("Common")]
        [Description("The display name of the activity")]
        public string DisplayName { get; set; }

                [Browsable(false)]
        public System.DateTime Timestamp { get; set; }
        [Browsable(false)]
        public string ExeName { get; set; }

                [Browsable(false)]
        public int TimeOutInMilliseconds { get; set; }
        [Description("Specifies if the automation should continue even when the activity throws an error")]
        [Category("Common")] public bool ContinueOnError { get; set; }

        [Description("Specifies the amount of time (in milliseconds) to pause after the current activity has finished execution")]
        [Category("Common")] public int PauseTimeAfterAction { get; set; }

        [Category("Common")] public int NumberOfRetries { get; set; }

        [Category("Internals")]
        public Guid SelfGuid { get; set; }

        [Category("Internals")]
        public Guid ErrorHandlerGuid { get; set; }
        [Browsable(false)]
        public int EditorLocationX { get; set; }
        [Browsable(false)]
        public int EditorLocationY { get; set; }
                        

        [Category("Internals")]
        public Guid NextGuid
        {
            get
            {
                return EvaluationResult ? TrueGuid : FalseGuid;
            }
            set { }
        }

        [Category("Internals")]
        public Guid TrueGuid { get; set; }

        [Category("Internals")]
        public Guid FalseGuid { get; set; }

        private Func<IZappyExecutionContext, bool> _VariableExpression;

        public event PropertyChangedEventHandler PropertyChanged;

        [Category("Output")]
        [System.Xml.Serialization.XmlIgnore]
        [Description("Evaluation Result ")]
        public bool EvaluationResult { get; protected set; }

        [Category("Input")]
        [RunScriptFunctionType(typeof(Func<ZappyExecutionContext, bool>))]
        [Description("EvaluationExpression")]
        public DynamicProperty<bool> EvaluationExpression { get; set; }

        public virtual bool Execute(IZappyExecutionContext context)
        {
                        try
            {

                if (EvaluationExpression.DymanicKeySpecified)
                    return Convert.ToBoolean(context.GetValueFromKey(EvaluationExpression.DymanicKey));
                else if (EvaluationExpression.RuntimeScriptSpecified)
                {
                    if (_VariableExpression == null)
                        _VariableExpression = ZappyExecutionContext.ExpandDynamicExpression<IZappyExecutionContext, bool>(EvaluationExpression.RuntimeScript);
                    return _VariableExpression(context);
                }
                else if (EvaluationExpression.ValueSpecified)
                    return Convert.ToBoolean(EvaluationExpression.Value);
                return false;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                throw;
            }
        }


                                                public void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            EvaluationResult = Execute(context);
                    }

        public virtual string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this) + " EvaluationExpression:" + this.EvaluationExpression + " EvaluationResult:" + this.EvaluationResult;
        }       
    }
}