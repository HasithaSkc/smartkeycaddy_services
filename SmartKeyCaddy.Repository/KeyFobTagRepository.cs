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

        public async Task<List<KeyFobTag>> GetPropertyRoomKeyFobTags(Guid propertyId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select keyfobtag.keyfobtagid, keyfobtag.keyfobtag as keyfobtagcode, keyfobtag.isactive, propertyroom.roomnumber, keyfobtagpropertyroom.createddatetime, keyfobtagpropertyroom.lastupdateddatetime,
                        propertyroom.roomnumber from {Constants.SmartKeyCaddySchemaName}.keyfobtagpropertyroom  
                        inner join {Constants.SmartKeyCaddySchemaName}.keyfobtag on keyfobtag.keyfobtagid = keyfobtagpropertyroom.keyfobtagid
                        inner join {Constants.SmartKeyCaddySchemaName}.propertyroom on propertyroomid = keyfobtagpropertyroom.propertyroomid
                        where keyfobtagpropertyroom.propertyid = @propertyId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                propertyId
            })).ToList();
        }

        public async Task<List<KeyFobTag>> GetKeyFobTags(Guid propertyId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select keyfobtagid, chainid, propertyid, keyfobtag, isactive, createddatetime, lastupdateddatetime from {Constants.SmartKeyCaddySchemaName}.keyfobtag
                        where keyfobtag.propertyid = @propertyId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                propertyId
            })).ToList();
        }
    }
}