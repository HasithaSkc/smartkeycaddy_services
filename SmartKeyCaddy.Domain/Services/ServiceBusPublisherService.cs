using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Domain.Contracts;
using System.Text;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusPublisherService : IServiceBusPublisherService
{
    private readonly IQueueClient _queueClient;
    private readonly ILogger<ServiceBusListenerService> _logger;
    
    public ServiceBusPublisherService(IQueueClient queueClient,
        ILogger<ServiceBusListenerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task PublishMessage<T>(T message, CancellationToken cancellationToken) where T : class
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Create a message to send to the queue
                string messageBody = JsonConvert.SerializeObject(message, JsonHelper.GetJsonSerializerSettings());
                var messageObj = new Message(Encoding.UTF8.GetBytes(messageBody));

                // Send the message to the queue
                await _queueClient.SendAsync(messageObj);

                _logger.LogInformation($"Message sent: {messageBody}");
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message");
            }
            finally
            {
                await _queueClient.CloseAsync();
            }
        }
    }

    public async Task CloseQueueAsync()
    {
        await _queueClient.CloseAsync();
    }
}
