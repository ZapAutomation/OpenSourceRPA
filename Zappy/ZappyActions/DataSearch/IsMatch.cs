using System.ComponentModel;
using System.Text.RegularExpressions;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;

namespace Zappy.ZappyActions.DataSearch
{
    [Description("Indicates whether the specified regular expression finds a match in the specified input string, using the specified matching options")]
    public class IsMatch : DecisionNodeAction
    {
        public IsMatch()
        {
            this.RegexOption = RegexOptions.IgnoreCase | RegexOptions.Compiled;
        }

        [Category("Input")]
        [Description("The regular expression pattern to match")]
        public DynamicProperty<string> Pattern { get; set; }

        [Category("Input")]
        [Description("The string to search for a match")]
        public DynamicProperty<string> Input { get; set; }

        [Category("Input")]
        [Description("A bitwise combination of the enumeration values that provide options for matching")]
        public RegexOptions RegexOption { get; set; }

                public override bool Execute(IZappyExecutionContext context)
        {
            return Regex.IsMatch(this.Input, this.Pattern, this.RegexOption);
                    }
    }
}
