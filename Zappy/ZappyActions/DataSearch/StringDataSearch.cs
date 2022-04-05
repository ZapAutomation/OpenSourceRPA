using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Miscellaneous.Helper;

namespace Zappy.ZappyActions.DataSearch
{
    [Description("Searches a string for a specified value, and returns the value of the match")]
    public class StringDataSearch : DataSearchBase
    {
        public StringDataSearch():base()
        {
            SearchText = new DynamicProperty<string>();
            SearchText.Value = "";
        }

                                        [Category("Input")]
        [Description("Search string to get required text - multiple search value seperated on new lines")]
        public DynamicProperty<string> SearchText { get; set; }

        [Category("Optional")]
        [Description("Leave blank if no parent to be matched")]
        public DynamicProperty<string> ParentMatch { get; set; }
        [Category("Optional")]
        [Description("True, if wants to save result in clipboard")]
        public DynamicProperty<bool> SaveResultToClipboard { get; set; }
        [Category("Output")]
        public DynamicProperty<bool> IsResultNullEmptyOrWhitespace { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Result = string.Empty;
            IsResultNullEmptyOrWhitespace = false;
            FoundMatch = false;
            string[] Seperator = new[] { Environment.NewLine, "\n" };

            string[] StringArray = SourceText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            string[] SearchTextArray = SearchText.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            bool foundParentText = false;
            for (int i = 0; i < StringArray.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(ParentMatch) && !foundParentText)
                {
                    int indexMatch = StringArray[i].IndexOf(ParentMatch, ComparisonOption);
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
                            if(string.IsNullOrWhiteSpace(Result))
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
                if (DataSearchOrientation == StringDataSearchOrentiation.Right)
                {
                    ResultValue = StringArray[i].Substring(indexMatch + textToSearch.Length);
                }
                else if (DataSearchOrientation == StringDataSearchOrentiation.Left)
                {
                    ResultValue = StringArray[i].Substring(0, indexMatch);
                }
                else if (DataSearchOrientation == StringDataSearchOrentiation.Bottom)
                {
                    ResultValue = StringArray[i + 1];
                }
                else if (DataSearchOrientation == StringDataSearchOrentiation.Top)
                {
                    ResultValue = StringArray[i - 1];
                }
                else if (DataSearchOrientation == StringDataSearchOrentiation.CustomOffsetVertical)
                {
                    ResultValue = StringArray[i + CustomOffsetValueVertical];
                }

                ResultValue = ResultValue.Trim(' ');
                                if (ResultTextPosition == DataSearchResultPosition.CustomOffsetHorizontal)
                {
                    string[] splitResult = ResultValue.Split(ResultValueSpiltChar);
                    Result = splitResult[CustomOffsetValueHorizontal];
                }
                else if (ResultTextPosition == DataSearchResultPosition.SingleWordFirst)
                {
                    string[] splitResult = ResultValue.Split(ResultValueSpiltChar);
                    Result = splitResult[0];
                }
                else if (ResultTextPosition == DataSearchResultPosition.SingleWordLast)
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