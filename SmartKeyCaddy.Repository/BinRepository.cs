using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class BinRepository : IBinRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public BinRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<Bin>> GetBins(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select binid,binnumber,binaddress,status,false as inuse,createddatetime,lastupdateddatetime from {Constants.SmartKeyCaddySchemaName}.bin  where deviceid = @deviceId";

            return (await connection.QueryAsync<Bin>(sql,
            new
            {
                deviceId
            })).ToList();
        }

        public async Task UpdateBinInUse(Guid binId, bool inUse)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"update {Constants.SmartKeyCaddySchemaName}.bin 
                        set inuse = @inUse,
                        lastupdateddatetime = lastUpdatedDatetime
                        where binid = @binId";

            await connection.ExecuteAsync(sql,
            new
            {
                binId,
                inUse,
                lastUpdatedDatetime = DateTime.UtcNow
            });
        }
    }
}