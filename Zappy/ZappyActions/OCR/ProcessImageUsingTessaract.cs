using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Zappy.Helpers;

namespace Zappy.ZappyActions.OCR
{
    public class ProcessImageUsingTessaract
    {
        public string ProcessBitmapImage(System.Drawing.Image img, string Language)
        {
            string imgSavePath = Path.Combine(CrapyConstants.TempFolder, "temp.tmp");
            img.Save(imgSavePath, ImageFormat.Tiff);
                                    string OcrOutputText = ProcessFileExternalTesseractEngine(imgSavePath, Language);
            File.Delete(imgSavePath);
            return OcrOutputText;
        }

        public string ProcessFileExternalTesseractEngine(string InputFilePath,
            string Language, int segmentation = 3, string outputfilepath = null, string AdditionalArgument = "")
        {
            bool deleteFile = true;
            string _TempFilePath;
            if (!string.IsNullOrWhiteSpace(outputfilepath))
            {
                deleteFile = false;
                _TempFilePath = outputfilepath;
            }
            else
            {
                _TempFilePath = CommonProgram.GetZappyTempFile();
            }
            string _Arguments = " \"{0}\" \"{1}\"";
            _Arguments = _Arguments + " --psm " + segmentation + " -l " + Language + " " + AdditionalArgument;
            string OcrOutputText = string.Empty;
            _Arguments = string.Format(_Arguments, InputFilePath, _TempFilePath);

            runTessractProcess( _Arguments);
            string OcrFilePath = _TempFilePath + ".txt";
            if (File.Exists(OcrFilePath))
            {
                OcrOutputText = File.ReadAllText(OcrFilePath);
                if (!string.IsNullOrEmpty(OcrOutputText))
                    OcrOutputText = OcrOutputText.Remove(OcrOutputText.Length - 1);
                if (deleteFile)
                    File.Delete(OcrFilePath);
            }
            OcrOutputText = OcrOutputText.Replace("\n", Environment.NewLine);
            return OcrOutputText;
        }


        public string GeneratePDFExternalTesseractEngine(string InputFilePath,
            string Language, string outputfilepath, string AdditionalArgument = "")
        {
            if (string.IsNullOrEmpty(outputfilepath))
                outputfilepath = InputFilePath;
            string _Arguments = " \"{0}\" \"{1}\"";
            _Arguments = _Arguments + " -l " + Language + " pdf " + AdditionalArgument;
            _Arguments = string.Format(_Arguments, InputFilePath, outputfilepath);
            runTessractProcess(_Arguments);
            outputfilepath = outputfilepath + ".pdf";
            return outputfilepath;
        }


        private void runTessractProcess(string _Arguments)
        {
            ConsoleOutputBuilder = new StringBuilder();
            string _AppPath = Path.Combine(CrapyConstants.TesseractFolder, "tesseract.exe");
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
            }
            else
            {
                throw new Exception("Tesseract Exe Not found - ERROR Performing OCR");
            }
        }

        public string ConsoleOutput;

        private StringBuilder ConsoleOutputBuilder;
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
                ConsoleOutputBuilder.Append(outLine.Data);
        }
    }
}