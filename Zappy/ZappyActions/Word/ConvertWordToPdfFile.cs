using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Application = Microsoft.Office.Interop.Word.Application;

namespace Zappy.ZappyActions.Word
{
    [Description("Convert word document to pdf")]
    public class ConvertWordToPdfFile : TemplateAction
    {
        [Category("Input")]
        [Description("PDF file path to convert into Word document")]
        public DynamicProperty<string> PdfFilePath { get; set; }

        [Category("Optional")]
        [Description("File path to save converted Word file")]
        public DynamicProperty<string> WordFilePath { get; set; }
       

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            CreatePDF(WordFilePath, PdfFilePath);
        }
        public string CreatePDF(string wordfilePath, string pdfPath)
        {
            Application app = new Application();
            app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            app.Visible = false;

            var objPresSet = app.Documents;
            var objPres = objPresSet.Open(wordfilePath, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            string _pdfPath = null;
            string ext = Path.GetExtension(pdfPath);
            if (string.IsNullOrEmpty(ext))
            {
                string WordFileName = Path.GetFileName(wordfilePath);
                string PdfFileName = WordFileName.Replace("docx", "pdf");
                _pdfPath = Path.GetFullPath(Path.Combine(pdfPath, PdfFileName));
            }
            else
            {
                _pdfPath = pdfPath;
            }

            try
            {
                objPres.ExportAsFixedFormat(
                    _pdfPath,
                    WdExportFormat.wdExportFormatPDF,
                    false,
                    WdExportOptimizeFor.wdExportOptimizeForPrint,
                    WdExportRange.wdExportAllDocument
                );
            }
            catch
            {
                pdfPath = null;
            }
            finally
            {
                objPres.Close();
                app.Quit();
            }
            return pdfPath;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WordFilePath:" + this.WordFilePath + " PdfFilePath:" + this.PdfFilePath;
        }
    }
}
