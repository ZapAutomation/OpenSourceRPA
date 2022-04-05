using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Documents
{
    [Description("Data Extraction Using Regular Expression")]
    public class DataExtraction : TemplateDocumentAction
    {
        public enum DataPosition
        {
            Bottom,
            Right,
            Top
        }

        [Description("Extracting information from any text by searching for one or more matches of a specific search pattern(i.e. a specific sequence of ASCII or unicode characters).")]
        [Category("Input")]
        public DynamicProperty<string> RegexPattern { get; set; }

        [Description(" Select Position of data")]
        [Category("Input")]
        public DataPosition Position { get; set; }

        [Description("Sets the Split Delimiter")]
        [Category("Input")]
        public DynamicProperty<string[]> SplitDelimiter { get; set; }

                                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            string[] splitArray = InputData.Value.Split(SplitDelimiter.Value, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitArray.Length; i++)
            {
                if (Regex.Match(splitArray[i], RegexPattern).Success)
                {
                    if (Position == DataPosition.Bottom)
                        OutputData = splitArray[i + 1];
                    else if (Position == DataPosition.Top)
                        OutputData = splitArray[i - 1];
                    else if (Position == DataPosition.Right)
                        OutputData =
                            splitArray[i].Replace(Regex.Match(splitArray[i], RegexPattern).Value, "");

                }

            }

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " RegexPattern:" + this.RegexPattern + " Position:" + this.Position;
        }
    }
}
