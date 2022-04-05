extern alias itextsharp;
using itextsharp::iTextSharp.text.pdf;
using System;
using System.ComponentModel;
using System.IO;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.PDF
{
    [Description("Enable Reading Of Protected PDF")]
    public class EnableReadingProtectedPDF : TemplateAction
    {
        [Category("Input")]
        [Description("PDF file path")]
        public DynamicProperty<string> Path { get; set; }


                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            PdfReader.unethicalreading = true;
            string tempPDfFile = System.IO.Path.Combine(CrapyConstants.TempFolder, "temp.pdf");
            File.Copy(Path, tempPDfFile);
            PdfReader reader = new itextsharp::iTextSharp.text.pdf.PdfReader(tempPDfFile);

            using (FileStream stream = new FileStream(Path, FileMode.Create))
            {
                itextsharp::iTextSharp.text.Document document = new itextsharp::iTextSharp.text.Document();
                itextsharp::iTextSharp.text.pdf.PdfCopy pdf =
                    new itextsharp::iTextSharp.text.pdf.PdfCopy(document, stream);
                try
                {
                    document.Open();

                    pdf.AddDocument(reader);
                    reader.Close();

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

            File.Delete(tempPDfFile);
        }

                                                                public override string AuditInfo()
        {
            return base.AuditInfo() + " Path:" + this.Path;
        }
    }
}