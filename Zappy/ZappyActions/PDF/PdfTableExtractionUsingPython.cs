using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.PDF.Helper;

namespace Zappy.ZappyActions.PDF
{
    public class PdfTableExtractionUsingPython : TemplateAction
    {
        public PdfTableExtractionUsingPython()
        {
                        FileType = PdfFileType.lattice;
            Pages = "all";
            Passwd = null;
            Line_overlap = 0.3;
            Char_margin = 1.0;
            Line_margin = 0.2;
            Word_margin = 2.0;
            Boxes_flow = 0.5;
        }

                [Description("Automatically generated. Enter system specific python path if required")]
        public DynamicProperty<string> PythonExePath { get; set; }

        [Category("Optional")]
        [Description("Enter line_Overlap Margin (Ex.0.5)")]
        public Double Line_overlap { get; set; }

        [Category("Optional")]
        [Description("Enter char Margin (Ex.2.0)")]
        public Double Char_margin { get; set; }

        [Category("Optional")]
        [Description("Enter line Margin (Ex.0.2)")]
        public Double Line_margin { get; set; }

        [Category("Optional")]
        [Description("Enter word Margin (Ex.0.5)")]
        public Double Word_margin { get; set; }

        [Category("Optional")]
        [Description("Enter boxes_flow Margin (Ex.0.5)")]
        public Double Boxes_flow { get; set; }

        [Category("Input")]
        [Description("Pdf file path")]
        public DynamicProperty<string> InputFilePath { get; set; }

        [Category("Optional")]
        [Description("Pdf file password if file is password protected ")]
        public string Passwd { get; set; }

        [Category("Optional")]
        [Description("Pdf File Type like lattice or stream ")]
        public PdfFileType FileType { get; set; }

        /* Commands:
            lattice  Use lines between text to parse the table.
            stream   Use spaces between text to parse the table.
             */

        [Category("Optional")]
        [Description("Page number with comma sepreted like(1,2,3,..)")]
        public string Pages { get; set; }


        [Category("Output")]
        [Description("Pdf file path")]
        public string OutputFilePath { get; set; }

        [Category("Output")]
        [Description("Gets command line text after running the script")]
        public string CommandText { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ConsoleOutput = new StringBuilder();
            string _Arguments = "\"{0}\"" + " " + "\"{1}\"" + " " + "\"{2}\"" + " " + "\"{3}\"" + " " + "\"{4}\" " + "\"{5}\" " + "\"{6}\" " + "\"{7}\" " + "\"{8}\" " + "\"{9}\" " + "\"{10}\" ";
                        if (string.IsNullOrWhiteSpace(PythonExePath))
                PythonExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Python\Python37\python.exe");
                        string myPythonApp = Path.Combine(CrapyConstants.PdfToolsFolder, "Pdf2Excel.py");
            string tempPath = InputFilePath.ToString();
            string Filename = Path.GetFileNameWithoutExtension(tempPath);
            string extention = Path.GetExtension(tempPath);
            string _extention = extention.Replace("pdf", "xlsx");
            string Directory = Path.GetDirectoryName(tempPath);
            string _OutputFile = Path.Combine(Directory, Filename + _extention);
            _Arguments = string.Format(_Arguments, myPythonApp, tempPath, _OutputFile, Pages, FileType, Line_overlap, Char_margin, Line_margin, Word_margin, Boxes_flow, Passwd);
            if (string.IsNullOrEmpty(_OutputFile))
            {
                _OutputFile = CommonProgram.GetZappyTempFile();
            }

                        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo();
            Process myProcess = new Process();
            myProcessStartInfo.FileName = PythonExePath;
            myProcessStartInfo.Arguments = _Arguments;
                        myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.RedirectStandardError = true;
                        myProcess.StartInfo = myProcessStartInfo;
            myProcess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            myProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                        myProcess.Start();
            myProcess.BeginOutputReadLine();
            myProcess.BeginErrorReadLine();
            myProcess.WaitForExit();
            CommandText = ConsoleOutput.ToString();
            OutputFilePath = _OutputFile;
            myProcess.Close();
        }


                                                private StringBuilder ConsoleOutput;
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutput.Append(outLine.Data);
                                }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " InputFilePath:" + this.InputFilePath + "Text:" + this.CommandText + " Pages:" + this.Pages;

        }
    }
}

