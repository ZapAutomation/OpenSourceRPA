using System;
using System.ComponentModel;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;

namespace Zappy.ZappyActions.DataSearch
{
                [Description("Compare File Text")]
    public class CompareText : DecisionNodeAction
    {
                                                        public CompareText()
        {
            ExactMatch = true;
        }

                                [Category("Input")]
        [Description("Actual value")]
        public DynamicProperty<string> ValueSearch { get; set; }

                                                
        [Category("Optional")]
        [Description("True, if value is empty; otherwise,False")]
        public DynamicProperty<bool> ReturnTrueOnEmptyValues { get; set; }

        [Category("Input")]
        [Description("Comparision value")]
        public DynamicProperty<string> ValueCompare { get; set; }

        [Category("Optional")]
        [Description("True, if the string has to match exactly")]
        public DynamicProperty<bool> ExactMatch { get; set; }

        [Category("Optional")]
        [Description("True, if the string is match without checking upper and lower case like 'abc is same as ABC")]
        public DynamicProperty<bool> ExactMatchWithIgnorecase { get; set; }

                        


        public override bool Execute(IZappyExecutionContext context)
        {
            if (ReturnTrueOnEmptyValues)
            {
                if ((string.IsNullOrWhiteSpace(ValueCompare.Value)
                    && string.IsNullOrWhiteSpace(ValueSearch.Value)) || ValueSearch.Value.Equals(ValueCompare.Value))
                    return true;
            }
                                                
            if (ExactMatch && ValueCompare.Value.Equals(ValueSearch.Value))
            {
                return true;
            }
            if (!ExactMatch && (ValueCompare.Value.Contains(ValueSearch.Value) || ValueSearch.Value.Contains(ValueCompare.Value)))
            {
                return true;
            }

                        if (ExactMatchWithIgnorecase && string.Equals(ValueSearch, ValueCompare, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}