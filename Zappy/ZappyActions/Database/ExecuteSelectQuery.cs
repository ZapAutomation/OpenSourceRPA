using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Database.Helper;

namespace Zappy.ZappyActions.Database
{
    [Description("Execute Select query")]
    public  class ExecuteSelectQuery : TemplateAction
    {

        [Category("Input")]
        [Description("Enter Select Query")]
        public DynamicProperty<string> SqlQuery { get; set; }

        [Category("Input")]
        [Description("Enter Connection String")]
        public DynamicProperty<string> ConnectionString { get; set; }


        [Category("Output")]
        [Description("Return data table")]
        public DynamicProperty<DataTable> DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DataTable = Query();
        }

        DataTable Query()
        {
            SqlConnection cnn;
            SqlDataAdapter sqlAdp = default(SqlDataAdapter);
            DataTable dt = new DataTable();
            cnn = new SqlConnection(ConnectionString);
            cnn.Open();
            sqlAdp = new SqlDataAdapter(SqlQuery, cnn);
            cnn.Close();
            sqlAdp.Fill(dt);
            return dt;
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " SQLQuery:" + this.SqlQuery + " Connection String" + this.ConnectionString+ " Output" + this.DataTable;
        }
    }
}
