namespace Zappy.ZappyActions.Database.Helper
{
    internal class DBConnectionFactory : IDBConnectionFactory
    {
        public DatabaseConnection Create(string connectionString, string providerName)
        {
            var conn = new DatabaseConnection();
            return conn.Initialize(connectionString, providerName);
        }
    }
}
