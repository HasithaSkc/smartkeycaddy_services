
namespace SmartKeyCaddy.Domain.Contracts;

public interface IServiceBusListenerService
{
    Task RegisterOnMessageHandlerAndReceiveMessages(CancellationToken cancellationToken);
    Task CloseQueueAsync();
}
