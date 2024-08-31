using SmartKeyCaddy.Domain.Contracts;

public class ServiceBusPublisherBackgroundService : BackgroundService
{
    private readonly IServiceBusPublisherService _serviceBusPublisherService;

    public ServiceBusPublisherBackgroundService(IServiceBusPublisherService serviceBusPublisherService)
    {
        _serviceBusPublisherService = serviceBusPublisherService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusPublisherService.ProcessUnsentKeyAllocationMessages(stoppingToken);
        return Task.CompletedTask;
    }
}
