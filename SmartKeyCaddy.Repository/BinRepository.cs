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

        public async Task<Bin> GetBin(Guid deviceId, Guid binId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select binid,binnumber,binaddress,status,false as inuse,createddatetime,lastupdateddatetime from 
                        {Constants.SmartKeyCaddySchemaName}.bin  where deviceid = @deviceId and binid =@binId";

            return (await connection.QueryAsync<Bin>(sql,
            new
            {
                deviceId,
                binId
            })).SingleOrDefault();
        }

        public async Task<List<Bin>> GetBins(Guid deviceId, bool includeChildBins = true)
        {
            if (!includeChildBins)
                return await GetBins(deviceId);

            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"with recursive recursivedevice as (
                        -- Base case: select the parent device's bin
                        select 
                            deviceid
                        from 
                            smartkeycaddyuser.device
                        where 
                            deviceid = @deviceId  -- Starting point: the parent device

                        union all

                        -- Recursive case: select the child devices related to the current device
                        select 
                           device.deviceid
                        from 
                            smartkeycaddyuser.device
                        inner join 
                            recursivedevice on recursivedevice.deviceid = device.parentdeviceid  -- Recursively get children
                    )
					
					select bin.binid, bin.binnumber, bin.binaddress, bin.status, bin.inuse, bin.createddatetime, bin.lastupdateddatetime
					from 
						smartkeycaddyuser.bin
					inner join
						recursivedevice on recursivedevice.deviceid = bin.deviceid;";

            return (await connection.QueryAsync<Bin>(sql,
            new
            {
                deviceId
            })).ToList();
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