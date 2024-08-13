using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Configurations;

namespace SmartKeyCaddy.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public DeviceRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Device> GetDevice(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"select deviceid, devicename, serialnumber, bincount, chainid, propertyid, ismasterlocker, isactive, isregistered
	                    from {Constants.SmartKeyCaddySchemaName}.device where deviceid = @deviceId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    deviceId
                })).SingleOrDefault();
        }

        public async Task<List<Device>> GetDevices(Guid locationId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"select deviceid, devicecode, devicename, serialnumber, numberofbins, locationid, ismaster, masterdeviceid, chainid
	                    from {Constants.SmartKeyCaddySchemaName}.device where locationid = @locationId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    locationId
                })).ToList();
        }

        public async Task RegisterDevice(Guid deviceId, bool isRegistered)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"update {Constants.SmartKeyCaddySchemaName}.device 
                            set isregistered = @isRegistered,
                            registereddate= @registeredDate where deviceid = @deviceId";

            await connection.QueryAsync<Device>(sql,
                new
                {
                    deviceId,
                    isRegistered,
                    registeredDate = DateTime.UtcNow,
                });
        }
    }
}