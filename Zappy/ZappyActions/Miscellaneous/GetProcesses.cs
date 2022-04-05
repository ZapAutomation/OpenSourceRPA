using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Gets All the running processes")]
    public class GetProcesses : TemplateAction
    {
        [XmlIgnore]
        [Description("Gets Process")]
        [Category("Output")]
        public DynamicProperty<Collection<Process>> Processes { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            try
            {
                Processes.Value = new Collection<Process>(Process.GetProcesses().ToList<Process>());
            }
            catch
            {
                throw;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Processes:" + this.Processes;
        }
    }
}

