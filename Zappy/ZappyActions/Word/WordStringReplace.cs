using System.ComponentModel;
using Xceed.Words.NET;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Word
{
    public class WordStringReplace : TemplateAction
    {
        [Category("Input")]
        [Description("Word(.docx/.doc) file path")]
        public DynamicProperty<string> FilePath { get; set; }

        [Category("Input")]
        [Description("The string to be replaced")]
        public DynamicProperty<string> OldString { get; set; }

        [Category("Input")]
        [Description("The string to replace all occurrences of OldString")]
        public DynamicProperty<string> NewString { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Replace(FilePath, OldString, NewString);
        }

                private void Replace(string filename, string oldString, string newString)
        {
                        using (DocX document = DocX.Load(filename))
            {
                                document.ReplaceText(oldString, newString);

                                document.Save();
            } 

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " OldString:" + this.OldString + " NewString:" + this.NewString;
        }

    }
}
