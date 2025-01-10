using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;

        private readonly string _deviceQuerybase = @"select deviceid, devicename, displayname, serialnumber, bincount, chainid, propertyid, ismasterlocker, isactive, isregistered";

        public DeviceRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Device> GetDevice(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"{_deviceQuerybase}
	                    from {Constants.SmartKeyCaddySchemaName}.device where deviceid = @deviceId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    deviceId
                })).SingleOrDefault();
        }

        public async Task<Device> GetDevice(Guid deviceId, string deviceName)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"{_deviceQuerybase}
	                    from {Constants.SmartKeyCaddySchemaName}.device where deviceid = @deviceId and devicename = @deviceName";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    deviceId,
                    deviceName
                })).SingleOrDefault();
        }

        public async Task<List<Bin>> GetDeviceBinDetails(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"select bin.binid
                            ,bin.status
                            ,bin.inuse
                            ,bin.binnumber
                            ,bin.chainid
                            ,bin.propertyid
                            ,bin.createddatetime
                            ,bin.lastupdateddatetime
                            ,bin.deviceid
                            ,ka.currentkey
                            ,ka.status as keyallocationstatus
                        from {Constants.SmartKeyCaddySchemaName}.bin
                        left join lateral (
                            select keyname as currentkey, status
                            from {Constants.SmartKeyCaddySchemaName}.keyallocation
                            where keyallocation.binid = bin.binid and date(keyallocation.checkindate) = '{DateTime.Now.AddHours(-2).ToString(Constants.ShortDateString)}'
                            order by keyallocation.lastupdateddatetime desc
                            limit 1
                        ) ka on true 
                        inner join {Constants.SmartKeyCaddySchemaName}.device on device.deviceid = bin.deviceid
                        where device.deviceid = @deviceId";

            return (await connection.QueryAsync<Bin>(sql,
                new
                {
                    deviceId,
                })).ToList();
        }

        public async Task<List<Bin>> GetDeviceBinDetailsWithKeyAllocation(Guid deviceId, DateTime localDateTime)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"select bin.binid
                            ,bin.status
                            ,bin.inuse
                            ,bin.binnumber
                            ,bin.chainid
                            ,bin.propertyid
                            ,bin.createddatetime
                            ,bin.lastupdateddatetime
                            ,bin.deviceid
                            ,ka.currentkey
                            ,ka.status as keyallocationstatus
                        from {Constants.SmartKeyCaddySchemaName}.bin
                        left join lateral (
                            select keyname as currentkey, status
                            from {Constants.SmartKeyCaddySchemaName}.keyallocation
                            where keyallocation.binid = bin.binid and date(keyallocation.checkindate) = cast(@localDateTime as date)
                            order by keyallocation.lastupdateddatetime desc
                            limit 1
                        ) ka on true 
                        inner join {Constants.SmartKeyCaddySchemaName}.device on device.deviceid = bin.deviceid
                        where device.deviceid = @deviceId";

            return (await connection.QueryAsync<Bin>(sql,
                new
                {
                    deviceId,
                    localDateTime = localDateTime.ToString(Constants.ShortDateString)
                })).ToList();
        }

        public async Task<List<Device>> GetDevices(Guid propertyId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"{_deviceQuerybase} from {Constants.SmartKeyCaddySchemaName}.device where propertyid = @propertyId";

            return (await connection.QueryAsync<Device>(sql,
                new
                {
                    propertyId
                })).ToList();
        }

        public async Task<List<DeviceSetting>> GetDeviceSettings(Guid deviceId, Guid propertyId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"select devicesettingid, chainid, propertyid, deviceid, settingname, settingvalue, settingdescription, createddatetime, lastupdateddatetime
	                    from {Constants.SmartKeyCaddySchemaName}.devicesetting where deviceid = @deviceId and propertyid = @propertyId";

            return (await connection.QueryAsync<DeviceSetting>(sql,
                new
                {
                    deviceId,
                    propertyId
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