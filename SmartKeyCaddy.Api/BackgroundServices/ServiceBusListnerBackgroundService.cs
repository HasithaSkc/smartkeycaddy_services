using SmartKeyCaddy.Domain.Contracts;

public class ServiceBusListnerBackgroundService : BackgroundService
{
    private readonly IServiceBusListenerService _serviceBusListener;

    public ServiceBusListnerBackgroundService(IServiceBusListenerService serviceBusListener)
    {
        _serviceBusListener = serviceBusListener;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusListener.RegisterMessageHandlerAndReceiveMessages(stoppingToken);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _serviceBusListener.CloseQueueAsync();
        await base.StopAsync(stoppingToken);
    }
}
