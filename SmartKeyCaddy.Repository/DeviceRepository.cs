using Dapper;
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
            var sql = @"select deviceid, devicename, serialnumber, bincount, chainid, propertyid, ismasterlocker, isactive
	                    from device where deviceid = @deviceId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    deviceId
                })).SingleOrDefault();
        }

        public async Task<List<Device>> GetDevices(Guid locationId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @"select deviceid, devicecode, devicename, serialnumber, numberofbins, locationid, ismaster, masterdeviceid, chainid
	                    from device where locationid = @locationId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    locationId
                })).ToList();
        }
    }
}