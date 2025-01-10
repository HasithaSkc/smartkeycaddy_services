using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Domain.Services;
using SmartKeyCaddy.Models.Configurations;
using SmartKeyCaddy.Repository;

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
        services.AddSingleton<IAdminService, AdminService>();
        services.AddSingleton<IIotHubServiceClient, IotHubServiceClient>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IKeyAllocationRepository, KeyAllocationRepository>();
        services.AddScoped<IPropertyRoomRepository, PropertyRoomRepository>();
        services.AddScoped<IMessageQueueRepository, MessageQueueRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IBinRepository, BinRepository>();
        services.AddScoped<IKeyFobTagRepository, KeyFobTagRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IKeyTransactionReposiotry, KeyTransactionReposiotry>();
        services.AddScoped<IPropertyRoomRepository, PropertyRoomRepository>();
        services.AddScoped<IKeyFobTagRepository, KeyFobTagRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IKeyAllocationService, KeyAllocationService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IPropertyRoomService, PropertyRoomService>();
        services.AddScoped<IKeyFobTagService, KeyFobTagService>();

        services.Configure<AzureServiceBusSettings>(hostContext.Configuration.GetSection("AzureServiceBusSettings"));
        services.Configure<IotHubSettings>(hostContext.Configuration.GetSection("IotHubSettings"));
        services.AddSingleton<IDBConnectionFactory>(new SqlConnectionFactory(hostContext.Configuration.GetConnectionString("DatabaseConnectionString")));
        services.AddSingleton(serviceProvider =>
        {
            var azureServiceBusSettings = hostContext.Configuration.GetSection("AzureServiceBusSettings").Get<AzureServiceBusSettings>();
            return new ServiceBusClient(azureServiceBusSettings.ConnectionString);
        });

    })
    .Build();


host.Run();
