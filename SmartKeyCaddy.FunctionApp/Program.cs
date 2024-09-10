using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Services;
using SmartKeyCaddy.Models.Configurations;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(config =>
    {
#if DEBUG
        config.AddJsonFile("appSettings.Development.json", optional: true, reloadOnChange: true);
#else
        config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
#endif
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton(typeof(ILogger), new LoggerFactory().CreateLogger<Program>());
        services.AddSingleton<IServiceBusListenerService, ServiceBusListenerService>();

        services.Configure<AzureServiceBusSettings>(hostContext.Configuration.GetSection("AzureServiceBusSettings"));

        services.AddSingleton(serviceProvider =>
        {
            var azureServiceBusSettings = hostContext.Configuration.GetSection("AzureServiceBusSettings").Get<AzureServiceBusSettings>();
            return new ServiceBusClient(azureServiceBusSettings.ConnectionString);
        });

    })
    .Build();


host.Run();
