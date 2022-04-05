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
            
    [Description("Convert PDF file into text")]
    public class PDFToText : TemplateAction
    {
        /*
        
   Usage: pdftotext [options] <PDF-file> [<text-file>]
  -f <int>             : first page to convert
  -l <int>             : last page to convert
  -layout              : maintain original physical layout
  -simple              : simple one-column page layout
  -table               : similar to -layout, but optimized for tables
  -lineprinter         : use strict fixed-pitch/height layout
  -raw                 : keep strings in content stream order
  -fixed <number>      : assume fixed-pitch (or tabular) text
  -linespacing <number>: fixed line spacing for LinePrinter mode
  -clip                : separate clipped text
  -nodiag              : discard diagonal text
  -enc <string>        : output text encoding name
  -eol <string>        : output end-of-line convention (unix, dos, or mac)
  -nopgbrk             : don't insert page breaks between pages
  -bom                 : insert a Unicode BOM at the start of the text file
  -opw <string>        : owner password (for encrypted files)
  -upw <string>        : user password (for encrypted files)
  -q                   : don't print any messages or errors
             
             */
        public PDFToText()
        {
                                    First_N_PagesToConvert = Last_N_PagesToConvert = -1;
            Encoding = "UTF-8";
            Arguments = " -raw -layout";
        }

        [DefaultValue(-1),
         Description("First N pages to convert")]
        [Category("Optional")]
        public int First_N_PagesToConvert { get; set; }


        [DefaultValue(-1),
         Description("Last N pages to convert")]
        [Category("Optional")]
        public int Last_N_PagesToConvert { get; set; }


        [DefaultValue("UTF-8")]
        public string Encoding { get; set; }

                
                
        [Category("Input")]
        [Description("Inputed Pdf file path")]
        public DynamicProperty<string> InputFilePath { get; set; }
        [Category("Input")]
        [Description("File path of the output text - generates temporary file if no output path specified")]
        public DynamicProperty<string> OutputFilePath { get; set; }
        [Description("-layout              : maintain original physical layout\r\n  -simple           " +
                     "   : simple one-column page layout\r\n  -table       " +
                     "        : similar to -layout, but optimized for tables\r\n  -lineprinter         " +
                     ": use strict fixed-pitch/height layout\r\n  -raw                 " +
                     ": keep strings in content stream order\r\n  -fixed <number>      " +
                     ": assume fixed-pitch (or tabular) text\r\n  -linespacing <number>: " +
                     "fixed line spacing for LinePrinter mode\r\n  -clip     " +
                     "           : separate clipped text\r\n  -nodiag             " +
                     " : discard diagonal text\r\n  -enc <string>        : output text encoding name\r\n  -eol <string>     " +
                     "   : output end-of-line convention (unix, dos, or mac)\r\n  -nopgbrk           " +
                     "  : don\'t insert page breaks between pages\r\n  -bom                " +
                     " : insert a Unicode BOM at the start of the text file\r\n  -opw <string>     " +
                     "   : owner password (for encrypted files)\r\n  -upw <string>    " +
                     "    : user password (for encrypted files)")]
        public DynamicProperty<string> Arguments { get; set; }
        [Description("True, if delete created text file; otherwise,False")]
        [Category("Optional")]
        public DynamicProperty<bool> DeleteOutputFile { get; set; }

        [Category("Output")]
        [Description("Converted text from PDF file")]
        public string OutputString { get; set; }

        [Category("Output")]
        [Description("Console output of PDF to Text")]
        public string ConsoleOutput { get; set; }

        [Category("Output")]
        [Description("Texts in array")]
        public string[] PageStringArray { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutputBuilder = new StringBuilder();
                        string _AppPath = Path.Combine(CrapyConstants.PdfToolsFolder, "pdftotext.exe");
            if (string.IsNullOrEmpty(OutputFilePath))
                OutputFilePath = CommonProgram.GetZappyTempFile();
                                                                                    string _Arguments = Arguments.Value + " -enc {0} \"{1}\" \"{2}\"";


            if (First_N_PagesToConvert > 0)
            {
                _Arguments = "-f " + First_N_PagesToConvert.ToString() + _Arguments;
            }

            if (Last_N_PagesToConvert > 0)
            {
                _Arguments = "-f " + Last_N_PagesToConvert.ToString() + _Arguments;
            }

            _Arguments = string.Format(_Arguments, Encoding, InputFilePath, OutputFilePath);

            if (File.Exists(_AppPath))
            {
                ProcessStartInfo _info = new ProcessStartInfo();
                Process proc = new Process();

                _info.FileName = _AppPath;
                _info.Arguments = _Arguments;
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
                if (File.Exists(OutputFilePath))
                {
                    OutputString = File.ReadAllText(OutputFilePath, System.Text.Encoding.UTF8);
                    if (DeleteOutputFile)
                        File.Delete(OutputFilePath);
                }

                PageStringArray = OutputString.Split('\f');
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
                   this.InputFilePath +  "Console Output: " + this.ConsoleOutput;
        }
    }
}
