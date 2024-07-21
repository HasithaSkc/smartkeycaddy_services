using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models.Configurations;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Repository
{
    public class MessageQueueRepository : IMessageQueueRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public MessageQueueRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InsertMessage(ServiceBusMessage serviceBusMessage)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = @$"insert into {Constants.SmartKeyCaddySchemaName}.messagequeue (deviceid, devicename, messagebody, isprocessed, createddatetime, enqueueddatetime)
                        values (@DeviceId, @DeviceName, @MessageBody, @IsProcessed, @CreatedDateTime, @EnqueuedDateTime)";

            await connection.ExecuteAsync(sql,
            new
            {
                serviceBusMessage.DeviceId,
                serviceBusMessage.DeviceName,
                serviceBusMessage.MessageBody,
                serviceBusMessage.IsProcessed,
                CreatedDateTime = DateTime.UtcNow,
                serviceBusMessage.EnqueuedDateTime
            });
        }
    }
}