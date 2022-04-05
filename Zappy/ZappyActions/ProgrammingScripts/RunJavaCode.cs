using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    [Description("Run Python Script ")]
    public class RunJavaCode : TemplateAction
    {
        public RunJavaCode()
        {
        }

        [Category("Input")]
        [Description("Sets java exe path for run the java program")]
        public DynamicProperty<string> JavaExePath { get; set; }

        [Category("Input")]
        [Description("Path of the Jar File")]
        public DynamicProperty<string> JarFilePath { get; set; }

        [Category("Optional")]
        [Description("Arguments for PythonScript to run the script")]
        public DynamicProperty<string> Arguments { get; set; }

        [Category("Output")]
        [Description("Gets command line text after running the script")]
        public string ConsoleOutput { get; set; }


                                                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutputBuilder = new StringBuilder();
            ProcessStartInfo _info = new ProcessStartInfo();
            Process proc = new Process();
            _info.FileName = JavaExePath;
            _info.Arguments = " -jar " + JarFilePath + " " + Arguments;
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

            ConsoleOutput = ConsoleOutputBuilder.ToString();
            proc.Close();

                                }

                                                private StringBuilder ConsoleOutputBuilder;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutputBuilder.Append(outLine.Data);
                                }


        public override string AuditInfo()
        {
            return base.AuditInfo() + "PythonExePath" + this.JavaExePath + "PythonScriptPath" + this.JarFilePath + "Arguments" + this.Arguments + "Text" + this.ConsoleOutput;

        }
    }
}
