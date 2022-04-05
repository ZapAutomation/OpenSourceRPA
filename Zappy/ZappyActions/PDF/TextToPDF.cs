extern alias itextsharp;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using System.ComponentModel;
using itextsharp::iTextSharp.text.pdf;

namespace Zappy.ZappyActions.PDF
{
    [Description("Convert Text file into PDF file")]
    public class TextToPdf : TemplateAction
    {
        [Category("Input")]
        [Description("Enter Text File path")]
        public DynamicProperty<string> TextFilePath { get; set; }

        [Category("Optional")]
        [Description("Enter PDF File Path")]
        public DynamicProperty<string> PDFPath { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            StreamReader rdr = new StreamReader(TextFilePath);
            itextsharp::iTextSharp.text.Document doc = new itextsharp::iTextSharp.text.Document();
            string _PDFPath = string.Empty;
            if (string.IsNullOrEmpty(PDFPath))
            {
                string TextFileName = Path.GetFileName(TextFilePath);
                string PDFFileName = TextFileName.Replace("txt", "pdf");
                _PDFPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(TextFilePath), PDFFileName));
            }
            else
            {
                _PDFPath = PDFPath;
            }
            PdfWriter.GetInstance(doc, new FileStream(_PDFPath, FileMode.Create));
            doc.Open();
            doc.Add(new itextsharp::iTextSharp.text.Paragraph(rdr.ReadToEnd()));
            doc.Close();

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " TextFilePath:" + this.TextFilePath + " PdfFilePath:" + this.PDFPath;
        }

    }

}