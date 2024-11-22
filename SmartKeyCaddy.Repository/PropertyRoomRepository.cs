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
                            ,keyfobtag.keyfobrfid
                            ,keyfobtag.keyfobtag as KeyFobTagCode
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

    public async Task DeletePropertyRoomKeyFobTag(Guid propertyId, Guid propertyRoomId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"delete from {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag where propertyid = @propertyId
                     and propertyroomid = @propertyRoomId";

        await connection.ExecuteAsync(sql,
        new
        {
            propertyId,
            propertyRoomId
        });
    }

    public async Task<PropertyRoomKeyFobtag> GetPropertyRoomKeyFobTag(Guid propertyId, Guid propertyRoomId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select propertyroomid
                            ,keyfobtagid 
                        from {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag 
                        where propertyroomkeyfobtag.propertyid = @propertyId and propertyroomkeyfobtag.propertyroomid = @propertyRoomId";

        return (await connection.QueryAsync<PropertyRoomKeyFobtag>(sql,
        new
        {
            propertyId,
            propertyRoomId
        })).SingleOrDefault();
    }

    public async Task UpdatePropertyRoomKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"update {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag
                    set keyfobtagid = @KeyFobTagId where propertyroomid = @PropertyRoomId and propertyid = @PropertyId";

        await connection.ExecuteAsync(sql,
        new
        {
            PropertyId = propertyId,
            propertyRoomkeyFobTag.KeyFobTagId,
            propertyRoomkeyFobTag.PropertyRoomId
        });
    }

    public async Task<PropertyRoom> GetPropertyRoomByKeyFobTag(Guid keyFobTagId, Guid propertyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select propertyroomid, keyfobtagid 
                    from {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag
                        where propertyroomkeyfobtag.keyfobtagid = @keyFobTagId and propertyroomkeyfobtag.propertyid = @propertyId";

        return (await connection.QueryAsync<PropertyRoom>(sql,
        new
        {
            propertyId,
            keyFobTagId
        })).SingleOrDefault();

    }
}