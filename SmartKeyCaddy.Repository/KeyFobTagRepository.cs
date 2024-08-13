using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class KeyFobTagRepository : IKeyFobTagRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public KeyFobTagRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<KeyFobTag>> GetKeyFobTags(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select keyfobtagid, keyfobtag, isactive from {Constants.SmartKeyCaddySchemaName}.keyfobtag  where deviceid = @deviceId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                deviceId
            })).ToList();
        }
    }
}