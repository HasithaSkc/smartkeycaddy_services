
namespace SmartKeyCaddy.Domain.Contracts;

public interface IServiceBusListenerService
{
    Task RegisterMessageHandlerAndReceiveMessages(string messageBody);
}
