﻿using Dapper;
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

            var sql = @$"select keyfobtag.keyfobtagid
                            ,keyfobtag.chainid
                            ,keyfobtag.propertyid
                            ,keyfobtag.keyfobrfid
                            ,keyfobtag.keyfobtag as keyfobtagcode
                            ,keyfobtag.isactive
                            ,keyfobtag.createddatetime
                            ,keyfobtag.lastupdateddatetime
                            ,(case when propertyroomkeyfobtag.propertyroomid is not null then 1 else 0 end) as isassigned
                        from {Constants.SmartKeyCaddySchemaName}.keyfobtag
                        left join {Constants.SmartKeyCaddySchemaName}.propertyroomkeyfobtag
                            on propertyroomkeyfobtag.keyfobtagid = keyfobtag.keyfobtagid
                        where keyfobtag.propertyid = @propertyId";

            return (await connection.QueryAsync<KeyFobTag>(sql,
            new
            {
                propertyId
            })).ToList();
        }
    }
}