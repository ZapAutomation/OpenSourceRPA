using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Web
{
    [Description("Gets Text/layout from the html code")]
    public class ConvertHtmlToText : TemplateAction
    {
        public ConvertHtmlToText()
        {
            UseAgilityPack = true;
        }

        [Category("Input")]
        [Description("Html code")]
        public DynamicProperty<string> HtmlString { get; set; }

        [Category("Optional")]
        [Description("Html code")]
        public DynamicProperty<bool> UseAgilityPack { get; set; }

        [Category("Output")]
        [Description("Text/Layout of the html code")]
        public string TextContent { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (UseAgilityPack)
            {
                                                                                                                                TextContent = HtmlToPlainText(HtmlString);
            }
            else
            {
                                                TextContent = StripHtml(HtmlString);
                            }
                                                                                }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";            const string stripFormatting = @"<[^>]*(>|$)";            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
                        text = System.Net.WebUtility.HtmlDecode(text);
                        text = tagWhiteSpaceRegex.Replace(text, "><");
                        text = lineBreakRegex.Replace(text, Environment.NewLine);
                        text = stripFormattingRegex.Replace(text, " ");

            text = string.Join(" ", text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            return text;
        }
        private string StripHtml(string source)
        {
            string output;

                        output = Regex.Replace(source, "<[^>]*>", string.Empty);

                        output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

            return output;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}