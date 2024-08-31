using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusPublisherService : IServiceBusPublisherService
{
    private readonly ILogger<ServiceBusPublisherService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ServiceBusPublisherService(ILogger<ServiceBusPublisherService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ProcessUnsentKeyAllocationMessages(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing unsent key allocation message background service srated.");

        // Keep the listener running until cancellation
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessUnsentKeyAllocationMessages();
            await Task.Delay(5000, cancellationToken); // Adjust delay as needed
        }
    }
}
