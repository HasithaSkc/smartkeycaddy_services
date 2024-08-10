using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class KeyAllocationRepository : IKeyAllocationRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public KeyAllocationRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
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

        public async Task Insertkey(KeyAllocation keyAllocation)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"insert into {Constants.SmartKeyCaddySchemaName}.keyallocation (chainid, propertyid, deviceid, checkindate, checkoutdate, keyname, keypincode, binid, keyfobtagid, guestwelcomemessage, keypickupinstruction, issuccessful, status, createddatetime)
                        values (@ChainId, @PropertyId, @DeviceId, @CheckInDate, @CheckOutDate, @KeyName, @KeyPinCode, @BinId, @KeyFobTagId, @GuestWelcomeMessage, @KeyPickupInstruction, @IsSuccessful, @Status, @CreatedDateTime)
                        returning keyallocationid";

            
            keyAllocation.KeyAllocationId = await connection.QuerySingleAsync<Guid>(sql,
            new
            {
                keyAllocation.ChainId,
                keyAllocation.PropertyId,
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

        public async Task<bool> KeyExists(string keyName, DateTime? checkinDate)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"select exists (select 1 from {Constants.SmartKeyCaddySchemaName}.keyallocation 
                        where keyname = @keyName and (@checkinDateString IS NULL OR checkindate::date = @checkinDateString::date))";

            return (await connection.QueryAsync<bool>(sql,
            new
            {
                keyName,
                checkinDateString = checkinDate?.ToString(Constants.ShortDateString)
            })).First();
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
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var sql = @$"update {Constants.SmartKeyCaddySchemaName}.keyallocation 
                        set status = @Status
                            ,issuccessful = @IsSuccessful
                            ,binid = @BinId
                            ,updateddatetime = @updatedDatetime 
                        where keyallocationid = @keyAllocationId";
                await connection.QueryAsync<KeyAllocation>(sql,
                new
                {
                    keyAllocation.KeyAllocationId,
                    keyAllocation.BinId,
                    keyAllocation.IsSuccessful,
                    keyAllocation.Status,
                    updatedDatetime = DateTime.UtcNow
                });
            }
            catch (Exception ss)
            {

                throw;
            }
           
        }
    }
}