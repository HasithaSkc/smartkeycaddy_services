using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public UserRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<UserInfo> GetUser(string userName, string password)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "select * from userinfo where username = @userName and password =@password";

                var user = (await connection.QueryAsync<UserInfo>(sql,
                    new
                    {
                        userName,
                        password
                    })).SingleOrDefault();

                return user;
            }
        }

        public async Task<UserInfo> GetResourceUser(string userName, string password)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @$"select * from {Constants.SmartKeyCaddySchemaName}.resourceuser where username = @userName and password = @password";

                return (await connection.QueryAsync<UserInfo>(sql,
                    new
                    {
                        userName,
                        password
                    })).SingleOrDefault();
            }
        }
    }
}