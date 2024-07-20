using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Domain.Services;
using SmartKeyCaddy.Repository;
using NLog.Web;
using SmartKeyCaddy.Domain.Contracts;

namespace SmartKeyCaddy.Api.Configuration
{
    public class ConfigureServices
    {
        public static IHost Configure(IServiceCollection services)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureLogging(logBuilder =>
                {
                    logBuilder.ClearProviders();
                    logBuilder.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddScoped<IUserRepository, UserRepository>();
                    serviceCollection.AddScoped<IUserService, UserService>();
                    serviceCollection.AddControllers();
                    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                    serviceCollection.AddEndpointsApiExplorer();
                    serviceCollection.AddSwaggerGen();
                })
                .Build();
        }
    }
}
