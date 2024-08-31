
namespace SmartKeyCaddy.Domain.Contracts;

public interface IServiceBusListenerService
{
    Task RegisterMessageHandlerAndReceiveMessages(CancellationToken cancellationToken);
    Task CloseQueueAsync();
}
