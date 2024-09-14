using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Common;
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
        try
        {
            var success = false;

            _logger.LogInformation($"Processing incomming message: {messageBody}");

            var messageType = GetMessageType(messageBody);

            switch (messageType)
            {
                case MessageType.KeyTransaction:
                    await ProcessDeviceKeyTransaction(messageBody);
                    break;
                case MessageType.DeviceRegistration:
                    await ProcessDeviceRegistration(messageBody);
                    break;
            }
            success = true;

            await InsertIntoServiceBusMessageQueue(messageBody, success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterMessageHandlerAndReceiveMessages");
        }
    }
}
