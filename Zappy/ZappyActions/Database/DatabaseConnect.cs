using Newtonsoft.Json;
using System.Activities;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Database.Helper;

namespace Zappy.ZappyActions.Database
{
    [Description("Connect To Database")]
    public abstract class DatabaseConnect : TemplateAction
    {
                                [DefaultValue(null)]
        [RequiredArgument]
        [Description("The ProviderName is used to set the name of the .NET Framework data provider ")]
        public string ProviderName { get; set; }

                                        [DependsOn(nameof(ProviderName))]
        [DefaultValue(null)]
        [RequiredArgument]
        [Description("Gets or sets the string used to open a database")]
        public string ConnectionString { get; set; }

                                [XmlIgnore, JsonIgnore]
        [Browsable(false)]
        public DatabaseConnection DatabaseConnection;

        private readonly IDBConnectionFactory _connectionFactory;

        public DatabaseConnect()
        {
            _connectionFactory = new DBConnectionFactory();
        }

        internal DatabaseConnect(IDBConnectionFactory factory)
        {
            _connectionFactory = factory;
        }

                                        protected DatabaseConnection CreateDatabaseConnection()
        {
            var connString = ConnectionString;
            var provName = ProviderName;
            DatabaseConnection = _connectionFactory.Create(connString, provName);
            return DatabaseConnection;
        }

                                                        public override string AuditInfo()
        {
            return base.AuditInfo() + " ProviderName:" + this.ProviderName + " Connection String:" + this.ConnectionString;
        }
    }
}
