using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Domain.Services;
using SmartKeyCaddy.Repository;
using HotelCheckIn.Models.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NLog.Web;
using SmartKeyCaddy.Api.ExceptionHandling;
using System.Text.Json;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models.Configurations;
using HotelCheckIn.Domain.Contracts;
using Azure.Messaging.ServiceBus;

var logger = new LoggerFactory().CreateLogger<Program>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(ILogger), logger);
builder.Services.Configure<IotHubSettings>(builder.Configuration.GetSection("IotHubSettings"));

// Add services to the container.
builder.Services.AddSingleton<IIotHubServiceClient, IotHubServiceClient>();
builder.Services.Configure<AdminFunctionsApiSettings>(builder.Configuration.GetSection("AdminFunctionsApiSettings"));
builder.Services.Configure<AzureServiceBusSettings>(builder.Configuration.GetSection("AzureServiceBusSettings"));
builder.Services.Configure<EmailApiSettings>(builder.Configuration.GetSection("EmailApiSettings"));
builder.Services.AddMemoryCache();

builder.Services.AddHostedService<ServiceBusListnerBackgroundService>();
builder.Services.AddHostedService<ServiceBusPublisherBackgroundService>();
builder.Services.AddSingleton<IServiceBusListenerService, ServiceBusListenerService>();
builder.Services.AddSingleton<IServiceBusPublisherService, ServiceBusPublisherService>();
builder.Services.AddSingleton<ServiceBusClient>(serviceProvider =>
{
    var azureServiceBusSettings = builder.Configuration.GetSection("AzureServiceBusSettings").Get<AzureServiceBusSettings>();
    return new ServiceBusClient(azureServiceBusSettings.ConnectionString);
});

builder.Services.AddSingleton<IDBConnectionFactory>(new SqlConnectionFactory(builder.Configuration.GetConnectionString("DatabaseConnectionString")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IKeyAllocationRepository, KeyAllocationRepository>();
builder.Services.AddScoped<IPropertyRoomRepository, PropertyRoomRepository>();
builder.Services.AddScoped<IMessageQueueRepository, MessageQueueRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IBinRepository, BinRepository>();
builder.Services.AddScoped<IKeyFobTagRepository, KeyFobTagRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IKeyAllocationService, KeyAllocationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IotHubServiceClient, IotHubServiceClient>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

//builder.Services.AddApiVersioning(x =>
//{
//    x.DefaultApiVersion = new ApiVersion(1, 0);
//    x.AssumeDefaultVersionWhenUnspecified = true;
//    x.ReportApiVersions = true;
//});

#if DEBUG
builder.Host.UseNLog();
#else
    builder.Logging.AddApplicationInsights(
            configureTelemetryConfiguration: (config) => 
                config.ConnectionString = builder.Configuration.GetConnectionString("AppInsightsConnectionString"),
                configureApplicationInsightsLoggerOptions: (options) => { }
        );

#endif

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});


# if DEBUG
    builder.Host.UseNLog();
#else
    builder.Logging.AddApplicationInsights(
            configureTelemetryConfiguration: (config) => 
                config.ConnectionString = builder.Configuration.GetConnectionString("AppInsightsConnectionString"),
                configureApplicationInsightsLoggerOptions: (options) => { }
        );

#endif

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.ConfigureCustomExceptionMiddleware();
app.MapControllers();

app.Run();
