namespace Zappy.ZappyActions.Database.Helper
{
    internal interface IDBConnectionFactory
    {
        DatabaseConnection Create(string connectionString, string providerName);
    }
}