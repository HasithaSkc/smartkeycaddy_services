using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models.Configurations;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusListenerService : IServiceBusListenerService
{
    private readonly ILogger<ServiceBusListenerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly AzureServiceBusSettings _azureServiceBusSettings;

    public ServiceBusListenerService(ServiceBusClient serviceBusClient,
        ILogger<ServiceBusListenerService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AzureServiceBusSettings> azureServiceBusSettings)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _serviceBusClient = serviceBusClient;
        _azureServiceBusSettings = azureServiceBusSettings.Value;
    }

    public async Task RegisterMessageHandlerAndReceiveMessages(string messageBody)
    {
        await ProcessIncomingDeviceMessages(messageBody);
    }
}
