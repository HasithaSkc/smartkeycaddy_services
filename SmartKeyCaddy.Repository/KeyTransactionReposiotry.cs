using Dapper;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class KeyTransactionReposiotry : IKeyTransactionReposiotry
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        private readonly ILogger<KeyTransactionReposiotry> _logger;
        public KeyTransactionReposiotry(IDBConnectionFactory dbConnectionFactory,
            ILogger<KeyTransactionReposiotry> logger) 
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task InsertKeyTransaction(KeyTransaction keyTransaction)
        {
            _logger.LogInformation($"Inserting key transaction");

            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"insert into {Constants.SmartKeyCaddySchemaName}.keytransaction (keyallocationid, chainid, propertyid, deviceid, keytransactiontype, createddatetime, binid)
                        values (@KeyAllocationId, @ChainId, @PropertyId, @DeviceId, @KeyTransactionType, @CreatedDateTime, @BinId)";

            await connection.ExecuteAsync(sql,

            new
            {
                keyTransaction.ChainId,
                keyTransaction.PropertyId,
                keyTransaction.KeyAllocationId,
                keyTransaction.DeviceId,
                keyTransaction.KeyTransactionType,
                keyTransaction.BinId,
                CreatedDateTime = DateTime.UtcNow
            });
        }
    }
}