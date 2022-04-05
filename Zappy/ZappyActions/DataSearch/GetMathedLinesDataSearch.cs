using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.DataSearch
{
    [Description("Search Value Accross Multiple Lines")]
    public class GetMathedLinesDataSearch : TemplateAction
    {
        public GetMathedLinesDataSearch()
        {
            SearchText = new DynamicProperty<string>();
            SearchText.Value = "";
        }

                                        [Category("Input")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public DynamicProperty<string> SearchText { get; set; }

        [Category("Input")]
        [Description("Source string")]
        public DynamicProperty<string> SourceText { get; set; }

        [Category("Output")]
        [Description("Return multiple line according to search text")]
        public DynamicProperty<string[]> MatchedLines { get; set; }
                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            List<string> matchingLines = new List<string>();
            string[] Seperator = new[] { Environment.NewLine, "\n" };

            string[] StringArray = SourceText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            string[] SearchTextArray = SearchText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < StringArray.Length; i++)
            {
                {
                    for (int j = 0; j < SearchTextArray.Length; j++)
                    {
                        int indexMatch = StringArray[i].IndexOf(SearchTextArray[j], StringComparison.CurrentCultureIgnoreCase);
                        if (indexMatch >= 0)
                        {
                            matchingLines.Add(StringArray[i]);
                        }

                    }
                }
            }
            MatchedLines = matchingLines.ToArray();
        }     
    }
}