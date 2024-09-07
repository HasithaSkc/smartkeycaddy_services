using SmartKeyCaddy.Domain.Contracts;

public class ServiceBusPublisherBackgroundService : BackgroundService
{
    private readonly IServiceBusPublisherService _serviceBusPublisherService;
    private readonly ILogger<ServiceBusPublisherBackgroundService> _logger;

    public ServiceBusPublisherBackgroundService(IServiceBusPublisherService serviceBusPublisherService, 
        ILogger<ServiceBusPublisherBackgroundService> logger)
    {
        _serviceBusPublisherService = serviceBusPublisherService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusPublisherService.ProcessUnsentKeyAllocationMessages(stoppingToken);
        return Task.CompletedTask;
    }
}
