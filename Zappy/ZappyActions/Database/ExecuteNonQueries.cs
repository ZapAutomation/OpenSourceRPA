using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Database
{
    [Description("Execute Database Non Query like Insert, Update, Delete, Create Table")]
    public class ExecuteNonQueries : TemplateAction
    {
        [Category("Input")]
        [Description("Enter Query like Insert, Update, Delete, Create Table")]
        public DynamicProperty<string> SqlQuery { get; set; }

        [Category("Input")]
        [Description("Enter Connection String")]
        public DynamicProperty<string> ConnectionString { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            SqlConnection cnn;
            cnn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SqlQuery;
            cnn.Open();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            cmd.ExecuteNonQuery();
            cnn.Close();
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " SQLQuery:" + this.SqlQuery + " Connection String" + this.ConnectionString;
        }

    }
}
