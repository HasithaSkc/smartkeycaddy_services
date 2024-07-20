using Npgsql;
using System.Data;
namespace SmartKeyCaddy.Repository;
public class SqlConnectionFactory : IDBConnectionFactory
{
    private string connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}