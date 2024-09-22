using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository;

public class PropertyRoomRepository : IPropertyRoomRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;
    public PropertyRoomRepository(IDBConnectionFactory dbConnectionFactory) 
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<PropertyRoom>> GetPropertyRooms1(Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select keyfobtag.keyfobtagid, keyfobtag.keyfobtag, propertyroom.roomnumber, propertyroom.propertyid 
                        from {Constants.SmartKeyCaddySchemaName}.propertyroom inner join {Constants.SmartKeyCaddySchemaName}.keyfobtag on keyfobtag.propertyroomid = propertyroom.propertyroomid 
                        where propertyroom.propertyid = @propertyId";

        return (await connection.QueryAsync<PropertyRoom>(sql,
        new
        {
            propertyId
        })).ToList();
    }

    public async Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select propertyroomid, chainid, propertyid, roomnumber, isactive, createddatetime, lastupdateddatetime from {Constants.SmartKeyCaddySchemaName}.propertyroom
                        where propertyroom.propertyid = @propertyId";

        return (await connection.QueryAsync<PropertyRoom>(sql,
        new
        {
            propertyId
        })).ToList();
    }
}