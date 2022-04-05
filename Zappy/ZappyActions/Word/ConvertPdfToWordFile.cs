using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Word
{
    [Description("Convert PDF document into word")]
    public class ConvertPdfToWordFile : TemplateAction
    {
        [Category("Input")]
        [Description("Enter PDF File path")]
        public DynamicProperty<string> PdfFilePath { get; set; }

        [Category("Optional")]
        [Description("Enter word file path to save converted PDF file")]
        public DynamicProperty<string> WordFilePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            CreateWordDoc(WordFilePath, PdfFilePath);
        }

        public string CreateWordDoc(string wordpath, string pdfPath)
        {
            Application app = new Application();
            app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            app.Visible = false;
            var objPresSet = app.Documents;
            WordPdfImportWarningRemover wordPdfImportWarningRemover = new WordPdfImportWarningRemover();
            wordPdfImportWarningRemover.EditRegistry();

            var objPres =
                objPresSet.Open(pdfPath, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoFalse);

            string _wordPath = string.Empty;
            if (string.IsNullOrEmpty(wordpath))
            {
                string pdfFileName = Path.GetFileName(pdfPath);
                string wordFileName = pdfFileName.Replace("pdf", "docx");
                _wordPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(pdfPath), wordFileName));
            }
            else
            {
                _wordPath = wordpath;
            }

            objPres.SaveAs2(_wordPath);
            wordPdfImportWarningRemover.Dispose();
            objPres.Close();
            app.Quit();
            return pdfPath;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WordFilePath:" + this.WordFilePath + " PdfFilePath:" + this.PdfFilePath;
        }
    }
}
