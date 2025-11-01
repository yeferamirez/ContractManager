using Amazon;
using Amazon.DynamoDBv2;
using ContractManager.Application.Configuration;
using ContractManager.Application.Storage;
using ContractManager.Infrastructure.Caching;
using ContractManager.Shared.Core.Context;
using ContractManager.Shared.Infrastructure.Caching;
using ContractManager.Shared.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ContractManager.Shared.Infrastructure;
public static class InfrastructureServiceRegister
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        CurrentEnvironment environment,
        CachedSettings cachedSettings,
        StorageSettings storageSettings)
    {
        services.AddSingleton<CachedSettings>(cachedSettings);
        services.AddSingleton<StorageSettings>(storageSettings);

        services.AddRegisterCached(cachedSettings);
        services.AddRegisterStorage(storageSettings);

        if (environment.IsDevelopment() || environment.IsLocal())
        {
            services.AddScoped<DynamoDbInitializer>();

            using (var provider = services.BuildServiceProvider())
            {
                var initializer = provider.GetRequiredService<DynamoDbInitializer>();
                initializer.InitializeAsync().GetAwaiter().GetResult();
            }
        }

        return services;
    }

    public static void AddRegisterCached(this IServiceCollection services, CachedSettings cachedSettings)
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{cachedSettings.Host}:{cachedSettings.Port}" },
            AbortOnConnectFail = false
        };

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
        services.AddScoped<ICacheService, RedisCache>();

        services.AddScoped(typeof(ICacheService), typeof(RedisCache));
    }

    public static void AddRegisterStorage(this IServiceCollection services, StorageSettings storageSettings)
    {
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(storageSettings.Region),
            ServiceURL = storageSettings.ServiceURL
        };

        services.AddSingleton<IAmazonDynamoDB>(
            _ => new AmazonDynamoDBClient(
                storageSettings.SecretAccessKey,
                storageSettings.SecretAccessKey,
                config));

        services.AddScoped(typeof(IDynamoRepository<>), typeof(DynamoRepository<>));
    }
}
