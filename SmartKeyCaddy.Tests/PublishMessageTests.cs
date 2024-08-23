using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Services;
using SmartKeyCaddy.Models.Configurations;
using System.IO;
using System.Runtime;

namespace SmartKeyCaddy.Tests
{
    [TestClass]
    public class PublishMessageTests
    {
        private IConfiguration _configuration;
        private IServiceBusPublisherService _serviceBusPublisherService;
        private IOptions<AzureServiceBusSettings> _azureServiceBusSettings;
        private CancellationToken _cancellationToken;

        [TestInitialize]
        public void OneTimeSetup()
        {
            var logger = new LoggerFactory().CreateLogger<ServiceBusListenerService>();

            _cancellationToken = new CancellationTokenSource().Token;
            // Build the configuration from appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            // Register configuration options
            serviceCollection.Configure<AzureServiceBusSettings>(_configuration.GetSection("AzureServiceBusSettings"));

            // Set up dependency injection
            
            serviceCollection.AddTransient<IServiceBusPublisherService, ServiceBusPublisherService>();
            serviceCollection.AddTransient<IServiceBusPublisherService, ServiceBusPublisherService>();
            serviceCollection.AddSingleton(typeof(ILogger), logger);
            serviceCollection.AddSingleton<IQueueClient>(serviceProvider =>
            {
                var azureServiceBusSettings = serviceProvider.GetRequiredService<IOptions<AzureServiceBusSettings>>().Value;
                return new QueueClient(azureServiceBusSettings.ConnectionString, azureServiceBusSettings.QueueName);
            });

            var _serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve the service you need to test
            _serviceBusPublisherService = _serviceProvider.GetService<IServiceBusPublisherService>();
        }

        [TestMethod]
        public async Task Publish_DeviceRegister_Message()
        {
            var message = Helper.GetDeviceRegisterMessage();
            await _serviceBusPublisherService.PublishMessage(message, _cancellationToken);
            
        }
    }
}
