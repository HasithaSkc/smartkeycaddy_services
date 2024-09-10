using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;

namespace FunctionApp6
{
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
        public void Run([ServiceBusTrigger("azu-aue-smartkeycaddy-queue-dev", Connection = "ConnectionString")] string messageBody, ILogger log)
        {
            _serviceBusListener.RegisterMessageHandlerAndReceiveMessages(messageBody);
        }
    }
}
