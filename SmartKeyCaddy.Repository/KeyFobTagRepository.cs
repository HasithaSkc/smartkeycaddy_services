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

        public async Task<List<KeyFobTag>> GetKeyFobTags(Guid propertyId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select keyfobtagid, chainid, propertyid, keyfobtag as keyfobtagcode, isactive, createddatetime, lastupdateddatetime from {Constants.SmartKeyCaddySchemaName}.keyfobtag
                        where keyfobtag.propertyid = @propertyId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                propertyId
            })).ToList();
        }
    }
}