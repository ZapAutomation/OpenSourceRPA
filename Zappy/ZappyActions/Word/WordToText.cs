using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Word
{
    public class WordToText : TemplateAction
    {
        [Category("Input")]
        [Description("Word(.docx/.doc) file path")]
        public DynamicProperty<string> FilePath { get; set; }

        [Category("Output")]
        [Description("Text of the document")]
        public string Text { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            using (DocX document = DocX.Load(FilePath))
            {
                Text = document.Text;
                            }
        }
 
                                
    }
}
