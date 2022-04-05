using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Miscellaneous.Helper;

namespace Zappy.ZappyActions.DataSearch
{
    public abstract class DataSearchBase : TemplateAction
    {
        public DataSearchBase()
        {
            ComparisonOption = StringComparison.CurrentCultureIgnoreCase;
            DataSearchOrientation = StringDataSearchOrentiation.Right;
            ResultTextPosition = DataSearchResultPosition.AllWords;
            ResultValueSpiltChar = ' ';
        }

        [Category("Input")]
        [Description("Orientation for searching the string ")]
        public StringDataSearchOrentiation DataSearchOrientation { get; set; }

        [Category("Optional")]
        public StringComparison ComparisonOption { get; set; }

        [Category("Optional")]
        public DataSearchResultPosition ResultTextPosition { get; set; }

        [Category("Input")]
        [Description("Source string")]
        public DynamicProperty<string> SourceText { get; set; }

        [Category("Optional")]
        [Description("How you want to split the matched text? Only required when getting single word from the matched text - " +
                     "SingleWordFirst, SingleWOrdLast and Horizontal offset")]
        public DynamicProperty<char> ResultValueSpiltChar { get; set; }

        [Category("Optional")]
        [Description("Only works when DataSearchOrientation is CustomOffsetVertical")]
        public int CustomOffsetValueVertical { get; set; }

        [Category("Optional")]
        [Description("Specify the number horizontally for the search string. Only works when DataSearchResultPosition is CustomOffsetHorizontal")]
        public int CustomOffsetValueHorizontal { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Result Value")]
        public string Result { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Line form the source text where the search string found")]
        public string MatchedLine { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Check if a match is found")]
        public bool FoundMatch { get; set; }

    }
}