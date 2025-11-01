using ContractManager.Shared.Application.Behaviors;
using ContractManager.Shared.Application.Clients.Handlers;
using ContractManager.Shared.Application.Security;
using ContractManager.Shared.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ContractManager.Shared.Application;
public static class SharedApplicationServiceRegister
{
    public static IServiceCollection AddSharedApplicationServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IJwtGeneratorService, JwtGeneratorService>();

        services.AddSharedDataServices();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

        services.AddHttpHandlers();

        return services;
    }

    private static IServiceCollection AddHttpHandlers(this IServiceCollection services)
    {
        services.AddTransient<JwtHandler>();

        return services;
    }
}
