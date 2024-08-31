using Dapper;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class KeyAllocationRepository : IKeyAllocationRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        private readonly ILogger<KeyAllocationRepository> _logger;
        public KeyAllocationRepository(IDBConnectionFactory dbConnectionFactory,
            ILogger<KeyAllocationRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        private const string keyAllocationSql = @"select keyallocation.keyallocationid
                                        ,keyallocation.chainid
                                        ,keyallocation.propertyid
                                        ,keyallocation.deviceid
                                        ,keyallocation.checkindate
                                        ,keyallocation.checkoutdate
                                        ,keyallocation.keyname
                                        ,keyallocation.keypincode
                                        ,keyallocation.binid
                                        ,keyallocation.keyfobtagid
                                        ,keyallocation.guestwelcomemessage
                                        ,keyallocation.keypickupinstruction
                                        ,keyallocation.issuccessful
                                        ,keyallocation.status
                                        ,keyallocation.createddatetime";

        public async Task<KeyAllocation> GetKeyAllocation(Guid deviceId, Guid keyAllocationId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $"{keyAllocationSql} from {Constants.SmartKeyCaddySchemaName}.keyallocation where deviceid = @deviceId and keyallocationid = @keyAllocationId";
            return await connection.QuerySingleAsync<KeyAllocation>(sql,
            new
            {
                deviceId,
                keyAllocationId
            });
        }

        public async Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $"{keyAllocationSql} from {Constants.SmartKeyCaddySchemaName}.keyallocation where deviceid = @deviceId";
            return (await connection.QueryAsync<KeyAllocation>(sql,
            new
            {
                deviceId
            })).ToList();
        }

        public async Task InsertkeyAllocation(KeyAllocation keyAllocation)
        {
            _logger.LogInformation($"Inserting key allocation: {keyAllocation.KeyName}");

            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"insert into {Constants.SmartKeyCaddySchemaName}.keyallocation (chainid, propertyid, keyallocationid, deviceid, checkindate, checkoutdate, keyname, keypincode, binid, keyfobtagid, guestwelcomemessage, keypickupinstruction, issuccessful, status, createddatetime)
                        values (@ChainId, @PropertyId, @KeyAllocationId, @DeviceId, @CheckInDate, @CheckOutDate, @KeyName, @KeyPinCode, @BinId, @KeyFobTagId, @GuestWelcomeMessage, @KeyPickupInstruction, @IsSuccessful, @Status, @CreatedDateTime)
                        returning keyallocationid";

            keyAllocation.KeyAllocationId = await connection.QuerySingleAsync<Guid>(sql,
            new
            {
                keyAllocation.ChainId,
                keyAllocation.PropertyId,
                keyAllocation.KeyAllocationId,
                keyAllocation.DeviceId,
                keyAllocation.CheckInDate,
                keyAllocation.CheckOutDate,
                keyAllocation.KeyName,
                keyAllocation.KeyPinCode,
                keyAllocation.BinId,
                keyAllocation.KeyFobTagId,
                keyAllocation.GuestWelcomeMessage,
                keyAllocation.KeyPickupInstruction,
                keyAllocation.IsSuccessful,
                keyAllocation.Status,
                CreatedDateTime = DateTime.UtcNow
            });
        }

        public async Task<KeyAllocation> GetKeyAllocationByKeyName(string keyName)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $"{keyAllocationSql} from {Constants.SmartKeyCaddySchemaName}.keyallocation where keyname = @keyName";
            return (await connection.QueryAsync<KeyAllocation>(sql,
            new
            {
                keyName
            })).SingleOrDefault();
        }

        public async Task<KeyAllocation> GetKeyAllocation(Guid keyAllocationId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $"{keyAllocationSql} from {Constants.SmartKeyCaddySchemaName}.keyallocation where keyallocationid = @keyAllocationId";
            return (await connection.QueryAsync<KeyAllocation>(sql,
            new
            {
                keyAllocationId
            })).SingleOrDefault();
        }

        public async Task UpdateKeyAllocation(KeyAllocation keyAllocation)
        {
            _logger.LogInformation($"Updating key allocation: {keyAllocation.KeyName}");

            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"update {Constants.SmartKeyCaddySchemaName}.keyallocation 
                    set  issuccessful = @IsSuccessful
                        ,status = @Status
                        ,checkindate = @CheckInDate
                        ,checkoutdate = @CheckOutDate
                        ,keypickupinstruction = @KeyPickupInstruction
                        ,guestwelcomemessage = @GuestWelcomeMessage
                        ,keypincode = @KeyPinCode
                        ,lastupdateddatetime = @lastUpdatedDatetime 
                    where keyallocationid = @keyAllocationId";
            await connection.ExecuteAsync(sql,
            new
            {
                keyAllocation.KeyAllocationId,
                keyAllocation.IsSuccessful,
                keyAllocation.Status,
                keyAllocation.CheckInDate,
                keyAllocation.CheckOutDate,
                keyAllocation.KeyPickupInstruction,
                keyAllocation.GuestWelcomeMessage,
                keyAllocation.KeyPinCode,
                lastUpdatedDatetime = DateTime.UtcNow
            });
        }
    }
}