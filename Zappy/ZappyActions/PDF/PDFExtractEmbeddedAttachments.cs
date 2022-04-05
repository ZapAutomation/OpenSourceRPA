extern alias itextsharp;
using itextsharp::iTextSharp.text.pdf;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.PDF
{
    [Description("PDF Extract Embedded Attachments")]
    public class PDFExtractEmbeddedAttachments : TemplateAction
    {


        [Category("Input")]
        [Description("PDF file path")]
        public DynamicProperty<string> InputFilePath { get; set; }
        [Category("Input")]
        [Description("Folder To Save Attachment")]
        public DynamicProperty<string> AttachmentsFolder { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExtractAttachments(InputFilePath, AttachmentsFolder);
        }

        internal void ExtractAttachments(string file_name, string folderName)
        {
            PdfDictionary documentNames = null;
            PdfDictionary embeddedFiles = null;
            PdfDictionary fileArray = null;
            PdfDictionary file = null;
            PRStream stream = null;

            PdfReader reader = new PdfReader(file_name);
            PdfDictionary catalog = reader.Catalog;

            documentNames = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.NAMES));

            if (documentNames != null)
            {
                embeddedFiles = (PdfDictionary)PdfReader.GetPdfObject(documentNames.Get(PdfName.EMBEDDEDFILES));
                if (embeddedFiles != null)
                {
                    PdfArray filespecs = embeddedFiles.GetAsArray(PdfName.NAMES);

                    for (int i = 0; i < filespecs.Size; i++)
                    {
                        i++;
                        fileArray = filespecs.GetAsDict(i);
                        file = fileArray.GetAsDict(PdfName.EF);

                        foreach (PdfName key in file.Keys)
                        {
                            stream = (PRStream)PdfReader.GetPdfObject(file.GetAsIndirectObject(key));
                            string attachedFileName = fileArray.GetAsString(key).ToString();
                            byte[] attachedFileBytes = PdfReader.GetStreamBytes(stream);

                            System.IO.File.WriteAllBytes(Path.Combine(AttachmentsFolder, attachedFileName), attachedFileBytes);
                        }

                    }
                }
            }

        }

    }
}