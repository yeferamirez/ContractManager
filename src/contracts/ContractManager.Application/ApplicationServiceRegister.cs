using ContractManager.Application.CachedServices;
using ContractManager.Application.Services;
using ContractManager.Shared.Application.Security.Configuration;
using FluentValidation;
using IdGen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ContractManager.Application;

public static class ApplicationServiceRegister
{
    public static IServiceCollection AddContractsApplicationServices(
        this IServiceCollection services,
        JwtSettings jwtSettings)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegister).Assembly);
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(ApplicationServiceRegister).Assembly));

        services.AddTrackingIdGenerator();

        services.AddScoped<ICacheRefresherService, CachedContractsService>();

        return services;
    }

    public static IServiceCollection AddRolesClaimsTransformation(this IServiceCollection services)
    {
        services.AddScoped<IClaimsTransformation, RolesClaimsTransformationService>();

        return services;
    }

    private static void AddTrackingIdGenerator(this IServiceCollection services)
    {
        var structure = new IdStructure(35, 20, 8);
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(DateTime.UtcNow));

        services.AddSingleton<IdGeneratorOptions>(options);
    }
}
