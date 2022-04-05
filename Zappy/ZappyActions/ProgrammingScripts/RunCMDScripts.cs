using System.Activities;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    public class RunCMDScripts : TemplateAction
    {
        public RunCMDScripts()
        {
            RunHiddenCmdProcess = true;
        }

        [Category("Input"), RequiredArgument]
        [Description("cmd command like python")]
        public DynamicProperty<string> Command { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("Path of the file which contain list of cmd commands")]
        public DynamicProperty<string> RunBatchFilePath { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("true, if user want to hide the cmd;otherwise false")]
        public DynamicProperty<bool> RunHiddenCmdProcess { get; set; }

                        [Category("Output")]
        [Description("Result of the command")]
        public string CommandLineOutput { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutput = new StringBuilder();
            if (!string.IsNullOrEmpty(RunBatchFilePath))
                Command = RunBatchFilePath;
            ExecuteCommand(Command);
            CommandLineOutput = ConsoleOutput.ToString();
        }
        void ExecuteCommand(string command)
        {
            
            ProcessStartInfo _info = new ProcessStartInfo();
            Process proc = new Process();

            _info.FileName = "cmd.exe";
            _info.Arguments = "/C " + command;
            if (RunHiddenCmdProcess)
            {
                _info.UseShellExecute = false;
                _info.RedirectStandardOutput = true;
                _info.RedirectStandardError = true;
                                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                proc.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                            }
            proc.StartInfo = _info;

            proc.Start();
            if (RunHiddenCmdProcess)
            {
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
            }
                        proc.WaitForExit();

                                                
            
                                                        }
        private StringBuilder ConsoleOutput;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutput.Append(outLine.Data);
                                }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " Command: " + this.Command + " RunBatchFilePaths: " + this.RunBatchFilePath;
        }
    }
}