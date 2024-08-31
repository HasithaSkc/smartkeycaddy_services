using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusListenerService : IServiceBusListenerService
{
    private readonly IQueueClient _queueClient;
    private readonly ILogger<ServiceBusListenerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public ServiceBusListenerService(IQueueClient queueClient,
        ILogger<ServiceBusListenerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _queueClient = queueClient;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;   
    }

    public async Task RegisterMessageHandlerAndReceiveMessages(CancellationToken cancellationToken)
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        // Keep the listener running until cancellation
        while (!cancellationToken.IsCancellationRequested)
        {
            _queueClient.RegisterMessageHandler(ProcessIncomingDeviceMessages, messageHandlerOptions);
            await Task.Delay(5000, cancellationToken); // Adjust delay as needed
        }
    }

    public async Task CloseQueueAsync()
    {
        await _queueClient.CloseAsync();
    }
}
