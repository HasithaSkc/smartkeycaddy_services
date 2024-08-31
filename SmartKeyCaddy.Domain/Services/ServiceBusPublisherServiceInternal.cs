using Microsoft.Extensions.DependencyInjection;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusPublisherService
{
    private async Task ProcessUnsentKeyAllocationMessages()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IKeyAllocationService>();
            await scopedService.ProcessIndirectKeyAllocationMessages();
        }
    }
}
