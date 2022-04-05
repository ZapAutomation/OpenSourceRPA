using System;
using System.ComponentModel;
using System.Data;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                        
    [Description("Gets DataTable From StringArray")]
    public class StringArrayToDataTable : TemplateAction
    {
        public StringArrayToDataTable()
        {
            Seperator = new[] { " " };
            StringSplitoptions = StringSplitOptions.RemoveEmptyEntries;
            RemoveWhiteSpace = true;
        }

                                [Category("Input")]
        [Description("Input string array")]
        public DynamicProperty<string[]> InputStringArray { get; set; }

                                [Category("Input")]
        [Description("Removes all whitespace in string")]
        public DynamicProperty<bool> RemoveWhiteSpace { get; set; }

                                                [Category("Input")]
        [Description("Splits the string")]
        public StringSplitOptions StringSplitoptions { get; set; }

                                [Category("Input"), DefaultValue(" ")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Seperator of string")]
        public string[] Seperator { get; set; }

                                [Category("Output")]
        [Description("Gets DataTable")]
        public DataTable DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DataTable = new DataTable();

                        int maxNoOfCloumns = 0;
            foreach (string inputString in InputStringArray.Value)
            {
                bool valid = false;
                if (RemoveWhiteSpace)
                {
                    valid = !string.IsNullOrWhiteSpace(inputString);
                }
                else
                {
                    valid = !string.IsNullOrEmpty(inputString);
                }

                if (valid)
                {
                    string[] splitString = inputString.Split(Seperator, StringSplitoptions);
                                                            
                    for (int i = 0; i < splitString.Length - maxNoOfCloumns; i++)
                    {

                        DataTable.Columns.Add("Column" + (i + maxNoOfCloumns));
                    }

                    if (maxNoOfCloumns < splitString.Length)
                        maxNoOfCloumns = splitString.Length;

                    DataTable.Rows.Add(splitString);
                }
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input String:" + this.InputStringArray + " Output:" + this.DataTable;
        }
    }
}