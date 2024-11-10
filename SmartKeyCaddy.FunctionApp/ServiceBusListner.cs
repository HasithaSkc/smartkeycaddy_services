using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.FunctionApp;

public class ServiceBusListner
{
    private readonly ILogger<ServiceBusListner> _logger;
    private readonly IServiceBusListenerService _serviceBusListener;

    public ServiceBusListner(ILogger<ServiceBusListner> logger,
        IServiceBusListenerService serviceBusListener)
    {
        _logger = logger;
        _serviceBusListener = serviceBusListener;
    }

    [Function(nameof(ServiceBusListner))]
    public async Task Run([ServiceBusTrigger("azu-aue-smartkeycaddy-queue-dev", Connection = "ConnectionString")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions, ILogger log)
    {
        await _serviceBusListener.RegisterMessageHandlerAndReceiveMessages(Encoding.UTF8.GetString(message.Body));

        await messageActions.CompleteMessageAsync(message);
    }


}
