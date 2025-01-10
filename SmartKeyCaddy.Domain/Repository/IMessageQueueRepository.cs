using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IMessageQueueRepository
    {
        Task InsertMessage(ServiceBusMessage serviceBusMessage);
    }
}
