using System.ComponentModel;
using Xceed.Words.NET;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Word
{
    public class ChangeWordFormating : TemplateAction
    {
        [Category("Input")]
        [Description("File path of document that you want to apply formatting")]
        public DynamicProperty<string> FilePath { get; set; }

        [Category("Input")]
        [Description("Title of the file")]
        public DynamicProperty<string> Title { get; set; }

        [Category("Input")]
        [Description("Subtitle of the file")]
        public DynamicProperty<string> Subtitle { get; set; }

        [Category("Input")]
        [Description("Body of the doc file")]
        public DynamicProperty<string> Body { get; set; }

        [Category("Input/Output")]
        [Description("File path where you want to save the formatted document.")]
        public DynamicProperty<string> OutputFilePath { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> Bold { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> Italic { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string fileName = FilePath;
            string headlineText = Title;
            string namsespace = Subtitle;
            string paraOne = Body;

            using (DocX document = DocX.Load(FilePath))
            {
                                var headLineFormat = new Formatting();
                headLineFormat.FontFamily = new Xceed.Words.NET.Font("Calibri (Body)");
                headLineFormat.Size = 18D;
                                headLineFormat.Bold = true;
                                                
                                var name = new Formatting();
                name.FontFamily = new Xceed.Words.NET.Font("Calibri (Body)");
                name.Size = 12D;
                name.Italic = Italic;
                name.Bold = Bold;

                                var paraFormat = new Formatting();
                paraFormat.FontFamily = new Xceed.Words.NET.Font("Calibri (Body)");
                paraFormat.Size = 12D;

                                var doc = DocX.Create(OutputFilePath, DocumentTypes.Document);

                                doc.InsertParagraph(headlineText, false, headLineFormat);
                doc.InsertParagraph(namsespace, false, name);
                doc.InsertParagraph("\n", false, name);
                doc.InsertParagraph(paraOne, false, paraFormat);
                                
                                doc.Save();

            }
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " Title:" + this.Title + " Subtitle:" + this.Subtitle + " Body:" + this.Body + " OutputFilePath:" + this.OutputFilePath;
        }
    }
}

