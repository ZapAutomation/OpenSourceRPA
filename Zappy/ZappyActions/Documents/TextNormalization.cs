using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Documents
{
    [Description("Normalize Text - Remove EmptyLines and converts multiple spaces to single space")]

    public class TextNormalization : TemplateAction
    {
        public TextNormalization()
        {
            NormalizeSpaces = true;
            ReplaceSpaceString = " ";
        }

        [Category("Input")]
        [Description("Text Value")]
        public DynamicProperty<string> TextValues { get; set; }

        [Category("Optional")]
        [Description("True, if user wants to remove multiple spaces")]
        public DynamicProperty<bool> NormalizeSpaces { get; set; }

        [Category("Optional")]
        [Description("Remove multiple spaces and moves the data with multiple spaces to new line and converts tabs to spaces")]
        public DynamicProperty<string> ReplaceSpaceString { get; set; }

        [Category("Output")]
        [Description("Return the normalize Text")]
        public string NoramalizeText { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            NoramalizeText = Regex.Replace(TextValues, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
            if (NormalizeSpaces)
            {
                NoramalizeText = Regex.Replace(NoramalizeText, @"\t", "  ");
                RegexOptions options = RegexOptions.None;
                Regex regexa = new Regex("[ ]{2,}", options);
                NoramalizeText = regexa.Replace(NoramalizeText, ReplaceSpaceString);
            }
            

        }
    }

}
