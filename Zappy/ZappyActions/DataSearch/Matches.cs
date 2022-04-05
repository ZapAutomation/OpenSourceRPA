using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.DataSearch
{
    [Description("Regex pattern for searching or manipulating string")]
    public class Matches : TemplateAction
    {
        public Matches()
        {
            this.RegexOption = RegexOptions.IgnoreCase | RegexOptions.Compiled;
        }

        [Category("Input")]
        [Description("Define Regex pattern for searching or manipulating strings")]
        public DynamicProperty<string> Pattern { get; set; }

        [Category("Input")]
        [Description("String Value")]
        public DynamicProperty<string> Input { get; set; }

        [Category("Input")]
        [Description("A bitwise combination of the enumeration values that provide options for matching")]
        public RegexOptions RegexOption { get; set; }

        [Category("Optional")]
        [Description("Set index number 'index start from 0'")]
        public DynamicProperty<int> IndexNumber { get; set; }

        [Category("Output")]
        [XmlIgnore]
        [Description("A collection of the System.Text.RegularExpressions.Match objects found by the search")]
        public MatchCollection MatchCollection { get; set; }

        [Category("Output")]
        [Description("Return the string to search for a match")]
        public string RegexOutput { get; set; }

                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            RegexOutput = String.Empty;
            int i = 1;
            var _MatchCollection = Regex.Matches(this.Input, this.Pattern, this.RegexOption);
            MatchCollection = _MatchCollection;
            foreach (var s in _MatchCollection)
            {
                if (i == IndexNumber)
                {
                    RegexOutput = s.ToString();
                }
                i++;
            }
        }
    }
}