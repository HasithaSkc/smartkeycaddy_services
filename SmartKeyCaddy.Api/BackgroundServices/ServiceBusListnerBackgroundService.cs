using SmartKeyCaddy.Domain.Contracts;

public class ServiceBusListnerBackgroundService : BackgroundService
{
    private readonly IServiceBusListenerService _serviceBusListener;
    private readonly ILogger<ServiceBusListnerBackgroundService> _logger;

    public ServiceBusListnerBackgroundService(IServiceBusListenerService serviceBusListener,
        ILogger<ServiceBusListnerBackgroundService> logger)
    {
        _serviceBusListener = serviceBusListener;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ServiceBusListnerBackgroundService is started");

        _serviceBusListener.RegisterMessageHandlerAndReceiveMessages(stoppingToken);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}
