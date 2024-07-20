using System.Data;

public interface IDBConnectionFactory
{
    IDbConnection CreateConnection();
}