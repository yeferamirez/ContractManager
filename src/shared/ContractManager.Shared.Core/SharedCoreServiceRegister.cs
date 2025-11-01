using Microsoft.Extensions.DependencyInjection;

namespace ContractManager.Shared.Core;
public static class SharedCoreServiceRegister
{
    public static IServiceCollection AddSharedCoreServices(this IServiceCollection services)
    {
        return services;
    }
}
