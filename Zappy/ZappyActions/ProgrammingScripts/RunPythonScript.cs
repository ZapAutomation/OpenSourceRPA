using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    [Description("Run Python Script ")]
    public class RunPythonScript : TemplateAction
    {
        public RunPythonScript()
        {
        }

        [Category("Input")]
        [Description("Sets python exe path for run the script")]
        public DynamicProperty<string> PythonExePath { get; set; }

        [Category("Input")]
        [Description("Path of the PythonScript")]
        public DynamicProperty<string> PythonScriptPath { get; set; }

        [Category("Optional")]
        [Description("Arguments for PythonScript to run the script")]
        public DynamicProperty<string> Arguments { get; set; }

        [Category("Output")]
        [Description("Gets command line text after running the script")]
        public string CommandText { get; set; }


                                                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutput = new StringBuilder();
            ProcessStartInfo _info = new ProcessStartInfo();
            Process proc = new Process();
            _info.FileName = PythonExePath;
            _info.Arguments = PythonScriptPath + " " + Arguments;
            _info.UseShellExecute = false;
            _info.RedirectStandardOutput = true;
            _info.RedirectStandardError = true;
            proc.StartInfo = _info;
                        proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            proc.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                        proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
                        proc.WaitForExit();

            CommandText = ConsoleOutput.ToString();
            proc.Close();

                                }

                                                private StringBuilder ConsoleOutput;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutput.Append(outLine.Data);
                                }


        public override string AuditInfo()
        {
            return base.AuditInfo() + "PythonExePath" + this.PythonExePath + "PythonScriptPath" + this.PythonScriptPath + "Arguments" + this.Arguments + "Text" + this.CommandText;

        }
    }
}
