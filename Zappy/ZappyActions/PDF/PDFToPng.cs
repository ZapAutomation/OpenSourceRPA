extern alias itextsharp;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Path = System.IO.Path;

namespace Zappy.ZappyActions.PDF
{
            
    [Description("Gets Text From PDF Files")]
    public class PDFToPng : TemplateAction
    {
        /*
        
   -f number
              Specifies the first page to convert.

       -l number
              Specifies the last page to convert.

       -r number
              Specifies the resolution, in DPI.  The default is 150 DPI.

       -mono  Generate a monochrome image (instead of a color image).

       -gray  Generate a grayscale image (instead of a color image).

       -alpha Generate  an alpha channel in the PNG file.  This is only useful
              with PDF files that have been  constructed  with  a  transparent
              background.  The -alpha flag cannot be used with -mono.

       -freetype yes | no
              Enable  or  disable  FreeType  (a TrueType / Type 1 font raster-
              izer).  This defaults to "yes".  [config file: enableFreeType]

       -aa yes | no
              Enable or disable font anti-aliasing.  This defaults  to  "yes".
              [config file: antialias]

       -aaVector yes | no
              Enable or disable vector anti-aliasing.  This defaults to "yes".
              [config file: vectorAntialias]

       -opw password
              Specify the owner password for the  PDF  file.   Providing  this
              will bypass all security restrictions.

       -upw password
              Specify the user password for the PDF file.

       -q     Don't print any messages or errors.  [config file: errQuiet]

       -v     Print copyright and version information.

       -h     Print usage information.  (-help and --help are equivalent.)
             
             */
        public PDFToPng()
        {
                                    First_N_PagesToConvert = Last_N_PagesToConvert = -1;

        }

        [DefaultValue(-1),
         Description("First N pages to convert")]
        [Category("Input")]
        public int First_N_PagesToConvert { get; set; }


        [DefaultValue(-1),
         Description("Last N pages to convert")]
        [Category("Input")]
        public int Last_N_PagesToConvert { get; set; }


                
                
        [Category("Input")]
        [Description("Inputed Pdf file path")]
        public DynamicProperty<string> InputFilePath { get; set; }
        [Category("Input/Output")]
        [Description("File path of the output text - generates temporary file if no output path specified")]
        public DynamicProperty<string> OutputFilePath { get; set; }
        [Category("Input")]
        public DynamicProperty<string> Arguments { get; set; }

        [Category("Output")]
        [Description("Console output of PDF to Text")]
        public string ConsoleOutput { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutputBuilder = new StringBuilder();
                        string _AppPath = Path.Combine(CrapyConstants.PdfToolsFolder, "pdftopng.exe");
            if (string.IsNullOrEmpty(OutputFilePath))
                OutputFilePath = CommonProgram.GetZappyTempFile();
                                                                                    string _Arguments = Arguments + "\"{0}\" \"{1}\"";


            if (First_N_PagesToConvert > 0)
            {
                _Arguments = "-f " + First_N_PagesToConvert.ToString() + _Arguments;
            }

            if (Last_N_PagesToConvert > 0)
            {
                _Arguments = "-f " + Last_N_PagesToConvert.ToString() + _Arguments;
            }

            _Arguments = string.Format(_Arguments, InputFilePath, OutputFilePath);

            if (File.Exists(_AppPath))
            {
                ProcessStartInfo _info = new ProcessStartInfo();
                Process proc = new Process();

                _info.FileName = _AppPath;
                _info.Arguments = _Arguments;
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

            }
        }

        private StringBuilder ConsoleOutputBuilder;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutputBuilder.Append(outLine.Data);
                                }

        public override string AuditInfo()
        {
            return base.AuditInfo()
                   + " Arguments:" + this.Arguments + " InputFilePath:" +
                   this.InputFilePath;
        }
    }
}
