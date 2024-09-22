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

        public async Task<AdminUser> GetUser(string userName, string password)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = $"select * from {Constants.SmartKeyCaddySchemaName}.adminuser where username = @userName and password =@password";

                var user = (await connection.QueryAsync<AdminUser>(sql,
                    new
                    {
                        userName,
                        password
                    })).SingleOrDefault();

                return user;
            }
        }

        public async Task<ResourceUser> GetResourceUser(string userName, string password)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select resourceuserid, username, password, createddate from {Constants.SmartKeyCaddySchemaName}.resourceuser where username = @userName and password = @password";

            return (await connection.QueryAsync<ResourceUser>(sql,
                new
                {
                    userName,
                    password
                })).SingleOrDefault();
        }

        public async Task<List<Property>> GetAdminUserProperties(Guid adminUserId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select 
                             property.propertyid
                            ,property.propertyname
                            ,property.propertycode
                            ,property.propertyshortcode
                        from {Constants.SmartKeyCaddySchemaName}.adminuserproperty
                        inner join {Constants.SmartKeyCaddySchemaName}.adminuser on adminuser.adminuserid = adminuserproperty.adminuserid
                        inner join {Constants.SmartKeyCaddySchemaName}.property on property.propertyid = adminuserproperty.propertyid
                        where adminuser.adminuserid = @adminUserId";

            return (await connection.QueryAsync<Property>(sql,
                new
                {
                    adminUserId
                })).ToList();
        }
    }
}