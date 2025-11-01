using Microsoft.AspNetCore.Cors.Infrastructure;

namespace ContractManager.Shared.Api.Extensions;
public static class CorsPolicyBuilderExtensions
{
    public const string DefaultCorsPolicyName = "ContractCorsPolicy";

    public static void AddDefaultCors(this CorsPolicyBuilder corsBuilder)
    {
        corsBuilder
            .WithOrigins([])
            .WithHeaders("Authorization", "Content-Type", "Accept", "X-Requested-With", "X-HTTP-Method-Override")
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
    }
}
