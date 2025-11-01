using Ardalis.GuardClauses;
using ContractManager.Api.BackgroudService;
using ContractManager.Api.Configuration;
using ContractManager.Api.Extensions;
using ContractManager.Api.Filters;
using ContractManager.Api.Mappers;
using ContractManager.Application;
using ContractManager.Application.Configuration;
using ContractManager.Application.Consumers;
using ContractManager.Data.EF;
using ContractManager.Shared.Api;
using ContractManager.Shared.Api.Attributes;
using ContractManager.Shared.Api.Extensions;
using ContractManager.Shared.Api.Middlewares;
using ContractManager.Shared.Application.Clients;
using ContractManager.Shared.Application.Security.Configuration;
using ContractManager.Shared.Core.Context;
using ContractManager.Shared.Infrastructure;
using MassTransit;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ContractManager.Api;

public static class ApiServiceRegister
{
    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        var storageSettings = configuration.GetSection("StorageDb").Get<StorageSettings>() ?? Guard.Against.Null<StorageSettings>(null, "StorageDb", "Invalid Storage settings");
        var cachedSettings = configuration.GetSection("Cached").Get<CachedSettings>() ?? Guard.Against.Null<CachedSettings>(null, "Cached", "Invalid Storage settings");
        var messagingSettings = configuration.GetSection("MessagingSettings").Get<MessagingSettings>() ?? Guard.Against.Null<MessagingSettings>(null, "Messaging", "Invalid Messaging settings");
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? Guard.Against.Null<JwtSettings>(null, "JwtSettings", "Invalid CivikaJwt settings");
        var clientsSettings = configuration.GetSection("Clients").Get<ClientsSettings>() ?? Guard.Against.Null<ClientsSettings>(null, "Clients", "Invalid Clients settings");

        var env = new CurrentEnvironment(webHostEnvironment.EnvironmentName);
        services.AddSingleton<CurrentEnvironment>(env)
            .AddSingleton(jwtSettings)
            .AddSingleton(clientsSettings);

        services
           .AddContractsApplicationServices(jwtSettings)
           //.AddAzureStorageServices(webHostEnvironment.IsDevelopment(), storageSettings)
           .AddHttpContextAccessor()
           .RegisterMassTransit(messagingSettings)
           .AddAutoMapper(Assembly.GetAssembly(typeof(ContractsProfile)));

        services.AddSingleton(TimeProvider.System);

        services
            .AddSharedApiServices()
            .AddJwtAuthentication(env, configuration, jwtSettings)
            .AddInfrastructureServices(env, cachedSettings, storageSettings)
            .RegisterDatabase(configuration, env.IsDevelopment() || env.IsLocal());

        services.AddRolesClaimsTransformation();

        services.AddHostedService<RefreshCachedBackgroundService>();
        services.AddHostedService<ContractExpirationCheckerService>();

        services.AddScoped<AuthorizationAttribute>();

        return services;
    }

    public static IServiceCollection AddAspNetDependencies(
        this IServiceCollection services)
    {
        services.AddSerilog();

        services.AddControllers(c =>
        {
            c.Filters.Add(typeof(BusinessExceptionAttribute));
        })
        .AddJsonOptions(c =>
        {
            c.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddCommonSwaggerOptions();
        })
            .AddCommonSwaggerServices<Program>();

        return services;
    }

    public static WebApplication ConfigureApp(
        this WebApplication app)
    {
        var env = new CurrentEnvironment(app.Environment.EnvironmentName);

        app.UseMiddleware<ExceptionsLoggerMiddleware>();

        app.UseCors(CorsPolicyBuilderExtensions.DefaultCorsPolicyName);

        app.UseSerilogRequestLogging();

        if (env.IsDevelopment() || env.IsLocal())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();

            app.TraceAllSettings();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static IServiceCollection RegisterMassTransit(this IServiceCollection services, MessagingSettings messagingSettings)
    {
        services.AddMassTransit(c =>
        {
            RegisterConsumers(c);

            c.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(messagingSettings.Host, messagingSettings.VirtualHost, h =>
                {
                    h.Username(messagingSettings.Username);
                    h.Password(messagingSettings.Password);
                });

                RegisterEndpoints(cfg, context);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static void RegisterConsumers(IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        busRegistrationConfigurator.AddConsumer<NotifyContractExpiredConsumer>();
    }

    private static void RegisterEndpoints<T>(IBusFactoryConfigurator<T> cfg, IBusRegistrationContext context) where T : IReceiveEndpointConfigurator
    {
        cfg.ReceiveEndpoint(NotifyContractExpiredConsumer.EndpointName, e =>
        {
            e.UseMessageRetry(rc => rc.Interval(3, TimeSpan.FromSeconds(1)));
            e.ConfigureConsumer<NotifyContractExpiredConsumer>(context);
        });
    }
}
