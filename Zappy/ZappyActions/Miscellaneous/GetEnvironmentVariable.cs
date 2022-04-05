using System;
using System.Activities;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Retrieves the value of an environment variable from the current process")]
    public class GetEnvironmentVariable : TemplateAction
    {

        [Category("Input"), RequiredArgument]
        [Description("The name of the environment variable")]
        public DynamicProperty<string> Variable { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("The value of the environment variable specified by variable")]
        public string VariableValue { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            this.VariableValue = Environment.GetEnvironmentVariable(this.Variable);
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " Variable:" + this.Variable + " VariableValue:" + this.VariableValue;
        }
    }
}

