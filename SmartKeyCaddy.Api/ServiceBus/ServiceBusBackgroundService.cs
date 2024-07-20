using SmartKeyCaddy.Domain.Contracts;

public class ServiceBusBackgroundService : BackgroundService
{
    private readonly IServiceBusListenerService _serviceBusListener;

    public ServiceBusBackgroundService(IServiceBusListenerService serviceBusListener)
    {
        _serviceBusListener = serviceBusListener;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusListener.RegisterOnMessageHandlerAndReceiveMessages(stoppingToken);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _serviceBusListener.CloseQueueAsync();
        await base.StopAsync(stoppingToken);
    }
}
