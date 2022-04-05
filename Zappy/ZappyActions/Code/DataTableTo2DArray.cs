using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                            
    [Description("Gets Data In 2DArray From DataTable")]
    public class DataTableTo2DArray : TemplateAction
    {
                                [Category("Input")]
        [Description("Input DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                [Category("Output"), XmlIgnore]
        [Description("Stores array values from DataTable")]
        public DataRow[] DataRowArray { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            EnumerableRowCollection<DataRow> tableEnumerable = DataTable.Value.AsEnumerable();
            DataRow[] tableArray = tableEnumerable.ToArray();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input DataTable" + this.DataTable + " Output Data in Row Array:" + this.DataRowArray;
        }
    }
}