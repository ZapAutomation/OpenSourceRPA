using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel
{
    public abstract class ExcelBaseAction : TemplateAction
    {
        [Category("Input")]
        [Description("Excel SheetName for finding Text")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }
      
    }
}