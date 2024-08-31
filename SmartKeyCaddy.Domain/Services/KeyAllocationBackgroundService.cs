using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Services;

public class KeyAllocationBackgroundService : BackgroundService
{
    private readonly ILogger<KeyAllocationBackgroundService> _logger;
    private readonly IKeyAllocationService _keyAllocationService;

    public KeyAllocationBackgroundService(ILogger<KeyAllocationBackgroundService> logger,
        IKeyAllocationService keyAllocationService)
    {
        _logger = logger;
        _keyAllocationService = keyAllocationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Background Service is doing background work.");

            await _keyAllocationService.ProcessIndirectKeyAllocationMessages();
            // Simulate some background work
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogInformation("Background Service is stopping.");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
