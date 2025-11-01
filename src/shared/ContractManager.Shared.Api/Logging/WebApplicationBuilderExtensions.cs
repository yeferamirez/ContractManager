using ContractManager.Shared.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ContractManager.Shared.Api.Logging;
public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.WithCorrelationIdHeader(ContractConstants.Headers.CorrelationId))
        .ConfigureAppConfiguration((c, b) => ConfigureApp(c, b));

        builder.Services.AddSerilog();

        return builder;
    }

    private static void ConfigureApp(HostBuilderContext context, IConfigurationBuilder builder)
    {

    }
}
