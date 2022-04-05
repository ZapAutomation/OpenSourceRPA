extern alias itextsharp;
using System;
using itextsharp::iTextSharp.text.pdf;
using itextsharp::iTextSharp.text.pdf.parser;
using System.ComponentModel;
using System.Text;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using System.Collections.Generic;

namespace Zappy.ZappyActions.PDF
{
    [Description("Extract text data from PDF file")]
    public class PDFToTextSharp : TemplateAction
    {
        public enum ExtractionStrategy
        {
            LocationTextExtractionStrategy,
            SimpleTextExtractionStrategy
        }
        public PDFToTextSharp()
        {

        }
        [Category("Input")]
        [Description("PDF file path")]
        public DynamicProperty<string> InputFilePath { get; set; }
        [Category("Input")]
        [Description("Check ExtractionStrategy of text")]
        public ExtractionStrategy Strategy { get; set; }
        [Category("Output")]
        [Description("Texts in array")]
        public string OutputString { get; set; }

        [Category("Output")]
        [Description("Texts in array")]
        public string[] PageStringArray { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutputString = ExtractTextFromPdf(InputFilePath);
            OutputString = OutputString.Replace("\n", Environment.NewLine);
        }

                                                                public string ExtractTextFromPdf(string path)
        {
            ITextExtractionStrategy its = new LocationTextExtractionStrategy();

                                    if (Strategy == ExtractionStrategy.SimpleTextExtractionStrategy)
                its = new SimpleTextExtractionStrategy();
            
            string data = String.Empty;
            List<string> PageText = new List<string>();
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);
                    text.Append(thePage);
                    PageText.Add(thePage);
                }
                PageStringArray = PageText.ToArray();
                return text.ToString();
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " InputFilePath:" + this.InputFilePath + " Output:" + this.OutputString;
        }

    }
}