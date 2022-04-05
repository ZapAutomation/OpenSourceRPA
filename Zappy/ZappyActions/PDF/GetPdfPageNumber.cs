using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.PDF
{
    public class GetPdfPageNumber : TemplateAction
    {
        public GetPdfPageNumber()
        {
            IgnoreCaseSensitive = true;
            ParentSearchText = "";
            SearchText = "";
            MatchLineNumber = 0;
            PageNumber = 0;
        }
        [Category("Input")]
        [Description("PDF String Array")]
        public DynamicProperty<string[]> PdfStringArray { get; set; }

        [Category("Input")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public DynamicProperty<string> SearchText { get; set; }

        [Category("Optional")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public DynamicProperty<string> ParentSearchText { get; set; }

        [Category("Optional")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public bool IgnoreCaseSensitive { get; set; }

        [Category("Output")]
        [Description("Texts in array")]
        public int PageNumber { get; set; }

        [Category("Output")]
        [Description("Texts in array")]
        public int MatchLineNumber { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Pdfpagenumber(PdfStringArray, ParentSearchText, SearchText);
        }

        public void Pdfpagenumber(string[] StringArraydata, string Parenttext,string ChildText)
        {
           int _pagenumber = 1;
            bool ParentMatch = false;
            string[] Seperator = new[] { Environment.NewLine, "\n" };
            Regex parent;
            Regex Child;
            if (IgnoreCaseSensitive)
            {
                parent = new Regex(Parenttext, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Child = new Regex(ChildText, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            else
            {
                parent = new Regex(Parenttext);
                Child = new Regex(ChildText);
            }
            
            foreach (string a in StringArraydata)
            {
                string[] StringArray = a.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (!ParentMatch && !string.IsNullOrEmpty(Parenttext))
                    {
                        if (parent.IsMatch(StringArray[i]))
                        {
                            ParentMatch = true;
                        }
                    }
                    else
                    {
                        if(Child.IsMatch(StringArray[i]))
                        {
                            MatchLineNumber = ++i;
                            PageNumber= _pagenumber;
                            goto end;
                        }
                    }
                }
                _pagenumber++;
            }
        end:;
        }
    }
}
