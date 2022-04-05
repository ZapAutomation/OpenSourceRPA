extern alias itextsharp;
using System;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using PdfReader = itextsharp::iTextSharp.text.pdf.PdfReader;

namespace Zappy.ZappyActions.PDF
{
    extern alias itextsharp;

    [Description("Merge The PDF Files")]
    public class MergePdf : TemplateAction
    {
        [Category("Input")]
        [Description("PDF files array")]
        public DynamicProperty<string[]> InputFiles { get; set; }

        [Category("Input")]
        [Description("Merged Pdf file path " +
                     "- creates output in same folder if kept blank")]
        public DynamicProperty<string> OutputFilePath { get; set; }

                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            if (string.IsNullOrEmpty(OutputFilePath))
            {
                OutputFilePath = Path.Combine(Path.GetDirectoryName(InputFiles.Value[0]), Guid.NewGuid() + ".pdf");
            }
            using (FileStream stream = new FileStream(OutputFilePath, FileMode.Create))
            {
                itextsharp::iTextSharp.text.Document document = new itextsharp::iTextSharp.text.Document();
                itextsharp::iTextSharp.text.pdf.PdfCopy pdf = new itextsharp::iTextSharp.text.pdf.PdfCopy(document, stream);
                itextsharp::iTextSharp.text.pdf.PdfReader reader = null;
                PdfReader.unethicalreading = true;
                try
                {
                    document.Open();
                    foreach (string file in InputFiles.Value)
                    {
                        reader = new itextsharp::iTextSharp.text.pdf.PdfReader(file);
                        pdf.AddDocument(reader);
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }
        }

                                                                public override string AuditInfo()
        {
            return base.AuditInfo() + " InputFiles:" + this.InputFiles + " OutputFile:" + this.OutputFilePath;
        }
    }
}
