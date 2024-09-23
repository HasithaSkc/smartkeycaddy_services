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

    public async Task<List<PropertyRoom>> GetPropertyRoomKeyFobTags(Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select  propertyroom.propertyroomid
                            ,propertyroom.chainid
                            ,propertyroom.propertyid
                            ,propertyroom.roomnumber
                            ,keyfobtag.keyfobtagid as keyfobtagkeyfobtagid
                            ,keyfobtag.keyfobtagid
                            ,keyfobtag.keyfobtag as keyfobtagcode
                            ,keyfobtag.isactive
                            ,propertyroomkeyfobtag.createddatetime
                            ,propertyroomkeyfobtag.lastupdateddatetime
                            ,keyfobtag.propertyid
                            ,keyfobtag.chainid
                        from {Constants.SmartKeyCaddySchemaName}.propertyroom  
                        left join {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag on propertyroomkeyfobtag.propertyroomid = propertyroom.propertyroomid
                        left join {Constants.SmartKeyCaddySchemaName}.keyfobtag on keyfobtag.keyfobtagid = propertyroomkeyfobtag.keyfobtagid
                        where propertyroom.propertyid = @propertyId";

        return (await connection.QueryAsync<PropertyRoom, KeyFobTag, PropertyRoom>(sql,(propertyRoom, keyFobTag) => {
            propertyRoom.KeyFobTag = keyFobTag?.KeyFobTagId == Guid.Empty ? null : keyFobTag;
            return propertyRoom;
        },
        new
        {
            propertyId
        },
        splitOn: "keyfobtagkeyfobtagid")).ToList();
    }

    public async Task InsertPropertyRoomKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId, Guid chainId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"insert into {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag (chainid, propertyid, propertyroomid, keyfobtagid) values
                    (@ChainId, @PropertyId, @PropertyRoomId, @KeyFobTagId)";

        await connection.ExecuteAsync(sql,
        new
        {
            PropertyId = propertyId,
            ChainId = chainId,
            propertyRoomkeyFobTag.PropertyRoomId,
            propertyRoomkeyFobTag.KeyFobTagId
        });
    }

    public async Task DeletePropertyRoomKeyFobTags(Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"delete from {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag where propertyid = @PropertyId";

        await connection.ExecuteAsync(sql,
        new
        {
            PropertyId = propertyId
        });
    }
}