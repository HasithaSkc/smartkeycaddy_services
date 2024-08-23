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

            var sql = @$"select keyfobtag.keyfobtagid, keyfobtag.keyfobtag as keyfobtagcode, keyfobtag.isactive, keyfobtag.createddatetime, keyfobtag.lastupdateddatetime, propertyroom.roomnumber from {Constants.SmartKeyCaddySchemaName}.keyfobtag  
                        inner join smartkeycaddyuser.propertyroom on propertyroom.propertyroomid = keyfobtag.propertyroomid
                        where propertyroom.propertyid = @propertyId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                propertyId
            })).ToList();
        }
    }
}