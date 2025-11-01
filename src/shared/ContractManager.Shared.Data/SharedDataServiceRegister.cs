using Microsoft.Extensions.DependencyInjection;

namespace ContractManager.Shared.Data;
public static class SharedDataServiceRegister
{
    public static IServiceCollection AddSharedDataServices(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

        return services;
    }
}
