
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IServiceBusPublisherService
{
    Task PublishMessage<T>(T message, CancellationToken cancellationToken) where T : class;
    Task CloseQueueAsync();
}
