using System.ComponentModel;
using System.Management.Automation;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    public class RunPowerShellScripts : TemplateAction
    {
        public RunPowerShellScripts()
        {
        }

        [Category("Input")]
        [Description("PowerShell Scripts")]
        public DynamicProperty<string> Command { get; set; }

        [Category("Optional")]
        [Description("Path of the file which contain PowerShell Script")]
        public DynamicProperty<string> PSScriptFilePath { get; set; }

                        [Category("Output")]
        [Description("Result of the command")]
        public string ScriptResult { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            PowerShell ps = PowerShell.Create();

            if(string.IsNullOrWhiteSpace(PSScriptFilePath))
                ps.AddCommand(Command);
            else
                ps.AddScript(PSScriptFilePath);

            ScriptResult = string.Empty;
            foreach (PSObject result in ps.Invoke())
            {
                ScriptResult += result;
            }         }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Command: " + this.Command + " PSScriptFilePath: " + this.PSScriptFilePath;
        }
    }
}