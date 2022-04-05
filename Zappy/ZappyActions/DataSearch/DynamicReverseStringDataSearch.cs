using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Miscellaneous.Helper;

namespace Zappy.ZappyActions.DataSearch
{
   public class DynamicReverseStringDataSearch : TemplateAction
    {
        [Description("Searches a string in reverse order for a specified value, and returns the value of the match")]
        public DynamicReverseStringDataSearch()
        {
            ComparisonOption = StringComparison.CurrentCultureIgnoreCase;
            DataSearchOrientation = "Right";
            ResultTextPosition = "AllWords";
            ResultValueSpiltChar = ' ';
            SearchText = new DynamicProperty<string>();
            SearchText.Value = "";
        }


        [Category("Input")]
        [Description("Orientation for searching the string - Right,Left,Bottom,Top,CustomOffsetVertical")]
        public DynamicProperty<string> DataSearchOrientation { get; set; }

        [Category("Optional")]
        [Description("Orientation for searching the string - " +
                     "CustomOffsetHorizontal,SingleWordFirst,SingleWordLast, AllWords")]
        public StringComparison ComparisonOption { get; set; }

        [Category("Optional")]
        public DynamicProperty<string> ResultTextPosition { get; set; }

        [Category("Input")]
        [Description("Source string")]
        public DynamicProperty<string> SourceText { get; set; }

        [Category("Optional")]
        [Description("How you want to split the matched text? Only required when getting single word from the matched text - " +
                     "SingleWordFirst, SingleWOrdLast and Horizontal offset")]
        public DynamicProperty<char> ResultValueSpiltChar { get; set; }

        [Category("Optional")]
        [Description("Only works when DataSearchOrientation is CustomOffsetVertical")]
        public DynamicProperty<int> CustomOffsetValueVertical { get; set; }

        [Category("Optional")]
        [Description("Specify the number horizontally for the search string. Only works when DataSearchResultPosition is CustomOffsetHorizontal")]
        public DynamicProperty<int> CustomOffsetValueHorizontal { get; set; }
                                        [Category("Input")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public DynamicProperty<string> SearchText { get; set; }

        [Category("Optional")]
        [Description("Leave blank if no parent to be matched")]
        public DynamicProperty<string> ParentMatch { get; set; }
        [Category("Optional")]
        [Description("True, if wants to save result in clipboard")]
        public DynamicProperty<bool> SaveResultToClipboard { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Result Value")]
        public string Result { get; set; }

        [Category("Output")]
        [Description("Line form the source text where the search string found")]
        public string MatchedLine { get; set; }

        [Category("Output")]
        [Description("Check if a match is found")]
        public bool FoundMatch { get; set; }

        [Category("Output")]
        public DynamicProperty<bool> IsResultNullEmptyOrWhitespace { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Result = string.Empty;
            int indexMatch = 0;
            IsResultNullEmptyOrWhitespace = false;
            FoundMatch = false;
            string[] Seperator = new[] { Environment.NewLine, "\n" };

            string[] StringArray = SourceText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            string[] SearchTextArray = SearchText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            bool foundParentText = false;
                        for (int i = StringArray.Length-1; i >=0; i--)
            {
                if (!string.IsNullOrWhiteSpace(ParentMatch) && !foundParentText)
                {
                    indexMatch = StringArray[i].IndexOf(ParentMatch, ComparisonOption);
                    if (indexMatch >= 0)
                    {
                        foundParentText = true;
                    }
                }
                else
                {
                    for (int j = 0; j < SearchTextArray.Length; j++)
                    {
                        FoundMatch = MatchString(i, StringArray, SearchTextArray[j]);
                        if (FoundMatch)
                        {
                            if (SaveResultToClipboard)
                                CommonProgram.SetTextInClipboard(Result);
                            if (string.IsNullOrWhiteSpace(Result))
                            {
                                IsResultNullEmptyOrWhitespace = true;
                            }
                            return;
                        }
                    }
                }
            }
            if (SaveResultToClipboard)
                CommonProgram.SetTextInClipboard(Result);
        }

        public bool MatchString(int i, string[] StringArray, string textToSearch)
        {
            int indexMatch = StringArray[i].IndexOf(textToSearch, ComparisonOption);
            if (indexMatch >= 0)
            {
                MatchedLine = StringArray[i];
                string ResultValue = null;
                                if(string.Equals(DataSearchOrientation, "Right", StringComparison.OrdinalIgnoreCase))
                {
                    ResultValue = StringArray[i].Substring(indexMatch + textToSearch.Length);
                }
                                else if(string.Equals(DataSearchOrientation, "Left", StringComparison.OrdinalIgnoreCase))
                {
                    ResultValue = StringArray[i].Substring(0, indexMatch);
                }
                                else if (string.Equals(DataSearchOrientation, "Bottom", StringComparison.OrdinalIgnoreCase))
                {
                    ResultValue = StringArray[i + 1];
                }
                                else if (string.Equals(DataSearchOrientation, "Top", StringComparison.OrdinalIgnoreCase))
                {
                    ResultValue = StringArray[i - 1];
                }
                               else if (string.Equals(DataSearchOrientation, "CustomOffsetVertical", StringComparison.OrdinalIgnoreCase))
                {
                    ResultValue = StringArray[i - CustomOffsetValueVertical];
                }

                ResultValue = ResultValue.Trim(' ');
                                                if (string.Equals(ResultTextPosition, "CustomOffsetHorizontal", StringComparison.OrdinalIgnoreCase))
                {
                    string[] splitResult = ResultValue.Split(ResultValueSpiltChar);
                    Result = splitResult[CustomOffsetValueHorizontal];
                }
                                else if (string.Equals(ResultTextPosition, "SingleWordFirst", StringComparison.OrdinalIgnoreCase))
                {
                    string[] splitResult = ResultValue.Split(ResultValueSpiltChar);
                    Result = splitResult[0];
                }
                                else if (string.Equals(ResultTextPosition, "SingleWordLast", StringComparison.OrdinalIgnoreCase))
                {
                    string[] splitResult = ResultValue.Split(ResultValueSpiltChar);
                    Result = splitResult[splitResult.Length - 1];
                }
                else
                {
                    Result = ResultValue;
                }
                                return true;
            }

            return false;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " SearchText: " + this.SearchText + " ParentMatch: " + this.ParentMatch + " ResultValueSplitChar: " +
                this.ResultValueSpiltChar + " Result: " + this.Result + " Matched: " + this.MatchedLine;
        }
    }
}
