using System;
using System.ComponentModel;
using System.Diagnostics;
using Xceed.Words.NET;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Word
{
    [Description("Add Text and Image in existing Word Document")]
    public class AddTextAndImageInWord : TemplateAction
    {
       public AddTextAndImageInWord()
        {
            FilePath = "";
            Text = "";
            ImgFilePath = "";
        }
        [Category("Input")]
        [Description("Enter Existing File path ")]
        public DynamicProperty<string> FilePath { get; set; }


        [Category("Input")]
        [Description("Enter Text/Paragraph")]
        public DynamicProperty<string> Text { get; set; }

        [Category("Optional")]
        [Description("Enter image path which you want to add in word document")]
        public DynamicProperty<string> ImgFilePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string fileName =FilePath;
           
            if (String.IsNullOrEmpty(ImgFilePath))
            {
                var doc = DocX.Load(fileName);
                Paragraph par = doc.InsertParagraph(Text);
                doc.Save();
                         }
            else
            {
            var doc = DocX.Load(fileName);

            var img = doc.AddImage(ImgFilePath);
            var p = img.CreatePicture();

            Paragraph par = doc.InsertParagraph(Text);
            par.AppendPicture(p);

            doc.Save();

           
            }
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " Text:" + this.Text + " ImgPath:" + this.ImgFilePath;
        }
    }
}