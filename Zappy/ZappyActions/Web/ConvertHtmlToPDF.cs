using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.Helpers;

namespace Zappy.ZappyActions.Web
{
    public class ConvertHtmlToPDF : TemplateAction
    {
        [Category("Input")]
        [Description("URL")]
        public DynamicProperty<string> URL { get; set; }

        [Category("Input")]
        [Description("Full Pdf File Path")]
        public DynamicProperty<string> PdfFilePath { get; set; }

        [Category("Optional")]
        [Description("HTML to PDF Options")]
                public DynamicProperty<string> Options { get; set; }

        [Category("Output")]
        [Description("Console output of HTML to PDF")]
        public string ConsoleOutput { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutputBuilder = new StringBuilder();
            string _AppPath = Path.Combine(ZappyMessagingConstants.ExtensionsFolder, "wkhtmltopdf.exe");


            if (File.Exists(_AppPath))
            {
                ProcessStartInfo _info = new ProcessStartInfo();
                Process proc = new Process();

                _info.FileName = _AppPath;
                _info.Arguments = Options + " " + URL + " \"" + PdfFilePath + "\"";
                _info.UseShellExecute = false;
                _info.CreateNoWindow = true;
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

            }
        }

        private StringBuilder ConsoleOutputBuilder;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutputBuilder.Append(outLine.Data);
                                }

    }
}
