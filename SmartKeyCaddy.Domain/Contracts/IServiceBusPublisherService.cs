
namespace SmartKeyCaddy.Domain.Contracts;

public interface IServiceBusPublisherService
{
    Task ProcessUnsentKeyAllocationMessages(CancellationToken cancellationToken);
}
